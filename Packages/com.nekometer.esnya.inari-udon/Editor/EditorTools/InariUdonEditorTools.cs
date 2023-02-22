using System;
using System.Linq;
using UdonSharp;
using UdonSharpEditor;
using UnityEditor;

namespace InariUdon.EditorTools
{
    public class InariUdonEditorTools
    {
        [MenuItem("Assets/InariUdon/Create UdonSharpProgramAsset")]
        private static void CreateUdonSharpProgramAssetMenu()
        {
            var sourceCsScripts = Selection.objects
                .Where(o => o is MonoScript)
                .Select(o => o as MonoScript)
                .Where(s => !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(s)))
                .ToArray();

            using (new AssetEditingScope())
            {
                using (var progressBar = new ProgressBarScope("Creating UdonSharpProgramAssets", "Initializing"))
                {
                    var i = 0;
                    progressBar.maxValue = sourceCsScripts.Length;

                    foreach (var sourceCsScript in sourceCsScripts)
                    {
                        progressBar.UpdateProgress(sourceCsScript.name, i++);
                        CreateUdonSharpProgramAsset(AssetDatabase.GetAssetPath(sourceCsScript).Replace(".cs", ".asset"), sourceCsScript);
                    }
                }
            }
        }

        public static void CreateUdonSharpProgramAsset(string path, MonoScript sourceCsScript)
        {
            var asset = new UdonSharpProgramAsset
            {
                sourceCsScript = sourceCsScript
            };
            AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(path));
        }
    }

    public class AssetEditingScope : IDisposable
    {
        public AssetEditingScope()
        {
            AssetDatabase.StartAssetEditing();
        }

        public void Dispose()
        {
            AssetDatabase.StopAssetEditing();
        }
    }

    public class ProgressBarScope : IDisposable
    {
        public float maxValue = 1.0f;

        private string title;

        public ProgressBarScope(string title, string info)
        {
            UpdateProgress(title, info, 0);
        }

        public void UpdateProgress(string title, string info, float value)
        {
            this.title = title;
            UpdateProgress(info, 0);
        }

        public void UpdateProgress(string info, float value)
        {
            EditorUtility.DisplayProgressBar(title, info, value / maxValue);
        }

        public void Dispose()
        {
            EditorUtility.ClearProgressBar();
        }
    }
}
