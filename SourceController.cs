using System;
using System.Collections;
using System.Collections.Generic;
using AudioControlling;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class SourceController : MonoBehaviour
{
    private AudioSource _source;
    private Coroutine _waitForEndCoroutine;

    public bool IsAvailable { get; private set; } = true;
    public bool IsPitched { get; private set; }

    public float Volume
    {
        get => _source.volume;
        set => _source.volume = value;
    }

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    public void Play(AudioTrackSettings trackSettings)
    {
        ResetPitch();
        Volume = 1;
        _source.loop = false;

        if (trackSettings.PlayOneShot)
        {
            if (trackSettings.IsPitched)
                SetPitch(Random.Range(trackSettings.PitchMin, trackSettings.PitchMax));
            
            _source.PlayOneShot(trackSettings.Clip, trackSettings.VolumeScale);
            
            WaitForPlayOneShot(trackSettings.Clip);
        }
        else
        {
            _source.loop = trackSettings.Loop;
            Volume = trackSettings.VolumeScale;
            
            _source.clip = trackSettings.Clip;
            _source.Play();
        }
    }

    public void Stop()
    {
        _source.Stop();
        _source.clip = null;
        ResetPitch();
        if (_waitForEndCoroutine != null)
            StopCoroutine(_waitForEndCoroutine);
        IsAvailable = true;
    }

    private void WaitForPlayOneShot(AudioClip clip)
    {
        IsAvailable = false;

        _waitForEndCoroutine = StartCoroutine(WaitForEndCoroutine(clip));
    }

    private void ResetPitch()
    {
        SetPitch(1);

        IsPitched = false;
    }

    private void SetPitch(float pitch)
    {
        _source.pitch = pitch;

        IsPitched = true;
    }

    private void Reset()
    {
        _source = GetComponent<AudioSource>();
        _source.playOnAwake = false;
    }

    private IEnumerator WaitForEndCoroutine(AudioClip clip)
    {
        var timeStart = 0f;

        while (timeStart < clip.length)
        {
            timeStart += Time.deltaTime;

            yield return null;
        }

        IsAvailable = true;
    }
}
