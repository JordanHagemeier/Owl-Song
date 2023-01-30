using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrasMovement : MonoBehaviour
{
    public Material[] materials;
    public Transform player;
    Vector3 playerPosition;


    void Start()
    {
        StartCoroutine(writeToMaterial());
    }

    IEnumerator writeToMaterial()
    {
        while(true)
        {
            playerPosition = player.transform.position;
            foreach (Material material in materials)
            {
                material.SetVector("_playerposition", playerPosition);
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
