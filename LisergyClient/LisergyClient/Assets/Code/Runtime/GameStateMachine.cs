using Assets.Code.Assets.Code.Assets;
using Assets.Code.Assets.Code.Runtime.UIScreens;
using Assets.Code.Assets.Code.UIScreens.Base;
using ClientSDK;
using ClientSDK.SDKEvents;
using Cysharp.Threading.Tasks;
using Game.Engine.DataTypes;
using Game.Engine.Events.Bus;
using Game.Systems.Battle;
using Game.Systems.Map;
using GameAssets;
using Stateless;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Assets.Code.Assets.Code
{
    /// <summary>
    /// Game state machine. 
    /// Should only be used for UI transitions and not logic.
    /// </summary>
    public class GameStateMachine : IEventListener
    {
        private enum State { Boot, Login, MapView, Battle };
        private enum Trigger { LoggedIn, LocalBattleStart, LocalBattleFinish };
        private StateMachine<State, Trigger> _stateMachine;
        private IUiService _screens;
        private IClientSdk _client;

        public GameStateMachine(IClientSdk client)
        {
            _client = client;
            _screens = UnityServicesContainer.Resolve<IUiService>();

            _stateMachine = new StateMachine<State, Trigger>(State.Login);

            _stateMachine.Configure(State.Login)
                .OnActivate(OnEnterLoginState)
                .OnExit(OnLeaveLoginState)
                .Permit(Trigger.LoggedIn, State.MapView);

            _stateMachine.Configure(State.MapView)
                .Permit(Trigger.LocalBattleStart, State.Battle)
                .OnExit(OnLeaveMapState)
                .OnEntry(() => OnEnterMapState().Forget());

            _stateMachine.Configure(State.Battle)
                .Permit(Trigger.LocalBattleFinish, State.MapView)
                .OnExit(OnEnterBattleState)
                .OnEntry(OnLeaveBattleState);

            AddListeners();
            _stateMachine.Activate();
        }

        private void AddListeners()
        {
            _client.ClientEvents.On<GameStartedEvent>(this, e => 
                _stateMachine.Fire(Trigger.LoggedIn));
        }

        private async UniTaskVoid OnEnterMapState()
        {
            await _client.UnityServices().Assets.LoadScene(SceneAsset.Map);
            _screens.Open<GameHudScreen>();
            _client.Network.SendToServer(new JoinWorldMapCommand());
        }

        private void OnLeaveMapState()
        {
            _client.UnityServices().Assets.UnloadScene(SceneAsset.Map);
            _screens.Close<GameHudScreen>();
        }

        private void OnLeaveLoginState()
        {
            _screens.Close<LoginScreen>();
        } 
        private void OnEnterLoginState()
        {
            _client.UnityServices().Assets.LoadScene(SceneAsset.Login);
            _screens.Open<LoginScreen>();
            AssetPreloader.StartPreload(_client, GameDataTest.TestSpecs.Generate()).Forget();
        }

        private void OnEnterBattleState() { }

        private void OnLeaveBattleState()
        {
            _screens.Close<BattleScreen>();
            var transition = _screens.Get<TransitionScreen>();
            if (transition != null)
                transition.CloseTransition();
        }

        private void OnBattleStartEvent(GameId battleId, BattleTeam attacker, BattleTeam defender)
        {
            Debug.Log("Battle start event received on state machine");
            if (attacker.OwnerID.IsMine())
            {
                _stateMachine.Fire(Trigger.LocalBattleStart);
                var transition = _screens.Open<TransitionScreen>();
                _ = transition.RunWhenScreenFilled(() =>
                {
                    _screens.Open<BattleScreen>(new BattleScreenParam()
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
