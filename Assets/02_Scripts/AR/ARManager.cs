using System;
using UnityEngine.XR.ARFoundation;

namespace UnityEngine.XR.AR
{
    public class ARManager : SingletonMonoBehaviour<ARManager>
    {
        [field: SerializeField] private ARPlaneManager ARPlaneManager { get; set; }
        [field: SerializeField] private ARSession ARSession { get; set; }

        public event Action OnGamePlacedEvent;

        protected override void OnDestroy()
        {
            base.OnDestroy();

            ARPlaneManager.enabled = false;

            foreach (ARPlane plane in ARPlaneManager.trackables)
            {
                plane.gameObject.SetActive(false);
            }

            ARSession.Reset();
        }

        public void PlaceGame()
        {
            if (ARPointer.Instance.IsTargetReachable == false)
            {
                return;
            }

            ARPlaneManager.subsystem.Stop();

            OnGamePlacedEvent?.Invoke();
        }
    }
}