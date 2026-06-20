using Godot;

namespace facepunchsteamworkstest.VOIP.MicrophoneStreamPlayer;

public partial class MicrophoneStreamPlayer : AudioStreamPlayer
{
    [Signal]
    public delegate void MicrophoneDataEmittedEventHandler(Vector2[] data);

    [Export(PropertyHint.Range, "1,1024")] public int FrameBuffer { get; set; } = 512;
    [Export] public bool IsDebug { get; set; }
    

    private AudioEffectCapture _audioCapture;
    private bool _isPlaying = false;
    
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
        
        FrameBuffer = _audioCapture.GetFramesAvailable();
        if (_audioCapture.CanGetBuffer(FrameBuffer))
        {
            var buffer = _audioCapture.GetBuffer(FrameBuffer);

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i].Y = buffer[i].X;
            }
            
            Rpc(nameof(SendMicrophoneData), buffer);
            
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
    
    /*
    [Rpc(TransferMode = MultiplayerPeer.TransferModeEnum.UnreliableOrdered, CallLocal = false, TransferChannel = 10)]
    private void SendMicrophoneData(byte[] data, int length)
    {
        var audioBuffer = new NetworkedAudioBuffer(CompressionUtil.Decompress(data, length));
        
        EmitSignal(SignalName.MicrophoneDataEmitted, audioBuffer.Frames);
    }
    */
    
    [Rpc(TransferMode = MultiplayerPeer.TransferModeEnum.UnreliableOrdered, CallLocal = false, TransferChannel = 0)]
    private void SendMicrophoneData(Vector2[] data)
    {
        EmitSignal(SignalName.MicrophoneDataEmitted, data);
    }
}