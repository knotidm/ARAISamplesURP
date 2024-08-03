using System;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.AR
{
    //
    // This script allows us to create anchors with
    // a prefab attached in order to visbly discern where the anchors are created.
    // Anchors are a particular point in space that you are asking your device to track.
    //

    [RequireComponent(typeof(ARAnchorManager))]
    [RequireComponent(typeof(ARRaycastManager))]
    [RequireComponent(typeof(ARPlaneManager))]
    public class ARAnchorCreator : MonoBehaviour
    {
        private static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
        private List<ARAnchor> m_AnchorPoints;
        private ARRaycastManager m_RaycastManager;
        private ARAnchorManager m_AnchorManager;
        private ARPlaneManager m_PlaneManager;

        // This is the prefab that will appear every time an anchor is created.
        [SerializeField] private GameObject m_AnchorPrefab;
        [SerializeField] private GameObject m_AnchorPrefab2;

        public bool justOnce;

        private bool isFirstPrefabInstantiated;
        private bool isSecondPrefabInstantiated;

        public Action<GameObject> OnFirstAnchorPrefabCreated;
        public Action<GameObject> OnSecondAnchorPrefabCreated;

        // Removes all the anchors that have been created.
        public void RemoveAllAnchors()
        {
            foreach (var anchor in m_AnchorPoints)
            {
                Destroy(anchor);
            }
            m_AnchorPoints.Clear();
        }

        // On Awake(), we obtains a reference to all the required components.
        // The ARRaycastManager allows us to perform raycasts so that we know where to place an anchor.
        // The ARPlaneManager detects surfaces we can place our objects on.
        // The ARAnchorManager handles the processing of all anchors and updates their position and rotation.
        private void Awake()
        {
            m_RaycastManager = GetComponent<ARRaycastManager>();
            m_AnchorManager = GetComponent<ARAnchorManager>();
            m_PlaneManager = GetComponent<ARPlaneManager>();
            m_AnchorPoints = new List<ARAnchor>();
        }

        private void Update()
        {
            if (justOnce && isFirstPrefabInstantiated && isSecondPrefabInstantiated)
                return;
            // If there is no tap, then simply do nothing until the next call to Update().
            if (Input.touchCount == 0)
                return;

            var touch = Input.GetTouch(0);
            if (touch.phase != TouchPhase.Began)
                return;

            if (m_RaycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                // Raycast hits are sorted by distance, so the first one
                // will be the closest hit.
                Pose hitPose = s_Hits[0].pose;
                hitPose.rotation = Quaternion.identity;
                TrackableId hitTrackableId = s_Hits[0].trackableId;
                ARPlane hitPlane = m_PlaneManager.GetPlane(hitTrackableId);

                // This attaches an anchor to the area on the plane corresponding to the raycast hit,
                // and afterwards instantiates an instance of your chosen prefab at that point.
                // This prefab instance is parented to the anchor to make sure the position of the prefab is consistent
                // with the anchor, since an anchor attached to an ARPlane will be updated automatically by the ARAnchorManager as the ARPlane's exact position is refined.
                ARAnchor anchor = m_AnchorManager.AttachAnchor(hitPlane, hitPose);

                GameObject anchorPrefab = isFirstPrefabInstantiated ? Instantiate(m_AnchorPrefab2, anchor.transform) : Instantiate(m_AnchorPrefab, anchor.transform);

                if (anchor == null)
                {
                    Debug.Log("Error creating anchor.");
                }
                else
                {
                    // Stores the anchor so that it may be removed later.
                    m_AnchorPoints.Add(anchor);

                    if (isFirstPrefabInstantiated)
                    {
                        OnSecondAnchorPrefabCreated?.Invoke(anchorPrefab);
                        isSecondPrefabInstantiated = true;
                    }
                    else
                    {
                        OnFirstAnchorPrefabCreated?.Invoke(anchorPrefab);
                        isFirstPrefabInstantiated = true;
                    }

                }
            }
        }

    }
}