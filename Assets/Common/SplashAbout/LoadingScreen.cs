/*===============================================================================
Copyright (c) 2015-2018 PTC Inc. All Rights Reserved.

Copyright (c) 2015 Qualcomm Connected Experiences, Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
===============================================================================*/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    #region PRIVATE_MEMBER_VARIABLES
    RawImage m_SpinnerImage;
    AsyncOperation m_AsyncOperation;
    public bool m_SceneReadyToActivate;
    #endregion // PRIVATE_MEMBER_VARIABLES

    #region PUBLIC_MEMBER_VARIABLES
    public static string SceneToLoad { get; set; }
    #endregion // PUBLIC_MEMBER_VARIABLES

    public static void Run()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("2-Loading");
    }

    #region MONOBEHAVIOUR_METHODS
    void Start()
    {
        m_SpinnerImage = GetComponentInChildren<RawImage>();
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        StartCoroutine(LoadNextSceneAsync());
    }

    void Update()
    {
        if (m_SpinnerImage)
        {
            if (!m_SceneReadyToActivate)
            {
                m_SpinnerImage.rectTransform.Rotate(Vector3.forward, 90.0f * Time.deltaTime);
            }
            else
            {
                m_SpinnerImage.enabled = false;
            }
        }

        if (m_AsyncOperation != null)
        {
            if (m_AsyncOperation.progress < 0.9f)
            {
                Debug.Log("Scene Loading Progress: " + m_AsyncOperation.progress * 100 + "%");
            }
            else
            {
                m_SceneReadyToActivate = true;
                m_AsyncOperation.allowSceneActivation = true;
            }
        }
    }
    #endregion // MONOBEHAVIOUR_METHODS


    #region PRIVATE_METHODS

    public void SetLevelToLoad(string sceneToLoad, float amountToWait = 0)
    {
        StartCoroutine(LoadNextSceneAsync(sceneToLoad,amountToWait));
    }

    IEnumerator LoadNextSceneAsync(string sceneToLoad = "", float amountToWait = 0)
    {
        int nextSceneIndex = 0;
        if (sceneToLoad == "")
            nextSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1;
        else
            nextSceneIndex = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneToLoad).buildIndex;
        yield return new WaitForSeconds(amountToWait);
        if (string.IsNullOrEmpty(SceneToLoad))
        {
            m_AsyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(nextSceneIndex);
        }
        else
        {
            m_AsyncOperation = UnityEngine.SceneManagement. SceneManager.LoadSceneAsync(SceneToLoad);
        }

        m_AsyncOperation.allowSceneActivation = false;

        yield return m_AsyncOperation;
    }
    #endregion // PRIVATE_METHODS
}
