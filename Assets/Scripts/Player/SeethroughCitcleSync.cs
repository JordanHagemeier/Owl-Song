using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeethroughCitcleSync : MonoBehaviour
{
    public static int posID = Shader.PropertyToID("_position");
    public static int sizeID = Shader.PropertyToID("_size");

    public Material [] allSeethroughs;
    public Material [] allSolids;

    public Camera camera;
    public LayerMask mask;

    public float circleSize = 1f;

    private Dictionary<string, Material> findMaterialDictToSee = new Dictionary<string, Material>();
    private Dictionary<string, Material> findMaterialDictToSolid = new Dictionary<string, Material>();

    RaycastHit[] pastHits;
    string[] split;
    string temp;
    bool IsEnteredView = false;

    int previousLength;

    private void Awake()
    {
        findMaterialDictToSee.Clear();
        findMaterialDictToSolid.Clear();

        int i = 0;
        foreach(Material key in allSeethroughs)
        {
            findMaterialDictToSolid.Add(key.name, allSolids[i]);
            i++;
        }

        i = 0;
        foreach(Material key in allSolids)
        {
            findMaterialDictToSee.Add(key.name, allSeethroughs[i]);
            i++;
        }
    }

    private void Update()
    {
        Vector3 direction = camera.transform.position - transform.position;


        //Debug.Log(camera.transform.position);
        //Debug.Log(transform.position);


        RaycastHit hit;
        var ray = new Ray(transform.position, direction.normalized);
        //Debug.Log(ray);
        
            RaycastHit[] hitObjects = Physics.RaycastAll(transform.position, direction, direction.magnitude, mask);
            bool success = Physics.Raycast(ray, out hit, 3000, mask);

            //Debug.Log(hitObjects.Length);

            if(hitObjects.Length != previousLength && hitObjects.Length != 0)
            {
            IsEnteredView = false;
            }

            previousLength = hitObjects.Length;

            if (success && !IsEnteredView)
            {
                IsEnteredView = true;
                if (pastHits != null && pastHits != hitObjects)
                {
                    foreach (RaycastHit singleHit in pastHits)
                    {
                        StartCoroutine(ChangeMaterials(singleHit,false));
                    }
            }
                
                IsEnteredView = true;
                
                foreach (RaycastHit singleHit in hitObjects)
                {
                    StartCoroutine(ChangeMaterials(singleHit,true));
                }

            pastHits = hitObjects;
            }

            else if (!success && pastHits != null && IsEnteredView)
            {

                //Debug.Log("off");
                IsEnteredView = false;
                foreach (RaycastHit singleHit in pastHits)
                    {
                    
                    for (int i = 0; i < singleHit.transform.childCount; i++)
                    {
                        StartCoroutine(ChangeMaterials(singleHit,false));
                    }

                }

                pastHits = null;
            }




        /*if (hit.collider != null)
            {
                Debug.Log(hit.collider.gameObject.layer);
            }*/



        // Bit shift the index of the layer (8) to get a bit mask


        if (Singletons.debugDrawingManager.enableDebugDrawings)
        {
            RaycastHit rayhit;
            //// Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, direction.normalized, out rayhit, Mathf.Infinity, mask))
            {
                DebugDrawingInterface.DrawPersistentRay(ray, 10.0f, Color.red, 60);
                //Debug.Log("Did Hit");
            }
            else
            {
                DebugDrawingInterface.DrawPersistentRay(ray, 10.0f, Color.green, 60);
                //Debug.Log("Did not Hit");
            }

        }

    }

    IEnumerator ChangeMaterials(RaycastHit raycastHit, bool isOn)
    {

        if(isOn)
        {
            for (int i = 0; i < raycastHit.transform.childCount; i++)
            {
                temp = raycastHit.transform.GetChild(i).transform.GetChild(0).GetComponent<MeshRenderer>().material.name;
                split = temp.Split(new string[] { " " }, System.StringSplitOptions.None);
                temp = split[0];

                if (findMaterialDictToSee.ContainsKey(temp))
                {
                    yield return ChangeSingleToSee(raycastHit, i);
                }
            }
        }

        else
        {
            for (int i = 0; i < raycastHit.transform.childCount; i++)
            {
                temp = raycastHit.transform.GetChild(i).transform.GetChild(0).GetComponent<MeshRenderer>().material.name;
                split = temp.Split(new string[] { " " }, System.StringSplitOptions.None);
                temp = split[0];

                if (findMaterialDictToSolid.ContainsKey(temp))
                {
                    yield return ChangeSingleToSolid(raycastHit, i);
                }
            }
        }
        

        yield return new WaitForEndOfFrame();
    }
    IEnumerator ChangeSingleToSee(RaycastHit hit, int i)
    {
        hit.transform.GetChild(i).transform.GetChild(0).GetComponent<MeshRenderer>().material = findMaterialDictToSee[temp];
        hit.transform.GetChild(i).transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat(sizeID, circleSize);
        yield return new WaitForEndOfFrame();
    }

    IEnumerator ChangeSingleToSolid(RaycastHit hit, int i)
    {
        hit.transform.GetChild(i).transform.GetChild(0).GetComponent<MeshRenderer>().material = findMaterialDictToSolid[temp];
        hit.transform.GetChild(i).transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat(sizeID, circleSize);
        yield return new WaitForEndOfFrame();
    }
}
