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
    const candidates = artifacts.flatMap(artifact => {
    const match = /^release-manifest-([1-9][0-9]*)$/.exec(
        artifact.name
    );

    return match
        ? [{ artifact, attempt: Number(match[1]) }]
        : [];
    });

    const currentAttempt = Number(expected.run_attempt);

    if (candidates.some(candidate => candidate.attempt > currentAttempt)) {
        throw new Error("Manifest artifact is from a future run attempt.");
    }

    const eligible = candidates.filter(
        candidate => candidate.attempt <= currentAttempt
    );

    const newestAttempt = Math.max(
        ...eligible.map(candidate => candidate.attempt)
    );

    const matches = eligible
        .filter(candidate => candidate.attempt === newestAttempt)
        .map(candidate => candidate.artifact);

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
        name: matches[0].name,
        run_attempt: String(newestAttempt)
    };
}

module.exports = { 
    selectCiRun, 
    selectReleaseArtifact 
};