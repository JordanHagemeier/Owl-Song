using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkippingMinigame
{

    public class MissedState : SkippingState
    {
        float skipDuration = .7f;
        Vector3 splashHeight = new Vector3(0, -2f, 0);

        public MissedState (SkippingSystem skippingSystem) : base (skippingSystem)
        { }

        public override IEnumerator OnEnter()
        {
            yield return MissedSkip();
            yield return new WaitForSeconds(1);

            SkippingSystem.SetState(new WaitingForInteractionState(SkippingSystem));
        }


        private IEnumerator MissedSkip()
        {
            float t = 0;

            Vector3 start = SkippingSystem.Stone3D.transform.position;
            Vector3 end = SkippingSystem.SeaLevel.transform.position;
            bool splashplayed = false;


            while (t < 1)
            {
                t += Time.smoothDeltaTime / skipDuration;
                SkippingSystem.Stone3D.transform.position = Vector3.Lerp(start, end + splashHeight, t);

                if (SkippingSystem.Stone3D.transform.position.y < SkippingSystem.SeaLevel.position.y && !splashplayed)
                {
                    splashplayed = true;
                    //play juice
                    SkippingSystem.StoneMissed();
                }

                yield return null;
            }
            
            yield return null;
        }
    }
}
