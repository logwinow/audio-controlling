using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AudioControlling;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioSettingsManagerSO))]
public class AudioSettingsManagerSOEditor : Editor
{
    private SerializedProperty _audioTrackSettingsProp;
    
    private void OnEnable()
    {
        _audioTrackSettingsProp = serializedObject.FindProperty("_audioTrackSettings");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var prevCount = _audioTrackSettingsProp.arraySize;
        EditorGUILayout.PropertyField(_audioTrackSettingsProp, true);
        var newCount = _audioTrackSettingsProp.arraySize;

        if (newCount > prevCount)
        {
            SetDefaultValues(_audioTrackSettingsProp.GetArrayElementAtIndex(_audioTrackSettingsProp.arraySize - 1));
            
            if (newCount > 1)
            {
                if (!TryFillGaps())
                {
                    _audioTrackSettingsProp
                            .GetArrayElementAtIndex(_audioTrackSettingsProp.arraySize - 1).FindPropertyRelative("_id")
                            .intValue =
                        _audioTrackSettingsProp.GetArrayElementAtIndex(_audioTrackSettingsProp.arraySize - 2)
                            .FindPropertyRelative("_id").intValue + 1;
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private bool TryFillGaps()
    {
        var ids = Enumerable
            .Range(0, _audioTrackSettingsProp.arraySize - 1)
            .Select(index =>
                _audioTrackSettingsProp.GetArrayElementAtIndex(index).FindPropertyRelative("_id").intValue)
            .OrderBy(id => id).ToList();

        var size = _audioTrackSettingsProp.arraySize;
        
        for (int i = 0; i < ids.Last(); i++)
        {
            if (!ids.Contains(i))
            {
                _audioTrackSettingsProp
                        .GetArrayElementAtIndex(size - 1)
                        .FindPropertyRelative("_id").intValue = i;
                return true;
            }
        }

        return false;
    }

    private void SetDefaultValues(SerializedProperty trackSettingsProp)
    {
        trackSettingsProp.FindPropertyRelative("_volumeScale").floatValue = 1f;
        trackSettingsProp.FindPropertyRelative("_pitchMin").floatValue = 0.9f;
        trackSettingsProp.FindPropertyRelative("_pitchMax").floatValue = 1.1f;
    }
}
