using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGameAfterCredit : MonoBehaviour
{
    [SerializeField] AudioSource creditsong;

    public void _playHumming()
    {
        creditsong.Play();
    }


    public void _QuitGameAfterCredit()
    {
        Application.Quit();
    }
   
}
