using Godot;
using System.Linq;

namespace facepunchsteamworkstest.VOIP.VoipTalker;

public partial class VoipTalker : Node
{
    [Export] public MicrophoneStreamPlayer.MicrophoneStreamPlayer MicrophoneStreamPlayer { get; set; }
    [Export] public QueuedAudioStream.QueuedAudioStream QueuedAudioStream { get; set; }

    public override void _Ready()
    {
        MicrophoneStreamPlayer.MicrophoneDataEmitted += 
            data => QueuedAudioStream.AudioFramesQueue.Enqueue(data.Select(val => new Vector2(val / 2, val / 2)).ToArray());
    }

    public void SetVoiceEnabled (bool enabled)
    {
        MicrophoneStreamPlayer.SetVoiceEnabled(enabled);
        GD.Print($"Set Voice Enabled: {enabled}");
    }
}