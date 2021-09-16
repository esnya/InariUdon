
using UdonSharp;
using UnityEngine;
using TMPro;
using UdonToolkit;

namespace InariUdon.UI
{
    [
        CustomName("Udon Logger"),
        HelpMessage("Rich log viewer in world with colord log-levels, timestamp and etc."),
        DefaultExecutionOrder(-1000),
        UdonBehaviourSyncMode(BehaviourSyncMode.None),
    ]
    public class UdonLogger : UdonSharpBehaviour
    {
        public int maxCharacters = 10000;
        public TextMeshProUGUI text;

        [ListView("Levels")] public string[] levels = {
            "Unknown",
            "Debug",
            "Info",
            "Notice",
            "Warning",
            "Error",
            "Fatal",
        };
        [ListView("Levels")] public Color[] colors = {
            Color.gray,
            Color.white,
            Color.green,
            Color.yellow,
            Color.red,
            Color.red,
        };
        public bool levelsIgnoreCase = true;

        [HideInInspector] public bool initialized;

        private string str;
        private string[] colorCodes;
        private void Start()
        {
            colorCodes = new string[colors.Length];
            for (int i = 0; i < colors.Length; i++)
            {
                var c = colors[i];
                var r = Mathf.FloorToInt(c.r * 255).ToString("X2");
                var g = Mathf.FloorToInt(c.g * 255).ToString("X2");
                var b = Mathf.FloorToInt(c.b * 255).ToString("X2");
                colorCodes[i] = $"#{r}{g}{b}";
            }
            initialized = true;
            Log("Info", gameObject.name, "Initialized");
        }

        private void AppendLine(string line)
        {
            str += string.IsNullOrEmpty(str) ? line : $"\n{line}";
        }

        private int GetLevelIndex(string level)
        {
            if (levelsIgnoreCase) level = level.ToLower();
            for (int i = 0; i < levels.Length; i++)
            {
                var l = levels[i];
                if (levelsIgnoreCase) l = l.ToLower();
                if (level == l) return i;
            }
            return 0;
        }

        public void Log(string level, string module, string log)
        {
            if (!initialized)
            {
                Debug.Log($"{level} [{module}] {log}");
                return;
            }

            var time = System.DateTime.Now.ToString("HH:mm:ss.fff");
            var levelIndex = GetLevelIndex(level);
            var color = colorCodes[levelIndex];
            var formattedLog = $"<color={color}>{level}</color> {time} [{module}] {log}";

            AppendLine(formattedLog);

            if (str.Length >= maxCharacters)
            {
                str = str.Substring(str.Length - maxCharacters);
            }

            text.text = str;
        }
    }
}
