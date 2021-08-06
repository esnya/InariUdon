
using System;
using UdonSharp;
using UdonToolkit;
using UnityEngine;
using VRC.Udon;
#if !COMPILER_UDONSHARP && UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UdonSharpEditor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.SceneManagement;
#endif

namespace InariUdon.Rendering
{
    [
        CustomName("MaterialPropertyBlock Writer"),
        HelpMessage(@"
Apply a `MaterialPropertyBlock`.
Override the material properties with various values, but they can share the same material. This is a first step for GPU instancing."),
        Documentation.ImageAttachments(new [] {
            "https://user-images.githubusercontent.com/2088693/121310202-160c6b00-c93e-11eb-92ec-91583c3f69f0.png",
            "https://user-images.githubusercontent.com/2088693/121310283-2cb2c200-c93e-11eb-9834-c99a901a0f1a.png",
        }),
        UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)
    ]
    public class MaterialPropertyBlockWriter : UdonSharpBehaviour
    {
        [Tooltip("Apply on start")] public bool onStart = true;

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

        public bool writeVectors;
        public Renderer[] vectorTargets = {};
        public int[] vectorIndices = {};
        public string[] vectorNames = {};
        public Vector4[] vectorValues = {};

        private void Start()
        {
            if (onStart) Trigger();
        }

        [Documentation.EventDescription("Apply overrides")]
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

