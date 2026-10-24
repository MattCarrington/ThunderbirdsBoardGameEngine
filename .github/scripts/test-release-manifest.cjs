const test = require("node:test");
const assert = require("node:assert/strict");

const {
  assembleReleaseManifest,
  validateReleaseManifest
} = require("./release-manifest.cjs");

function createFixture() {
  const expected = {
    owner: "ExampleOwner",
    repo: "ExampleProject",
    commit: "a".repeat(40),
    run_id: "12345",
    run_attempt: "1",
    version: "1.2.3"
  };

  const receipts = ["app", "migrator", "provisioner"].map(
    (role, index) => {
      const image = `example-${role}`;
      const digest = String(index + 1).repeat(64);

      return {
        role,
        image,
        reference: `ghcr.io/exampleowner/${image}@sha256:${digest}`,
        commit: expected.commit,
        run_id: expected.run_id,
        run_attempt: expected.run_attempt,
        version: expected.version
      };
    }
  );

  return { receipts, expected };
}

function createManifestFixture() {
  const expected = {
    owner: "ExampleOwner",
    repo: "ExampleProject",
    commit: "a".repeat(40),
    run_id: "12345",
    run_attempt: "1"
  };

  const manifest = {
    schema_version: 1,
    repository: "ExampleOwner/ExampleProject",
    commit: "a".repeat(40),
    run_id: "12345",
    run_attempt: "1",
    version: "1.2.3",
    images: {
      app: `ghcr.io/exampleowner/example-app@sha256:${"1".repeat(64)}`,
      migrator: `ghcr.io/exampleowner/example-migrator@sha256:${"2".repeat(64)}`,
      provisioner: `ghcr.io/exampleowner/example-provisioner@sha256:${"3".repeat(64)}`
    }
  };

  return { manifest, expected };
}

test("assembles three images into one release manifest", () => {
  const { receipts, expected } = createFixture();

  const manifest = assembleReleaseManifest(receipts, expected);

  assert.deepStrictEqual(manifest, {
    schema_version: 1,
    repository: "ExampleOwner/ExampleProject",
    commit: expected.commit,
    run_id: "12345",
    run_attempt: "1",
    version: "1.2.3",
    images: {
      app: receipts[0].reference,
      migrator: receipts[1].reference,
      provisioner: receipts[2].reference
    }
  });
});

test("rejects images built from different commits", () => {
  const { receipts, expected } = createFixture();
  receipts[1].commit = "b".repeat(40);

  assert.throws(
    () => assembleReleaseManifest(receipts, expected),
    /Receipt does not match this release/
  );
});

test("rejects duplicate roles", () => {
  const { receipts, expected } = createFixture();
  receipts[2].role = "app";

  assert.throws(
    () => assembleReleaseManifest(receipts, expected),
    /Unexpected or duplicate image role/
  );
});

test("rejects an image from another registry", () => {
  const { receipts, expected } = createFixture();
  receipts[0].reference = receipts[0].reference.replace(
    "ghcr.io/",
    "other.example/"
  );

  assert.throws(
    () => assembleReleaseManifest(receipts, expected),
    /Invalid registry or image digest/
  );
});

test("expects exactly three image receipts", () => {
  const { receipts, expected } = createFixture();
  receipts.pop(); // Remove one image to have only two

  assert.throws(
    () => assembleReleaseManifest(receipts, expected),
    /Expected exactly three image receipts./
  );
});

test("rejects an image with a different receipt version", () => {
  const { receipts, expected } = createFixture();
  receipts[0].version = "9.9.9";

  assert.throws(
    () => assembleReleaseManifest(receipts, expected),
    /Receipt does not match this release/
  );
});

test("validates a correct release manifest", () => {
  const { manifest, expected } = createManifestFixture();

  const result = validateReleaseManifest(manifest, expected);

  assert.deepStrictEqual(result, manifest);
});

