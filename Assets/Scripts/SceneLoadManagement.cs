using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoadManagement
{
    #region PRIVATE_MEMBER_VARIABLES
    internal static AsyncOperation m_AsyncOperation;
    internal static Dictionary<string, int> buildScenes = new Dictionary<string, int>
    {
        {"Loading",0 },
        {"MenuBar",1},
        {"MainMenu",2},
        {"ManageTargets",3},
        {"PreviewTargets",4},
        {"SelectingObject",5},
        {"UploadImageVuphoria",6}
    };
    #endregion

    public delegate void OnSceneLoading(string sceneToLoad);
    public static event OnSceneLoading unloadingScene;
    public static event OnSceneLoading loadingScene;
    public static event OnSceneLoading loadingSceneFinished;
    internal static string mainSceneLoaded;
    
    internal static IEnumerator SwitchToSceneAsync(string sceneToUnload,string sceneToLoad)
    {
        Debug.Log("Switching to scene " + sceneToLoad + " from " + sceneToLoad);
        yield return LoadSceneASync(sceneToLoad);
        yield return UnLoadActiveAndLoadNextSceneAsync(sceneToUnload);
    }

    internal static IEnumerator LoadSceneASync(string sceneToLoad)
    {
        //Fire Loading Scene Event
        if (loadingScene != null)
            loadingScene(sceneToLoad);
        m_AsyncOperation = SceneManager.LoadSceneAsync(buildScenes[sceneToLoad], LoadSceneMode.Additive);
        m_AsyncOperation.allowSceneActivation = false;
        yield return m_AsyncOperation;
        if (loadingSceneFinished != null)
            loadingSceneFinished(sceneToLoad);
    }

    internal static IEnumerator UnLoadActiveAndLoadNextSceneAsync(string sceneToUnload)
    {
        Debug.Log("Unloading Scene " + sceneToUnload);
        m_AsyncOperation = SceneManager.UnloadSceneAsync(sceneToUnload);
        m_AsyncOperation.allowSceneActivation = false;
        yield return m_AsyncOperation;
    }
}
