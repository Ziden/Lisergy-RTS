using ClientSDK.Services;
using GameData.Specs;
using Godot;
using GodotClient;
using GodotClient.Services;
using LisergyGodotClient.Src;

namespace LisergyGodotClient.Systems.Login
{
	public partial class LoginScreen : GameUi
	{
		[Export] private NodePath _usernameFieldPath;
		[Export] private NodePath _passwordFieldPath;
		[Export] private NodePath _loginButtonPath;

		private IAccountModule _accounts;
		private LineEdit _usernameField;
		private LineEdit _passwordField;
		private Button _loginButton;

		public override ArtSpec GetArt() => "res://Content/Screens/LoginScreen.tscn";

		public override void _Ready()
		{
			_usernameField = GetNode<LineEdit>(_usernameFieldPath);
			_passwordField = GetNode<LineEdit>(_passwordFieldPath);
			_loginButton = GetNode<Button>(_loginButtonPath);
			_loginButton.ButtonDown += OnLoginButtonPressed;
			_accounts = ClientServices.ServerSdk.Server.Account;

			if(MainNode.OFFLINE_MODE)
			{
				OnLoginButtonPressed();
			}
		}

		private void OnLoginButtonPressed()
		{
			string username = _usernameField.Text;
			string password = _passwordField.Text;
			_accounts.SendAuthenticationPacket(username, password);
		}
	}
}
