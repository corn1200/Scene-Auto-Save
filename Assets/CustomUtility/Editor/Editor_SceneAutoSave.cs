using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class Editor_SceneAutoSave
{
    private static readonly string prefs = "CUSTOMUTILITY_SCENEAUTOSAVE_";
    private static readonly string[] timeTable = { "5분", "10분", "30분", "1시간", "Custom" };
    private static readonly double[] timeToSeconds = { 300, 600, 1800, 3600 };
    private static readonly string timeNotation = "[yyyy-MM-dd] HH:mm:ss";

    private static bool isActivated = false;
    private static bool isShowLogExpanded = false;
    private static int selcetedTimeTableIndex = 0;

    private static double saveCycle = 0;
    private static double nextSaveRemainingTime = 0;
    private static DateTime lastSaveTime = DateTime.Now;

    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        Load();

        lastSaveTime = DateTime.Now;

        var handlers = EditorApplication.update.GetInvocationList();


        bool hasAlready = false;
        foreach (var handler in handlers)
        {
            if (handler.Method.Name.Equals(nameof(UpdateAutoSave)))
            {
                hasAlready = true;
                break;
            }
        }

        if (hasAlready == false)
        {
            EditorApplication.update += UpdateAutoSave;
        }
    }

    public static void OnEditorGUI(GUIStyle titleStyle)
    {
        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
        EditorGUILayout.LabelField("Scene Auto Save", titleStyle);

        EditorGUI.BeginChangeCheck();

        // 자동저장 On/Off Toggle UI
        string textActive = isActivated == true ? "ON" : "OFF";
        isActivated = EditorGUILayout.Toggle($"자동 저장 {textActive}", isActivated);

        EditorGUI.BeginDisabledGroup(!isActivated);

        // 저장 주기 Dropdown UI
        selcetedTimeTableIndex = EditorGUILayout.Popup("저장 주기", selcetedTimeTableIndex, timeTable);
        // 저장 주기를 "Custom"으로 설정했을 때 (초 단위의 저장 주기 입력)
        if (selcetedTimeTableIndex == timeTable.Length - 1)
        {
            EditorGUI.indentLevel++;
            saveCycle = EditorGUILayout.DoubleField("시간(초) : ", saveCycle);
            EditorGUI.indentLevel--;

            // 저장 주기는 최소 10초 이상
            if (saveCycle < 10)
            {
                saveCycle = 10;
            }
        }
        else
        {
            saveCycle = timeToSeconds[selcetedTimeTableIndex];
        }

        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight * 0.5f);
        isShowLogExpanded = EditorGUILayout.Foldout(isShowLogExpanded, "로그 정보", true);
        if (isShowLogExpanded == true)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField($"마지막 저장 시간 : {lastSaveTime.ToString(timeNotation)}");
            EditorGUILayout.LabelField($"다음 저장까지 남은 시간 : {nextSaveRemainingTime:00.00}");
            EditorGUI.indentLevel--;
        }

        EditorGUI.EndDisabledGroup();

        if (EditorGUI.EndChangeCheck())
        {
            Save();
        }
    }

    private static void Load()
    {
        Debug.Log("Editor_SceneAutoSave::Load()");

        isActivated = EditorPrefs.GetBool($"{prefs}{nameof(isActivated)}", false);
        isShowLogExpanded = EditorPrefs.GetBool($"{prefs}{nameof(isShowLogExpanded)}", false);
        selcetedTimeTableIndex = EditorPrefs.GetInt($"{prefs}{nameof(selcetedTimeTableIndex)}", 0);
        saveCycle = EditorPrefs.GetFloat($"{prefs}{nameof(saveCycle)}", 300f);
    }

    private static void Save()
    {
        Debug.Log("Editor_SceneAutoSave::Save()");

        EditorPrefs.SetBool($"{prefs}{nameof(isActivated)}", isActivated);
        EditorPrefs.SetBool($"{prefs}{nameof(isShowLogExpanded)}", isShowLogExpanded);
        EditorPrefs.SetInt($"{prefs}{nameof(selcetedTimeTableIndex)}", selcetedTimeTableIndex);
        EditorPrefs.SetFloat($"{prefs}{nameof(saveCycle)}", (float)saveCycle);
    }

    private static void UpdateAutoSave()
    {
        DateTime dateTime = DateTime.Now;

        if (isActivated == false || EditorApplication.isPlaying == true)
        {
            lastSaveTime = dateTime;
            nextSaveRemainingTime = saveCycle;
            
            return;

        }

        double diff = dateTime.Subtract(lastSaveTime).TotalSeconds;

        nextSaveRemainingTime = saveCycle - diff;

        if (nextSaveRemainingTime < 0)
        {
            nextSaveRemainingTime = 0;
            

        }
        
        // 저장 시간이 되었을 때 씬 자동 저장
        if (diff > saveCycle)
        {
            Debug.Log($"Editor_SceneAutoSave::UpdateAutoSave() - Save Scene");

            EditorSceneManager.SaveOpenScenes();
            lastSaveTime = dateTime;
            
        }
    }
}