test("rejects a manifest from a different commit", () => {
  const { manifest, expected } = createManifestFixture();
  manifest.commit = "b".repeat(40);

  assert.throws(
    () => validateReleaseManifest(manifest, expected),
    /Manifest does not match the selected release/
  );
});

test("rejects a manifest from a different repository", () => {
  const { manifest, expected } = createManifestFixture();
  manifest.repository = "OtherOwner/OtherProject";

  assert.throws(
    () => validateReleaseManifest(manifest, expected),
    /Manifest does not match the selected release/
  );
});

test("rejects a manifest with an invalid schema version", () => {
  const { manifest, expected } = createManifestFixture();
  manifest.schema_version = 2;

  assert.throws(
    () => validateReleaseManifest(manifest, expected),
    /Manifest does not match the selected release/
  );
});

test("rejects a manifest from a different owner", () => {
  const { manifest, expected } = createManifestFixture();
  manifest.images.app = manifest.images.app.replace(
    "ghcr.io/exampleowner/",
    "ghcr.io/otherowner/"
    );
  assert.throws(
    () => validateReleaseManifest(manifest, expected),
    /Invalid image reference for app/
  );
});

test("rejects a manifest with an extra image reference", () => {
  const { manifest, expected } = createManifestFixture();
  manifest.images.extra = "ghcr.io/" + expected.owner.toLowerCase() + "/extra@sha256:" + "a".repeat(64);

  assert.throws(
    () => validateReleaseManifest(manifest, expected),
    /Expected three release image references./
  );
});

test("rejects a manifest with a missing image reference", () => {    
  const { manifest, expected } = createManifestFixture();
  delete manifest.images.app;

  assert.throws(
    () => validateReleaseManifest(manifest, expected),
    /Expected three release image references./
  );
});

test("rejects a manifest from a different CI run", () => {
  const { manifest, expected } = createManifestFixture();
  manifest.run_id = "67890";

  assert.throws(
    () => validateReleaseManifest(manifest, expected),
    /Manifest does not match the selected release/
  );
});

test("rejects a manifest from a different run attempt", () => {
  const { manifest, expected } = createManifestFixture();
  manifest.run_attempt = "2";

  assert.throws(
    () => validateReleaseManifest(manifest, expected),
    /Manifest does not match the selected release/
  );
});

test("rejects a mutable image tag instead of a digest", () => {
  const { manifest, expected } = createManifestFixture();
  manifest.images.app = "ghcr.io/exampleowner/example-app:latest";

  assert.throws(
    () => validateReleaseManifest(manifest, expected),
    /Invalid image reference for app/
  );
});

test("rejects a malformed image digest", () => {
  const { manifest, expected } = createManifestFixture();
  manifest.images.app =
    "ghcr.io/exampleowner/example-app@sha256:invalid";

  assert.throws(
    () => validateReleaseManifest(manifest, expected),
    /Invalid image reference for app/
  );
});
test("rejects the same image reference assigned to multiple roles", () => {
  const { manifest, expected } = createManifestFixture();
  manifest.images.app = manifest.images.migrator;

  assert.throws(
    () => validateReleaseManifest(manifest, expected),
    /Release roles must identify distinct images/
  );
});

test("rejects an unknown role replacing a required role", () => {
  const { manifest, expected } = createManifestFixture();
  manifest.images.unknown = manifest.images.app;
  delete manifest.images.app;

  assert.throws(
    () => validateReleaseManifest(manifest, expected),
    /Invalid image reference for app/
  );
});

test("rejects a manifest without a version", () => {
  const { manifest, expected } = createManifestFixture();
  delete manifest.version;

  assert.throws(
    () => validateReleaseManifest(manifest, expected),
    /Manifest does not match the selected release/
  );
});

test("rejects a null manifest", () => {
  const manifest = null;
  const expected = createManifestFixture().expected;

  assert.throws(
    () => validateReleaseManifest(manifest, expected),
    /Manifest does not match the selected release/
  );
});