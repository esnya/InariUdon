
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EsnyaFactory.InariUdon
{
    public class UdonLogger : UdonSharpBehaviour
    {
        public int maxLines = 20;

        public TextMeshPro tmpText;
        public Text uiText;

        [HideInInspector] public bool initialized;

        int index;
        string text;

        void Start()
        {

            if (tmpText == null && uiText == null)
            {
                tmpText = GetComponentInChildren<TextMeshPro>();
                if (tmpText == null) uiText = GetComponentInChildren<Text>();
            }

            initialized = true;
            Log("Info", gameObject.name, "Initialized");
        }

        void AppendLine(string line)
        {
            text += string.IsNullOrEmpty(text) ? line : $"\n{line}";
        }

        public void Log(string level, string module, string log)
        {
            var time = System.DateTime.Now.ToString("HH:mm:ss.fff");
            var formattedLog = $"{level} {time} [{module}] {log}";;
            Debug.Log(formattedLog);

            if (!initialized) return;

            AppendLine(formattedLog);

            var lines = text.Split("\n".ToCharArray());
            if (lines.Length > maxLines)
            {
                text = "";
                for (int i = lines.Length - maxLines; i < lines.Length; i++) AppendLine(lines[i]);
            }

            if (tmpText != null) tmpText.text = text;
            if (uiText != null) uiText.text = text;
        }
    }
}
