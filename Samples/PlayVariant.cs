using System.Collections;
using System.Collections.Generic;
using AudioControlling;
using UnityEngine;

public class PlayVariant : MonoBehaviour
{
    [SerializeField] private string _groupTag;

    public void Play()
    {
        Debug.Log("Play Variant");
        
        AudioManager.Instance.Play(AudioTrackSettingsManager.Instance.GetTrackVariant(_groupTag));
    }
}
