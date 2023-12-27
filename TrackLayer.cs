using System.Collections;
using System.Collections.Generic;
using AudioControlling;
using DG.Tweening;
using UnityEngine;

public class TrackLayer
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
        get => _audioSource.mute;
        set => _audioSource.mute = value;
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
        _audioSource.volume = Partial * MaxVolume;
    }
}
