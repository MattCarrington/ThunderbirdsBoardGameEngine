# Rules Bounded Context

The Rules bounded context evaluates game rules based on
inputs from the Reference Data and player choices.

## Responsibilities
- Calculates rescue targets
- Applies bonuses by identity
- Calculates movement routes
- Produces deterministic results

## Design Notes
- Bonus keys are opaque identifiers
- Rules never parse or interpret bonus keys
- Unknown bonus keys are ignored
