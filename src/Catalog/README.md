# Catalog Bounded Context

The Catalog bounded context defines the static game data:
- **Disaster Card** is the aggregate root
- Bonus conditions and rewards are **owned by** and **scoped to** a Disaster Card
- Bonus conditions and rewards do not exist independently

## Responsibilities
- Owns the definition of Disaster Cards
- Owns bonus conditions and rewards as part of a Disaster Card
- Owns domain enums (Character, PodVehicle, Thunderbird)
- Generates opaque bonus identity keys for external consumers

## Non-responsibilities
- Does not calculate rescue targets
- Does not interpret applied bonuses
- Does not depend on Rules
