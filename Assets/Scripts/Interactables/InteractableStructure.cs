using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class InteractableStructure : Interactable
{
    public NavMeshObstacle m_OwnNavMeshObstacle;
    public float m_OwnNavMeshObstacleRadius = 1.0f;
    public float m_NoDropRange; public float noDropRange { get { return m_NoDropRange; } set { m_NoDropRange = value; } }

    private void Awake()
    {
        base.Awake();
        Singletons.interactableManager.RegisterStructure(this);
        gameObject.layer            = LayerMask.NameToLayer("Structure");
        if(m_OwnNavMeshObstacle == null && gameObject.GetComponent<NavMeshObstacle>() == null)
        {
            m_OwnNavMeshObstacle = gameObject.AddComponent<NavMeshObstacle>();
            m_OwnNavMeshObstacle.size = new Vector3(m_OwnNavMeshObstacleRadius, m_OwnNavMeshObstacleRadius, m_OwnNavMeshObstacleRadius);
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        DebugDrawHelper.DrawCircle(Mode2D.OnYPlane, gameObject.transform.position.XZ(), gameObject.transform.position.y, m_NoDropRange, Color.black, false);
        //DebugDrawingInterface.DrawPersistentCircleXZ(gameObject.transform.position, m_NoDropRange, Color.black, 1);
    }

    public override void Update()
    {
        base.Update();
    }
}
