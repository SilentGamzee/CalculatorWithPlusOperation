
using System;
using System.Collections.Generic;

public class Tokens
{
    private readonly List<string> _cachedTokens;

    public Tokens()
    {
        _cachedTokens = new List<string>(16);
    }
    
    public IReadOnlyList<string> Tokenize(string expression)
    {
        Tokenize(expression);
        return _cachedTokens;
    }
    
    private void Tokenize(ReadOnlySpan<char> expression)
    {
        _cachedTokens.Clear();
        int i = 0;
        int len = expression.Length;

        while (i < len)
        {
            char c = expression[i];

            if (char.IsWhiteSpace(c))
            {
                i++;
                continue;
            }

            if (char.IsDigit(c))
            {
                int start = i;
                i++;

                while (i < len && char.IsDigit(expression[i]))
                    i++;

                if (i < len && (expression[i] == '.' || expression[i] == ','))
                {
                    i++;
                    while (i < len && char.IsDigit(expression[i]))
                        i++;
                }

                _cachedTokens.Add(expression.Slice(start, i - start).ToString());
                continue;
            }

            _cachedTokens.Add(c.ToString());
            i++;
        }
    }
}