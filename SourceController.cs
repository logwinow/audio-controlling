using System;
using System.Collections;
using System.Collections.Generic;
using AudioControlling;
using DG.Tweening;
using DiractionTeam.AdditionalLinq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace AudioControlling
{
    [RequireComponent(typeof(AudioSource))]
    public class SourceController : MonoBehaviour
    {
        private AudioSource _source;
        private Coroutine _processTrackCoroutine;
        private Coroutine _waitForPlayOneShotCoroutine;
        private Tween _fadeTween;
        private List<TrackLayer> _layers = null;

        public bool IsAvailable => TrackSettings == null;
        public bool IsPitched => Pitch != 1;
        public AudioTrackSettings TrackSettings { get; private set; }

        public float Volume
        {
            get => _source.volume;
            set
            {
                _source.volume = value;
                _layers?.ForEach(l => l.MaxVolume = value);
            }
        }

        public float Pitch
        {
            get => _source.pitch;
            set
            {
                _source.pitch = value;
                _layers?.ForEach(l => l.Pitch = value);
            }
        }

        public bool Loop
        {
            get => _source.loop;
            set
            {
                _source.loop = value;
                _layers?.ForEach(l => l.Loop = value);
            }
        }

        private class TrackLayer
        {
            public TrackLayer(AudioSource audioSource, AudioTrackSettings trackSettings)
            {
                _audioSource = audioSource;
                _trackSettings = trackSettings;
                
                Partial = 0;
                MaxVolume = trackSettings.VolumeScale;
                
                UpdateVolume();
            }

            private AudioSource _audioSource;
            private AudioTrackSettings _trackSettings;
            private float _partial;
            private float _maxVolume;
            private bool _mute;
            private Tween _fadeTween;

            private float Partial
            {
                get => _partial;
                set
                {
                    _partial = value;

                    UpdateVolume();
                }
            }

            public float MaxVolume
            {
                get => _maxVolume;
                set
                {
                    _maxVolume = value;
                    
                    UpdateVolume();
                }
            }

            public float Pitch
            {
                get => _audioSource.pitch;
                set => _audioSource.pitch = value;
            }

            public bool Loop
            {
                get => _audioSource.loop;
                set => _audioSource.loop = value;
            }

            public bool Mute
            {
                get => _mute;
                set
                {
                    _mute = value;
                    
                    UpdateVolume();
                }
            }
            
            public void Destroy()
            {
                Object.Destroy(_audioSource.gameObject);
            }

            public void Play()
            {
                _audioSource.Play();
            }

            public void FadeIn()
            {
                if (Partial == 1)
                    return;
                
                DoFade(1f, _trackSettings.FadeInDuration);
            }

            public void FadeOut()
            {
                if (Partial == 0)
                    return;
                
                DoFade(0, _trackSettings.FadeOutDuration);
            }

            private void DoFade(float endValue, float duration)
            {
                _fadeTween?.Kill();
                _fadeTween = DOTween.To(() => Partial, value =>
                {
                    Partial = value;
                    UpdateVolume();
                }, endValue, duration);
            }

            private void UpdateVolume()
            {
                if (Mute)
                {
                    _audioSource.volume = 0;
                    return;
                }

                _audioSource.volume = Partial * MaxVolume;
            }
        }

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
        }

        public void Play(AudioTrackSettings trackSettings)
        {
            TrackSettings = trackSettings;

            if (!trackSettings.PlayOneShot && trackSettings.HasLayers)
            {
                _layers = new();
                
                trackSettings.Layers.ForEach(clip =>
                {
                    var layerAudioSource = new GameObject($"Layer {clip.name}").AddComponent<AudioSource>();
                    layerAudioSource.playOnAwake = false;
                    layerAudioSource.transform.SetParent(transform);
                    layerAudioSource.clip = clip;
                    
                    _layers.Add(new TrackLayer(layerAudioSource, trackSettings));
                });
            }

            Pitch = 1f;
            Volume = 1f;
            Loop = false;

            if (trackSettings.PlayOneShot)
            {
                if (trackSettings.IsPitched)
                    Pitch = Random.Range(trackSettings.PitchMin, trackSettings.PitchMax);

                _source.PlayOneShot(trackSettings.Clip, trackSettings.VolumeScale);

                WaitForPlayOnShot();
            }
            else
            {
                Loop = trackSettings.Loop;
                Volume = trackSettings.VolumeScale;

                _source.clip = trackSettings.Clip;
                _Play();
                
                ProcessTrack();
            }
        }

        private void _Play()
        {
            _source.Play();
            
            _layers?.ForEach(l => l.Play());
        }

        public void Stop()
        {
            if (_layers != null)
            {
                _layers.ForEach(l => l.Destroy());
                _layers = null;
            }
            
            _source.clip = null;
            Pitch = 1f;
            if (_processTrackCoroutine != null)
                StopCoroutine(_processTrackCoroutine);
            if (_waitForPlayOneShotCoroutine != null)
                StopCoroutine(_waitForPlayOneShotCoroutine);
            TrackSettings = null;
            _fadeTween?.Kill();
        }

        public void MuteLayers(bool value)
        {
            _layers?.ForEach(l => l.Mute = value);
        }

        public void FadeInLayers()
        {
            _layers?.ForEach(l => l.FadeIn());
        }

        public void FadeOutLayers()
        {
            _layers?.ForEach(l => l.FadeOut());
        }

        private void ProcessTrack()
        {
            _processTrackCoroutine = StartCoroutine(ProcessTrackCoroutine());
        }

        private void WaitForPlayOnShot()
        {
            _waitForPlayOneShotCoroutine = StartCoroutine(WaitForPlayOneShotCoroutine());
        }

        private IEnumerator WaitForPlayOneShotCoroutine()
        {
            yield return new WaitForSeconds(TrackSettings.Clip.length);

            TrackSettings = null;
        }

        private IEnumerator ProcessTrackCoroutine()
        {
            if (TrackSettings.Fade)
            {
                Volume = 0;
                
                DoFadeInTween();
                //Debug.Log($"Track {TrackSettings.Clip.name} is playing. Fade in started");
            }
            
            var fadeOut = false;
            var previousSourceTime = -0.1f;
            
            while (_source.time >= previousSourceTime && _source.time < TrackSettings.Clip.length)
            {
                //Debug.Log($"Track {TrackSettings.Clip.name} is playing. Progress: {_source.time:F1}/{TrackSettings.Clip.length:F1}");

                if (!fadeOut && TrackSettings.Fade && TrackSettings.FadeOutDuration > 0)
                {
                    var fadeStartTime = TrackSettings.Clip.length - TrackSettings.FadeOutDuration - 0.5f;

                    if (_source.time > fadeStartTime)
                    {
                        DoFadeOutTween();
                        //Debug.Log($"Track {TrackSettings.Clip.name} is playing. Fade out started");
                        
                        fadeOut = true;
                    }
                }

                previousSourceTime = _source.time;

                yield return null;
            }

            if (TrackSettings.Loop)
            {
                //Debug.Log($"Track {TrackSettings.Clip.name} has finished. It is loop and would be restarted");
                ProcessTrack();
                yield break;
            }

            Stop();
            
            //Debug.Log($"{nameof(SourceController)}/{nameof(ProcessTrackCoroutine)}: Coroutine has finished");
        }

        private void DoFadeInTween()
        {
            DoFadeTween(TrackSettings.VolumeScale, TrackSettings.FadeInDuration);
        }

        private void DoFadeOutTween()
        {
            DoFadeTween(0, TrackSettings.FadeOutDuration);
        }

        private void DoFadeTween(float endValue, float duration)
        {
            _fadeTween?.Kill();
            _fadeTween = DOTween.To(() => _source.volume, value => Volume = value, endValue, duration);
        }
        
        private void Reset()
        {
            _source = GetComponent<AudioSource>();
            _source.playOnAwake = false;
        }
    }
}