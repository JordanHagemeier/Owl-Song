using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class PlayerInteractionController : MonoBehaviour
{
    public static event Action<CornerTextLocaliser.TranslatedInteractions[]> OnCornerTextChanged;
    public static event Action<bool> OnDrop;

    //interaction
    [Header("Interaction Values")]
    [SerializeField] private float m_Radius;
    [SerializeField] private float m_ViewConeInDegrees;
    [SerializeField] private GameObject m_CurrentInteractable = null; public Interactable currentInteractable{ get { return m_CurrentInteractable.GetComponent<Interactable>(); } }

    //Picking up and dropping down
    [Header("Picking Up")]
    [SerializeField] private bool m_HandsAreFull = false;
    [SerializeField] private bool m_inMenu = false; // rika hacky
    [SerializeField] private bool m_inMinigame = false; // rika hacky
    [SerializeField] private Transform m_HandPosition;
    [SerializeField] private Vector3 m_PickUpRotationOfItem;

    //if hands are full, we want to slow down


    [Header("Dropping")]
    [SerializeField] private float m_PlacementDistanceInFrontOfPlayer;
    [SerializeField] private float m_NavMeshDistance;


    //Animation
    private Animator m_PlayerAnimator;

    //TODO
    // check all the time whats in our interaction radius and highlight it
    private void Awake()
    {
        Singletons.gameStateManager.m_PlayerInteractionController = this;
        ConstructionManager.OnInMenu += SetInMenu;
        SkippingMinigame.SkippingSystem.OnInMinigame += SetMini;
    }
    private void Start()
    {
        if (m_HandPosition == null)
        {
            Debug.LogWarning("Hand position is not given!");
        }

        m_PlayerAnimator = gameObject.GetComponent<Animator>();
        PlayerNotCarry();
    }

    private void OnDestroy()
    {
        ConstructionManager.OnInMenu += SetInMenu;
        SkippingMinigame.SkippingSystem.OnInMinigame -= SetMini;
    }

    void Update()
    {
        if (!m_HandsAreFull && !m_inMenu && !m_inMinigame)
        { HighlightNextInteractionTarget(); }

        else if (m_inMenu)
        {
            CornerTextLocaliser.TranslatedInteractions[] translateds = { CornerTextLocaliser.TranslatedInteractions.Cancel, CornerTextLocaliser.TranslatedInteractions.Left, CornerTextLocaliser.TranslatedInteractions.Right, CornerTextLocaliser.TranslatedInteractions.Confirm };
            OnCornerTextChanged(translateds);
        }

        CheckForInput();
    }

    public void GiveGoodToPlayer(GameObject good)
    {
        m_HandsAreFull = true;

        good.transform.parent = m_HandPosition;
        good.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        good.transform.localRotation = Quaternion.Euler(m_PickUpRotationOfItem);
        m_CurrentInteractable = good;

        //m_PlayerAnimator.SetBool(Animator.StringToHash("CanPickUp"), true);
        PlayerCannotMove();
        m_PlayerAnimator.SetTrigger(Animator.StringToHash("WantPickUp"));
    }

    public void TellPlayerToDropGood()
    {
        m_HandsAreFull = false;
        m_CurrentInteractable.transform.parent = null;
        m_CurrentInteractable.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        m_CurrentInteractable.SetActive(false);
        m_CurrentInteractable = null;

        PlayerCannotMove();
        m_PlayerAnimator.SetTrigger(Animator.StringToHash("WantDrop"));
        //m_PlayerAnimator.SetBool(Animator.StringToHash("CanDrop"), true);
        //gotta implement object pooling, so that I don't delete and new on runtime
    }



    private void CheckForInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChooseInteraction();
        }
    }

    private void HighlightNextInteractionTarget()
    {
        Interactable interactableInRadius = CheckIfSomethingIsInFrontOfUs();
        if (interactableInRadius != null)
        {
            interactableInRadius.isCurrentlyInFocus = true;
            CornerTextLocaliser.TranslatedInteractions[] translatedInteractions = { CornerTextLocaliser.TranslatedInteractions.Take};
            OnCornerTextChanged(translatedInteractions);
        }

        else
        {
            CornerTextLocaliser.TranslatedInteractions[] translatedInteractions = { CornerTextLocaliser.TranslatedInteractions.None};
            OnCornerTextChanged(translatedInteractions);
        }
    }

    public void TellPlayerToStopInteractionAnimation()
    {
        m_PlayerAnimator.SetBool(Animator.StringToHash("WantInteract"), false);
    }


    public void PlayerCanMove()
    {
      Singletons.gameStateManager.ChangePlayerMovementState(true);
    }

    public void PlayerCannotMove()
    {
      Singletons.gameStateManager.ChangePlayerMovementState(false);
    }

    public void FreezePlayer()
    {
      m_PlayerAnimator.SetBool("Freeze", true);
    }

    public void UnfreezePlayer()
    {
      m_PlayerAnimator.SetBool("Freeze", false);
    }

    public void PlayerCarry()
    {
      m_PlayerAnimator.SetLayerWeight(1, 1.0f);
    }

    public void PlayerNotCarry()
    {
      m_PlayerAnimator.SetLayerWeight(1, 0.0f);
    }

    public void SetInMenu(bool inmenu)
    {
        m_inMenu = inmenu;
    }
    public void SetMini(bool inmini)
    {
        m_inMinigame = inmini;
    }

    private void ChooseInteraction()
    {
        //this interaction is for -INTERACTION-, not for EATING!!

        //1. checking for something in our radius (whether in our hands or in front of us)
        //2. distinguishing between structures and goods, because the player needs to place the goods while the structures do all of their logic themselves
        //3. choose interaction type based on whether we have something in our hands or not


        Interactable interactableInRadius = CheckIfSomethingIsInFrontOfUs();
        if (interactableInRadius != null)
        {

            if(interactableInRadius.gameObject.layer == LayerMask.NameToLayer("Structure"))
            {
                if (m_HandsAreFull)
                {
                    if(!interactableInRadius.OnInteract(InteractionTypes.DropOff, this))
                    {
                        //m_PlayerAnimator.SetBool(Animator.StringToHash("CanDrop"), false);
                        //m_PlayerAnimator.SetTrigger(Animator.StringToHash("WantDrop"));
                        PlayerCannotMove();
                        m_PlayerAnimator.SetTrigger(Animator.StringToHash("FailedDrop"));
                    }

                }
                else
                {
                    interactableInRadius.OnInteract(InteractionTypes.PickUp, this);
                    if(interactableInRadius.gameObject.tag == "EmptyInteraction")
                    {
                        m_PlayerAnimator.SetFloat(Animator.StringToHash("Velocity"), 0.0f);
                        m_PlayerAnimator.SetBool(Animator.StringToHash("WantInteract"), true);

                    }
                }

            }
            if(interactableInRadius.gameObject.layer == LayerMask.NameToLayer("Goods"))
            {
                if (!m_HandsAreFull)
                {
                    if (!PickUpItem(interactableInRadius))
                    {
                        //m_PlayerAnimator.SetBool(Animator.StringToHash("CanPickUp"), false);
                        PlayerCannotMove();
                        m_PlayerAnimator.SetTrigger(Animator.StringToHash("WantPickUp"));
                        CornerTextLocaliser.TranslatedInteractions[] translateds = { CornerTextLocaliser.TranslatedInteractions.Throw };
                        OnCornerTextChanged(translateds);
                        return;
                    }

                    //m_PlayerAnimator.SetBool(Animator.StringToHash("CanPickUp"), true);
                    PlayerCannotMove();
                    m_PlayerAnimator.SetTrigger(Animator.StringToHash("WantPickUp"));
                }
                else
                {
                    if (!DropItem())
                    {
                        //m_PlayerAnimator.SetBool(Animator.StringToHash("CanDrop"), false);
                        //m_PlayerAnimator.SetTrigger(Animator.StringToHash("WantDrop"));
                        PlayerCannotMove();
                        m_PlayerAnimator.SetTrigger(Animator.StringToHash("FailedDrop"));
                        return;
                    }
                    //m_PlayerAnimator.SetBool(Animator.StringToHash("CanDrop"), true);
                    PlayerCannotMove();
                    m_PlayerAnimator.SetTrigger(Animator.StringToHash("WantDrop"));
                    OnDrop(true);
                    PlayerNotCarry();


                }
            }
        }
        else
        {
            if (m_HandsAreFull)
            {

                if (!DropItem())
                {
                    //m_PlayerAnimator.SetBool(Animator.StringToHash("CanDrop"), false);
                    //m_PlayerAnimator.SetTrigger(Animator.StringToHash("WantDrop"));
                    PlayerCannotMove();
                    m_PlayerAnimator.SetTrigger(Animator.StringToHash("FailedDrop"));
                    return;
                }
                //m_PlayerAnimator.SetBool(Animator.StringToHash("CanDrop"), true);
                PlayerCannotMove();
                m_PlayerAnimator.SetTrigger(Animator.StringToHash("WantDrop"));
                OnDrop(true);
                PlayerNotCarry();
            }
        }

    }

    private Interactable CheckIfSomethingIsInFrontOfUs()
    {
        Vector3 currentPos = gameObject.transform.position;
        Vector3 currentForward = gameObject.transform.forward;

        //setting one to default




        Interactable closestInteractable = Singletons.interactableManager.GetClosestInteractableWithinRadiusAndInFront(currentPos, currentForward, m_Radius, m_ViewConeInDegrees, m_CurrentInteractable);
        if (closestInteractable == null)
        {


            return null;
        }


        return closestInteractable;
    }





    private bool PickUpItem(Interactable closestInteractable)
    {
        bool success = false;


        if (!m_HandsAreFull)
        {

            closestInteractable.OnInteract(InteractionTypes.PickUp, this);
            closestInteractable.gameObject.transform.parent = m_HandPosition;
            closestInteractable.gameObject.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            closestInteractable.gameObject.transform.localRotation = Quaternion.Euler(m_PickUpRotationOfItem);
            m_CurrentInteractable = closestInteractable.gameObject;

            m_HandsAreFull = true;
            //m_PlayerAnimator.SetBool(Animator.StringToHash("IsCarrying"), true);
            success = true;
        }

        return success;
    }



    private bool DropItem()
    {
        bool success = false;


        //1. sample for position on nav mesh in front of player
        Vector3 sampledPosition = SamplePointInFrontOfPlayer();

        //2. check if that position has another object within radius
        if (sampledPosition == null | float.IsNaN(transform.position.x) && float.IsNaN(transform.position.y) && float.IsNaN(transform.position.z))
        {
            Debug.LogError("Sampled Position in front of player not found / is not a number!");
            return success;
        }
        //bool positionIsClear = CheckForOtherObstructionInRadiusAroundPoint(sampledPosition, m_CurrentInteractable.GetComponent<Interactable>().m_PlaceholderRadius);
        bool positionIsClear = CheckForOtherObstructionInRadiusAroundPoint(sampledPosition, m_CurrentInteractable.GetComponent<SphereCollider>().radius);
        if (!positionIsClear)
        {
            Debug.Log("Uh oh, something's already there!");
            return success;
        }

        //3. put object to that position and unparent it
        m_HandsAreFull = false;
        m_CurrentInteractable.gameObject.transform.position = sampledPosition;
        m_CurrentInteractable.gameObject.transform.parent = null;
        m_CurrentInteractable.GetComponent<InteractableGood>().OnInteract(InteractionTypes.DropOff, this);
        m_CurrentInteractable = null;
        //m_PlayerAnimator.SetBool(Animator.StringToHash("IsCarrying"), false);
        success = true;
        return success;
    }


    private Vector3 SamplePointInFrontOfPlayer()
    {
        Vector3 posInFrontOfPlayer = gameObject.transform.position + (gameObject.transform.forward * m_PlacementDistanceInFrontOfPlayer);
        NavMeshHit hit; // NavMesh Sampling Info Container

        // from randomPos find a nearest point on NavMesh surface in range of maxDistance
        NavMesh.SamplePosition(posInFrontOfPlayer, out hit, m_NavMeshDistance, NavMesh.AllAreas);
        DebugDrawingInterface.DrawPersistentLine(gameObject.transform.position, hit.position, Color.red, 300);
        return hit.position;
    }

    private bool CheckForOtherObstructionInRadiusAroundPoint(Vector3 pos, float radiusOfItem)
    {
        bool success = false;
        LayerMask GoodsLayermask = 1 << LayerMask.NameToLayer("Goods");
        LayerMask SpawnerLayermask = 1 << LayerMask.NameToLayer("Structure");
        LayerMask together = GoodsLayermask | SpawnerLayermask;
        Collider[] objectsInRange = Physics.OverlapSphere(pos, radiusOfItem, together);

        bool notCollidingWithOtherObjects               = false;
        bool notIntersectingWithStructureNoDropZone     = false;

        if (objectsInRange.Length == 0 | (objectsInRange.Length > 0 && objectsInRange[0] == m_CurrentInteractable.GetComponent<Collider>()))
        {
            notCollidingWithOtherObjects = true;
        }

        if (!Singletons.interactableManager.PointIntersectsWithStructureNoDropRadius(pos))
        {
            notIntersectingWithStructureNoDropZone = true;
        }

        if(notCollidingWithOtherObjects && notIntersectingWithStructureNoDropZone)
        {
            success = true;
        }
            return success;
    }


    //if this is annoying, look into the queueing of debug drawings and handles
    private void OnDrawGizmosSelected()
    {

        //draw the radius


        Vector2 playerForwardWS = new Vector2(gameObject.transform.forward.x, gameObject.transform.forward.z);

        // 1) Get player rotation  [world space]
        // Vector2ToAngle
        float playerRotYWS = Vector2.SignedAngle(new Vector2(0.0f, 1.0f), playerForwardWS);

        // 2) Get cone limit angles [world space]
        float coneAngle1WS = playerRotYWS + m_ViewConeInDegrees;
        float coneAngle2WS = playerRotYWS - m_ViewConeInDegrees;

        // 2a) Optimally a function itself
        // AngleToVector2
        Vector2 coneVector1WS = new Vector2(-Mathf.Sin(Mathf.Deg2Rad * coneAngle1WS), Mathf.Cos(Mathf.Deg2Rad * coneAngle1WS));
        Vector2 coneVector2WS = new Vector2(-Mathf.Sin(Mathf.Deg2Rad * coneAngle2WS), Mathf.Cos(Mathf.Deg2Rad * coneAngle2WS));

        // 3) Get cone edge points
        Vector3 coneCorner1WS = gameObject.transform.position + coneVector1WS.To3D(0.0f) * m_Radius;
        Vector3 coneCorner2WS = gameObject.transform.position + coneVector2WS.To3D(0.0f) * m_Radius;

        // 4) Draw
        DebugDrawHelper.DrawLine(gameObject.transform.position, coneCorner1WS, Color.green, 3.0f);
        DebugDrawHelper.DrawLine(gameObject.transform.position, coneCorner2WS, Color.green, 3.0f);
        DebugDrawHelper.DrawCircle(Mode2D.OnYPlane, gameObject.transform.position.XZ(), gameObject.transform.position.y, m_Radius, Color.black, false);


        if (m_CurrentInteractable != null)
        {

            //draw line from player to interactable in focus
            DebugDrawHelper.DrawLine(gameObject.transform.position, m_CurrentInteractable.transform.position, Color.red, 5.0f);
        }


        //Draw Line for in which distance to position a dropped object
        DebugDrawHelper.DrawLine(gameObject.transform.position, gameObject.transform.position + (gameObject.transform.forward * m_PlacementDistanceInFrontOfPlayer), Color.blue, 5.0f);
    }

    
}
