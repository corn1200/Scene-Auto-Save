using System;
using UnityEditor;
using UnityEngine;

public class SceneAutoSaveEditor : EditorWindow
{
    private static SceneAutoSaveEditor window;

    private GUIStyle titleStyle;

    [MenuItem("Window/SceneAutoSave/SceneAutoSave - Custom Editor")]
    private static void Setup()
    {
        window = GetWindow<SceneAutoSaveEditor>();
        window.titleContent = new GUIContent("SceneAutoSave");
        window.minSize = new Vector2(300, 300);
        window.maxSize = new Vector2(1920, 1080);
    }

    private void Awake()
    {
        titleStyle = new GUIStyle(EditorStyles.label);
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.normal.textColor = Color.yellow;
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Scene Auto Save", titleStyle);
        Editor_SelectPlayScene.OnEditorGUI(titleStyle);
    }
}
