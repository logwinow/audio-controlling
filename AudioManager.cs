using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DiractionTeam.AdditionalLinq;
using DiractionTeam.Utils.Patterns;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AudioControlling
{
    public class AudioManager : SingletonMono<AudioManager>
    {
        [SerializeField] private AudioTrackSettingsManager _audioTrackSettingsManager;
        [SerializeField] private SourceController _sourcePrefab;

        private Pool<SourceController> _pool;

        protected override void OnAwake()
        {
            _pool = new Pool<SourceController>(
                () =>
                {
                    var source = Instantiate(_sourcePrefab, transform);

                    return source;
                },
                source =>
                {
                    source.Stop();
                },
                source =>
                {
                    return source.IsAvailable;
                });
        }

        public void Play(int id, SourceController source)
        {
            Play(_audioTrackSettingsManager.Get(id), source);
        }

        public void Play(AudioTrackSettings trackSettings, SourceController source)
        {
            source.Play(trackSettings);
        }

        public void Play(AudioTrackSettings trackSettings)
        {
            var audioSource = GetSource();
            
            Play(trackSettings, audioSource);
        }
        
        public void Play(int id)
        {
            var trackSettings = _audioTrackSettingsManager.Get(id);

            Play(trackSettings);
        }

        public void AddListener()
        {
            FindObjectsOfType<AudioListener>().ForEach(Destroy);
            gameObject.AddComponent<AudioListener>();
        }

        public void RemoveListener()
        {
            if (TryGetComponent<AudioListener>(out var listener))
            {
                Destroy(listener);
            }
        }

        private SourceController GetSource()
        {
            return _pool.GetOrCreate();
        }
    }
}