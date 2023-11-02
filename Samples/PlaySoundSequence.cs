using System;
using System.Collections;
using System.Collections.Generic;
using AudioControlling;
using UnityEngine;

namespace AudioControlling.Samples
{
    public class PlaySoundSequence : MonoBehaviour
    {
        [SerializeField] private AudioIDHelper[] _sequence;
        [SerializeField] private SourceController _source;

        private int _current = 0;

        public void Next()
        {
            if (_source)
                AudioManager.Instance.Play(_sequence[_current++].AudioID, _source);
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
}