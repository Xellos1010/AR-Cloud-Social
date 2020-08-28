/*===============================================================================
Copyright (c) 2015-2018 PTC Inc. All Rights Reserved.
Copyright (c) 2015 Qualcomm Connected Experiences, Inc. All Rights Reserved.
Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
===============================================================================*/
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using System;

public class LoadingScreen : MonoBehaviour
{
    #region PRIVATE_MEMBER_VARIABLES
    //SceneManagement cannot access scenes build index by string unless scene is preloaded. 
    //Initialize Dictionary with string to build number.
    #endregion // PRIVATE_MEMBER_VARIABLES

    #region Internal_Variables
    internal bool m_SceneReadyToActivate = false;
    #endregion

    #region PUBLIC_MEMBER_VARIABLES
    public RawImage m_SpinnerImage;
    public RawImage m_BackgroundImage;
    public RawImage m_logo;
    public static string SceneToLoad { get; set; }
    public float rotationSpeed = 90;
    bool loadingGraphicsToggled;
    #endregion // PUBLIC_MEMBER_VARIABLES

    #region MONOBEHAVIOUR_METHODS
    void Start()
    {
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        StartCoroutine(LoadInitialScenes());
    }

    private void OnEnable()
    {
        SceneLoadManagement.loadingSceneFinished += SceneLoadManagement_loadingSceneFinished;
        SceneLoadManagement.loadingScene += SceneLoadManagement_loadingScene;
        SceneLoadManagement.unloadingScene += SceneLoadManagement_unloadingScene;
    }

    private void SceneLoadManagement_unloadingScene(string sceneToLoad)
    {
        ToggleLoadingGraphics(true);
    }

    private void SceneLoadManagement_loadingScene(string sceneToLoad)
    {
        ToggleLoadingGraphics(true);
    }

    private void SceneLoadManagement_loadingSceneFinished(string sceneToLoad)
    {
        Debug.Log("Scene Loaded = " + sceneToLoad);
        //Check if scene to load is a valid button option for main menu. Check with Button Manager
        if (sceneToLoad == "MainMenu" || sceneToLoad == "UploadImageVuphoria" || sceneToLoad == "PreviewTargets")
        {
            ToggleLoadingGraphics(false);
            m_SceneReadyToActivate = false;
        }
    }

    private void OnDisable()
    {
        SceneLoadManagement.loadingSceneFinished -= SceneLoadManagement_loadingSceneFinished;
    }

    private IEnumerator LoadInitialScenes()
    {
        //Load Main Menu Bar
        yield return SceneLoadManagement.LoadSceneASync("MenuBar");


        yield return new WaitForSeconds(.333f);
        //Load Main Scene
        yield return SceneLoadManagement.LoadSceneASync("MainMenu");
        SceneLoadManagement.mainSceneLoaded = "MainMenu";
    }

    private void ToggleLoadingGraphics(bool onOFf)
    {
        loadingGraphicsToggled = onOFf;
        m_BackgroundImage.enabled = onOFf;
        m_SpinnerImage.enabled = onOFf;
        m_logo.enabled = onOFf;
    }

    void Update()
    {
        if (SceneLoadManagement.m_AsyncOperation != null)
        {
            //TODO Stop spinner when all scenes loaded
            if (m_SpinnerImage)
            {
                if (!m_SceneReadyToActivate)
                {
                    m_SpinnerImage.rectTransform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
                }
            }
            if (SceneLoadManagement.m_AsyncOperation.progress < 0.9f)
            {
                Debug.Log("Scene Loading Progress: " + SceneLoadManagement.m_AsyncOperation.progress * 100 + "%");
            }
            else
            {
                SceneLoadManagement.m_AsyncOperation.allowSceneActivation = true;
                SceneLoadManagement.m_AsyncOperation = null;
            }
        }
    }

    internal void LoadMainMenuTest()
    {
        StartCoroutine(SceneLoadManagement.LoadSceneASync("MainMenu"));
    }

    internal void LoadMenuBarTest()
    {
        StartCoroutine(SceneLoadManagement.LoadSceneASync("MenuBar"));
    }
    #endregion // MONOBEHAVIOUR_METHODS
}

#if UNITY_EDITOR
[CustomEditor(typeof(LoadingScreen))]
public class LoadingScreenEditor : Editor
{
    private LoadingScreen _target;

    private void OnEnable()
    {
        _target = (LoadingScreen)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Load Menu Bar"))
        {
            _target.LoadMenuBarTest();
        }
        if (GUILayout.Button("Load Main Menu"))
        {
            _target.LoadMainMenuTest();
        }
    }
}
#endif