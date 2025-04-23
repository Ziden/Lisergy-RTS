using ClientSDK.SDKEvents;
using Game.Engine.DataTypes;
using Game.Engine.Events.Bus;
using Game.Systems.Battle;
using LisergyGodotClient.Src.Systems.GameHud;
using LisergyGodotClient.Systems.Login;
using Stateless;

namespace LisergyGodotClient.Src;

public class GameStateMachine : IEventListener
{
	private readonly StateMachine<State, Trigger> _stateMachine;

	public GameStateMachine()
	{
		_stateMachine = new StateMachine<State, Trigger>(State.Login);
		_stateMachine.Configure(State.Login)
			.OnActivate(OnEnterLoginState)
			.OnExit(OnLeaveLoginState)
			.Permit(Trigger.LoggedIn, State.MapView);

		_stateMachine.Configure(State.MapView)
			.Permit(Trigger.LocalBattleStart, State.Battle)
			.OnExit(OnLeaveMapState)
			.OnEntry(OnEnterMapState);

		_stateMachine.Configure(State.Battle)
			.Permit(Trigger.LocalBattleFinish, State.MapView)
			.OnExit(OnEnterBattleState)
			.OnEntry(OnLeaveBattleState);

		ClientServices.Log.Info("Running StateMachine");
		AddListeners();
		_stateMachine.Activate();
	}

	private void AddListeners()
	{
		ClientServices.ServerSdk.ClientEvents.On<GameStartedEvent>(this,
			e => { _stateMachine.Fire(Trigger.LoggedIn); });
	}

	private void OnEnterMapState()
	{
		_ = ClientServices.Ui.Open<GameHudScreen>();
	}

	private void OnLeaveMapState()
	{
		ClientServices.Ui.Close<GameHudScreen>();
	}

	private void OnLeaveLoginState()
	{
		ClientServices.Ui.Close<LoginScreen>();
	}

	private void OnEnterLoginState()
	{
		_ = ClientServices.Ui.Open<LoginScreen>();
	}

	private void OnEnterBattleState()
	{
	}

	private void OnLeaveBattleState()
	{
	}

	private void OnBattleStartEvent(GameId battleId, BattleTeam attacker, BattleTeam defender)
	{
	}

	private enum State
	{
		Boot,
		Login,
		MapView,
		Building,
		Battle
	}

	private enum Trigger
	{
		LoggedIn,
		LocalBattleStart,
		LocalBattleFinish
	}
}