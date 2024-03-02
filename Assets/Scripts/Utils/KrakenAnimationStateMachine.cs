
using UnityEngine;

using Bytes;
using Kraken;

namespace Kraken
{
    public class KrakenAnimationStateMachine : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private IKrakenAnimState currentState;
        private Animate currentPlayOnceAnim;

        public void Initialize()
        {
            animator = GetComponentInChildren<Animator>();
        }

        public void SetLoopedState(IKrakenAnimState newState, string prefix = "", string suffix = "", bool force = true)
        {
            if (currentState == newState) { return; }

            currentState = newState;
            PlayStateLoopedAnimation(BuildClipName(prefix, suffix, newState.ClipName));
        }

        // Only use variation suffix if nbVariation is defined
        public void PlayAnimOnce(IKrakenAnimState animState, string prefix, string suffix = "", string speedParamName = "", float speedMult = 1f, System.Action callback = null, float animTransitionDuration = 0.3f)
        {
            if (animator == null) return;

            CancelCurrentPlayOnceAnim();

            bool paramExists = false;
            foreach (var par in animator.parameters) { if (par.name == speedParamName) paramExists = true; }

            if (speedParamName != "" && paramExists)
            {
                animator.SetFloat(speedParamName, speedMult);
            }

            currentPlayOnceAnim = PlayAnimatorClip(animator, BuildClipName(prefix, suffix, animState.ClipName, animState.NbVariations), () => {
                if (this == null) return;
                currentPlayOnceAnim = null;
                PlayStateLoopedAnimation(BuildClipName(prefix, suffix, currentState.ClipName));
                callback?.Invoke();
            }, speedMult, animTransitionDuration);
        }

        public void CancelCurrentPlayOnceAnim()
        {
            if (currentPlayOnceAnim != null)
            {
                currentPlayOnceAnim.Stop(callEndFunction: true);
            }
        }

        private void PlayStateLoopedAnimation(string clipName, bool force = false)
        {
            if (force != true && currentPlayOnceAnim != null || enabled == false || animator == null) { return; }
            animator?.CrossFade(clipName, 0.3f, 0);
        }

        private string BuildClipName(string prefix, string suffix, string clipName, int nbVariation = -1)
        {
            if (clipName == "empty") { return clipName; }

            string variationSuffix; ;
            if (nbVariation == -1) { variationSuffix = ""; }
            else { variationSuffix = Random.Range(1, nbVariation + 1).ToString(); }

            string suffixUsed = (suffix == "") ? "" : "_" + suffix;
            string f = prefix + "_" + clipName + suffixUsed + variationSuffix;

            return f;
        }

        public void Stop()
        {
            SetLoopedState(KrakenBaseAnimState.Nothing);
            currentPlayOnceAnim?.Stop(false);
        }

        public Animator GetAnimator()
        {
            return animator;
        }

        // Used to be able to set a callback at the end of animation played by target animator
        private Animate PlayAnimatorClip(Animator animator, string clipName, System.Action callback, float animSpeed, float animTransitionDuration = 0.0f)
        {
            float lDuration = GetAnimatorLayerDuration(animator, clipName) / animSpeed;
            animator.CrossFade(clipName, animTransitionDuration, 0);
            return Animate.Delay(lDuration, callback, true);
        }

        // Return animator clip (also known as a layer) duration by name (if it exists) 
        private float GetAnimatorLayerDuration(Animator pAnimator, string pClipName)
        {
            AnimationClip[] clips = pAnimator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (pClipName == clip.name) { return clip.length; }
            }
            return -1;
        }

        public IKrakenAnimState GetCurrentLoopedState() { return currentState; }
    }

    public interface IKrakenAnimState
    {
        string ClipName { get; }
        float AnimSpeed { get; }
        int NbVariations { get; }
    }

    public class KrakenBaseAnimState : IKrakenAnimState
    {
        static public readonly KrakenBaseAnimState Nothing = new KrakenBaseAnimState("empty", 1f);

        private readonly string _clipName = "";
        private readonly float _animSpeed = 1f;
        private readonly int _nbVariations = -1;

        public KrakenBaseAnimState(string pClipName, float pAnimSpeed = 1f, int pNbVariations = -1)
        {
            _clipName = pClipName;
            _animSpeed = pAnimSpeed;
            _nbVariations = pNbVariations;
        }

        public string ClipName { get { return _clipName; } }
        public float AnimSpeed { get { return _animSpeed; } }
        public int NbVariations { get { return _nbVariations; } }
    }
}
