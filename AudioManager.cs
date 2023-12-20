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
        [SerializeField] private AudioSettingsManagerSO _audioSettingsManager;
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
            Play(_audioSettingsManager.Get(id), source);
        }

        public void Play(AudioTrackSettings trackSettings, SourceController source)
        {
            if (trackSettings.DontPlayOnCollision)
            {
                var currentPlayingSources = _pool.GetUnavailable();
                var hasCollision = currentPlayingSources.Any(s =>
                {
                    if (s.TrackSettings == trackSettings)
                    {
                        Debug.Log($"{nameof(AudioManager)}/{nameof(Play)}: collision detected with {trackSettings.Clip.name}. It is already playing");
                        return true;
                    }

                    if (!trackSettings.PlayOneShot || !s.TrackSettings.PlayOneShot)
                        return false;

                    if (!string.IsNullOrEmpty(trackSettings.GroupTag) &&
                        s.TrackSettings.GroupTag == trackSettings.GroupTag)
                    {
                        Debug.Log($"{nameof(AudioManager)}/{nameof(Play)}: collision detected with {trackSettings.Clip.name}'s group '{trackSettings.GroupTag}'. It's group already playing");
                        return true;
                    }

                    return false;
                });
                
                if (hasCollision)
                {
                    _pool.Release(source);
                    
                    Debug.Log($"{nameof(AudioManager)}/{nameof(Play)}: collision detected with {trackSettings.Clip.name}. It wouldn't be played");
                    
                    return;
                }
            }
            
            Debug.Log($"{nameof(AudioManager)}/{nameof(Play)}: play track {trackSettings.Clip.name}");
            
            source.Play(trackSettings);
        }

        public void Play(AudioTrackSettings trackSettings)
        {
            var audioSource = GetSource();
            
            Play(trackSettings, audioSource);
        }
        
        public void Play(int id)
        {
            if (!_audioSettingsManager)
            {
                Debug.LogError($"You need to set {nameof(_audioSettingsManager)} of {name}/{nameof(AudioManager)}");
                return;
            }
            
            var trackSettings = _audioSettingsManager.Get(id);

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

        public SourceController GetSource()
        {
            return _pool.GetOrCreate();
        }

        public bool TryGetSource(int audioTrackID, out SourceController source)
        {
            source = _pool.GetUnavailable().FirstOrDefault(s => s.TrackSettings.ID == audioTrackID);

            return source;
        }
    }
}