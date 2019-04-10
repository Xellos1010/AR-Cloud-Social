using UnityEngine;

public class SceneLoadPass : MonoBehaviour {
   
    public void LoadMainScene(string sceneName)
    {
        StartCoroutine(SceneLoadManagement.SwitchToSceneAsync(SceneLoadManagement.mainSceneLoaded,sceneName));
        SceneLoadManagement.mainSceneLoaded = sceneName;
    }
}
