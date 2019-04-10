using UnityEngine;
using System.Collections.Generic;

public static class StaticPanelManager{

    public static Dictionary<string,Panel> InGameScenes
    {
        get
        {
            if (_inGameScenes == null)
                _inGameScenes = new Dictionary<string, Panel>();
            return _inGameScenes;
        }
    }

    public static CanvasGroup AlphaManagerCanvasGroup;
    private static Dictionary<string, Panel> _inGameScenes;

    public static void RegisterPanel(Panel scene)
    {
        InGameScenes[scene.name] = scene;
    }

    public static void LoadIngameScene(string sceneName)
    {
        if (InGameScenes.ContainsKey(sceneName))
        {
            DisableAllScenes();
            SetSceneState(sceneName, true);
        }
        else
        {
            Debug.Log("Scene Does not contain " + sceneName);
        }
        
    }

    public static void LoadIngameScene(Panel scene)
    {
        if (InGameScenes.ContainsKey(scene.name))
        {
            DisableAllScenes();
            SetSceneState(scene.name, true);
        }
        else
        {
            Debug.Log("Application does not contain scene " + scene.name);
        }

    }

    static void SetSceneState(string scene, bool sceneState)
    {
        try {
            InGameScenes[scene].gameObject.SetActive(sceneState);
        }
        catch { Debug.Log("scene is no longer accessible"); }
    }

    public static void DisableAllScenes()
    {
        foreach(string s in InGameScenes.Keys)
        {
            SetSceneState(s,false);
        }
    }

    public static void InitializeDefaultScene()
    {
        bool defaultSceneSet = false;
        if (InGameScenes.Count > 0)
        {
            foreach (string s in InGameScenes.Keys)
            {
                if (InGameScenes[s].setActiveOnAwake && !defaultSceneSet)
                {
                    SetSceneState(s, true);
                    defaultSceneSet = true;
                }
                else
                    SetSceneState(s, false);
            }
        }
    }

}
