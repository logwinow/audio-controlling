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
        [SerializeField] private ClassifiedAudioSource[] _audioSources;

        private Pool<AudioSource> _pool;
        private Transform _poolParent;

        protected override void OnAwake()
        {
            _poolParent = new GameObject("Pool").transform;
            _poolParent.SetParent(transform);
            
            _pool = new Pool<AudioSource>(
                () =>
                {
                    var audioSourceGO = new GameObject("Pool Audio Source");
                    audioSourceGO.transform.SetParent(_poolParent);
                    var audioSource = audioSourceGO.AddComponent<AudioSource>();
                    audioSource.playOnAwake = false;

                    return audioSource;
                },
                source =>
                {
                    source.Stop();
                    source.clip = null;
                },
                source =>
                {
                    return source.isPlaying;
                });
        }

        public AudioSource PlayInPool(int id)
        {
            var source = _pool.GetOrCreate();
            
            Play(id, source);

            return source;
        }

        public void StopPooledAudioSource(AudioSource audioSource)
        {
            _pool.Release(audioSource);
        }

        public void Play(int id, AudioSource audioSource)
        {
            Play(_audioTrackSettingsManager.Get(id), audioSource);
        }

        public void Play(AudioTrackSettings trackSettings, AudioSource audioSource)
        {
            audioSource.pitch = trackSettings.IsPitched
                ? Random.Range(trackSettings.PitchMin, trackSettings.PitchMax)
                : 1;

            if (trackSettings.PlayOneShot)
                audioSource.PlayOneShot(trackSettings.Clip, trackSettings.VolumeScale);
            else
            {
                audioSource.loop = trackSettings.Loop;
                audioSource.clip = trackSettings.Clip;
                audioSource.volume = trackSettings.VolumeScale;
                audioSource.Play();
            }
        }

        public void Play(AudioTrackSettings trackSettings)
        {
            var audioSource = GetSource(trackSettings.AudioSourceType);
            
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

        private AudioSource GetSource(AudioSourceType audioSourceType)
        {
            return _audioSources
                .First(classifiedAS => classifiedAS.AudioSourceType == audioSourceType)
                .AudioSource;
        }

        [Serializable]
        private class ClassifiedAudioSource
        {
            public AudioSource AudioSource;
            public AudioSourceType AudioSourceType;
        }
    }
}