### Authserver
- Account ban/suspensions (time expiration, status, action)

### Realmserver
- Proper player logout
- NPC systems
	- Targeting players
	- Behaviors
	- Dialogue
- Fix column data types for each Model

### Client
- Implement camera bounds/Tiled boundaries 
- Addon system (this system is non-essential for PTR #1)

### Framework
*Includes any shared development tasks between either server and the client

- Implement Logging factory
- Realmserver flag usage (IsRestricted, etc)
- NPC flag usage (IsAggressive,)
- Prediction and reconciliation on input updates
- Functions that form packets in one location
	- i,e: "PacketManager.SendChat(string message, string fromId, ???)"
- Revamp the player chat
	- Formatting (color, text, etc)
	- GM interface
	- Channels (say, whisper, guild, etc)
