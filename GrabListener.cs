using System;
using System.Collections;
using System.Collections.Generic;
using AudioControlling;
using UnityEngine;

public class GrabListener : MonoBehaviour
{
    [SerializeField] private bool _onAwake = true;
    
    private void Awake()
    {
        if (_onAwake)
            Grab();
    }

    public void Grab()
    {
        if (AudioManager.IsInitialized)
        {
            AudioManager.Instance.RemoveListener();
        }

        gameObject.AddComponent<AudioListener>();
    }
}
