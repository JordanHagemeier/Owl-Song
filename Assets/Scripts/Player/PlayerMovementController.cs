using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovementController : MonoBehaviour
{
    NavMeshAgent m_OwnNavMeshAgent; 
    [SerializeField] private Vector3 m_InitialDirectionForward;
    [SerializeField] private Vector3 m_InitialDirectionRight;
    [SerializeField] private Vector2 m_CurrentInput;
    [SerializeField] private Vector3 m_CalculatedPosition;

    [SerializeField] private float m_LerpSpeed;
    private float m_RotationChangeSpeedX;
    private float m_RotationChangeSpeedZ;

    private bool m_CanMove = true; public bool CanMove { get { return m_CanMove; } set { m_CanMove = value; } }
    private bool m_IsBeingMoved = false;

    //Animations

    private Animator m_PlayerAnimator;
    private Vector3 m_PreviousPosition;
    private float m_CurrentSpeed;

    public void MovePlayerToSpecificPosition(Vector3 position)
    {
        m_IsBeingMoved = true;
        m_OwnNavMeshAgent.destination = position;
        m_OwnNavMeshAgent.speed = 3.0f;
        
    }


    private void Awake()
    {
        Singletons.gameStateManager.m_PlayerMovementController = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        if((m_OwnNavMeshAgent = gameObject.GetComponent<NavMeshAgent>()) == null)
        {
            Debug.LogWarning("No NavMesh Agent found!");
        }
        m_InitialDirectionForward = gameObject.transform.forward;
        m_InitialDirectionRight = gameObject.transform.right;

        m_PlayerAnimator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_CanMove)
        {
            UpdateMovement();
        }
        else
        {
            if (m_IsBeingMoved)
            {
                UpdateControlledMovement();
            }
        }
    }



    private void UpdateControlledMovement()
    {
        int velocityID = Animator.StringToHash("Velocity");
        if (!m_OwnNavMeshAgent.pathPending)
        {
            if (m_OwnNavMeshAgent.remainingDistance <= m_OwnNavMeshAgent.stoppingDistance)
            {
                if (!m_OwnNavMeshAgent.hasPath || m_OwnNavMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    m_PlayerAnimator.SetFloat(velocityID, 0.0f);
                    float rotSpeed = 3.0f;
                    Vector3 direzione = m_OwnNavMeshAgent.destination - transform.position;
                    direzione.y = 0;//This allows the object to only rotate on its y axis
                    Quaternion rotazione = Quaternion.LookRotation(direzione);
                    transform.rotation = Quaternion.Lerp(transform.rotation, rotazione, rotSpeed * Time.deltaTime);
                    m_IsBeingMoved = false;
                    return;
                }
            }
        }



        m_PlayerAnimator.SetFloat(velocityID, 1.0f);

        float rotationSpeed = 3.0f;
        Vector3 dir = m_OwnNavMeshAgent.destination - transform.position;
        dir.y = 0;//This allows the object to only rotate on its y axis
        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
        
    }

    private void UpdateMovement()
    {
        m_CurrentInput = GetKeyInput();

        Vector3 rightWithInput          = new Vector3(m_InitialDirectionRight.x * m_CurrentInput.x, m_InitialDirectionRight.y, m_InitialDirectionRight.z * m_CurrentInput.x);
        Vector3 forwardWithInput        = new Vector3((m_InitialDirectionForward.x) * m_CurrentInput.y, m_InitialDirectionForward.y, (m_InitialDirectionForward.z) * m_CurrentInput.y);
        Vector3 addedVectors            = Vector3.Normalize(rightWithInput + forwardWithInput);



        m_CalculatedPosition            = gameObject.transform.position;
        m_CalculatedPosition            += addedVectors;

        Vector3 curMove     = transform.position - m_PreviousPosition;
        m_CurrentSpeed      = curMove.magnitude / Time.deltaTime;
        m_PreviousPosition  = transform.position;

        Vector3 smoothedPos = new Vector3(Mathf.SmoothDamp(gameObject.transform.position.x, m_CalculatedPosition.x, ref m_RotationChangeSpeedX, m_LerpSpeed), gameObject.transform.position.y, Mathf.SmoothDamp(gameObject.transform.position.z, m_CalculatedPosition.z, ref m_RotationChangeSpeedZ, m_LerpSpeed));

        gameObject.transform.LookAt(smoothedPos);
        m_OwnNavMeshAgent.nextPosition = smoothedPos;
        int velocityID = Animator.StringToHash("Velocity");
        m_PlayerAnimator.SetFloat(velocityID, m_CurrentSpeed);
        
    }


    Vector2 GetKeyInput()
    {
        Vector2 axisInput;
        axisInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        return axisInput;
    }
}
