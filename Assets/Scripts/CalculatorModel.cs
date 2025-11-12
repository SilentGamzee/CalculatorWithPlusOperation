using System;
using System.Collections.Generic;

public class CalculatorModel
{
    public string Expression => _expression;
    public IReadOnlyList<string> History => _history;

    public event Action OnStateChanged;

    private string _expression = "";
    private readonly List<string> _history = new(32);
    private readonly char[] _buffer = new char[64];

    public void SetExpression(string value)
    {
        _expression = value ?? "";
        OnStateChanged?.Invoke();
    }

    public bool Calculate(ReadOnlySpan<char> expr)
    {
        if (expr.IsEmpty || expr.IsWhiteSpace())
        {
            _history.Add(FormatEntry(expr, "Error"));
            return false;
        }

        int plusIndex = -1;
        for (int i = 0; i < expr.Length; i++)
        {
            if (expr[i] == '+')
            {
                if (plusIndex != -1)
                {
                    _history.Add(FormatEntry(expr, "Error"));
                    return false;
                }

                plusIndex = i;
            }
            else if (!char.IsDigit(expr[i]))
            {
                _history.Add(FormatEntry(expr, "Error"));
                return false;
            }
        }

        if (plusIndex <= 0 || plusIndex >= expr.Length - 1)
        {
            _history.Add(FormatEntry(expr, "Error"));
            return false;
        }

        if (!TryParseInt(expr.Slice(0, plusIndex), out int a) ||
            !TryParseInt(expr.Slice(plusIndex + 1), out int b))
        {
            _history.Add(FormatEntry(expr, "Error"));
            return false;
        }

        long result = (long)a + b;
        if (result > int.MaxValue || result < int.MinValue)
            return false;

        _history.Add(FormatEntry(expr, result.ToString()));
        OnStateChanged?.Invoke();

        return true;
    }

    public void LoadState(string expr, List<string> history)
    {
        _expression = expr ?? "";
        _history.Clear();
        if (history != null) _history.AddRange(history);
        OnStateChanged?.Invoke();
    }

    private bool TryParseInt(ReadOnlySpan<char> span, out int result)
    {
        result = 0;
        if (span.IsEmpty) return false;

        for (int i = 0; i < span.Length; i++)
        {
            if (!char.IsDigit(span[i])) return false;
            int digit = span[i] - '0';
            if (result > (int.MaxValue - digit) / 10) return false;
            result = result * 10 + digit;
        }

        return true;
    }

    private string FormatEntry(ReadOnlySpan<char> expr, string result)
    {
        expr.CopyTo(_buffer);
        int len = expr.Length;
        _buffer[len] = ' ';
        _buffer[len + 1] = '=';
        _buffer[len + 2] = ' ';

        for (int i = 0; i < result.Length; i++)
            _buffer[len + 3 + i] = result[i];

        return new string(_buffer, 0, len + 3 + result.Length);
    }
}