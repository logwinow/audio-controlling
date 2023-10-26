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
        [SerializeField] private AudioSource _audioSource;

        private void Start()
        {
            _warmed = true;
        }

        public void Play()
        {
            if (!_warmed)
                return;
            
            if (_audioSource)
                AudioManager.Instance.Play(_audioID, _audioSource);
            else
                AudioManager.Instance.Play(_audioID);
        }
    }
}