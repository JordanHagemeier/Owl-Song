using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace SkippingMinigame
{
    public class SkippingSystem : SkippingFSM
    {
        #region vars
        [SerializeField] public bool m_SkippingGameActive = false;
        [SerializeField] public GameObject Stone3D;
        [SerializeField] public Transform StartPosition3D;
        [SerializeField] public Transform SeaLevel;
        [SerializeField] public Transform SkipHeight;

        [SerializeField] public Camera m_MainGameCamera;
        [SerializeField] public Camera m_MiniGameCamera;
        [SerializeField] public Transform InitCamera;

        [SerializeField] public Canvas BarUI;
        [SerializeField] public Canvas QTE_UI;

        [SerializeField] public Image stone;
        [SerializeField] private Image progressBar;
        [SerializeField] public Image perfectHit;
        [SerializeField] public Image goodHit;
        [SerializeField] public Image averageHit;

        [SerializeField] public float [] timerMultiplicators = new float [2];
        [SerializeField] public float [] BarPercentages = new float [2];

        [SerializeField] public Image QuicktimeTarget;
        [SerializeField] public Animator UI_Animator;



        [HideInInspector] public float progressBarWidth;
        [HideInInspector] public float stoneWidth;

        [HideInInspector] public Vector2 startPosition;
        [HideInInspector] public Vector2 endPosition;
        [HideInInspector] public float newPosition;

        [HideInInspector] public float hitzoneStart;
        [HideInInspector] public float hitzoneEnd;

        [HideInInspector] public float QTE_count;

        [SerializeField] Animator Lotta;

        public static event Action OnStoneSkipped;
        public static event Action OnStoneMissed;

        public static event Action<CornerTextLocaliser.TranslatedInteractions[]> OnCornerTextChanged;
        public static event Action<bool> OnInMinigame;

        #endregion

        public void SetMiniGameActive()
        {
            SetState(new WaitingForInteractionState(this));
        }

        private void EndSkippingGame()
        {
            m_SkippingGameActive = false;
            SetState(new InactiveState(this));
            m_MiniGameCamera.gameObject.SetActive(false);
            m_MainGameCamera.gameObject.SetActive(true);
            Singletons.audioBookManager.audioClipsAreAllowedToPlay = true;
            CornerTextLocaliser.TranslatedInteractions[] translateds = { CornerTextLocaliser.TranslatedInteractions.None };
            ChangeCorner(translateds);
            Lotta.SetBool("WantInteract", false);
            StartCoroutine(WaitTimeEsc());
            Singletons.gameStateManager.ChangePlayerMovementState(true);
            OnInMinigame(false);
        }

        IEnumerator WaitTimeEsc()
        {
            yield return new WaitForSeconds(2);
            Singletons.gameStateManager.m_InMenu = false;
            yield return null;
        }

        private void Start()
        {
            SetState(new InactiveState(this));

            progressBarWidth = progressBar.rectTransform.rect.width;
            stoneWidth = stone.rectTransform.rect.width/ 2 ;

            float stoneEndDestination = progressBarWidth / 2 - stoneWidth / 2;
            float stoneStartDestination = -stoneEndDestination;

            startPosition = new Vector2(stoneStartDestination, 0);
            endPosition = new Vector2(stoneEndDestination, 0);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape) && m_SkippingGameActive)
            {
                this.StopAllCoroutines();
                EndSkippingGame();
            }
            if (Input.GetKeyDown(KeyCode.Space) && m_SkippingGameActive)
            {
                StopCoroutine(State.OnUpdate());
                StartCoroutine(State.OnInteract());
            }

            StartCoroutine(State.OnUpdate());
        }

        public void ChangeDestination()
        {
            Vector2 temp = endPosition;
            endPosition = startPosition;
            startPosition = temp;
        }


        public void StoneMissed()
        {
            OnStoneMissed();
        }

        public void StoneSkipped()
        {
            OnStoneSkipped();
        }

        public void ChangeCorner(CornerTextLocaliser.TranslatedInteractions[] translation)
        {
            OnCornerTextChanged(translation);
        }

        public void StopState()
        {
            StopCoroutine(State.OnUpdate());
        }

        public void InMingame(bool inmini)
        {
            OnInMinigame(inmini);
        }
    }
}
