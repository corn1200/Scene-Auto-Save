using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class Editor_SelectPlayScene
{
    private static readonly string prefs = "SceneAutoSave_SELECTPLAYSCENE_";
    private static readonly string[] playSceneTable = { "0번 씬부터 재생", "현재 씬부터 재생" };

    private static int selectedIndex = 0;

    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        Load();
    }

    public static void OnEditorGUI(GUIStyle titleStyle)
    {
        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
        EditorGUILayout.LabelField("Select Play Scene", titleStyle);

        EditorGUI.BeginChangeCheck();
        selectedIndex = EditorGUILayout.Popup("재생 씬 선택", selectedIndex, playSceneTable);

        if (EditorGUI.EndChangeCheck())
        {
            Debug.Log("Debug Check.. 재생 씬 선택 상호작용 [확인 후 삭제]");

            Save();

            if (selectedIndex == 0)
            {
                StartFromFirstScene();
            }
            else if (selectedIndex == 1)
            {
                StartFromCurrentScene();
            }
        }
    }

    private static void Load()
    {
        selectedIndex = EditorPrefs.GetInt($"{prefs}{nameof(selectedIndex)}");
    }

    private static void Save()
    {
        EditorPrefs.SetInt($"{prefs}{nameof(selectedIndex)}", selectedIndex);
    }

    private static void StartFromFirstScene()
    {
        var pathOfFirstScene = EditorBuildSettings.scenes[0].path;
        var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathOfFirstScene);

        EditorSceneManager.playModeStartScene = sceneAsset;
    }

    private static void StartFromCurrentScene()
    {
        EditorSceneManager.playModeStartScene = null;
    }
}