            if (writeVectors)
            {
                for (int i = 0; i < vectorTargets.Length; i++)
                {
                    var target = vectorTargets[i];
                    var materialIndex = vectorIndices[i];
                    target.GetPropertyBlock(block, materialIndex);
                    block.SetVector(vectorNames[i], vectorValues[i]);
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
        public static IEnumerable<int> GetShaderPropertyIndices(Shader shader, IEnumerable<ShaderUtil.ShaderPropertyType> propertyTypes)
        {
            return GetShaderPropertyIndices(shader).Where(i => propertyTypes.Contains(ShaderUtil.GetPropertyType(shader, i)));
        }
        public static IEnumerable<string> GetShaderPropertyNames(Shader shader)
        {
            return GetShaderPropertyIndices(shader).Select(i => ShaderUtil.GetPropertyName(shader, i));
        }
        public static IEnumerable<string> GetShaderPropertyNames(Shader shader, IEnumerable<ShaderUtil.ShaderPropertyType> propertyTypes)
        {
            return GetShaderPropertyIndices(shader, propertyTypes).Select(i => ShaderUtil.GetPropertyName(shader, i));
        }
        public static int GetIndexOfProperty(Shader shader, string propertyName)
        {
            return GetShaderPropertyIndices(shader).Where(i => ShaderUtil.GetPropertyName(shader, i) == propertyName).Append(-1).First();
        }
        public static int GetIndexOfProperty(Shader shader, string propertyName, IEnumerable<ShaderUtil.ShaderPropertyType> propertyTypes)
        {
            return GetShaderPropertyNames(shader, propertyTypes)
                .Select((n, i) => (n, i))
                .Where(t => ShaderUtil.GetPropertyName(shader, t.i) == propertyName)
                .Select(t => t.i)
                .Append(-1)
                .First();
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
        ) : this(serializedObject, enabledProperty, targetsProperty, Enumerable.Repeat(propertyType, 1))
        {
        }

        public MaterialPropertyList(
            SerializedObject serializedObject,
            SerializedProperty enabledProperty,
            SerializedProperty targetsProperty,
            IEnumerable<ShaderUtil.ShaderPropertyType> propertyTypes
        ) : base(serializedObject, targetsProperty)
        {
            var margin = EditorStyles.objectField?.margin ?? new RectOffset();
            var height = EditorStyles.objectField?.lineHeight ?? 0.0f;
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
                    indicesProperty.arraySize = targetsProperty.arraySize;
                    namesProperty.arraySize = targetsProperty.arraySize;
                    valuesProperty.arraySize = targetsProperty.arraySize;

                    var fieldRect = rect;
                    fieldRect.xMax /= fieldCount;
                    var verticalMargin = Math.Max(rect.height - height, 0) / 2;
                    fieldRect.yMin += verticalMargin;
                    fieldRect.yMax -= verticalMargin;

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
                    var valueProperty = valuesProperty.GetArrayElementAtIndex(index);
                    if (shader != null)
                    {
                        var count = ShaderUtil.GetPropertyCount(shader);
                        var propertyIndices = GetShaderPropertyIndices(shader, propertyTypes);
                        var propertyNames = GetShaderPropertyNames(shader, propertyTypes).ToArray();

                        var currentSelectedIndex = propertyNames.Select((n, i) => (n, i)).Where(t => t.n == nameProperty.stringValue).Select(t => t.i).FirstOrDefault();

                        var nextSelectedIndex = EditorGUI.Popup(fieldRect,currentSelectedIndex,propertyNames);
                        fieldRect.x += rect.width / fieldCount;
                        nameProperty.stringValue = propertyNames.Skip(nextSelectedIndex).Append(nameProperty.stringValue).First();

                        var shaderPropertyIndex = propertyIndices.SkipWhile(i => GetShaderPropertyName(shader, i) != nameProperty.stringValue).FirstOrDefault();

                        if (propertyTypes.Contains(ShaderUtil.ShaderPropertyType.Color))
                        {
                            valueProperty.colorValue = EditorGUI.ColorField(fieldRect, emptyLabel, valueProperty.colorValue, true, true, true);
                        }
                        else if (propertyTypes.Contains(ShaderUtil.ShaderPropertyType.Range) && ShaderUtil.GetPropertyType(shader, shaderPropertyIndex) == ShaderUtil.ShaderPropertyType.Range && ShaderUtil.GetRangeLimits(shader, shaderPropertyIndex, 1) != ShaderUtil.GetRangeLimits(shader, shaderPropertyIndex, 2))
                        {
                            valueProperty.floatValue = EditorGUI.Slider(
                                fieldRect,
                                valueProperty.floatValue,
                                ShaderUtil.GetRangeLimits(shader, shaderPropertyIndex, 1),
                                ShaderUtil.GetRangeLimits(shader, shaderPropertyIndex, 2)
                            );
                        }
                        else if (propertyTypes.Contains(ShaderUtil.ShaderPropertyType.Vector))
                        {
                            valueProperty.vector4Value = EditorGUI.Vector4Field(
                                fieldRect,
                                emptyLabel,
                                valueProperty.vector4Value
                            );
                        }
                        else
                        {
                            EditorGUI.PropertyField(fieldRect, valueProperty, emptyLabel);
                        }

                        fieldRect.x += rect.width / fieldCount;
                    }
                    else
                    {
                        EditorGUI.PropertyField(fieldRect, nameProperty, emptyLabel);
                        fieldRect.x += rect.width / fieldCount;

                        EditorGUI.PropertyField(fieldRect, valueProperty, emptyLabel);
                        fieldRect.x += rect.width / fieldCount;
                    }

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

        private static readonly ShaderUtil.ShaderPropertyType[] floatLikePropertyTypes = {
            ShaderUtil.ShaderPropertyType.Float,
            ShaderUtil.ShaderPropertyType.Range,
        };

        public Color overrideColor = Color.white;
        public float overrideFloat;
        public Texture overrideTexture;

        public bool includeChildren;
        public bool applyToScene = true;
        public Material materialFilter;
        public string propertyName;
        private SerializedProperty colorTargets, floatTargets, textureTargets, vectorTargets;
        private ReorderableList colorList, floatList, textureList, vectorList;

        private void OnEnable()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            colorTargets = serializedObject.FindProperty(nameof(MaterialPropertyBlockWriter.colorTargets));
            floatTargets = serializedObject.FindProperty(nameof(MaterialPropertyBlockWriter.floatTargets));
            textureTargets = serializedObject.FindProperty(nameof(MaterialPropertyBlockWriter.textureTargets));
            vectorTargets = serializedObject.FindProperty(nameof(MaterialPropertyBlockWriter.vectorTargets));

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
                floatLikePropertyTypes
            );
            textureList = new MaterialPropertyList(
                serializedObject,
                serializedObject.FindProperty(nameof(MaterialPropertyBlockWriter.writeTextures)),
                textureTargets,
                ShaderUtil.ShaderPropertyType.TexEnv
            );
            vectorList = new MaterialPropertyList(
                serializedObject,
                serializedObject.FindProperty(nameof(MaterialPropertyBlockWriter.writeVectors)),
                vectorTargets,
                ShaderUtil.ShaderPropertyType.Vector
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

        private bool ContainsRenderer(SerializedProperty targetsProperty, Renderer renderer, int index, string name)
        {
            var indicesProperty = GetListPropertyOf(targetsProperty, "Indices");
            var namesProperty= GetListPropertyOf(targetsProperty, "Names");
            for (int i = 0; i < targetsProperty.arraySize; i++)
            {
                if (targetsProperty.GetArrayElementAtIndex(i).objectReferenceValue as Renderer != renderer) continue;
                if (i >= indicesProperty.arraySize || indicesProperty.GetArrayElementAtIndex(i).intValue != index) continue;
                if (i >= namesProperty.arraySize || namesProperty.GetArrayElementAtIndex(i).stringValue != name) continue;
                return true;
            }
            return false;
        }

        private void OverrideValue(SerializedProperty valueProperty)
        {
                switch (valueProperty.propertyType)
                {
                    case SerializedPropertyType.Color:
                        valueProperty.colorValue = overrideColor;
                        break;
                    case SerializedPropertyType.Float:
                        valueProperty.floatValue = overrideFloat;
                        break;
                    case SerializedPropertyType.ObjectReference:
                        valueProperty.objectReferenceValue = overrideTexture;
                        break;
                }
        }

        private void AddRenderer(Renderer renderer, int index, string propertyName, SerializedProperty targetsProperty)
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
            namesProperty.GetArrayElementAtIndex(i).stringValue = propertyName;

            OverrideValue(valuesProperty.GetArrayElementAtIndex(i));
        }

        private MaterialPropertyBlockWriter GetWriter()
        {
            return target as MaterialPropertyBlockWriter;;
        }

        private static IEnumerable<MaterialPropertyBlockWriter> GetAllWriters()
        {
            return SceneManager.GetActiveScene().GetRootGameObjects()
                .SelectMany(o => o.GetComponentsInChildren<UdonBehaviour>())
                .Where(udon => UdonSharpEditorUtility.IsUdonSharpBehaviour(udon))
                .Select(udon => UdonSharpEditorUtility.GetProxyBehaviour(udon) as MaterialPropertyBlockWriter)
                .Where(writer => writer != null);
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

        private void OverrideAll(SerializedProperty targetsProperty)
        {
            var valuesProperty = GetListPropertyOf(targetsProperty, "Values");
            for (int i = 0; i < valuesProperty.arraySize; i++)  OverrideValue(valuesProperty.GetArrayElementAtIndex(i));
        }

        private void OnListGUI(ReorderableList list, Action RenderField)
        {
            if (list == null) InitializeMembers();
            using (new EditorGUILayout.HorizontalScope())
            {
                RenderField();
                if (GUILayout.Button("Set All", EditorStyles.miniButton, miniButtonLayout)) OverrideAll(list?.serializedProperty);
            }
            list?.DoLayoutList();
        }

        private void OnListsGUI()
        {
            OnListGUI(
                colorList,
                () => overrideColor = EditorGUILayout.ColorField(new GUIContent(), overrideColor, true, true, true)
            );
            EditorGUILayout.Space();
            OnListGUI(
                floatList,
                () => overrideFloat = EditorGUILayout.FloatField(overrideFloat)
            );
            EditorGUILayout.Space();
            OnListGUI(
                textureList,
                () => overrideTexture = EditorGUILayout.ObjectField(overrideTexture, typeof(Texture), true) as Texture
            );
            OnListGUI(
                vectorList,
                () => { }
            );
        }

        private void OnPreviewToolsGUI()
        {
            EditorGUILayout.LabelField("Preview Tools", EditorStyles.boldLabel);
            applyToScene = EditorGUILayout.Toggle("Target to all of scene", applyToScene);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Preview")) ApplyNow(applyToScene);
                if (GUILayout.Button("Clear")) ClearNow(applyToScene);
            }
        }
        private void OnAddingToolsGUI()
        {
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
                        var inColor = ContainsRenderer(colorTargets, renderer, index, propertyName);
                        var inFloat = ContainsRenderer(floatTargets, renderer, index, propertyName);
                        var inTexture = ContainsRenderer(textureTargets, renderer, index, propertyName);
                        if (inColor && inFloat && inTexture) continue;

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.ObjectField(renderer, typeof(Renderer), false);
                            EditorGUILayout.ObjectField(renderer.sharedMaterials[index], typeof(Material), false);

                            using (new EditorGUI.DisabledGroupScope(inColor)) if (GUILayout.Button("Color", EditorStyles.miniButtonLeft, GUILayout.ExpandWidth(false))) AddRenderer(renderer, index, propertyName, colorTargets);
                            using (new EditorGUI.DisabledGroupScope(inFloat)) if (GUILayout.Button("Float", EditorStyles.miniButtonMid, GUILayout.ExpandWidth(false))) AddRenderer(renderer, index, propertyName, floatTargets);
                            using (new EditorGUI.DisabledGroupScope(inTexture)) if (GUILayout.Button("Texture", EditorStyles.miniButtonRight, GUILayout.ExpandWidth(false))) AddRenderer(renderer, index, propertyName, textureTargets);
                        }
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;

            serializedObject.Update();

            EditorGUILayout.LabelField("Write On", EditorStyles.boldLabel);
            var property = serializedObject.FindProperty(nameof(MaterialPropertyBlockWriter.onStart));
            EditorGUILayout.PropertyField(property);

            EditorGUILayout.Space();
            OnListsGUI();
            EditorGUILayout.Space();

            OnPreviewToolsGUI();

            EditorGUILayout.Space();

            OnAddingToolsGUI();

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
