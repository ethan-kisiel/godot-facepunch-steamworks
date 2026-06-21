using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace facepunchsteamworkstest.VOIP.QueuedAudioStream;

[GlobalClass]
public partial class QueuedAudioStream : Node
{
	private AudioStreamPlayer2D _audioStreamPlayer;
	private AudioStreamGeneratorPlayback _audioStreamPlayback; 
	
	[Export(PropertyHint.NodeType, "AudioStreamPlayer,AudioStreamPlayer2D,AudioStreamPlayer3D")]
	public Node AudioStreamPlayer
	{
		get => _audioStreamPlayer;
		set => _audioStreamPlayer = (AudioStreamPlayer2D)value;
	}

	public readonly Queue<Vector2[]> AudioFramesQueue = new();
	private Array<Vector2> _currentAudioFrames = new();

	public override void _Ready()
	{
		if (GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
		{
			return;
		}
		_audioStreamPlayer.Play();
	}
    
	public override void _PhysicsProcess(double delta)
	{
		if (GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
		{
			return;
		}
		
		_audioStreamPlayback = 
			(AudioStreamGeneratorPlayback)_audioStreamPlayer.GetStreamPlayback();

		if (_currentAudioFrames.Count == 0 && AudioFramesQueue.Count > 0)
		{
			_currentAudioFrames = new Array<Vector2>(AudioFramesQueue.Dequeue());
		}
        
		if (_currentAudioFrames.Count > 0)
		{
			var availableFrames = _audioStreamPlayback.GetFramesAvailable();
			if (availableFrames == 0)
			{
				return;
			}
            
			if (availableFrames >= _currentAudioFrames.Count)
			{
				_audioStreamPlayback.PushBuffer(_currentAudioFrames.ToArray());
				_currentAudioFrames.Clear();
			}
			else
			{
				Array<Vector2> currentAudioFrames = new();
				for (int i = 0; i < availableFrames; i++)
				{
					currentAudioFrames.Add(_currentAudioFrames[0]);
					_currentAudioFrames.RemoveAt(0);
				}
                
				_audioStreamPlayback.PushBuffer(currentAudioFrames.ToArray());
			}
		}
	}
}