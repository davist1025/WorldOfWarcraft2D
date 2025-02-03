### Authserver
- Account ban/suspensions (time expiration, status, action)
- Account email address (promotions, new releases, surveys, login)

### Realmserver
- Character name verification (censored words/phrases, name in-use)
- Remove Tilesets from map data
- Proper player logout
- Attach TiledProcessor to the Entity instead of vice-versa
- Ensure security levels can use the level above/below them
	- i.e: Administrator (1) can use all Gamemaster (2) commands, but Gamemasters' cannot use Administrator commands.
- NPC systems
	- Targeting players
	- Behaviors
	- Dialogue
- Fix column data types for each Model

### Client
- Implement camera bounds/Tiled boundaries 
- Addon system

### Framework
*Includes any shared development tasks between either server and the client

- Implement Logging factory
- Realmserver flag usage (IsRestricted, etc)
- NPC flag usage (IsAggressive,)
- Prediction and reconciliation on input updates
