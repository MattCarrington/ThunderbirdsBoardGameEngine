import http from 'k6/http';
import { check, fail, sleep } from 'k6';
import { Counter, Rate, Trend } from 'k6/metrics';

const profile = __ENV.PROFILE || 'smoke';
const baseUrl = (__ENV.BASE_URL || 'http://localhost:5198').replace(/\/$/, '');
const p95Budget = Number(__ENV.P95_MS || 500);
if (!Number.isFinite(p95Budget) || p95Budget <= 0) {
  throw new Error('P95_MS must be a positive number.');
}

const constantUsers = (vus, duration, startTime = '0s') => ({
  executor: 'constant-vus', vus, duration, startTime, gracefulStop: '5s',
});
const profiles = {
  smoke: { smoke: constantUsers(1, '10s') },
  baseline: { baseline: constantUsers(1, '30s') },
  load: {
    five_users: constantUsers(5, '30s'),
    ten_users: constantUsers(10, '30s', '35s'),
    recovery: constantUsers(1, '30s', '70s'),
  },
};
if (!profiles[profile]) throw new Error('PROFILE must be smoke, baseline, or load.');

const successDuration = new Trend('movement_success_duration', true);
const successes = new Counter('movement_successes');
const rejected = new Rate('movement_rate_limited');
const unexpected = new Rate('movement_unexpected_response');
const incorrect = new Rate('movement_incorrect_body');
const thresholds = {
  movement_successes: ['count>0'],
  movement_rate_limited: ['rate==0'],
  movement_unexpected_response: ['rate==0'],
  movement_incorrect_body: ['rate==0'],
};
// Separate phase percentiles: an overall average can hide degradation at 10 users.
for (const scenario of Object.keys(profiles[profile])) {
  thresholds[`movement_success_duration{scenario:${scenario}}`] = [`p(95)<${p95Budget}`];
  thresholds[`movement_successes{scenario:${scenario}}`] = ['count>0'];
}

export const options = {
  scenarios: profiles[profile], thresholds,
  summaryTrendStats: ['avg', 'med', 'p(95)', 'max'],
};

const url = `${baseUrl}/api/rules/movement/thunderbird-2/validate`;
const body = JSON.stringify({ startLocation: 'south-pacific', destinationLocation: 'europe' });
const params = {
  headers: { 'Content-Type': 'application/json', 'X-API-Version': '1' },
  timeout: '5s', redirects: 0,
  responseCallback: http.expectedStatuses(200),
};

function hasExpectedResult(response) {
  if (response.status !== 200) return false;
  try {
    const result = response.json();
    return result.isValid === true && result.spacesTravelled === 3 &&
      result.actionPointCost === 2 && result.thunderbirdTopSpeed === 2 &&
      result.effectiveTopSpeed === 2 && Array.isArray(result.route) &&
      result.route.length > 0 && Array.isArray(result.messages) && result.messages.length === 0;
  } catch (_) {
    return false;
  }
}

export function setup() {
  // Fail early for a wrong target/payload, and warm this route before measuring.
  for (let i = 0; i < 5; i++) {
    const response = http.post(url, body, params);
    if (!hasExpectedResult(response)) {
      fail(`Preflight failed: expected a valid movement result; HTTP ${response.status}. Check the API and limiter.`);
    }
    sleep(1);
  }
}

export default function () {
  const response = http.post(url, body, params);
  const correct = hasExpectedResult(response);
  rejected.add(response.status === 429);
  unexpected.add(response.status !== 200 && response.status !== 429);
  incorrect.add(response.status === 200 && !correct);
  successes.add(correct ? 1 : 0);
  check(response, { 'correct movement response': () => correct });
  if (correct) successDuration.add(response.timings.duration);
  // Exploratory player pacing: at most roughly one action per second per VU.
  sleep(1);
}

export function handleSummary(data) {
  const lines = [`Movement performance: ${profile}`, `Target: ${baseUrl}`,
    `Exploratory p95 budget: ${p95Budget} ms (not a production SLO)`];
  for (const [name, metric] of Object.entries(data.metrics).sort()) {
    if (name.startsWith('movement_')) lines.push(`${name}: ${JSON.stringify(metric.values)}`);
  }
  const failed = Object.entries(data.metrics).flatMap(([name, metric]) =>
    Object.entries(metric.thresholds || {}).filter(([, value]) => !value.ok)
      .map(([threshold]) => `${name}: ${threshold}`));
  lines.push(failed.length ? `FAILED thresholds: ${failed.join('; ')}` : 'All thresholds passed.');
  return {
    stdout: `${lines.join('\n')}\n`,
    [__ENV.SUMMARY_PATH || 'summary.json']: JSON.stringify({
      experiment: { profile, baseUrl, p95BudgetMs: p95Budget, thinkTimeSeconds: 1 }, ...data,
    }, null, 2),
  };
}
