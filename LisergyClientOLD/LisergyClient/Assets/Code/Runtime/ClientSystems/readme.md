# Client Systems

Game-specific behaviours and listeners to react to what happens on the game and display on screen the view.

## Structure & Naming Conventions

- EntityListeners: Responsible for listening to game/sdk events to update UI or Views
- EntityView: Represents the visualization of an entity
- MonoBehaviour: Unity specific behaviour to hook update calls
- Component: A reference client-only component to be used only for display purposes
- UI/
   Addressables/ - Holds screens uxmls
   Resources/    - Holds widget uxmls

   Screen:       - Represents a specific game screen that will be loaded in its separate UXML
   Widget:       - Represents a uxml visual element that can be reused and re-attached elsewhere
- 