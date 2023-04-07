using Assets.Code.Assets.Code.UIScreens;
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
        private enum State { Login, GameScreen };
        private enum Trigger { LoggedIn };

        private StateMachine<State, Trigger> _stateMachine;
        private IScreenService _screens;

        public GameStateMachine()
        {
            _screens = ServiceContainer.Resolve<IScreenService>();

            _stateMachine = new StateMachine<State, Trigger>(State.Login);
            _stateMachine.Configure(State.Login)
                .OnActivate(OnLogin)
                .Permit(Trigger.LoggedIn, State.GameScreen);

            _stateMachine.Configure(State.GameScreen)
                .OnActivate(OnGameScreen);

            AddListeners();
            _stateMachine.Activate();
        }

        private void AddListeners()
        {
            ClientEvents.OnPlayerLogin += e => _stateMachine.Fire(Trigger.LoggedIn);
        }

        private void OnGameScreen()
        {
            _screens.Open<PartySelectbar>();
        }

        private void OnLogin()
        {
            Debug.Log("Login Screen");
            _screens.Open<LoginScreen>();
           
        }
    }
}
