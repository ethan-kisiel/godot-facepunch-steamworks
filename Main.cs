using System;
using facepunchsteamworkstest.core.domain.steam_multiplayer;
using Godot;
using Steamworks;

namespace facepunchsteamworkstest;

public partial class Main : Node2D
{
	private bool isServer = false;
	
	[Export]
	private PackedScene PlayerScene { get; set; }
	
	[Export]
	private ui.MenuUi MenuUi { get; set; }

	public override void _Ready()
	{
		try
		{
			SteamClient.Init(480, false);
			GD.Print($"Name: {SteamClient.Name}, SteamId: {SteamClient.SteamId}");
			GD.Print($"Steam Initialized: {SteamClient.IsValid}");
			GD.Print($"Logged On: {SteamClient.IsLoggedOn}");
			GD.Print($"My ID: {SteamClient.SteamId}");
		}
		catch (Exception e)
		{
			GD.PrintErr(e.StackTrace);
		}

		MenuUi.CreateServer += () =>
		{
			isServer = true;
			var peer = new SteamMultiplayerPeer();
			peer.CreateServer();

			Multiplayer.MultiplayerPeer = peer;
			
			SpawnPlayer(1);

			peer.PeerConnected += SpawnPlayer;
		};

		MenuUi.JoinServer += (steamId) =>
		{
			var id = new SteamId();
			id.Value = steamId;
			
			var peer = new SteamMultiplayerPeer();
			peer.CreateClient(id);
			Multiplayer.MultiplayerPeer = peer;
		};
	}

	public override void _Process(double delta)
	{
		SteamClient.RunCallbacks();
	}


	private void SpawnPlayer(long networkId)
	{
		var playerNode = PlayerScene.Instantiate<debug.multiplayer_debug.PlayerScene>();
		
		playerNode.SetName($"{networkId}");
		
		GD.Print($"Spawned player {networkId}");
		AddChild(playerNode);
	}
}