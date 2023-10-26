using System;
using System.Collections;
using System.Collections.Generic;
using AudioControlling;
using UnityEngine;

public class PlaySoundSequence : MonoBehaviour
{
    [SerializeField] private AudioIDHelper[] _sequence;
    [SerializeField] private AudioSource _audioSource;

    private int _current = 0;

    public void Next()
    {
        if (_audioSource)
            AudioManager.Instance.Play(_sequence[_current++].AudioID, _audioSource);
        else
            AudioManager.Instance.Play(_sequence[_current++].AudioID);

        if (_current >= _sequence.Length)
            _current = 0;
    }
    
    [Serializable]
    private class AudioIDHelper
    {
        [SerializeField] [AudioID] public int AudioID;
    }
}
