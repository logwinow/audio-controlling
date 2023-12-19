using System.Collections;
using System.Collections.Generic;
using AudioControlling;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AudioTrackSettings))]
public class AudioTrackSettingsPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.height = EditorGUIUtility.singleLineHeight;
        
        var idProp = property.FindPropertyRelative("_id");
        EditorGUI.HelpBox(position, "ID: " + idProp.intValue, MessageType.Info);
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        
        var clipProp = property.FindPropertyRelative("_clip");

        EditorGUI.PropertyField(position, clipProp);
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        if (!clipProp.objectReferenceValue)
        {
            return;
        }

        var playOneShotProp = property.FindPropertyRelative("_playOneShot");

        EditorGUI.PropertyField(position, playOneShotProp);
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        var volumeScaleProp = property.FindPropertyRelative("_volumeScale");
        EditorGUI.PropertyField(position, volumeScaleProp);
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        
        var dontPlayOnCollisionProp = property.FindPropertyRelative("_dontPlayOnCollision");
        EditorGUI.PropertyField(position, dontPlayOnCollisionProp);
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        
        if (playOneShotProp.boolValue)
        {
            var isPitchedProp = property.FindPropertyRelative("_isPitched");
            EditorGUI.PropertyField(position, isPitchedProp);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (isPitchedProp.boolValue)
            {
                position.x += 15;
                
                var pitchMinProp = property.FindPropertyRelative("_pitchMin");
                EditorGUI.PropertyField(position, pitchMinProp);
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                var pitchMaxProp = property.FindPropertyRelative("_pitchMax");
                EditorGUI.PropertyField(position, pitchMaxProp);
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                
                position.x -= 15;
            }
            
            var groupTagProp = property.FindPropertyRelative("_groupTag");
            EditorGUI.PropertyField(position, groupTagProp);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }
        else
        {
            var loopProp = property.FindPropertyRelative("_loop");
            EditorGUI.PropertyField(position, loopProp);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            
            var fadeProp = property.FindPropertyRelative("_fade");
            EditorGUI.PropertyField(position, fadeProp);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (fadeProp.boolValue)
            {
                position.x += 15;
                
                var fadeInDuration = property.FindPropertyRelative("_fadeInDuration");
                EditorGUI.PropertyField(position, fadeInDuration);
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                
                var fadeOutDuration = property.FindPropertyRelative("_fadeOutDuration");
                EditorGUI.PropertyField(position, fadeOutDuration);
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                
                position.x -= 15;
            }
            
            var hasLayersProp = property.FindPropertyRelative("_hasLayers");
            EditorGUI.PropertyField(position, hasLayersProp);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (hasLayersProp.boolValue)
            {
                var layersProp = property.FindPropertyRelative("_layers");
                EditorGUI.PropertyField(position, layersProp);
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // id + space
        
        var clipProp = property.FindPropertyRelative("_clip");
        height += EditorGUIUtility.singleLineHeight; // clip

        if (!clipProp.objectReferenceValue)
        {
            return height;
        }

        height += EditorGUIUtility.standardVerticalSpacing; // space
        
        var playOneShot = property.FindPropertyRelative("_playOneShot");
        height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // play one shot + space
        height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // volume + space
        height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // dont play on collision + space

        if (playOneShot.boolValue)
        {
            height += EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing; // isPitched + groupTag + space
            
            var isPitchedProp = property.FindPropertyRelative("_isPitched");

            if (isPitchedProp.boolValue)
            {
                height += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2; // min + max + space x2
            }
        }
        else
        {
            height += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 3; // loop + has layers + fade + space x3
            
            var fadeProp = property.FindPropertyRelative("_fade");

            if (fadeProp.boolValue)
            {
                height += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2; // fade in + fade out durations
            }
            
            var hasLayersProp = property.FindPropertyRelative("_hasLayers");

            if (hasLayersProp.boolValue)
            {
                height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_layers")); // layers
                
                height += EditorGUIUtility.standardVerticalSpacing; // space
            }
        }

        return height;
    }
}
