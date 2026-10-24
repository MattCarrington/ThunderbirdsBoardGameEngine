const test = require("node:test");
const assert = require("node:assert/strict");

const { selectCiRun } = require("./release-selection.cjs");

function createFixture() {
    return {
        run: {
            id: 12345,
            run_attempt: 1,
            path: ".github/workflows/ci.yml",
            event: "push",
            status: "completed",
            conclusion: "success",
            head_repository: {
                full_name: "ExampleOwner/ExampleProject"
            },
            head_sha: "a".repeat(40)
        },
        expected: {
            owner: "ExampleOwner",
            repo: "ExampleProject",
            automatic: true,
            currentMainCommit: "a".repeat(40)
        }
    };
}

test("selects a successful CI run for the current main commit", () => {
    const { run, expected } = createFixture();

    const result = selectCiRun(run, expected);

    assert.deepEqual(result, {
        eligible: true,
        run_id: "12345",
        run_attempt: "1",
        commit: run.head_sha
    });
});

test("rejects if run path not expected", () => {
    const { run, expected } = createFixture();
    run.path = ".github/workflows/other.yml";

    assert.throws(() => selectCiRun(run, expected), {
        message: "Select a successful push CI run from this repository."
    });
});

test("rejects if run is not a push event", () => {
    const { run, expected } = createFixture();
    run.event = "pull_request";

    assert.throws(() => selectCiRun(run, expected), {
        message: "Select a successful push CI run from this repository."
    });
});

test("rejects if run is not completed", () => {
    const { run, expected } = createFixture();
    run.status = "in_progress";

    assert.throws(() => selectCiRun(run, expected), {
        message: "Select a successful push CI run from this repository."
    });
});

test("rejects if run did not succeed", () => {
    const { run, expected } = createFixture();
    run.conclusion = "failure";

    assert.throws(() => selectCiRun(run, expected), {
        message: "Select a successful push CI run from this repository."
    });
});

test("rejects if run is from a different repository", () => {
    const { run, expected } = createFixture();
    run.head_repository.full_name = "OtherOwner/OtherProject";

    assert.throws(() => selectCiRun(run, expected), {
        message: "Select a successful push CI run from this repository."
    });
});

test("marks as ineligible if commit has been superseded on main", () => {
    const { run, expected } = createFixture();
    expected.currentMainCommit = "b".repeat(40);

    const result = selectCiRun(run, expected);
    assert.deepEqual(result, {
        eligible: false,
        reason: "This commit has been superseded on main."
    });
});

test("allows manual delivery of a commit different from current main", () => {
    const { run, expected } = createFixture();
    expected.automatic = false;
    expected.currentMainCommit = "b".repeat(40);

    const result = selectCiRun(run, expected);

    assert.deepEqual(result, {
        eligible: true,
        run_id: "12345",
        run_attempt: "1",
        commit: run.head_sha
    });
});

test("rejects a run with no source repository", () => {
    const { run, expected } = createFixture();
    run.head_repository = null;

    assert.throws(() => selectCiRun(run, expected), {
        message: "Select a successful push CI run from this repository."
    });
});