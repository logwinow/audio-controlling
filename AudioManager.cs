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
        private Pool<SourceController> _pool;
        private List<RuntimeAudioSettings> _audioSettings = new();
        
        [SerializeField] private AudioSettingsManagerSO _audioSettingsManager;
        [SerializeField] private SourceController _sourcePrefab;

        public List<RuntimeAudioSettings> AudioSettings => _audioSettings;
        public bool Initialized { get; private set; }

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

            Initialized = true;
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

            if (trackSettings.HasType && TryGetSettings(trackSettings.AudioType, out var settings))
            {
                source.ApplySettings(settings);
            }
            
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

        public IEnumerable<SourceController> GetSources(string sourceName)
        {
            return _pool.GetUnavailable().Where(s => s.SourceName == sourceName);
        }

        public bool TryGetSource(int audioTrackID, out SourceController source)
        {
            source = _pool.GetUnavailable().FirstOrDefault(s => s.TrackSettings.ID == audioTrackID);

            return source;
        }

        private bool TryGetSettings(string audioType, out RuntimeAudioSettings settings)
        {
            return _audioSettings.TryFind(s => s.AudioType == audioType, out settings);
        }

        private RuntimeAudioSettings GetSettings(string audioType)
        {
            if (!TryGetSettings(audioType, out var settings))
            {
                settings = new RuntimeAudioSettings(audioType);
            }

            return settings;
        }

        public void SetMute(string audioType, bool value)
        {
            var settings = GetSettings(audioType); 
            settings.Mute = value;
            
            ApplySettings(settings);
        }

        public void SetVolume(string audioType, float volume)
        {
            var settings = GetSettings(audioType); 
            settings.Volume = volume;
            
            ApplySettings(settings);
        }

        public void ApplySettings(RuntimeAudioSettings settings)
        {
            var oldSettings = _audioSettings.Find(s => s.AudioType == settings.AudioType);

            if (oldSettings != null)
            {
                _audioSettings.Remove(oldSettings);
            }
            
            _audioSettings.Add(settings);
            
            _pool.GetUnavailable().Where(s => s.TrackSettings.HasType).ForEach(s => s.ApplySettings(settings));
        }
    }
}