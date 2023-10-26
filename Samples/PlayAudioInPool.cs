using System.Collections;
using System.Collections.Generic;
using AudioControlling;
using UnityEngine;

public class PlayAudioInPool : MonoBehaviour
{
    [AudioID] [SerializeField] private int _audioID;

    private AudioSource _source;

    public void Play()
    {
        _source = AudioManager.Instance.PlayInPool(_audioID);
    }

    public void Stop()
    {
        AudioManager.Instance.StopPooledAudioSource(_source);
    }
}
