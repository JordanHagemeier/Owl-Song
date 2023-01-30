using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SkippingMinigame
{
    public class HitState : SkippingState
    {
        #region vars

        Vector3 splashHeight = new Vector3(0, -2f, 0);
        int qte_treshold = 8;
        float startTimeWeight = 1.15f;
        float splashTimeWeight = 1.2f;
        float skipLengthDefault = 2f;


        float skipDuration = 1f;
        float skipLength;
        int skipAmount;

        float cameraSpeed = .8f;

        Vector3 distanceVector;
        Vector3 cameraStart;
        #endregion

        public HitState (SkippingSystem skippingSystem) : base (skippingSystem)
        { }

        public override IEnumerator OnEnter()
        {
            cameraStart = SkippingSystem.m_MiniGameCamera.transform.position;

            #region determine quality of the skip
            if (SkippingSystem.stone.rectTransform.anchoredPosition.x < SkippingSystem.newPosition + SkippingSystem.perfectHit.rectTransform.rect.width / 2 && SkippingSystem.stone.rectTransform.anchoredPosition.x > SkippingSystem.newPosition - SkippingSystem.perfectHit.rectTransform.rect.width / 2)
            {
                skipDuration = Random.Range(0.25f,0.28f);
                skipLength = skipLengthDefault / skipDuration;
                distanceVector = new Vector3 (-skipLength, 0, 0);

                skipAmount = Random.Range(8,12);

                yield return Skip(skipAmount);
                Debug.Log("perfect hit");
            }

            else if (SkippingSystem.stone.rectTransform.anchoredPosition.x < SkippingSystem.newPosition + SkippingSystem.goodHit.rectTransform.rect.width/2 && SkippingSystem.stone.rectTransform.anchoredPosition.x > SkippingSystem.newPosition - SkippingSystem.goodHit.rectTransform.rect.width / 2)
            {
                skipDuration = Random.Range(0.3f, 0.35f);
                skipLength = skipLengthDefault / skipDuration;
                distanceVector = new Vector3(-skipLength, 0, 0);

                skipAmount = Random.Range(5, 7);
                yield return Skip(skipAmount);
                Debug.Log("good hit");
            }

            else
            {
                skipDuration = Random.Range(0.33f, 0.4f);
                skipLength = skipLengthDefault / skipDuration;
                distanceVector = new Vector3(-skipLength, 0, 0);

                skipAmount = Random.Range(1, 4);
                yield return Skip(skipAmount);
                Debug.Log("average hit");
            }
            #endregion

            yield break;
        }

        public override IEnumerator OnInteract()
        {
            yield break;
        }


        #region Skipping
        private IEnumerator Skip(int skipAmount)
        {
            float firstSkipX = (SkippingSystem.SeaLevel.position.x - SkippingSystem.StartPosition3D.position.x);
            Vector3 cameraTotalDistance = (skipAmount - 1) * distanceVector + new Vector3(firstSkipX, 0, 0);
            Vector3 cameraSingleDistance = cameraTotalDistance / skipAmount;

            for (int i = 0; i<= skipAmount; i++)
            {
                if (i == 0)
                {
                    yield return StartingSkip(cameraSingleDistance);
                }
                
                else if (i < skipAmount)
                {
                    yield return SingleSkip(i,cameraSingleDistance);
                }

                else if (skipAmount >= qte_treshold && i == skipAmount)
                {
                    SkippingSystem.SetState(new QuickTimeState(SkippingSystem));
                    yield break;
                }

                else
                {
                    Debug.Log("final skip");

                    yield return FinalSkip(i,cameraSingleDistance);
                    yield return new WaitForSeconds(2);
                    yield return CameraMovementToInit(skipAmount);
                    SkippingSystem.SetState(new WaitingForInteractionState(SkippingSystem));
                }
            }
            yield break;
        }


        private IEnumerator StartingSkip(Vector3 cameraEnd)
        {
            float t = 0;
            
            cameraStart = SkippingSystem.m_MiniGameCamera.transform.position;

            Vector3 start = SkippingSystem.Stone3D.transform.position;
            Vector3 end = SkippingSystem.SeaLevel.transform.position;

            while (t < 1)
            {
                t += Time.smoothDeltaTime / skipDuration / startTimeWeight;
                SkippingSystem.Stone3D.transform.position = Vector3.Lerp(start, end, t);

                SkippingSystem.m_MiniGameCamera.transform.position = Vector3.Lerp(cameraStart, cameraEnd + cameraStart, t);
                yield return null;
            }
            SkippingSystem.StoneSkipped();

            yield break;
        }


        private IEnumerator SingleSkip (int iteration, Vector3 cameraEnd)
        {
            float t = 0;

            cameraStart = SkippingSystem.m_MiniGameCamera.transform.position;

            Vector3 start = SkippingSystem.Stone3D.transform.position;
            Vector3 end = SkippingSystem.SeaLevel.transform.position + iteration * distanceVector;
            Vector3 height = new Vector3((SkippingSystem.Stone3D.transform.position.x + end.x) / 2, SkippingSystem.SkipHeight.position.y, end.z) ;

            while(t < 1)
            {
                t += Time.smoothDeltaTime / skipDuration;
                SkippingSystem.Stone3D.transform.position = QuadraticLerp(start,height,end,t);

                SkippingSystem.m_MiniGameCamera.transform.position = Vector3.Lerp(cameraStart, cameraEnd + cameraStart, t);
                yield return null;
            }
            SkippingSystem.StoneSkipped();
            yield return null;
        }
   

        private IEnumerator FinalSkip(int iteration, Vector3 cameraEnd)
        {
            float t = 0;

            cameraStart = SkippingSystem.m_MiniGameCamera.transform.position;

            Vector3 start = SkippingSystem.Stone3D.transform.position;
            Vector3 end = SkippingSystem.SeaLevel.transform.position + iteration * distanceVector + splashHeight;
            Vector3 height = new Vector3((SkippingSystem.Stone3D.transform.position.x + end.x) / 2, SkippingSystem.SkipHeight.position.y, end.z);

            while (t < 1)
            {
                t += Time.smoothDeltaTime / skipDuration /splashTimeWeight;
                SkippingSystem.Stone3D.transform.position = QuadraticLerp(start, height, end ,t);

                SkippingSystem.m_MiniGameCamera.transform.position = Vector3.Lerp(cameraStart,cameraStart + cameraEnd, t);
                yield return null;
            }
            yield break;
        }
        #endregion


        private IEnumerator CameraMovementToInit( int intervals)
        {
            Vector3 lastPosition = SkippingSystem.m_MiniGameCamera.transform.position;
            float t = 0;

            while(t < 1)
            {
                t += Time.smoothDeltaTime / (intervals * cameraSpeed);

                SkippingSystem.m_MiniGameCamera.transform.position = Vector3.Lerp(lastPosition, SkippingSystem.InitCamera.position, t);

                yield return null;
            }
            yield break;
        }


        private Vector3 QuadraticLerp (Vector3 a, Vector3 b, Vector3 c, float t)
        {
            Vector3 ab = Vector3.Lerp(a,b,t);
            Vector3 bc = Vector3.Lerp(b, c, t);

            return Vector3.Lerp(ab, bc, t);
        }
    }
}
