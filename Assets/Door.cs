using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] AudioSource doorSource;
    [SerializeField] AudioClip openClip;
    [SerializeField] AudioClip closeClip;

    void PlayOpenSound()
    {
        doorSource.clip = openClip;
        doorSource.Play();
    }

    void PlayCloseSound()
    {
        doorSource.clip = closeClip;
        doorSource.Play();
    }
}
