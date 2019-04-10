using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

public class MenuBarStateManager : SceneLoadPass
{
    public static MenuBarStateManager instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindGameObjectWithTag("MainMenuBar").GetComponent<MenuBarStateManager>();
            return _instance;
        }
    }
    private static MenuBarStateManager _instance;
    public bool menuBarLoading = false;
    public RectTransform ArrowMovementBar; //currently manuelly setting the arrow to move
    public Transform buttonsUIParent;
    public float arrowTransitionTime;
    public float pendingColorBlinkDuration;
    public Color activeButtonColor;
    public Color inactiveButtonColor;
    public Color pendingButtonColor;
    public Color errorButtonColor;
    internal ButtonIndicatorStateManager currentPendingButton;
    public iTween.EaseType easeType = iTween.EaseType.easeOutExpo;
    public GameObject currentActiveButton; //Currently setting starting active button to main menu
    public CanvasGroup alphaCanvas;

    private void Awake()
    {
        _instance = this;
    }

    private void OnEnable()
    {
        SceneLoadManagement.loadingScene += SceneLoadManagement_loadingScene;
        SceneLoadManagement.loadingSceneFinished += SceneLoadManagement_loadingSceneFinished;
    }

    private void OnDisable()
    {
        SceneLoadManagement.loadingScene -= SceneLoadManagement_loadingScene;
        SceneLoadManagement.loadingSceneFinished -= SceneLoadManagement_loadingSceneFinished;
    }

    //TODO Refactor
    private void SceneLoadManagement_loadingScene(string sceneToLoad)
    {
        Debug.Log("MenuBarStateManager sees " + sceneToLoad + " was loaded");
        //Check if scene to load is a valid button option for main menu. Check with Button Manager
        if(sceneToLoad == "MainMenu" || sceneToLoad == "UploadImageVuphoria" || sceneToLoad == "PreviewTargets")
        {
            menuBarLoading = true;
            GameObject Button;
            for (int i = 0; i < buttonsUIParent.transform.childCount; i++)
            {
                if (buttonsUIParent.GetChild(i).gameObject.name == sceneToLoad)
                {
                    Button = buttonsUIParent.GetChild(i).gameObject;
                    SetActiveButton(Button);
                    break;
                }
            }
            
        }
    }

    internal static void SetAlphaTo(int v)
    {
        instance.alphaCanvas.alpha = v;
    }

    private void SceneLoadManagement_loadingSceneFinished(string sceneToLoad)
    {
        menuBarLoading = false;
    }

    public void LoadScene(GameObject Button) // Label the button name the same as the scene name
    {
        Debug.Log("Loading Main Scene " + Button.name);
        if (!menuBarLoading)
        {
            StartCoroutine(SceneLoadManagement.SwitchToSceneAsync(SceneLoadManagement.mainSceneLoaded,Button.name));
            SceneLoadManagement.mainSceneLoaded = Button.name;
        }
    }


    bool CheckLoadingFinished()
    {
        GameObject[] SceneObjects = UnityEngine.SceneManagement.SceneManager.GetSceneByName("Loading").GetRootGameObjects();
        foreach (GameObject rootObject in SceneObjects)
        {
            if (rootObject.name.Contains("LoadingManager"))
            {
                Debug.Log("" + rootObject.GetComponent<LoadingScreen>().m_SceneReadyToActivate);
                return rootObject.GetComponent<LoadingScreen>().m_SceneReadyToActivate;
            }
        }
        throw new Exception("There was an issue accessing LoadingManager. Ensure it's added to your scene builds and loaded before the MainBar Scene \"Main\"");
    }

    /// <summary>
    /// Called from VWSCloudConnecter to blink a color while image target is uploading
    /// </summary>
    /// <param name="button">name of the UploadImageVuphoria or any button</param>
    public void BlinkButtonPendingColor(string button = "UploadImageVuphoria")
    {
        Debug.Log("Checking for currentPendingButton");
        if (currentPendingButton == null)
        {
            Debug.Log("currentPendingButton == null");
            foreach (Transform child in buttonsUIParent)
            {
                Debug.Log("Checking for child with name " + button);
                if (child.gameObject.name == button)
                {
                    Debug.Log("found transform child with name " + button);
                    currentPendingButton = child.GetComponent<ButtonIndicatorStateManager>();
                    currentPendingButton.ActivatePendingState(pendingButtonColor, inactiveButtonColor, pendingColorBlinkDuration);
                    break;
                }
            }
        }
        else
        {
            Debug.Log("There is a current pending button " + currentPendingButton.gameObject.name);
            currentPendingButton.ActivatePendingState(pendingButtonColor, inactiveButtonColor, pendingColorBlinkDuration);
        }
    }

    /// <summary>
    /// Called from VWSCloudConnecter to blink a color while image target is uploading
    /// </summary>
    /// <param name="button">name of the UploadImageVuphoria or any button</param>
    public void ErrorButtonPending(string button = "UploadImageVuphoria")
    {
        if (currentPendingButton == null)
        {
            foreach (Transform child in buttonsUIParent)
                if (child.gameObject.name == button)
                {
                    currentPendingButton = child.GetComponent<ButtonIndicatorStateManager>();
                    currentPendingButton.ChangeColor(errorButtonColor);
                    break;
                }
        }
        else
        {
            currentPendingButton.ChangeColor(errorButtonColor);
        }
    }

    public void SwitchPendingToActive()
    {
        if (currentPendingButton)
            currentPendingButton.DeactivatePendingState();
        currentPendingButton.ChangeColor(activeButtonColor);
        StartCoroutine(TurnCheckTurnOffPendingButton());
    }

    IEnumerator TurnCheckTurnOffPendingButton()
    {
        yield return new WaitForSeconds(5);
        if (currentPendingButton != currentActiveButton)
            ToggleButtonStatusIndicator(currentPendingButton.gameObject, false);
        currentPendingButton = null;
    }

    public void SetActiveButton(GameObject buttonToActivate)
    {
        //Transition arrow first to show some response then toggle off all buttons and activate the button light before arrow arrival
        Debug.Log("Setting Active Button " + buttonToActivate.name);
        TransitionArrowToButton(buttonToActivate);
        ToggleAllButtonsStatusIndicator(false);
        currentActiveButton = buttonToActivate;
        ToggleButtonStatusIndicator(currentActiveButton, true);
    }

    void ToggleAllButtonsStatusIndicator(bool onOff)
    {
        foreach(Transform child in buttonsUIParent)
        {
            if (onOff)
                currentActiveButton.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = activeButtonColor;
            else
                currentActiveButton.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = inactiveButtonColor;
        }
    }

    private void ToggleButtonStatusIndicator(GameObject currentActiveButton, bool state)
    {
        if (state)
            currentActiveButton.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = activeButtonColor;
        else
            currentActiveButton.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = inactiveButtonColor;
    }

    private void TransitionArrowToButton(GameObject targetButton)
    {
        Debug.Log("Tweening arrow to " + targetButton.name);
        float placeOnScreen = targetButton.GetComponent<RectTransform>().anchoredPosition.x;
 
        Hashtable ValueToArguments = iTween.Hash(
            "from", ArrowMovementBar.offsetMin.x,
            "to", placeOnScreen,
            "time", arrowTransitionTime,
            "easetype", easeType,
            "onupdate", "UpdateArrowValue",
            "onupdatetarget", gameObject
            );
        iTween.ValueTo(gameObject, ValueToArguments);
        Debug.Log("itween Done");
    }

    public void UpdateArrowValue(float newValue)
    {
        ArrowMovementBar.anchoredPosition = new Vector2(newValue, ArrowMovementBar.anchoredPosition.y);
    }

    private int OrderInParent(Transform target)
    {
        for (int i = 0; i < target.parent.childCount; i++)
            if (target.parent.GetChild(i).name == target.name)
                return i;
        return 0;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MenuBarStateManager))]
public class MenuBarStateManagersEditor : Editor
{
    MenuBarStateManager _target;

    private void OnEnable()
    {
        _target = (MenuBarStateManager)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //Make option to cycle through each button
    }
}
#endif