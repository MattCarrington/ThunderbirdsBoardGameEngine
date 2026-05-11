namespace ThunderbirdsBoardGameEngine.ReferenceData
{
    /// <summary>
    /// Defines version constants for reference data snapshot compatibility and tracking.
    /// </summary>
    public static class SnapshotVersions
    {
        /// <summary>
        /// The schema version of the snapshot format.
        /// </summary>
        /// <remarks>
        /// This version must match between the compiler and runtime to ensure compatibility.
        /// Increment this value when making breaking changes to the snapshot structure.
        /// </remarks>
        public const int SchemaVersion = 1;

        /// <summary>
        /// The version of the compiler tool that generates snapshots.
        /// </summary>
        /// <remarks>
        /// Currently hardcoded for simplicity. Future versions may derive this from assembly metadata.
        /// </remarks>
        public const string GeneratorVersion = "1.0.0";

        /// <summary>
        /// The default content version for generated snapshots.
        /// </summary>
        /// <remarks>
        /// Currently hardcoded for simplicity. Future versions may use semantic versioning 
        /// or timestamp-based versioning.
        /// </remarks>
        public const string ContentVersion = "1.0.0";
    }
}
