using System;
using System.Collections;
using System.Collections.Generic;
using DiractionTeam.AdditionalLinq;
using UnityEngine;

namespace AudioControlling.Samples
{
    public class PlaySoundOnEvent : MonoBehaviour
    {
        [SerializeField] private bool _warmed = true;
        [SerializeField] private bool _playOnStart = false;
        [SerializeField] private string _sourceName;
        [AudioID] [SerializeField] private int _audioID;
        [SerializeField] private SourceController _source;
        [SerializeField] private string _groupTag;

        public int AudioID => _audioID;

        protected virtual void Start()
        {
            _warmed = true;
            
            if (_playOnStart)
                Play();
        }

        public void Play()
        {
            if (!_warmed)
            {
                Debug.Log($"{nameof(PlaySoundOnEvent)}/{GetType().Name}/{name}/{nameof(Play)}: audio player isn't warmed yet");
                return;
            }

            var audioID = _audioID;

            if (!string.IsNullOrEmpty(_groupTag))
            {
                audioID = AudioSettingsManagerSO.Instance.GetTrackVariant(_groupTag).ID;
            }
            
            if (_source)
                AudioManager.Instance.Play(audioID, _source);
            else if (!string.IsNullOrEmpty(_sourceName))
            {
                AudioManager.Instance.GetSources(_sourceName).ForEach(source => source.FadeOutAndStop());
                var source = AudioManager.Instance.GetSource();
                source.SourceName = _sourceName;
                AudioManager.Instance.Play(audioID, source);
            }
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