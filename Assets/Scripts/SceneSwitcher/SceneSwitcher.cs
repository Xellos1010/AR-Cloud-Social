using UnityEngine;
using System.Collections;

public class SceneSwitcher : MonoBehaviour {
    public GameObject[] sceneObjects;
		
	void Start()
    {
        StaticPanelManager.DisableAllScenes();
        StaticPanelManager.InitializeDefaultScene();
    }

    public void ActivateScene(string name)
    {
        StaticPanelManager.LoadIngameScene(name);
    }

    public void ActivateScene(GameObject sceneObject)
    {
        ActivateScene(sceneObject.name);
    }
}
