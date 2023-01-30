using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBookPoolTrigger : MonoBehaviour
{
    [SerializeField] AudioBookPool m_OwnAudiobookPool;

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            m_OwnAudiobookPool.TryPlayingNextAudioClip();
        }
       
    }
}
