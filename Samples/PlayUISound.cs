using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AudioControlling.Samples
{
    public class PlayUISound : MonoBehaviour, IPointerClickHandler
    {
        [AudioID]
        [SerializeField] private int _audioClipID;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            AudioManager.Instance.Play(_audioClipID);
        }
    }
}