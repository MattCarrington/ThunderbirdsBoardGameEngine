function assembleReleaseManifest(receipts, expected) {
  const roles = ["app", "migrator", "provisioner"];
  const images = {};

  if (!Array.isArray(receipts) || receipts.length !== roles.length) {
    throw new Error("Expected exactly three image receipts.");
  }

  for (const receipt of receipts) {
    if (
      !receipt ||
      !roles.includes(receipt.role) ||
      Object.hasOwn(images, receipt.role)
    ) {
      throw new Error("Unexpected or duplicate image role.");
    }

    if (
      receipt.commit !== expected.commit ||
      receipt.run_id !== expected.run_id ||
      receipt.run_attempt !== expected.run_attempt ||
      receipt.version !== expected.version
    ) {
      throw new Error("Receipt does not match this release.");
    }

    if (
      typeof receipt.image !== "string" ||
      !/^[a-z0-9]+(?:[._-][a-z0-9]+)*$/.test(receipt.image) ||
      typeof receipt.reference !== "string"
    ) {
      throw new Error("Invalid image identity.");
    }

    const prefix =
      `ghcr.io/${expected.owner.toLowerCase()}/${receipt.image}@`;

    if (
      !receipt.reference.startsWith(prefix) ||
      !/^sha256:[a-f0-9]{64}$/.test(
        receipt.reference.slice(prefix.length)
      )
    ) {
      throw new Error("Invalid registry or image digest.");
    }

    images[receipt.role] = receipt.reference;
  }

  return {
    schema_version: 1,
    repository: `${expected.owner}/${expected.repo}`,
    commit: expected.commit,
    run_id: expected.run_id,
    run_attempt: expected.run_attempt,
    version: expected.version,
    images
  };
}

module.exports = { assembleReleaseManifest };