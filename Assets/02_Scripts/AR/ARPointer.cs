using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.AR
{
    public class ARPointer : SingletonMonoBehaviour<ARPointer>
    {
        public event Action<bool> OnHitPointerToggled = delegate { };
        public bool IsTargetReachable { get; private set; }
        public Vector3 TargetPosition { get; private set; }
        public Quaternion TargetRotation { get; private set; }

        [field: SerializeField]
        private ARReticle HitPointerObject { get; set; }
        [field: SerializeField]
        private ARReticle MissPointerObject { get; set; }
        private ARRaycastManager ARRaycastManagerComponent { get; set; }
        private bool UpdatePointer { get; set; }

        protected override void Awake()
        {
            base.Awake();

            if (ARRaycastManagerComponent == null)
            {
                ARRaycastManagerComponent = GetComponent<ARRaycastManager>();
            }
        }

        protected virtual void Start()
        {
            HitPointerObject.gameObject.SetActiveOptimized(false);
            MissPointerObject.gameObject.SetActiveOptimized(false);

            TargetPosition = new Vector3();
            TargetRotation = new Quaternion();
            IsTargetReachable = false;

            UpdatePointer = true;
        }

        protected virtual void OnEnable()
        {
            StartCoroutine(SubscribeOnGamePlace());
        }

        protected virtual void Update()
        {
            if (UpdatePointer == false)
            {
                return;
            }

            List<ARRaycastHit> hitPoints = new List<ARRaycastHit>();
            ARRaycastManagerComponent.Raycast(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f), hitPoints, TrackableType.Planes);

            ARRaycastHit hitPoint;

            if (hitPoints.Count > 0)
            {
                hitPoint = hitPoints[0];

                for (int index = 0; index < hitPoints.Count; index++)
                {
                    if (hitPoints[index].hitType.HasFlag(TrackableType.PlaneWithinPolygon) == true)
                    {
                        hitPoint = hitPoints[index];
                        break;
                    }
                }

                if (hitPoint.hitType.HasFlag(TrackableType.PlaneWithinPolygon) == true)
                {
                    if (HitPointerObject.gameObject.activeSelf == false)
                    {
                        HitPointerObject.gameObject.SetActive(true);
                        MissPointerObject.gameObject.SetActive(false);

                        OnHitPointerToggled(true);
                    }
                }
                else
                {
                    if (MissPointerObject.gameObject.activeSelf == false)
                    {
                        HitPointerObject.gameObject.SetActiveOptimized(false);
                        MissPointerObject.gameObject.SetActiveOptimized(true);

                        OnHitPointerToggled(false);
                    }
                }

                TargetPosition = hitPoint.pose.position;
                TargetRotation = hitPoint.pose.rotation;

                HitPointerObject.SetScale(hitPoint.sessionRelativeDistance);
                MissPointerObject.SetScale(hitPoint.sessionRelativeDistance);

                if (IsTargetReachable == false)
                {
                    HitPointerObject.transform.position = TargetPosition;
                    MissPointerObject.transform.position = TargetPosition;

                    HitPointerObject.transform.rotation = TargetRotation;
                    MissPointerObject.transform.rotation = TargetRotation;

                    IsTargetReachable = true;
                }

                HitPointerObject.transform.position = Vector3.Lerp(HitPointerObject.transform.position, TargetPosition, 5f * Time.deltaTime);
                MissPointerObject.transform.position = HitPointerObject.transform.position;

                HitPointerObject.transform.rotation = Quaternion.Lerp(HitPointerObject.transform.rotation, TargetRotation, 5f * Time.deltaTime);
                MissPointerObject.transform.rotation = HitPointerObject.transform.rotation;
            }
            else
            {
                HitPointerObject.gameObject.SetActiveOptimized(false);
                MissPointerObject.gameObject.SetActiveOptimized(false);

                IsTargetReachable = false;
            }
        }

        protected virtual void OnDisable()
        {
            if (ARManager.Instance != null)
            {
                ARManager.Instance.OnGamePlacedEvent -= OnGamePlacedCallback;
            }
        }

        private IEnumerator SubscribeOnGamePlace()
        {
            while (ARManager.Instance == null)
            {
                yield return null;
            }
            ARManager.Instance.OnGamePlacedEvent += OnGamePlacedCallback;
        }

        private void OnGamePlacedCallback()
        {
            UpdatePointer = false;

            HitPointerObject.gameObject.SetActiveOptimized(false);
            MissPointerObject.gameObject.SetActiveOptimized(false);
            IsTargetReachable = false;
        }
    }
}