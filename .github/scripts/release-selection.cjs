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

module.exports = { selectCiRun };