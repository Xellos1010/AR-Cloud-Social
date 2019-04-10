using UnityEngine;
using System.Collections;

public class UploadImageTargetManager : GameManager {

    public void RetakePhoto()
    {
        StaticPanelManager.LoadIngameScene("TakePictureMode");
    }   
}
