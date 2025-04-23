using Godot;
using LisergyGodotClient.Src.Services.LisergyGodotClient.Src.Controllers;

namespace LisergyGodotClient.Src.Controllers;

public partial class CameraNode : Camera3D
{
	private GodotCameraInputService _input;

	public override void _UnhandledInput(InputEvent e)
	{
		if (_input == null) _input = (GodotCameraInputService) ClientServices.Input;
		if (_input == null) return;

		if (e is InputEventMouseMotion mouseMotion)
			_input.ReceiveDrag(mouseMotion.Position);
		else if (e is InputEventMouseButton mouseButton)
			if (mouseButton.ButtonIndex == MouseButton.Left)
			{
				if (mouseButton.Pressed)
					_input.ReceiveClickDown(mouseButton.Position);
				else
					_input.ReceiveClickUp(mouseButton.Position);
			}
	}
}