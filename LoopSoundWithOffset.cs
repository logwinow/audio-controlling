using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopSoundWithOffset : MonoBehaviour
{
    [SerializeField] private AudioSource _source1;
    [SerializeField] private AudioSource _source2;
    [SerializeField] private AudioClip _clip;
    [SerializeField] private float _offset;
    [SerializeField] private bool _playOnStart = true;

    private AudioSource CurrentSource { get; set; }

    private bool _started;

    private void Start()
    {
        if (_playOnStart)
            Play();
    }

    public void Play()
    {
        Play(_source1);

        _started = true;
    }

    private void Update()
    {
        if (!_started)
            return;
        
        if (CurrentSource.time > _clip.length - _offset)
        {
            Play(CurrentSource == _source1 ? _source2 : _source1);
        }
    }

    private void Play(AudioSource audioSource)
    {
        audioSource.clip = _clip;
        audioSource.Play();

        CurrentSource = audioSource;
    }
}
