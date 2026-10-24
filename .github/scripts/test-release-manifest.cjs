const test = require("node:test");
const assert = require("node:assert/strict");

const {
  assembleReleaseManifest
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