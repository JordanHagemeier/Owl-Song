using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientTrigger : MonoBehaviour
{
    [SerializeField] private AudioTransitionInfo info;

    private void Start()
    {
        if(gameObject.GetComponent<AudioTransitionInfo>() != null)
        {
            info = gameObject.GetComponent<AudioTransitionInfo>();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Singletons.ambientSoundManager.TryStartingTransition(info);
    }
}
