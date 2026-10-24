const test = require("node:test");
const assert = require("node:assert/strict");

const {
    selectCiRun,
    selectReleaseArtifact
} = require("./release-selection.cjs");

function createCiFixture() {
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

function createReleaseArtifactFixture() {
    const artifacts = [
        {
            name: "release-manifest-1",
            expired: false
        }
    ];

    const expected = {
        run_attempt: "1",
        automatic: true
    };

    return { artifacts, expected };
}

test("selects a successful CI run for the current main commit", () => {
    const { run, expected } = createCiFixture();

    const result = selectCiRun(run, expected);

    assert.deepEqual(result, {
        eligible: true,
        run_id: "12345",
        run_attempt: "1",
        commit: run.head_sha
    });
});

test("rejects if run path not expected", () => {
    const { run, expected } = createCiFixture();
    run.path = ".github/workflows/other.yml";

    assert.throws(() => selectCiRun(run, expected), {
        message: "Select a successful push CI run from this repository."
    });
});

test("rejects if run is not a push event", () => {
    const { run, expected } = createCiFixture();
    run.event = "pull_request";

    assert.throws(() => selectCiRun(run, expected), {
        message: "Select a successful push CI run from this repository."
    });
});

test("rejects if run is not completed", () => {
    const { run, expected } = createCiFixture();
    run.status = "in_progress";

    assert.throws(() => selectCiRun(run, expected), {
        message: "Select a successful push CI run from this repository."
    });
});

test("rejects if run did not succeed", () => {
    const { run, expected } = createCiFixture();
    run.conclusion = "failure";

    assert.throws(() => selectCiRun(run, expected), {
        message: "Select a successful push CI run from this repository."
    });
});

test("rejects if run is from a different repository", () => {
    const { run, expected } = createCiFixture();
    run.head_repository.full_name = "OtherOwner/OtherProject";

    assert.throws(() => selectCiRun(run, expected), {
        message: "Select a successful push CI run from this repository."
    });
});

test("marks as ineligible if commit has been superseded on main", () => {
    const { run, expected } = createCiFixture();
    expected.currentMainCommit = "b".repeat(40);

    const result = selectCiRun(run, expected);
    assert.deepEqual(result, {
        eligible: false,
        reason: "This commit has been superseded on main."
    });
});

test("allows manual delivery of a commit different from current main", () => {
    const { run, expected } = createCiFixture();
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
    const { run, expected } = createCiFixture();
    run.head_repository = null;

    assert.throws(() => selectCiRun(run, expected), {
        message: "Select a successful push CI run from this repository."
    });
});

test("selects one matching unexpired release manifest", () => {
    const { artifacts, expected } = createReleaseArtifactFixture();

    const result = selectReleaseArtifact(artifacts, expected);

    assert.deepEqual(result, {
        available: true,
        name: "release-manifest-1"
    });
});

test("ignores artifacts from other run attempts", () => {
    const { artifacts, expected } = createReleaseArtifactFixture();
    artifacts.push({
        name: "release-manifest-2",
        expired: false
    });

    const result = selectReleaseArtifact(artifacts, expected);

    assert.deepEqual(result, {
        available: true,
        name: "release-manifest-1"
    });
});

test("skips automatic delivery when the release manifest is missing", () => {
    const { artifacts, expected } = createReleaseArtifactFixture();
    expected.automatic = true;
    artifacts.length = 0;

    const result = selectReleaseArtifact(artifacts, expected);

    assert.deepEqual(result, {
        available: false,
        reason: "No release manifest exists for this CI attempt; delivery skipped."
    });
});

test("rejects manual delivery when the release manifest is missing", () => {
    const { artifacts, expected } = createReleaseArtifactFixture();
    expected.automatic = false;
    artifacts.length = 0;

    assert.throws(() => selectReleaseArtifact(artifacts, expected), {
        message: "The selected CI run has no release manifest."
    });
});

test("rejects duplicate matching release manifests", () => {
    const { artifacts, expected } = createReleaseArtifactFixture();
    artifacts.push({
        name: "release-manifest-1",
        expired: false
    });

    assert.throws(() => selectReleaseArtifact(artifacts, expected), {
        message: "Expected one unexpired release manifest."
    });
});

test("rejects an expired release manifest", () => {
    const { artifacts, expected } = createReleaseArtifactFixture();
    artifacts[0].expired = true;

    assert.throws(() => selectReleaseArtifact(artifacts, expected), {
        message: "Expected one unexpired release manifest."
    });
});