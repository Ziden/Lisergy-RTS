using ClientSDK.Services;
using GameData.Specs;
using Godot;
using GodotClient;
using GodotClient.Services;
using LisergyGodotClient.Src;

namespace LisergyGodotClient.Systems.Login;

public partial class LoginScreen : GameUi
{
	private IAccountModule _accounts;
	private Button _loginButton;
	[Export] private NodePath _loginButtonPath;
	private LineEdit _passwordField;
	[Export] private NodePath _passwordFieldPath;
	private LineEdit _usernameField;
	[Export] private NodePath _usernameFieldPath;

	public override ArtSpec GetArt()
	{
		return "res://Content/UI/Screens/LoginScreen.tscn";
	}

	public override void _Ready()
	{
		_usernameField = GetNode<LineEdit>(_usernameFieldPath);
		_passwordField = GetNode<LineEdit>(_passwordFieldPath);
		_loginButton = GetNode<Button>(_loginButtonPath);
		_loginButton.ButtonDown += OnLoginButtonPressed;
		_accounts = ClientServices.ServerSdk.Server.Account;

		if (MainNode.OFFLINE_MODE) OnLoginButtonPressed();
	}

	private void OnLoginButtonPressed()
	{
		var username = _usernameField.Text;
		var password = _passwordField.Text;
		_accounts.SendAuthenticationPacket(username, password);
	}
}