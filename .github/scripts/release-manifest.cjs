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

function validateReleaseManifest(manifest, expected) {
  if (
    !manifest ||
    manifest.schema_version !== 1 ||
    manifest.repository !== `${expected.owner}/${expected.repo}` ||
    manifest.run_id !== expected.run_id ||
    manifest.run_attempt !== expected.run_attempt ||
    manifest.commit !== expected.commit ||
    typeof manifest.version !== "string" ||
    !/^[A-Za-z0-9_][A-Za-z0-9_.-]{0,127}$/.test(manifest.version)
  ) {
    throw new Error("Manifest does not match the selected release.");
  }

  const roles = ["app", "migrator", "provisioner"];

  if (
    !manifest.images ||
    typeof manifest.images !== "object" ||
    Array.isArray(manifest.images) ||
    Object.keys(manifest.images).length !== roles.length
  ) {
    throw new Error("Expected three release image references.");
  }

  const prefix = `ghcr.io/${expected.owner.toLowerCase()}/`;

  for (const role of roles) {
    const reference = manifest.images[role];

    if (
      typeof reference !== "string" ||
      !reference.startsWith(prefix) ||
      !/^[a-z0-9]+(?:[._-][a-z0-9]+)*@sha256:[a-f0-9]{64}$/.test(
        reference.slice(prefix.length)
      )
    ) {
      throw new Error(`Invalid image reference for ${role}.`);
    }
  }

  if (new Set(Object.values(manifest.images)).size !== roles.length) {
    throw new Error("Release roles must identify distinct images.");
  }

  return manifest;
}

module.exports = { 
    assembleReleaseManifest, 
    validateReleaseManifest 
};