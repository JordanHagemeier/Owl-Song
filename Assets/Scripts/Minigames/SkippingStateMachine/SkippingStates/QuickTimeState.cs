using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkippingMinigame
{
    public class QuickTimeState : SkippingState
    {
        float cameraSpeed = 0.4f;
        float defaultBuffer = 8;
        float slowMoSpeed = 2f;
        float normalSpeed = .6f;
        Vector3 defaultDistanceVector = new Vector3(-3f,0f,0f);

        Vector3 cameraStart;
        Vector3 cameraEnd;

        bool QTE_active;
        bool QTE_was_hit;
        bool QTE_was_missed;

        float biggerScale =4f;
        float smallerScale = -3f;

        float QTE_Duration = 0.6f;

        float upperTarget = 1.05f;
        float lowerTarget = 0.85f;
        float splashBuffer = 3;

        float qteCountCalc = 0.1f;
        float calcWeight = 0.5f;


        public QuickTimeState (SkippingSystem skippingSystem) : base (skippingSystem)
        { }

        public override IEnumerator OnEnter()
        {
            SkippingSystem.QTE_count++;
            SkippingSystem.QTE_UI.gameObject.SetActive(true);

            Debug.Log("quicktime");

            QTE_was_hit = false;
            QTE_was_missed = false;
            QTE_active = true;

            yield return QTE_Skip_Start();

            if(QTE_was_hit)
            {
                yield return QTE_Hit_Skip();
                SkippingSystem.SetState(new QuickTimeState(SkippingSystem));
                yield break;
            }
            yield return QTE_Missed_Skip();
            yield return new WaitForSeconds(2);
            yield return CameraMovementToInit();

            SkippingSystem.SetState(new WaitingForInteractionState(SkippingSystem));
            yield break;
        }

        public override IEnumerator OnInteract()
        {
            if (QTE_active && !QTE_was_hit)
            {
                if (SkippingSystem.QuicktimeTarget.transform.localScale.x < upperTarget && SkippingSystem.QuicktimeTarget.transform.localScale.x > lowerTarget)
                {
                    Debug.Log("hit qte");
                    QTE_was_hit = true;
                }

                else
                {
                    Debug.Log("missed qte");
                    SkippingSystem.QTE_UI.gameObject.SetActive(false);
                }

                QTE_active = false;
                yield break;
            }

            else if(QTE_active && !QTE_was_missed)
            {
                Debug.Log("missed qte");

                QTE_was_missed = true;
                SkippingSystem.QTE_UI.gameObject.SetActive(false);
                yield break;
            }
        }

        private IEnumerator QTE_Skip_Start()
        {
            QTE_active = true;

            float t = 0;

            cameraStart = SkippingSystem.m_MiniGameCamera.transform.position;
            cameraEnd = cameraStart + defaultDistanceVector;

            Vector3 start = SkippingSystem.Stone3D.transform.position;
            Vector3 end = SkippingSystem.Stone3D.transform.position + defaultDistanceVector;
            Vector3 height = new Vector3((SkippingSystem.Stone3D.transform.position.x + end.x) / 2, SkippingSystem.SkipHeight.position.y, end.z);

            Vector3 startScale = new Vector3(biggerScale, biggerScale, 1);
            Vector3 endScale = new Vector3(smallerScale, smallerScale, 1);

            while (t < QTE_Duration)
            {
                float calc = slowMoSpeed - qteCountCalc;
                if (qteCountCalc * SkippingSystem.QTE_count *calcWeight < slowMoSpeed)
                { calc = slowMoSpeed - qteCountCalc * SkippingSystem.QTE_count * calcWeight; }

                t += Time.smoothDeltaTime / calc;
                SkippingSystem.Stone3D.transform.position = QuadraticLerp(start, height, end, t);

                SkippingSystem.m_MiniGameCamera.transform.position = Vector3.Lerp(cameraStart, cameraEnd,t);

                if (QTE_active)
                {
                    SkippingSystem.QuicktimeTarget.transform.localScale = Vector3.Lerp(startScale, endScale, t);
                }
                yield return null;                
            }
            yield break;
        }

        private IEnumerator QTE_Missed_Skip()
        {
            if (SkippingSystem.QTE_UI.gameObject.activeSelf) { SkippingSystem.QTE_UI.gameObject.SetActive(false); }
            QTE_was_missed = true;
            bool splashplayed = false;
            float t = 0;

            cameraStart = SkippingSystem.m_MiniGameCamera.transform.position;
            cameraEnd = cameraStart + defaultDistanceVector;

            Vector3 start = SkippingSystem.Stone3D.transform.position;
            Vector3 end = new Vector3(SkippingSystem.Stone3D.transform.position.x + defaultDistanceVector.x + splashBuffer/ 2, SkippingSystem.SeaLevel.position.y - splashBuffer, SkippingSystem.SeaLevel.position.z);

            Vector3 startScale = new Vector3(biggerScale, biggerScale, 1);
            Vector3 endScale = new Vector3(smallerScale, smallerScale, 1);

            

            while (t < 1)
            {
                t += Time.smoothDeltaTime / normalSpeed;
                SkippingSystem.Stone3D.transform.position = Vector3.Lerp(start, end, t);
                SkippingSystem.m_MiniGameCamera.transform.position = Vector3.Lerp(cameraStart, cameraEnd, t);

                if (SkippingSystem.Stone3D.transform.position.y < SkippingSystem.SeaLevel.position.y && !splashplayed)
                {
                    //play juice
                    SkippingSystem.StoneMissed();
                    splashplayed = true;
                }

                yield return null;
            }
            

            yield break;
        } 
        
        private IEnumerator QTE_Hit_Skip()
        {
            QTE_active = false;

            float t = 0;

            cameraStart = SkippingSystem.m_MiniGameCamera.transform.position;
            cameraEnd = cameraStart + defaultDistanceVector;

            Vector3 start = SkippingSystem.Stone3D.transform.position;
            Vector3 end = new Vector3(SkippingSystem.Stone3D.transform.position.x + defaultDistanceVector.x, SkippingSystem.SeaLevel.position.y,SkippingSystem.SeaLevel.position.z);

            while (t < 1)
            {
                t += Time.smoothDeltaTime / normalSpeed;
                SkippingSystem.Stone3D.transform.position = Vector3.Lerp(start, end, t);

                SkippingSystem.m_MiniGameCamera.transform.position = Vector3.Lerp(cameraStart, cameraEnd, t);
                yield return null;
            }
            //play juice
            SkippingSystem.StoneSkipped();

            yield break;
        }

        private IEnumerator CameraMovementToInit()
        {
            Vector3 lastPosition = SkippingSystem.m_MiniGameCamera.transform.position;
            float t = 0;

            while (t < 1)
            {
                t += Time.smoothDeltaTime / ((defaultBuffer) * cameraSpeed);

                SkippingSystem.m_MiniGameCamera.transform.position = Vector3.Lerp(lastPosition, SkippingSystem.InitCamera.position, t);

                yield return null;
            }
            yield break;
        }


        private Vector3 QuadraticLerp(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            Vector3 ab = Vector3.Lerp(a, b, t);
            Vector3 bc = Vector3.Lerp(b, c, t);

            return Vector3.Lerp(ab, bc, t);
        }

    }
}
