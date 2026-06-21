using Godot;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace facepunchsteamworkstest.VOIP.MicrophoneStreamPlayer;

public partial class MicrophoneStreamPlayer : AudioStreamPlayer
{
    [Signal]
    public delegate void MicrophoneDataEmittedEventHandler(float[] data);

    [Export(PropertyHint.Range, "1,1024")] public int FrameBuffer { get; set; } = 512;
    [Export] public bool IsDebug { get; set; }
    

    private AudioEffectCapture _audioCapture;
    private bool _isPlaying = false;

    private double _uptime = 0;
    private double _tickrate = 20;
    
    public override void _Ready()
    {
        if (GetMultiplayerAuthority() != Multiplayer.GetUniqueId())
        {
            return;
        }
        
        var index = AudioServer.GetBusIndex("Record");
        _audioCapture = (AudioEffectCapture)AudioServer.GetBusEffect(index, 0);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (GetMultiplayerAuthority() != Multiplayer.GetUniqueId() || !_isPlaying)
        {
            return;
        }
        _uptime += delta;

        FrameBuffer = _audioCapture.GetFramesAvailable();
        if (_audioCapture.CanGetBuffer(FrameBuffer) && _uptime > 1 / _tickrate)
        {
            var buffer = _audioCapture.GetBuffer(FrameBuffer);

            Rpc(nameof(SendMicrophoneData), buffer.Select(vec => vec.X).ToArray());
            _uptime = 0;
            
            _audioCapture.ClearBuffer();
        }
    }


    public void SetVoiceEnabled(bool enabled)
    {
        _isPlaying = enabled;

        if (enabled)
        {
            Play();
        }
        else
        {
            Stop();
        }
    }
    
    
    [Rpc(TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable, CallLocal = false, TransferChannel = 0)]
    private void SendMicrophoneData(float[] data)
    {
        EmitSignal(SignalName.MicrophoneDataEmitted, data);
    }
}