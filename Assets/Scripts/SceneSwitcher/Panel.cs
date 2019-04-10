using UnityEngine;
using System.Collections;
using UnityEditor;
/// <summary>
/// Used to define the parent scene GameObject.
/// Holds all objects to activate in-game scene for UI or 3d Objects.
/// </summary>
public class Panel : MonoBehaviour
{
    /// <summary>
    /// Toggle on to set this scene as the scene that remains active on Start
    /// </summary>
    public bool setActiveOnAwake;
	// Use this for initialization
	void Awake()
    {
        //Registers the scene with the Scene Manager
        StaticPanelManager.RegisterPanel(this);

        if (!setActiveOnAwake)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }
	
}

#if UNITY_EDITOR
[CustomEditor(typeof(Panel))]
public class SceneObjectEditorUI : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Panel _target = (Panel)target;
        if (GUILayout.Button("Focus On Scene"))
        {
            try
            {
                _target.transform.parent.GetComponent<PanelManager>().FocusPanel(_target);
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
                Debug.Log("Make sure the parent transform of " + _target.gameObject.name + " has InGameSceneManager Component Attached");
            }
        }
    }
}
#endif