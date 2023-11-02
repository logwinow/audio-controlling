using System.Collections;
using System.Collections.Generic;
using AudioControlling;
using UnityEngine;

namespace AudioControlling
{
    public class AddListenerToAudioManager : MonoBehaviour
    {
        public void AddListener()
        {
            AudioManager.Instance.AddListener();
        }
    }
}