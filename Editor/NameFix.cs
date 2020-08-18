namespace EsnyaFactory {
  using System.Collections.Generic;
  using System.Linq;
  using UnityEngine;
  using UnityEditor;

  public class NameFix : EditorWindow {

    [MenuItem("EsnyaTools/Name Fix")]
    private static void ShowWindow() {
      var window = GetWindow<NameFix>();
      window.Show();
    }

    private List<GameObject> list = new List<GameObject>();
    private int progressMax = 0;
    private int progressValue = 0;

    private void OnEnable() {
      titleContent = new GUIContent("Name Fix");
    }

    private void OnGUI() {
      EEU.Button("Scan", () => {
        GenList();
      });

      EditorGUILayout.LabelField($"{list.Count} objects has children with duplicated names.");

      EEU.Disabled(list.Count == 0, () => {
        EEU.Button("Fix", () => {
          Fix();
        });
      });
    }

    private void GenList() {
      list.Clear();

      var all = FindObjectsOfType<GameObject>();
      progressValue = 0;
      progressMax = all.Length;

      EditorUtility.DisplayProgressBar("Name Fix", "Scanning", 0.0f);

      foreach (var o in all) {
        if (o.transform.parent == null) {
          GenList(o);
        }
      }

      EditorUtility.ClearProgressBar();
    }

    private void GenList(GameObject target) {
      progressValue++;
      EditorUtility.DisplayProgressBar("Name Fix", $"Scanning: {target.name}", (float)progressValue / progressMax);

      var table = new Dictionary<string, int>();
      for (int i = 0; i < target.transform.childCount; i++) {
        var child = target.transform.GetChild(i);
        table[child.name] = table.FirstOrDefault(p => p.Key == child.name).Value + 1;

        if (table.Any(p => p.Value >= 2)) {
          list.Add(target);
        }
      }

      for (int i = 0; i < target.transform.childCount; i++) {
        GenList(target.transform.GetChild(i).gameObject);
      }
    }

    private void Fix() {
      var progressMax = list.Count;
      var progressValue = 0;

      foreach (var target in list) {
        progressValue++;
        EditorUtility.DisplayProgressBar("Name Fix", $"Fix: {target.name}", (float)progressValue / progressMax);

        var table = new Dictionary<string, List<GameObject>>();
        for (int i = 0; i < target.transform.childCount; i++) {
          var child = target.transform.GetChild(i).gameObject;
          if (!table.ContainsKey(child.name)) {
            table[child.name] = new List<GameObject>();
          }
          table[child.name].Add(child);
        }

        table.Where(p => p.Value.Count() >= 2).Select(p => {
          p.Value.Select((o, i) => {
            o.name = $"{o.name}_{i}";

            return 0;
          }).ToList();
          return 0;
        }).ToList();
      }

      EditorUtility.ClearProgressBar();
      list.Clear();
    }
  }
}
