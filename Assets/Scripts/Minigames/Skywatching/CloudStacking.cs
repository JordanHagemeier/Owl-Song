using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudStacking : MonoBehaviour
{
    public int horizontalStackSize = 20;
    public float cloudHeight;
    public Material cloudMaterial;
    public Mesh quadMesh;


    public int layer;
    public Camera camera;

    private Matrix4x4 matrix;

    private float offset;



    private void Update()
    {
        cloudMaterial.SetFloat("midYValue", transform.position.y);
        cloudMaterial.SetFloat("cloudHeight", cloudHeight);

        offset = cloudHeight / horizontalStackSize / 2f;
        Vector3 startPosition = transform.position + (Vector3.up * (offset * horizontalStackSize / 2f));

        for (int i = 0; i < horizontalStackSize; i++)
        {

            matrix = Matrix4x4.TRS(startPosition - (Vector3.up * offset * i), transform.rotation, transform.localScale);
            Graphics.DrawMesh(quadMesh, matrix, cloudMaterial, layer, camera, 0, null, false, false, false);
        }
    }
}

