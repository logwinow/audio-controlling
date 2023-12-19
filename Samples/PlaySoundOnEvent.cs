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
        [SerializeField] private string _groupTag;

        private void Start()
        {
            _warmed = true;
        }

        public void Play()
        {
            if (!_warmed)
                return;

            var audioID = _audioID;

            if (!string.IsNullOrEmpty(_groupTag))
            {
                audioID = AudioSettingsManagerSO.Instance.GetTrackVariant(_groupTag).ID;
            }
            
            if (_source)
                AudioManager.Instance.Play(audioID, _source);
            else
                AudioManager.Instance.Play(audioID);
        }

        public void SetLayersMute(bool value)
        {
            if (AudioManager.Instance.TryGetSource(_audioID, out var source))
            {
                source.MuteLayers(value);
            }
        }

        public void FadeInLayers()
        {
            if (AudioManager.Instance.TryGetSource(_audioID, out var source))
            {
                source.FadeInLayers();
            }
        }
        
        public void FadeOutLayers()
        {
            if (AudioManager.Instance.TryGetSource(_audioID, out var source))
            {
                source.FadeOutLayers();
            }
        }
    }
}