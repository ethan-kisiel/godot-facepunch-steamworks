using System;
using facepunchsteamworkstest.VOIP.VoipTalker;
using Godot;
using Steamworks;

namespace facepunchsteamworkstest.debug.multiplayer_debug;

public partial class PlayerScene : Node2D
{

	private VoipTalker _voipTalker;
	
	[Export] public Label NameLabel;
	[Export] public Node VoipTalker
	{
		get => _voipTalker;
		set => _voipTalker = (VoipTalker)value;	
	}

	public override void _EnterTree()
	{
		SetMultiplayerAuthority(Int32.Parse(Name));
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (IsMultiplayerAuthority())
		{
			NameLabel.Text = SteamClient.Name;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (GetMultiplayerAuthority() != Multiplayer.GetUniqueId()) return;
		
		if (Input.IsActionJustPressed("push_to_talk"))
		{
			_voipTalker.SetVoiceEnabled(true);
		}

		if (Input.IsActionJustReleased("push_to_talk"))
		{
			_voipTalker.SetVoiceEnabled(false);
		}
		
		Vector2I input = new Vector2I();
		if (Input.IsActionPressed("move_up"))
		{
			input.Y = int.Clamp(input.Y - 1, -1, 1);
		}
		if (Input.IsActionPressed("move_down"))
		{
			input.Y = int.Clamp(input.Y + 1, -1, 1);
		}
		
		if (Input.IsActionPressed("move_left"))
		{
			input.X = int.Clamp(input.X - 1, -1, 1);
		}
		if (Input.IsActionPressed("move_right"))
		{
			input.X = int.Clamp(input.X + 1, -1, 1);
		}
		
		Rpc(nameof(MovePlayer), GlobalPosition + input);
	}


	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable, TransferChannel=0)]
	public void MovePlayer(Vector2I newPosition)
	{
		GlobalPosition = newPosition;
	}
}