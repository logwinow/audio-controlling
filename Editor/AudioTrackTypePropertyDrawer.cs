using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AudioControlling;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AudioTrackTypeAttribute))]
public class AudioTrackTypePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.height = EditorGUIUtility.singleLineHeight;
        var types = AudioSettingsManagerSO.Instance.AudioTrackSettings
            .Where(track => track.HasType)
            .Select(track => track.AudioType);

        property.stringValue = new EditorGUIPopup<string>(label, position, types)
            .SetSelectedIndex(value => value == property.stringValue)
            .SetOptionsNames()
            .DrawAndGet();
    }
}
