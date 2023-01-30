using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatManager : MonoBehaviour
{
    //[SerializeField] private GameObject m_HiddenEntrance;
    //private Vector3 m_InitialPosition;
    //private bool m_HiddenEntranceState = true;

    [SerializeField] private bool m_ShowDebugText = false;

    private void Start()
    {
        //m_InitialPosition = m_HiddenEntrance.transform.position;
    }
    public void IAmAlive()
    {
        //Debug.Log("I AM OLIVE (a good one)", this);
        Singletons.debugTextHelper.AddText(DebugTextFeature.Test, "I Am Alive");
    }

    private void OnDrawGizmosSelected()
    {
        Debug.DrawLine(Vector3.zero, transform.position);
    }

    //public void ChangeWallState()
    //{
    //    m_HiddenEntranceState = !m_HiddenEntranceState;
    //    if (m_HiddenEntranceState)
    //    {
    //        m_HiddenEntrance.transform.position = m_InitialPosition;
    //    }
    //    else
    //    {
    //        m_HiddenEntrance.transform.position = m_InitialPosition + new Vector3(0.0f, m_InitialPosition.y -5.0f, 0.0f);
    //    }
    //}

    public void ChangeDebugTextState(bool state)
    {
        m_ShowDebugText = state;
        
    }

    public bool CheckForActiveDebugText()
    {
        return m_ShowDebugText;
    }
}
