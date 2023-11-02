using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DiractionTeam.General.Patterns;
using UnityEngine;
using Utils;

namespace AudioControlling
{
    [CreateAssetMenu(fileName = "Audio Settings Manager.asset", menuName = "Game/Audio/Create Audio Settings Manager")]
    public class AudioSettingsManagerSO : SingletonSO<AudioSettingsManagerSO>
    {
        [SerializeField] private AudioTrackSettings[] _audioTrackSettings;

        public AudioTrackSettings[] AudioTrackSettings => _audioTrackSettings;

        public AudioTrackSettings Get(int id)
        {   
            return _audioTrackSettings.First(settings => settings.ID == id);
        }

        public IEnumerable<AudioTrackSettings> GetGroup(string groupTag)
        {
            return _audioTrackSettings.Where(trackSettings => trackSettings.GroupTag == groupTag);
        }

        public AudioTrackSettings GetTrackVariant(string groupTag)
        {
            return GetGroup(groupTag).Random();
        }
    }
}