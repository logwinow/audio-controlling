using System.Collections;
using System.Collections.Generic;
using AudioControlling;
using UnityEngine;

public class AddListenerToAudioManager : MonoBehaviour
{
    public void AddListener()
    {
        AudioManager.Instance.AddListener();
    }
}
