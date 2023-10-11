using Assets.Code.Assets.Code.Runtime.UIScreens;
using Assets.Code.Assets.Code.UIScreens;
using Assets.Code.Assets.Code.UIScreens.Base;
using Assets.Code.UI;
using Game.Battle;
using Game.DataTypes;
using Stateless;
using UnityEngine;

namespace Assets.Code.Assets.Code
{
    /// <summary>
    /// Game state machine. 
    /// Should only be used for UI transitions and not logic.
    /// </summary>
    public class GameStateMachine
    {
        private enum State { Login, MapView, Battle };
        private enum Trigger { LoggedIn, LocalBattleStart, LocalBattleFinish };

        private StateMachine<State, Trigger> _stateMachine;
        private IScreenService _screens;

        public GameStateMachine()
        {
            _screens = ServiceContainer.Resolve<IScreenService>();

            _stateMachine = new StateMachine<State, Trigger>(State.Login);
            _stateMachine.Configure(State.Login)
                .OnActivate(OnEnterLoginState)
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
            ClientEvents.OnPlayerLogin += e =>
            {
                _stateMachine.Fire(Trigger.LoggedIn);
            };

            ClientEvents.OnBattleStart += OnBattleStartEvent;
        }

        private void OnEnterMapState()
        {
            Debug.Log("Entered Map State");
            _screens.Open<PartySelectbar>();
        }

        private void OnLeaveMapState()
        {
             Debug.Log("Left Map State");
            _screens.Close<PartySelectbar>();
        }

        private void OnEnterLoginState()
        {
            Debug.Log("Entering Login State");
            _screens.Open<LoginScreen>();
        }

        private void OnEnterBattleState()
        {
            Debug.Log("Entering Battle State");
        }

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
            var pl = MainBehaviour.LocalPlayer;
            if (pl.ViewBattles && attacker.OwnerID == pl.EntityId)
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
