using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class CalculatorView : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _input;
        [SerializeField] private Button _calcButton;
        [SerializeField] private TMP_Text _historyText;
        [SerializeField] private ScrollRect _scroll;
        [SerializeField] private GameObject _errorPanel;
        [SerializeField] private Button _errorClose;

        public event Action<string> OnCalculate;
        
        private readonly StringBuilder _sb = new(512);
        private Action _errorCallback;
        private RectTransform _historyContainerRect;

        private void Awake()
        {
            _historyContainerRect = _historyText.transform.parent as RectTransform;
        }

        private void Start()
        {
            _calcButton.onClick.AddListener(() => OnCalculate?.Invoke(_input.text));
            _errorClose.onClick.AddListener(CloseError);
            _errorPanel.SetActive(false);
        }

        public void UpdateView(string expression, IReadOnlyList<string> history)
        {
            _input.text = expression;

            _sb.Clear();
            for (int i = 0; i < history.Count; i++)
            {
                if (i > 0) _sb.Append('\n');
                _sb.Append(history[i]);
            }

            _historyText.text = _sb.ToString();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_historyContainerRect);
            _scroll.verticalNormalizedPosition = 0f;
        }

        public void ShowError(Action onClose)
        {
            _errorCallback = onClose;
            _errorPanel.SetActive(true);
        }

        private void CloseError()
        {
            _errorPanel.SetActive(false);
            _errorCallback?.Invoke();
            _errorCallback = null;
        }

        private void OnDestroy()
        {
            _calcButton.onClick.RemoveAllListeners();
            _errorClose.onClick.RemoveAllListeners();
        }
    }
}