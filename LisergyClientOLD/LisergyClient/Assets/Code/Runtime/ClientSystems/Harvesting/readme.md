# Client Systems - Harvesting

While on game logic harvesting stops and resources are transferred to cargo only when harvesting stops, on client things are different.

To have the "real-time" flow-ish aspect, on client harvesting is predicted.

When a player starts to harvest, a `HarvestingPredictionComponent` is added to its entity which controls a UniTask that handles predicting how much of the harvest should
have been made to this stage.