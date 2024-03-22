namespace UnityEngine.XR.AR
{
    public class ARReticle : MonoBehaviour
    {
        [field: SerializeField]
        private GameObject CircleObject { get; set; }

        [field: SerializeField]
        private GameObject ShadowObject { get; set; }

        [field: SerializeField]
        private GameObject DepthMask { get; set; }

        [field: SerializeField]
        private Vector3 DepthMaskReferenceScale { get; set; }

        public void SetScale(float distance)
        {
            CircleObject.transform.localScale = Vector3.one * distance;
            ShadowObject.transform.localScale = Vector3.one * distance;
            DepthMask.transform.localScale = new Vector3(DepthMaskReferenceScale.x * distance, DepthMaskReferenceScale.y, DepthMaskReferenceScale.z * distance);
        }
    }
}