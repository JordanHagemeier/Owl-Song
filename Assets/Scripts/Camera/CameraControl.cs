using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private GameObject m_Player;
    [SerializeField] private Vector3 m_InitialOffset;

    private Vector3 m_CurrentVelocity;
    [SerializeField] private float m_SmoothTime;

    private void Awake()
    {
        Singletons.gameStateManager.camera = this.GetComponent<Camera>();
    }
    // Start is called before the first frame update
    void Start()
    {
        m_InitialOffset = gameObject.transform.position - m_Player.transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        gameObject.transform.position = Vector3.SmoothDamp(gameObject.transform.position, m_Player.transform.position + m_InitialOffset, ref m_CurrentVelocity, m_SmoothTime);
    }
}
