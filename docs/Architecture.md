## Architecture Overview

The system is organised around multiple bounded contexts with strict dependency rules:

- **Catalog** owns bonus definitions and identity generation.
- **Rules** applies bonuses by identity only.
- Bonus identities are opaque and shared via a Published Language.
- Lookup models are projections/adapters and may depend on both contexts.
- Domain models do not depend on each other.
