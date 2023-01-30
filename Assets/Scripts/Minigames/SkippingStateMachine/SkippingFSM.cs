using UnityEngine;

namespace SkippingMinigame
{
    public abstract class SkippingFSM : MonoBehaviour
    {
        protected SkippingState State;

        public void SetState (SkippingState _state)
        {
            if (State != null) {StartCoroutine(State.OnExit());}   
            State = _state;
            StartCoroutine(State.OnEnter());
        }
    }
}