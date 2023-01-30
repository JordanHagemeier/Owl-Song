using System.Collections;

namespace SkippingMinigame
{
    public abstract class SkippingState
    {
        protected SkippingSystem SkippingSystem;

        public SkippingState (SkippingSystem skippingSystem)
        {
            SkippingSystem = skippingSystem;
        }


        public virtual IEnumerator OnEnter()
        {
            yield break;
        }
        public virtual IEnumerator OnInteract()
        {
            yield break;
        }

        public virtual IEnumerator OnUpdate()
        {
            yield break;
        }

        public virtual IEnumerator OnExit()
        {
            yield break;
        }
    }
}
