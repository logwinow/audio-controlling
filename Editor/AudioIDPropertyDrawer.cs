using System.Collections;
using System.Collections.Generic;
using AudioControlling;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AudioIDAttribute))]
public class AudioIDPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.Integer)
        {
            EditorGUI.HelpBox(position, "Use with int!", MessageType.Error);
            return;
        }

        property.intValue =
            new EditorGUIPopup<AudioTrackSettings>(label, position,
                    AudioSettingsManagerSO.Instance.AudioTrackSettings)
                .SetSelectedIndex(audioTrack => audioTrack.ID == property.intValue)
                .SetOptionsNames(track => $"{track.Title} ({track.ID})")
                .AddOption(new Separator())
                .AddOption(new SelectAssetCommand(AudioSettingsManagerSO.Instance))
                .DrawAndGet()?.ID ?? -1;
    }
}
