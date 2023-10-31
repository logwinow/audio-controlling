using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioControlling.Samples
{
    public class PlaySoundOnEvent : MonoBehaviour
    {
        [SerializeField] private bool _warmed = true;
        [AudioID] [SerializeField] private int _audioID;
        [SerializeField] private SourceController _source;

        private void Start()
        {
            _warmed = true;
        }

        public void Play()
        {
            if (!_warmed)
                return;
            
            if (_source)
                AudioManager.Instance.Play(_audioID, _source);
            else
                AudioManager.Instance.Play(_audioID);
        }
    }
}