function selectCiRun(run, expected) {
    const repository = `${expected.owner}/${expected.repo}`;

    if (
        run.path !== ".github/workflows/ci.yml" ||
        run.event !== "push" ||
        run.status !== "completed" ||
        run.conclusion !== "success" ||
        run.head_repository?.full_name !== repository
    ) {
        throw new Error(
            "Select a successful push CI run from this repository."
        );
    }

    if (
        expected.automatic &&
        run.head_sha !== expected.currentMainCommit
    ) {
        return {
            eligible: false,
            reason: "This commit has been superseded on main."
        };
    }

    return {
        eligible: true,
        run_id: String(run.id),
        run_attempt: String(run.run_attempt),
        commit: run.head_sha
    };
}

function selectReleaseArtifact(artifacts, expected) {
    const name = `release-manifest-${expected.run_attempt}`;
    const matches = artifacts.filter(artifact => artifact.name === name);

    if (matches.length === 0) {
        if (!expected.automatic) {
            throw new Error(
                "The selected CI run has no release manifest."
            );
        }

        return {
            available: false,
            reason: "No release manifest exists for this CI attempt; delivery skipped."
        };
    }

    if (matches.length !== 1 || matches[0].expired) {
        throw new Error(
            "Expected one unexpired release manifest."
        );
    }

    return {
        available: true,
        name: matches[0].name
    };
}

module.exports = { 
    selectCiRun, 
    selectReleaseArtifact 
};