### Authserver
- Account ban/suspensions (time expiration, status, action)
- Account email address (promotions, new releases, surveys, login)

### Realmserver
- Proper player logout
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
- Functions that form packets in one location
	- i,e: "PacketManager.SendChat(string message, string fromId, ???)"
