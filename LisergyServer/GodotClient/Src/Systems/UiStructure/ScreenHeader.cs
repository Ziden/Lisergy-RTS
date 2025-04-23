using System;
using Godot;

namespace LisergyGodotClient.Src.Systems.UiStructure;

public partial class ScreenHeader : Control
{
	[Export] public NodePath _backButton;

	private TextureButton _btn;
	private Label _lbl;
	[Export] public NodePath _title;

	public void SetData(string title, Action onBack)
	{
		_btn ??= GetNode<TextureButton>(_backButton);
		_lbl ??= GetNode<Label>(_title);
		_lbl.Text = title;
		_btn.Visible = onBack != null;
		if (onBack != null) _btn.Pressed += onBack;
	}
}