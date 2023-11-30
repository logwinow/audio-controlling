using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioControlling
{
    [Serializable]
    public class AudioTrackSettings
    {
        [SerializeField] private AudioClip _clip;
        [SerializeField] private int _id;
        [SerializeField] private bool _isPitched;
        [SerializeField] private float _pitchMin = 0.9f;
        [SerializeField] private float _pitchMax = 1.1f;
        [SerializeField] private bool _playOneShot;
        [SerializeField] private float _volumeScale = 1f;
        [SerializeField] private bool _loop;
        [SerializeField] private string _groupTag;
        [SerializeField] private bool _dontPlayOnCollision;

        public AudioClip Clip => _clip;
        public int ID => _id;
        public string Title => _clip == null ? "null" : _clip.name;

        public bool IsPitched => _isPitched;

        public float PitchMin => _pitchMin;
        public float PitchMax => _pitchMax;

        public bool PlayOneShot => _playOneShot;

        public float VolumeScale => _volumeScale;

        public bool Loop => _loop;

        public string GroupTag => _groupTag;

        public bool IsInGroup => !string.IsNullOrEmpty(_groupTag);
        public bool DontPlayOnCollision => _dontPlayOnCollision;
    }
}