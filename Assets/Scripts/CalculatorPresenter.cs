using System;
using DefaultNamespace;

public class CalculatorPresenter
{
    private readonly CalculatorModel _model;
    private readonly CalculatorView _view;
    private readonly PlayerPrefsStorage  _storage;
    private string _lastExpr;
        
    public CalculatorPresenter(CalculatorModel model, CalculatorView view, PlayerPrefsStorage  storage)
    {
        _model = model;
        _view = view;
        _storage = storage;
            
        _view.OnCalculate += HandleCalculate;
        _model.OnStateChanged += UpdateView;
            
        LoadState();
    }
        
    private void HandleCalculate(string expr)
    {
        _lastExpr = expr;
        
        var success = _model.Calculate(expr.AsSpan());
        if (success)
            _model.SetExpression("");
        else
            _view.ShowError(() => _model.SetExpression(_lastExpr));
            
        SaveState();
    }
        
    private void UpdateView() => 
        _view.UpdateView(_model.Expression, _model.History);
        
    private void LoadState()
    {
        var state = _storage.Load();
        _model.LoadState(state.expr, state.history);
    }
        
    private void SaveState() => 
        _storage.Save(_model.Expression, _model.History);
        
    public void Dispose()
    {
        SaveState();
        _view.OnCalculate -= HandleCalculate;
        _model.OnStateChanged -= UpdateView;
    }
}