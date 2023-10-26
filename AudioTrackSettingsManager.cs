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
    public class AudioTrackSettingsManager : SingletonSO<AudioTrackSettingsManager>
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

        #if UNITY_EDITOR

        private const string AUDIOS_COUNT_PREF = "audios count";
        private const string LAST_ID = "audio last id";
        
        private void OnValidate()
        {
            if (UnityEditor.EditorPrefs.HasKey(AUDIOS_COUNT_PREF))
            {
                var count = UnityEditor.EditorPrefs.GetInt(AUDIOS_COUNT_PREF);

                if (count == _audioTrackSettings.Length) 
                    return;
                
                if (_audioTrackSettings.Length != 0 && _audioTrackSettings.Length > count)
                {
                    if (!UnityEditor.EditorPrefs.HasKey(LAST_ID))
                    {
                        UnityEditor.EditorPrefs.SetInt(LAST_ID, _audioTrackSettings.Max(setting => setting.ID));
                    }
                        
                    var id = UnityEditor.EditorPrefs.GetInt(LAST_ID) + 1;

                    var serializedObj = new UnityEditor.SerializedObject(this);
                    var lastElementProp = serializedObj
                        .FindProperty(nameof(_audioTrackSettings))
                        .GetArrayElementAtIndex(_audioTrackSettings.Length - 1);

                    lastElementProp.FindPropertyRelative("_id").intValue = id;
                        
                    UnityEditor.EditorPrefs.SetInt(LAST_ID, id);
                    UnityEditor.EditorPrefs.SetInt(AUDIOS_COUNT_PREF, _audioTrackSettings.Length);

                    serializedObj.ApplyModifiedProperties();
                }
                else
                    UnityEditor.EditorPrefs.SetInt(AUDIOS_COUNT_PREF, _audioTrackSettings.Length);
            }
            else
            {
                UnityEditor.EditorPrefs.SetInt(AUDIOS_COUNT_PREF, _audioTrackSettings.Length);
            }
        }
        
        #endif
    }
}