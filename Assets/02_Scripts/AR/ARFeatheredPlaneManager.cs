using UnityEngine.XR.ARFoundation;

namespace UnityEngine.XR.AR
{
    [RequireComponent(typeof(ARPlane))]
    [RequireComponent(typeof(Animator))]
    public class ARFeatheredPlaneManager : MonoBehaviour
    {
        private readonly string FADE_OFF_ANIMATION = "FadeOff";
        private readonly string FADE_ON_ANIMATION = "FadeOn";
        private readonly float TIMEOUT = 2.0f;
        private float showTime;
        private bool timerOn;
        private bool timerLocked;

        private Animator AnimatorComponent { get; set; }
        private ARPlane ARPlaneComponent { get; set; }

        protected virtual void Awake()
        {
            showTime = 0;
            timerOn = false;
            timerLocked = false;
        }

        protected virtual void OnEnable()
        {
            ARPlaneComponent = GetComponent<ARPlane>();
            AnimatorComponent = GetComponent<Animator>();

            ARPlaneComponent.boundaryChanged += HandleBoundaryChanged;

            if (ARPointer.Instance) ARPointer.Instance.OnHitPointerToggled += HandleHitPointerToggled;
            if (ARManager.Instance) ARManager.Instance.OnGamePlacedEvent += HandleGamePlaced;
        }

        protected virtual void OnDisable()
        {
            ARPlaneComponent.boundaryChanged -= HandleBoundaryChanged;

            if (ARPointer.Instance) ARPointer.Instance.OnHitPointerToggled -= HandleHitPointerToggled;
            if (ARManager.Instance) ARManager.Instance.OnGamePlacedEvent -= HandleGamePlaced;
        }

        protected virtual void Update()
        {
            if (timerLocked)
            {
                return;
            }

            if (timerOn)
            {
                showTime -= Time.deltaTime;

                if (showTime <= 0)
                {
                    timerOn = false;
                    AnimatorComponent.SetBool(FADE_OFF_ANIMATION, true);
                    AnimatorComponent.SetBool(FADE_ON_ANIMATION, false);
                }
            }
        }

        private void HandleBoundaryChanged(ARPlaneBoundaryChangedEventArgs eventArgs)
        {
            if (timerLocked)
            {
                return;
            }

            AnimatorComponent.SetBool(FADE_OFF_ANIMATION, false);
            AnimatorComponent.SetBool(FADE_ON_ANIMATION, true);

            timerOn = true;
            showTime = TIMEOUT;
        }

        private void HandleHitPointerToggled(bool hit)
        {
            if (hit)
            {
                timerOn = true;
                showTime = TIMEOUT;
                timerLocked = false;
            }
            else
            {
                AnimatorComponent.SetBool(FADE_OFF_ANIMATION, false);
                AnimatorComponent.SetBool(FADE_ON_ANIMATION, true);
                timerOn = true;
                showTime = TIMEOUT;
                timerLocked = true;
            }
        }

        private void HandleGamePlaced()
        {
            AnimatorComponent.SetBool(FADE_OFF_ANIMATION, true);
            AnimatorComponent.SetBool(FADE_ON_ANIMATION, false);
            timerOn = true;
            showTime = TIMEOUT;
            timerLocked = true;
        }
    }
}