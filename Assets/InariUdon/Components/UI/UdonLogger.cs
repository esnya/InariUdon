
using UdonSharp;
using UnityEngine;
using TMPro;

namespace EsnyaFactory.InariUdon
{
    public class UdonLogger : UdonSharpBehaviour
    {
        public int maxLines = 20;

        public TextMeshPro tmpText;

        [HideInInspector] public bool initialized;

        int index;
        string text;

        void Start()
        {

            if (tmpText == null) tmpText = GetComponentInChildren<TextMeshPro>();

            initialized = true;
            Log("Info", "Logger", "Initialized");
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

            if (!initialized || tmpText == null) return;

            AppendLine(formattedLog);

            var lines = text.Split("\n".ToCharArray());
            if (lines.Length > maxLines)
            {
                text = "";
                for (int i = lines.Length - maxLines; i < lines.Length; i++) AppendLine(lines[i]);
            }

            tmpText.text = text;
        }
    }
}
