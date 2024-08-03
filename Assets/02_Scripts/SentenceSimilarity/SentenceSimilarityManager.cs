using TMPro;
using UnityEngine;
using UnityEngine.XR.AR;

public class SentenceSimilarityManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private ARAnchorCreator arAnchorCreator;

    private void Start()
    {
        arAnchorCreator.OnSecondAnchorPrefabCreated += SetSentenceTextInput;
    }

    protected void OnDestroy()
    {
        arAnchorCreator.OnSecondAnchorPrefabCreated -= SetSentenceTextInput;
    }

    private void SetSentenceTextInput(GameObject gameObject)
    {
        JammoBehavior jammoBehavior = gameObject.GetComponent<JammoBehavior>();
        jammoBehavior.inputField = inputField;
        inputField.onEndEdit.AddListener(jammoBehavior.OnOrderGiven);
    }
}
