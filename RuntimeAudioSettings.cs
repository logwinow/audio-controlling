using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace AudioControlling
{
    [Serializable]
    [JsonObject(MemberSerialization.Fields)]
    public class RuntimeAudioSettings
    {
        // [JsonConstructor]
        // public RuntimeAudioSettings()
        // {
        //
        // }
        public RuntimeAudioSettings(string audioType)
        {
            _audioType = audioType;
        }
        
        private string _audioType;
        private bool _mute;
        private float _volume = 1f;

        public string AudioType => _audioType;

        public bool Mute
        {
            get => _mute;
            set => _mute = value;
        }

        public float Volume
        {
            get => _volume;
            set => _volume = value;
        }
    }
}
