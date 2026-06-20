using Godot;

namespace facepunchsteamworkstest.ui;

public partial class MenuUi : Control
{
	[Export] public LineEdit SteamIdEdit { get; set; }

	[Export] public Button CreateServerButton { get; set; }
	[Export] public Button JoinServerButton { get; set; }
	
	[Signal]
	public delegate void CreateServerEventHandler();
	[Signal]
	public delegate void JoinServerEventHandler(ulong steamId);
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CreateServerButton.ButtonDown += () =>
		{
			EmitSignal(SignalName.CreateServer);
            QueueFree();
        };
		
		JoinServerButton.ButtonDown += () =>
		{
			ulong.TryParse(SteamIdEdit.Text, out ulong steamId);
			
			if (steamId == 0)
			{
				return;
			}
			
			EmitSignal(SignalName.JoinServer, steamId);
			QueueFree();
		};
	}
}