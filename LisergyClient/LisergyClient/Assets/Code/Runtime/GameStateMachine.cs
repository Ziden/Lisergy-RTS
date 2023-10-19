using Assets.Code.Assets.Code.Runtime.UIScreens;
using Assets.Code.Assets.Code.UIScreens;
using Assets.Code.Assets.Code.UIScreens.Base;
using Assets.Code.UI;
using ClientSDK;
using ClientSDK.SDKEvents;
using Game.Battle;
using Game.DataTypes;
using Game.Events.Bus;
using Stateless;
using UnityEngine;

namespace Assets.Code.Assets.Code
{
    /// <summary>
    /// Game state machine. 
    /// Should only be used for UI transitions and not logic.
    /// </summary>
    public class GameStateMachine : IEventListener
    {
        private enum State { Login, MapView, Battle };
        private enum Trigger { LoggedIn, LocalBattleStart, LocalBattleFinish };

        private StateMachine<State, Trigger> _stateMachine;
        private IScreenService _screens;
        private IGameClient _client;

        public GameStateMachine(IGameClient client)
        {
            _client = client;
            _screens = UnityServicesContainer.Resolve<IScreenService>();

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

            AddListeners();
            _stateMachine.Activate();
        }

        private void AddListeners()
        {
            _client.ClientEvents.Register<GameStartedEvent>(this, e => _stateMachine.Fire(Trigger.LoggedIn));
        }

        private void OnEnterMapState()
        {
            _screens.Open<PartySelectbar>();
        }

        private void OnLeaveMapState()
        {
            _screens.Close<PartySelectbar>();
        }

        private void OnLeaveLoginState() => _screens.Close<LoginScreen>();
        private void OnEnterLoginState() => _screens.Open<LoginScreen>();

        private void OnEnterBattleState() {}

        private void OnLeaveBattleState()
        {
            _screens.Close<BattleScreen>();
            var transition = _screens.Get<TransitionScreen>();
            if(transition != null)
                transition.CloseTransition();
        }

        private void OnBattleStartEvent(GameId battleId, BattleTeam attacker, BattleTeam defender)
        {
            Debug.Log("Battle start event received on state machine");
            if(attacker.OwnerID.IsMine())
            {
                _stateMachine.Fire(Trigger.LocalBattleStart);
                var transition = _screens.Open<TransitionScreen>();
                _ = transition.RunWhenScreenFilled(() => {             
                    _screens.Open<BattleScreen, BattleScreenSetup>(new BattleScreenSetup()
                    {
                        Attacker = attacker,
                        Defender = defender,
                        BattleId = battleId,
                        OnFinish = () =>
                        {
                            _stateMachine.Fire(Trigger.LocalBattleFinish);
                        }
                    });
                });
            }
        }
    }
}
