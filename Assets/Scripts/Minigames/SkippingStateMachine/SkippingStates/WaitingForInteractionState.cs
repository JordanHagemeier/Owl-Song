using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SkippingMinigame
{
    public class WaitingForInteractionState : SkippingState
    {
        private float timerWeigth;
        private float goodHitPercentag = 0.6f;
        private float perfectHitPercentage = 0.3f;



        public WaitingForInteractionState (SkippingSystem skippingSystem) : base (skippingSystem)
        { }

        public override IEnumerator OnEnter()
        {
            SkippingSystem.InMingame(true);
            CornerTextLocaliser.TranslatedInteractions[] translateds = { CornerTextLocaliser.TranslatedInteractions.Cancel, CornerTextLocaliser.TranslatedInteractions.Throw };
            SkippingSystem.ChangeCorner(translateds);
            SkippingSystem.QTE_count = 0;

            SkippingSystem.BarUI.gameObject.SetActive(true);
            SkippingSystem.Stone3D.gameObject.SetActive(true);
            SkippingSystem.Stone3D.transform.position = SkippingSystem.StartPosition3D.position;

            SkippingSystem.stone.rectTransform.anchoredPosition = SkippingSystem.startPosition;
            timerWeigth = Random.Range(SkippingSystem.timerMultiplicators[0], SkippingSystem.timerMultiplicators[1]);

            ChangeHitzone();

            //animation
            SkippingSystem.UI_Animator.SetTrigger("show_UI");
            yield return new WaitForSeconds(.6f);

            SkippingSystem.m_SkippingGameActive = true;
            yield return null;
        }

        public override IEnumerator OnExit()
        {
            CornerTextLocaliser.TranslatedInteractions[] translateds = { CornerTextLocaliser.TranslatedInteractions.Cancel};
            SkippingSystem.ChangeCorner(translateds);

            //animation
            SkippingSystem.UI_Animator.SetTrigger("hide_UI");
            yield return new WaitForSeconds(.6f);
            SkippingSystem.BarUI.gameObject.SetActive(false);

            

            yield break;
        }

        public override IEnumerator OnInteract()
        {
            Debug.Log("hoi");

            if (SkippingSystem.stone.rectTransform.anchoredPosition.x < SkippingSystem.hitzoneStart - SkippingSystem.stoneWidth/2 && SkippingSystem.stone.rectTransform.anchoredPosition.x > SkippingSystem.hitzoneEnd + SkippingSystem.stoneWidth/2)
            {
                SkippingSystem.SetState(new HitState(SkippingSystem));
            }

            else
            {
                SkippingSystem.SetState(new MissedState(SkippingSystem));
            }
            yield break;
        }

        public override IEnumerator OnUpdate()
        {           
            if (Vector2.Distance(SkippingSystem.stone.rectTransform.anchoredPosition, SkippingSystem.endPosition) <= 0)
            {
               SkippingSystem.ChangeDestination();
                yield break;
            }

            float percentage = Time.deltaTime * SkippingSystem.progressBarWidth / timerWeigth;
            SkippingSystem.stone.rectTransform.anchoredPosition = Vector2.MoveTowards(SkippingSystem.stone.rectTransform.anchoredPosition, SkippingSystem.endPosition, percentage);
            yield break;
        }

        private void ChangeHitzone()
        {
            float barPercentag = Random.Range(SkippingSystem.BarPercentages[0], SkippingSystem.BarPercentages[1]);

            float hitzoneWidth = SkippingSystem.progressBarWidth * barPercentag;

            SkippingSystem.averageHit.rectTransform.sizeDelta = new Vector2(hitzoneWidth, SkippingSystem.averageHit.rectTransform.rect.height);

            float start = SkippingSystem.progressBarWidth / 2 - hitzoneWidth / 2;

            SkippingSystem.newPosition = Random.Range(start, -start);

            SkippingSystem.averageHit.rectTransform.anchoredPosition = new Vector2(SkippingSystem.newPosition,0);

            SkippingSystem.hitzoneStart = SkippingSystem.averageHit.rectTransform.anchoredPosition.x + SkippingSystem.averageHit.rectTransform.sizeDelta.x / 2;
            SkippingSystem.hitzoneEnd = SkippingSystem.averageHit.rectTransform.anchoredPosition.x - SkippingSystem.averageHit.rectTransform.sizeDelta.x / 2;

            SkippingSystem.perfectHit.rectTransform.sizeDelta = new Vector2(perfectHitPercentage * hitzoneWidth, SkippingSystem.perfectHit.rectTransform.rect.height);
            SkippingSystem.goodHit.rectTransform.sizeDelta = new Vector2(goodHitPercentag * hitzoneWidth, SkippingSystem.goodHit.rectTransform.rect.height);
        }      
    }
}
