### Authserver
- Account ban/suspensions (time expiration, status, action)
- Account email address (promotions, new releases, surveys, login)

### Realmserver
- Character name verification (censored words/phrases, name in-use)
- Remove Tilesets from map data
- Proper player logout
- Attach TiledProcessor to the Entity instead of vice-versa
- Add a function in TiledMapProcessor to add a new Entity to Creatures and set their collision layer(s)
- Ensure security levels can use the level above/below them
	- i.e: Administrator (1) can use all Gamemaster (2) commands, but Gamemasters' cannot use Administrator commands.
- NPC systems
	- Targeting players
	- Behaviors
	- Dialogue

### Client
- Implement camera bounds/Tiled boundaries 
- Addon system
- Add an 'Escape' menu

### Framework
*Includes any shared development tasks between either server and the client

- Add animated player characters
- Implement Logging factory
- Realmserver flag usage (IsRestricted, etc)
- NPC flag usage (IsAggressive,)
- NPCs should persist, visually, only on the map the local player exists, data should persist everywhere
	- See Devlog 1/26/25
- Prediction and reconciliation on input updates
