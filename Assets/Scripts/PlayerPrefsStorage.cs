using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class PlayerPrefsStorage
    {
        private const string ExprKey = "Calc_Expr";
        private const string HistKey = "Calc_Hist";

        public void Save(string expr, IReadOnlyList<string> history)
        {
            var historyJson = JsonUtility.ToJson(new Wrapper { items = new List<string>(history) });

            PlayerPrefs.SetString(ExprKey, expr);
            PlayerPrefs.SetString(HistKey, historyJson);
            PlayerPrefs.Save();
        }

        public (string expr, List<string> history) Load()
        {
            string expr = PlayerPrefs.GetString(ExprKey, "");
            string json = PlayerPrefs.GetString(HistKey, "");

            List<string> history = new List<string>();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    history = JsonUtility.FromJson<Wrapper>(json).items ?? history;
                }
                catch
                {
                    // ignored
                }
            }

            return (expr, history);
        }

        [System.Serializable]
        private class Wrapper
        {
            public List<string> items;
        }
    }
}