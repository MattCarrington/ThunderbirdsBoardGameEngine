function assembleReleaseManifest(receipts, expected) {
  const roles = ["app", "migrator", "provisioner"];
  const selectedReceipts = {};
  const seenReceipts = new Set();

  if (!Array.isArray(receipts)) {
    throw new Error("Expected image receipts.");
  }

  for (const receipt of receipts) {
    if (!receipt || !roles.includes(receipt.role)) {
      throw new Error("Unexpected image role.");
    }

    if (
      receipt.commit !== expected.commit ||
      receipt.run_id !== expected.run_id ||
      receipt.version !== expected.version
    ) {
      throw new Error("Receipt does not match this release.");
    }

    const receiptAttempt = Number(receipt.run_attempt);
    const currentAttempt = Number(expected.run_attempt);

    if (
      !/^[1-9][0-9]*$/.test(receipt.run_attempt) ||
      !/^[1-9][0-9]*$/.test(expected.run_attempt) ||
      !Number.isSafeInteger(receiptAttempt) ||
      !Number.isSafeInteger(currentAttempt) ||
      receiptAttempt > currentAttempt
    ) {
      throw new Error("Invalid receipt run attempt.");
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

    const selected = selectedReceipts[receipt.role];

    if (
      !selected ||
      receiptAttempt > Number(selected.run_attempt)
    ) {
      selectedReceipts[receipt.role] = receipt;
    }

    const receiptKey = `${receipt.role}:${receiptAttempt}`;

    if (seenReceipts.has(receiptKey)) {
      throw new Error("Duplicate receipt for image role and attempt.");
    }

    seenReceipts.add(receiptKey);
  }

  if (roles.some(role => !Object.hasOwn(selectedReceipts, role))) {
    throw new Error("Missing receipt for required image role.");
  }

  const images = Object.fromEntries(
    roles.map(role => [
      role,
      selectedReceipts[role].reference
    ])
  );

  const imageAttempts = Object.fromEntries(
    roles.map(role => [
      role,
      selectedReceipts[role].run_attempt
    ])
  );

  return {
    schema_version: 1,
    repository: `${expected.owner}/${expected.repo}`,
    commit: expected.commit,
    run_id: expected.run_id,
    run_attempt: expected.run_attempt,
    version: expected.version,
    images,
    image_attempts: imageAttempts
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
    !manifest.image_attempts ||
    typeof manifest.image_attempts !== "object" ||
    Array.isArray(manifest.image_attempts) ||
    Object.keys(manifest.image_attempts).length !== roles.length ||
    roles.some(role => !Object.hasOwn(manifest.image_attempts, role))
  ) {
    throw new Error("Invalid image attempt provenance.");
  }

  if (
    !manifest.images ||
    typeof manifest.images !== "object" ||
    Array.isArray(manifest.images) ||
    Object.keys(manifest.images).length !== roles.length
  ) {
    throw new Error("Expected three release image references.");
  }

  const manifestAttempt = Number(manifest.run_attempt);

  for (const role of roles) {
    const value = manifest.image_attempts[role];
    const attempt = Number(value);

    if (
      typeof value !== "string" ||
      !/^[1-9][0-9]*$/.test(value) ||
      !Number.isSafeInteger(attempt) ||
      attempt > manifestAttempt
    ) {
      throw new Error("Invalid image attempt provenance.");
    }
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