# Client Events

Declares what are the client-only specific events (for prediction for instance) can be used for inter-module communication.

There are 3 types of events
- Game Logic events fired from the compiled game logic
- Client SDK events, fired from the client SDK while integrating with game logic
- Client events, custom events that the client fires for internal handling.

This package is where the third type of events are located.