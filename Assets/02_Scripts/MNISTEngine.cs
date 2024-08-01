using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.Sentis
{
    public class MNISTEngine : SingletonMonoBehaviour<MNISTEngine>
    {
        [SerializeField] private ModelAsset _modelAsset;

        public readonly Dictionary<int, int> _map = new()
        {
            { 0, 48 }, { 1, 49 }, { 2, 50 }, { 3, 51 }, { 4, 52 }, { 5, 53 }, { 6, 54 }, { 7, 55 }, { 8, 56 },
            { 9, 57 }, { 10, 65 }, { 11, 66 }, { 12, 67 }, { 13, 68 }, { 14, 69 }, { 15, 70 }, { 16, 71 }, { 17, 72 },
            { 18, 73 }, { 19, 74 }, { 20, 75 }, { 21, 76 }, { 22, 77 }, { 23, 78 }, { 24, 79 }, { 25, 80 }, { 26, 81 },
            { 27, 82 }, { 28, 83 }, { 29, 84 }, { 30, 85 }, { 31, 86 }, { 32, 87 }, { 33, 88 }, { 34, 89 }, { 35, 90 },
            { 36, 97 }, { 37, 98 }, { 38, 100 }, { 39, 101 }, { 40, 102 }, { 41, 103 }, { 42, 104 }, { 43, 110 },
            { 44, 113 }, { 45, 114 }, { 46, 116 }
        };

        private readonly BackendType _backendType = BackendType.GPUCompute;

        public const int imageWidth = 28;

        private Model _runtimeModel;
        private Tensor _inputTensor;
        private IWorker _worker;
        private TensorFloat _outputTensor;
        private Ops _operations;

        private Camera _lookCamera;

        private void Start()
        {
            _lookCamera = Camera.main;
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

        protected override void OnDestroy()
        {
            _inputTensor?.Dispose();
            _worker?.Dispose();
            _operations?.Dispose();
            _outputTensor?.Dispose();
        }

        private void MouseClicked()
        {
            Ray ray = _lookCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.name == "Screen")
            {
                ControlPanel panel = hit.collider.GetComponentInParent<ControlPanel>();
                if (!panel) return;
                panel.ScreenMouseDown(hit);
            }
        }

        private void MouseIsDown()
        {
            Ray ray = _lookCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.name == "Screen")
            {
                ControlPanel panel = hit.collider.GetComponentInParent<ControlPanel>();
                if (!panel) return;
                panel.ScreenGetMouse(hit);
            }
        }

        public (float, int) GetResults(Texture2D texture)
        {
            OnDestroy();

            _runtimeModel = ModelLoader.Load(_modelAsset);
            _inputTensor = TextureConverter.ToTensor(texture, imageWidth, imageWidth, 1);
            _worker = WorkerFactory.CreateWorker(_backendType, _runtimeModel);
            _worker.Execute(_inputTensor);
            _operations = WorkerFactory.CreateOps(_backendType, null);
            _outputTensor = _worker.PeekOutput() as TensorFloat;

            TensorFloat probabilities = _operations.Softmax(_outputTensor);

            probabilities.MakeReadable();

            float[] results = probabilities.ToReadOnlyArray();

            float probability = results.Max();
            int predictedIndex = results.ToList().IndexOf(probability);

            return (probability, predictedIndex);
        }
    }
}