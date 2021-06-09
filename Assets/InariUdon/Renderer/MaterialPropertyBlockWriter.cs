
using UdonSharp;
using UnityEngine;
using Collections.Pooled;
#if !COMPILER_UDONSHARP && UNITY_EDITOR
using System.Linq;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine.SceneManagement;
using UnityEditor;
using UdonSharpEditor;
#endif

namespace EsnyaFactory.InariUdon
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class MaterialPropertyBlockWriter : UdonSharpBehaviour
    {
        public bool onStart = true;

        public bool writeColors;
        public Renderer[] colorTargets = { };
        public int[] colorIndices = { };
        public string[] colorNames = { };
        public Color[] colorValues = { };

        public bool writeFloats;
        public Renderer[] floatTargets = { };
        public int[] floatIndices = { };
        public string[] floatNames = { };
        public float[] floatValues = { };

        public bool writeTextures;
        public Renderer[] textureTargets = { };
        public int[] textureIndices = { };
        public string[] textureNames = { };
        public Texture[] textureValues = { };

        private void Start()
        {
            if (onStart) Trigger();
        }

        public void Trigger()
        {
            var block = new MaterialPropertyBlock();

            if (writeColors)
            {
                for (int i = 0; i < colorTargets.Length; i++)
                {
                    var target = colorTargets[i];
                    var materialIndex = colorIndices[i];
                    target.GetPropertyBlock(block, materialIndex);
                    block.SetColor(colorNames[i], colorValues[i]);
                    target.SetPropertyBlock(block, materialIndex);
                }
            }

            if (writeFloats)
            {
                for (int i = 0; i < floatTargets.Length; i++)
                {
                    var target = floatTargets[i];
                    var materialIndex = floatIndices[i];
                    target.GetPropertyBlock(block, materialIndex);
                    block.SetFloat(floatNames[i], floatValues[i]);
                    target.SetPropertyBlock(block, materialIndex);
                }
            }

            if (writeTextures)
            {
                for (int i = 0; i < textureTargets.Length; i++)
                {
                    var target = textureTargets[i];
                    var materialIndex = textureIndices[i];
                    target.GetPropertyBlock(block, materialIndex);
                    block.SetTexture(textureNames[i], textureValues[i]);
                    target.SetPropertyBlock(block, materialIndex);
                }
            }
        }

        private void ClearProperties(Renderer[] renderers)
        {
            var properties = new MaterialPropertyBlock();
            foreach (var renderer in renderers)
            {
                for (int i = 0; i < renderer.sharedMaterials.Length; i++) renderer.SetPropertyBlock(properties, i);
            }
        }

        public void ClearTargetProperties()
        {
            ClearProperties(colorTargets);
            ClearProperties(floatTargets);
            ClearProperties(textureTargets);
        }
    }

