using UnityEngine;
using UnityEditor;

public class PanelManager : MonoBehaviour
{
    public static PanelManager instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<PanelManager>().GetComponent<PanelManager>();
            return _instance;
        }
    }
    private static PanelManager _instance;

    void OnEnable()
    {
        _instance = this;
        StaticPanelManager.AlphaManagerCanvasGroup = GetComponent<CanvasGroup>();
    }

    void OnDisable()
    {
        _instance = null;
    }

    public void LoadIngameScene(GameObject sceneName)
    {
        StaticPanelManager.LoadIngameScene(sceneName.name);
    }

    public void LoadIngameScene(string sceneName)
    {
        StaticPanelManager.LoadIngameScene(sceneName);
    }

    public void ToggleAllScenes(bool onOff)
    {
        foreach(Transform childScene in transform)
        {
            if (childScene.GetComponent<Panel>() != null)
                childScene.gameObject.SetActive(onOff);
        }
    }

    public void FocusAwakeScene()
    {
        foreach (Transform childScene in transform)
        {
            if (childScene.GetComponent<Panel>() != null)
                childScene.gameObject.SetActive(childScene.GetComponent<Panel>().setActiveOnAwake);
        }
    }

    public void FocusPanel(Panel panel)
    {
        ToggleAllScenes(false);
        panel.gameObject.SetActive(true);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PanelManager))]
public class PanelManagerEditorUI : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PanelManager targetScript = (PanelManager)target;
        if (GUILayout.Button("Activate All Scenes"))
        {
            targetScript.ToggleAllScenes(true);
        }
        if (GUILayout.Button("Focus Scenes Activate on Awake"))
        {
            targetScript.FocusAwakeScene();
        }
    }
}
#endif