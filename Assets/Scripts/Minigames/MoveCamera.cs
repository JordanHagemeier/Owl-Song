using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{

    [SerializeField] private GameObject m_Stone;
    [SerializeField] private float oldXPosition;
    [SerializeField] private float newXPosition; 
    [SerializeField] private GameObject m_MiniGameCam;
    private float m_Velocity;


    // Start is called before the first frame update
    void Start()
    {
        newXPosition = m_Stone.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        LerpToStonePosition();
    }

    private void LerpToStonePosition()
    {
        //newXPosition = m_Stone.transform.position.x;
        //m_MiniGameCam.transform.position = new Vector3(Mathf.SmoothDamp(oldXPosition, newXPosition, ref m_Velocity, 0.1f), m_MiniGameCam.transform.position.y, m_MiniGameCam.transform.position.z);
        //oldXPosition = newXPosition;

    }
}
