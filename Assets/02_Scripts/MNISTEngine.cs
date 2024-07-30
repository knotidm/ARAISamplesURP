using UnityEngine;

namespace Unity.Sentis
{
    public class MNISTEngine : SingletonMonoBehaviour<MNISTEngine>
    {
        [SerializeField] private ModelAsset mnistONNX;

        private IWorker worker;
        private readonly BackendType backendType = BackendType.GPUCompute;

        public const int imageWidth = 28;

        private TensorFloat inputTensor = null;
        private Ops operations;
        private Camera lookCamera;

        private void Start()
        {
            Model model = ModelLoader.Load(mnistONNX);
            worker = WorkerFactory.CreateWorker(backendType, model);
            operations = WorkerFactory.CreateOps(backendType, null);

            lookCamera = Camera.main;
        }

        public (float, int) GetMostLikelyDigitProbability(Texture2D drawableTexture)
        {
            inputTensor?.Dispose();
            inputTensor = TextureConverter.ToTensor(drawableTexture, imageWidth, imageWidth, 1);
            worker.Execute(inputTensor);

            TensorFloat result = worker.PeekOutput() as TensorFloat;
            TensorFloat probabilities = operations.Softmax(result);
            TensorInt predictedNumbers = operations.ArgMax(probabilities, -1, false);

            probabilities.MakeReadable();
            predictedNumbers.MakeReadable();

            int predictedNumber = predictedNumbers[0];
            float probability = probabilities[predictedNumber];

            return (probability, predictedNumber);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                MouseClicked();
            }
            else if (Input.GetMouseButton(0))
            {
                MouseIsDown();
            }
        }

        private void MouseClicked()
        {
            Ray ray = lookCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.name == "Screen")
            {
                ControlPanel panel = hit.collider.GetComponentInParent<ControlPanel>();
                if (!panel) return;
                panel.ScreenMouseDown(hit);
            }
        }

        private void MouseIsDown()
        {
            Ray ray = lookCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.name == "Screen")
            {
                ControlPanel panel = hit.collider.GetComponentInParent<ControlPanel>();
                if (!panel) return;
                panel.ScreenGetMouse(hit);
            }
        }

        protected override void OnDestroy()
        {
            inputTensor?.Dispose();
            worker?.Dispose();
            operations?.Dispose();
        }
    }
}