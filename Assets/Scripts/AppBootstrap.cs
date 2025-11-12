using UnityEngine;

namespace DefaultNamespace
{
    public class AppBootstrap : MonoBehaviour
    {
        [SerializeField] private CalculatorView view;
        private CalculatorPresenter _presenter;
    
        private void Start()
        {
            var model = new CalculatorModel();
            var storage = new PlayerPrefsStorage();
            _presenter = new CalculatorPresenter(model, view, storage);
        }
    
        private void OnDestroy() => _presenter?.Dispose();
        private void OnApplicationQuit() => _presenter?.Dispose();
    }
}