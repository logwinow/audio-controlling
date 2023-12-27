using System.Collections;
using System.Collections.Generic;
using AudioControlling;
using UnityEngine;

public class AudioManagerSettings : MonoBehaviour
{
    [SerializeField] [AudioTrackType] private string _audioType;

    public void SetVolume(float value)
    {
        AudioManager.Instance.SetVolume(_audioType, value);
    }

    public void Mute(bool value)
    {
        AudioManager.Instance.SetMute(_audioType, value);
    }
}
