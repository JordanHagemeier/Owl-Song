using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkippingMinigame
{
    public class InactiveState : SkippingState
    {
        public InactiveState(SkippingSystem skippingSystem) : base(skippingSystem)
        { }

        public override IEnumerator OnEnter()
        {

            SkippingSystem.m_MiniGameCamera.transform.position = SkippingSystem.InitCamera.position;

            if (SkippingSystem.Stone3D.gameObject.activeSelf)
            {
                SkippingSystem.Stone3D.SetActive(false);
            }

            if (SkippingSystem.QTE_UI.gameObject.activeSelf)
            {
                SkippingSystem.QTE_UI.gameObject.SetActive(false);
            }

            if (SkippingSystem.BarUI.gameObject.activeSelf)
            {
                SkippingSystem.BarUI.gameObject.SetActive(false);
            }

            Debug.Log("minigame inactive");
            yield break;
        }

        public override IEnumerator OnExit()
        {
            yield break;
        }
    }  
}