#if !COMPILER_UDONSHARP && UNITY_EDITOR
    public class MaterialPropertyList : ReorderableList
    {
        private const int fieldCount = 4;
        public static IEnumerable<int> GetShaderPropertyIndices(Shader shader)
        {
            return Enumerable.Range(0, ShaderUtil.GetPropertyCount(shader));
        }
        public static IEnumerable<int> GetShaderPropertyIndices(Shader shader, ShaderUtil.ShaderPropertyType propertyType)
        {
            return GetShaderPropertyIndices(shader).Where(i => ShaderUtil.GetPropertyType(shader, i) == propertyType);
        }
        public static IEnumerable<string> GetShaderPropertyNames(Shader shader)
        {
            return GetShaderPropertyIndices(shader).Select(i => ShaderUtil.GetPropertyName(shader, i));
        }
        public static IEnumerable<string> GetShaderPropertyNames(Shader shader, ShaderUtil.ShaderPropertyType propertyType)
        {
            return GetShaderPropertyIndices(shader, propertyType).Select(i => ShaderUtil.GetPropertyName(shader, i));
        }
        public static int GetIndexOfProperty(Shader shader, string name)
        {
            return GetShaderPropertyIndices(shader).Where(i => ShaderUtil.GetPropertyName(shader, i) == name).Append(-1).First();
        }
        public static string GetShaderPropertyName(Shader shader, int index)
        {
            return ShaderUtil.GetPropertyName(shader, Mathf.Clamp(index, 0, ShaderUtil.GetPropertyCount(shader)));
        }

        public MaterialPropertyList(
            SerializedObject serializedObject,
            SerializedProperty enabledProperty,
            SerializedProperty targetsProperty,
            ShaderUtil.ShaderPropertyType propertyType
        ) : base(serializedObject, targetsProperty)
        {
            var margin = EditorStyles.objectField.margin;
            var emptyLabel = new GUIContent();

            var indicesProperty = serializedObject.FindProperty(targetsProperty.name.Replace("Targets", "Indices"));
            var namesProperty = serializedObject.FindProperty(targetsProperty.name.Replace("Targets", "Names"));
            var valuesProperty = serializedObject.FindProperty(targetsProperty.name.Replace("Targets", "Values"));

            bool enabled = true;

            drawHeaderCallback = (rect) =>
            {
                enabled = EditorGUI.ToggleLeft(margin.Add(rect), enabledProperty.displayName, enabledProperty.boolValue);
                enabledProperty.boolValue = enabled;
            };

            drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                using (new EditorGUI.DisabledGroupScope(!enabled))
                {
                    var fieldRect = rect;
                    fieldRect.xMax /= fieldCount;
                    fieldRect = margin.Remove(fieldRect);

                    var targetProperty = targetsProperty.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(fieldRect, targetProperty, emptyLabel);
                    fieldRect.x += rect.width / fieldCount;

                    var target = targetProperty.objectReferenceValue as Renderer;
                    var indexProperty = indicesProperty.GetArrayElementAtIndex(index);
                    if (target != null)
                    {
                        indexProperty.intValue = EditorGUI.Popup(fieldRect, indexProperty.intValue, target.sharedMaterials.Select(m => m.name).ToArray());
                    }
                    else EditorGUI.PropertyField(fieldRect, indexProperty, emptyLabel);
                    fieldRect.x += rect.width / fieldCount;

                    var shader = target?.sharedMaterials.Skip(indexProperty.intValue).FirstOrDefault()?.shader;
                    var nameProperty = namesProperty.GetArrayElementAtIndex(index);
                    if (shader != null)
                    {
                        var propertyIndices = Enumerable.Range(0, ShaderUtil.GetPropertyCount(shader)).Where(i => ShaderUtil.GetPropertyType(shader, i) == propertyType);

                        var selected = EditorGUI.Popup(
                            fieldRect,
                            GetIndexOfProperty(shader, nameProperty.stringValue),
                            GetShaderPropertyNames(shader, propertyType).ToArray()
                        );

                        nameProperty.stringValue = ShaderUtil.GetPropertyName(shader, selected);
                    }
                    else EditorGUI.PropertyField(fieldRect, nameProperty, emptyLabel);
                    fieldRect.x += rect.width / fieldCount;

                    var valueProperty = valuesProperty.GetArrayElementAtIndex(index);
                    if (propertyType == ShaderUtil.ShaderPropertyType.Color)
                    {
                        valueProperty.colorValue = EditorGUI.ColorField(fieldRect, emptyLabel, valueProperty.colorValue, true, true, true);
                    }
                    else
                    {
                        EditorGUI.PropertyField(fieldRect, valueProperty, emptyLabel);
                    }
                    fieldRect.x += rect.width / fieldCount;
                }
            };
            onChangedCallback = (list) =>
            {
                indicesProperty.arraySize = targetsProperty.arraySize;
                namesProperty.arraySize = targetsProperty.arraySize;
                valuesProperty.arraySize = targetsProperty.arraySize;
            };
            onReorderCallbackWithDetails = (list, oldIndex, newIndex) =>
            {
                indicesProperty.MoveArrayElement(oldIndex, newIndex);
                namesProperty.MoveArrayElement(oldIndex, newIndex);
                valuesProperty.MoveArrayElement(oldIndex, newIndex);
            };
        }
    }

    [CustomEditor(typeof(MaterialPropertyBlockWriter)), CanEditMultipleObjects]
    public class MaterialPropertyBlockWriterEditor : Editor
    {
        private static readonly GUILayoutOption[] miniButtonLayout = {
            GUILayout.ExpandWidth(false),
        };

        public Color overrideColor = Color.white;
        public float overrideFloat;
        public Texture overrideTexture;

        public bool includeChildren;
        public bool applyToScene = true;
        public Material materialFilter;
        public string propertyName;
        private SerializedProperty colorTargets, floatTargets, textureTargets;
        private ReorderableList colorList, floatList, textureList;
        private void OnEnable()
        {
            colorTargets = serializedObject.FindProperty(nameof(MaterialPropertyBlockWriter.colorTargets));
            floatTargets = serializedObject.FindProperty(nameof(MaterialPropertyBlockWriter.floatTargets));
            textureTargets = serializedObject.FindProperty(nameof(MaterialPropertyBlockWriter.textureTargets));

            colorList = new MaterialPropertyList(
                serializedObject,
                serializedObject.FindProperty(nameof(MaterialPropertyBlockWriter.writeColors)),
                colorTargets,
                ShaderUtil.ShaderPropertyType.Color
            );
            floatList = new MaterialPropertyList(
                serializedObject,
                serializedObject.FindProperty(nameof(MaterialPropertyBlockWriter.writeFloats)),
                floatTargets,
                ShaderUtil.ShaderPropertyType.Float
            );
            textureList = new MaterialPropertyList(
                serializedObject,
                serializedObject.FindProperty(nameof(MaterialPropertyBlockWriter.writeTextures)),
                textureTargets,
                ShaderUtil.ShaderPropertyType.TexEnv
            );

            var colorValues = GetListPropertyOf(colorTargets, "Values");
            if (colorValues.arraySize > 0) overrideColor = colorValues.GetArrayElementAtIndex(0).colorValue;
            var floatValues = GetListPropertyOf(floatTargets, "Values");
            if (floatValues.arraySize > 0) overrideFloat = floatValues.GetArrayElementAtIndex(0).floatValue;
            var textureValues = GetListPropertyOf(textureTargets, "Values");
            if (textureValues.arraySize > 0) overrideTexture = textureValues.GetArrayElementAtIndex(0).objectReferenceValue as Texture ?? overrideTexture;
        }

        private SerializedProperty GetListPropertyOf(SerializedProperty targetsProperty, string typeName)
        {
            return targetsProperty.serializedObject.FindProperty(targetsProperty.name.Replace("Targets", typeName));
        }

        private bool ContainsRenderer(SerializedProperty targetsProperty, Renderer renderer, int index)
        {
            var indicesProperty = GetListPropertyOf(targetsProperty, "Indices");
            for (int i = 0; i < targetsProperty.arraySize; i++)
            {
                if ((targetsProperty.GetArrayElementAtIndex(i).objectReferenceValue as Renderer) == renderer && indicesProperty.GetArrayElementAtIndex(i).intValue == index) return true;
            }
            return false;
        }

        private void AddRenderer(Renderer renderer, int index, SerializedProperty targetsProperty)
        {
            var indicesProperty = GetListPropertyOf(targetsProperty, "Indices");
            var namesProperty = GetListPropertyOf(targetsProperty, "Names");
            var valuesProperty = GetListPropertyOf(targetsProperty, "Values");

            var i = targetsProperty.arraySize;
            targetsProperty.arraySize++;
            indicesProperty.arraySize = targetsProperty.arraySize;
            namesProperty.arraySize = targetsProperty.arraySize;
            valuesProperty.arraySize = targetsProperty.arraySize;

            targetsProperty.GetArrayElementAtIndex(i).objectReferenceValue = renderer;
            indicesProperty.GetArrayElementAtIndex(i).intValue = index;
            namesProperty.GetArrayElementAtIndex(i).stringValue = namesProperty.GetArrayElementAtIndex(Mathf.Max(i - 1, 0)).stringValue;

            ApplyNow(targetsProperty.serializedObject.targetObject as MaterialPropertyBlockWriter);
        }

        private MaterialPropertyBlockWriter GetWriter()
        {
            return target as MaterialPropertyBlockWriter;;
        }

        private static IEnumerable<MaterialPropertyBlockWriter> GetAllWriters()
        {
            return SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(o => o.GetUdonSharpComponentsInChildren<MaterialPropertyBlockWriter>());
        }


        private void ApplyNow(MaterialPropertyBlockWriter writer)
        {
            if (writer == null) return;
            writer.UpdateProxy();
            writer.Trigger();
        }
        private void ApplyNow(bool applyToScene = false)
        {
            if (applyToScene) ApplyAllOfScene();
            else
            {
                ApplyNow(GetWriter());
            }
        }

        private void ApplyAllOfScene()
        {
            foreach (var writer in GetAllWriters())
            {
                ApplyNow(writer);
            }
        }

        private void ClearNow(MaterialPropertyBlockWriter writer)
        {
            writer.UpdateProxy();
            writer.ClearTargetProperties();
        }

        private void ClearNow(bool applyToScene)
        {
            if (applyToScene)
            {
                foreach (var writer in GetAllWriters()) ClearNow(writer);
            }
            else ClearNow(GetWriter());
        }

        private void OverrideAll(SerializedProperty targets, System.Action<SerializedProperty> Action)
        {
            var values = GetListPropertyOf(targets, "Values");
            for (int i = 0; i < values.arraySize; i++) Action(values.GetArrayElementAtIndex(i));
            serializedObject.ApplyModifiedProperties();
            ApplyNow();
        }

        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;

            serializedObject.Update();

            EditorGUILayout.LabelField("Write On", EditorStyles.boldLabel);
            var property = serializedObject.FindProperty(nameof(MaterialPropertyBlockWriter.onStart));
            EditorGUILayout.PropertyField(property);

            EditorGUILayout.Space();

            colorList.DoLayoutList();
            using (new EditorGUILayout.HorizontalScope())
            {
                overrideColor = EditorGUILayout.ColorField(new GUIContent(), overrideColor, true, true, true);
                if (GUILayout.Button("Set All", EditorStyles.miniButton, miniButtonLayout)) OverrideAll(colorTargets, p => p.colorValue = overrideColor);
            }

            EditorGUILayout.Space();

            floatList.DoLayoutList();
            using (new EditorGUILayout.HorizontalScope())
            {
                overrideFloat = EditorGUILayout.FloatField(overrideFloat);
                if (GUILayout.Button("Set All", EditorStyles.miniButton, miniButtonLayout)) OverrideAll(floatTargets, p => p.floatValue = overrideFloat);
            }

            EditorGUILayout.Space();

            textureList.DoLayoutList();
            using (new EditorGUILayout.HorizontalScope())
            {
                overrideTexture = EditorGUILayout.ObjectField(overrideTexture, typeof(Texture), true) as Texture;
                if (GUILayout.Button("Set All", EditorStyles.miniButton, miniButtonLayout)) OverrideAll(textureTargets, p => p.objectReferenceValue = overrideTexture);
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Preview Tools", EditorStyles.boldLabel);
            applyToScene = EditorGUILayout.Toggle("Target to all of scene", applyToScene);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Preview")) ApplyNow(applyToScene);
                if (GUILayout.Button("Clear")) ClearNow(applyToScene);
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Adding Tools", EditorStyles.boldLabel);
            includeChildren = EditorGUILayout.Toggle("Include Children", includeChildren);
            using (new EditorGUILayout.HorizontalScope())
            {
                materialFilter = EditorGUILayout.ObjectField("Find By Material", materialFilter, typeof(Material), false) as Material;
                if (GUILayout.Button("Find", EditorStyles.miniButton, miniButtonLayout)) {
                    materialFilter = (includeChildren
                        ? Selection.activeGameObject?.GetComponentInChildren<Renderer>()?.sharedMaterial
                        : Selection.activeGameObject?.GetComponent<Renderer>()?.sharedMaterial
                     ) ?? materialFilter;
                }
            }
            var shader = materialFilter?.shader;
            if (shader != null)
            {
                var propertyNameIndex = EditorGUILayout.Popup(
                    "Property Name",
                    MaterialPropertyList.GetIndexOfProperty(materialFilter.shader, propertyName),
                    MaterialPropertyList.GetShaderPropertyNames(materialFilter.shader).ToArray()
                );
                propertyName = MaterialPropertyList.GetShaderPropertyName(shader, propertyNameIndex);
            }
            else
            {
                propertyName = EditorGUILayout.TextField("Property Name", propertyName);
            }

            if (materialFilter != null)
            {
                var writer = target as MaterialPropertyBlockWriter;
                var renderers = (includeChildren ? writer.GetComponentsInChildren<Renderer>() : Enumerable.Repeat(writer.GetComponent<Renderer>(), 1)).Where(r => r?.sharedMaterials?.Contains(materialFilter) ?? false);

                foreach (var renderer in renderers)
                {
                    var indices = renderer.sharedMaterials.Select((m, i) => (m, i)).Where(t => t.m == materialFilter).Select(t => t.i);
                    foreach (var index in indices)
                    {
                        var inColor = ContainsRenderer(colorTargets, renderer, index);
                        var inFloat = ContainsRenderer(floatTargets, renderer, index);
                        var inTexture = ContainsRenderer(textureTargets, renderer, index);
                        if (inColor && inFloat && inTexture) continue;

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.ObjectField(renderer, typeof(Renderer), false);
                            EditorGUILayout.ObjectField(renderer.sharedMaterials[index], typeof(Material), false);

                            using (new EditorGUI.DisabledGroupScope(inColor)) if (GUILayout.Button("Color", EditorStyles.miniButtonLeft, GUILayout.ExpandWidth(false))) AddRenderer(renderer, index, colorTargets);
                            using (new EditorGUI.DisabledGroupScope(inFloat)) if (GUILayout.Button("Float", EditorStyles.miniButtonMid, GUILayout.ExpandWidth(false))) AddRenderer(renderer, index, floatTargets);
                            using (new EditorGUI.DisabledGroupScope(inTexture)) if (GUILayout.Button("Texture", EditorStyles.miniButtonRight, GUILayout.ExpandWidth(false))) AddRenderer(renderer, index, textureTargets);
                        }
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
