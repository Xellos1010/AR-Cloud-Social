using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Advertisements;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(VWSCloudConnecter))]
public class VWSCloudConnecterEditor : CustomBaseManagerEditor
{
    VWSCloudConnecter _target;
    public void OnEnable()
    {
        _target = (VWSCloudConnecter)target;
    }

    public override async void OnInspectorGUI()
    {
        if (GUILayout.Button("Send Unity Get Test IEnum"))
        {
            VWSCloudConnecterService service = new VWSCloudConnecterService();
            service.SendGetRequestTest((MonoBehaviour)target,"8a26156f7bdf43cfbe605039e23b0f95");
        }
        if (GUILayout.Button("Send Unity Post Test IEnum"))
        {
            VWSCloudConnecterService service = new VWSCloudConnecterService();
            service.SendPostRequestTest((MonoBehaviour)target, "8a26156f7bdf43cfbe605039e23b0f95");
        }
        if (GUILayout.Button("Send Unity Get Test Task"))
        {
            VWSCloudConnecterService service = new VWSCloudConnecterService();
            await service.SendGRTask("8a26156f7bdf43cfbe605039e23b0f95");
        }
        if (GUILayout.Button("Send Unity Post Test Task"))
        {
            VWSCloudConnecterService service = new VWSCloudConnecterService();
            await service.SendPRTask("8a26156f7bdf43cfbe605039e23b0f95");
        }
        if (GUILayout.Button("Test Get Target Summary"))
        {
            VWSCloudConnecterService service = new VWSCloudConnecterService();
            Debug.Log(await service.GetTargetIDSummary("8a26156f7bdf43cfbe605039e23b0f95"));
        }
        //base.OnInspectorGUI();
    }
}
#endif
public class VWSCloudConnecter : SingletonBehaviour<VWSCloudConnecter> {

    public ContentTypeEnum contentType;
    public string webURL;
    public string videoURL;
    public string objectName;

    public GameObject dropDownLoading;
    public Texture2D previewImage;
    public UnityEngine.UI.RawImage previewImageDisplay;
    public UnityEngine.UI.RawImage previewImageDisplayChoose;
    public UnityEngine.UI.InputField ImageTitle;
    public UnityEngine.UI.InputField URLVideo;
    public UnityEngine.UI.InputField URLWebsite;
    public SceneSwitcher ingameSceneManager;
    public List<string> targetIDs;
    public JSONObject jsonTargetSummary;
    public UnityEngine.UI.Dropdown targetIDDropdown;
    public UnityEngine.UI.RawImage CapturePreview;

    protected MenuBarSceneLoader menuBarInstance
    {
        get
        {
            if (_menuBarInstance == null)
            {
                GameObject[] SceneObjects = UnityEngine.SceneManagement.SceneManager.GetSceneByName("Main").GetRootGameObjects();
                foreach (GameObject rootObject in SceneObjects)
                {
                    if (rootObject.name.Contains("SceneManager"))
                    {
                        _menuBarInstance = rootObject.GetComponent<MenuBarSceneLoader>();
                    }
                }
            }
            return _menuBarInstance;
        }
    }

    public MenuBarSceneLoader _menuBarInstance;

    public GameObject[] AreYouSureWindows;

    void Awake()
    {
        _instance = this;
        //if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Manage"))
        //    GetAllImageTargetIDS();
    }

    public void ShowAd()
    {
        if (Advertisement.IsReady())
        {
            Advertisement.Show();
        }
    }

    void SetTargetIdsToDropdown()
    {
        Debug.Log("Setting Dropdown Informations");
        if (dropDownLoading != null)
        {
            dropDownLoading.SetActive(false);            
        }
        targetIDDropdown.AddOptions(targetIDs);
    }

    void UpdateDropdownItems()
    {
        targetIDDropdown.ClearOptions();
        SetTargetIdsToDropdown();
    }

    void RemoveDropdownItem(int index)
    {
        targetIDs.RemoveAt(index);
        UpdateDropdownItems();
    }

    string BuildMetaData()
    {
        string returnValue = "{" +
            "\"name\"" + ":" + "\"" +ImageTitle.text + "\"" + "," +
            "\"ContentType\"" + ":" + "\"" + contentType.ToString() + "\"" + "," +
            ReturnFormattedContentType() +
            "}";
        return returnValue;
    }

    string ReturnFormattedContentType()
    {
        switch (contentType)
        {
            case ContentTypeEnum.Video:
                return "\"urlVideo\"" + ":" + "\""+videoURL + "\"";
            case ContentTypeEnum.Website:
                if (!webURL.Contains("http://"))
                    webURL = "http://" + webURL;
                return "\"urlWeb\"" + ":" + "\""+webURL + "\"";
            case ContentTypeEnum.Object:
                return "\"objectName\"" + ":" + "\""+objectName + "\"";
            default:
                Debug.Log("a valid ContentTypeEnum option was not selected for compiling meta data");
                break;
        }
        return "";
    }
    public void SetConsolePanelTextTo(string text)
    {
        ConsoleOutput.instance.SetConsolePanelTextTo(text);
    }

    public void AppendTextToConsole(string text)
    {
        ConsoleOutput.instance.AppendTextToConsole (text);
    }

  //  IEnumerator UploadFileCo()
  //  {
  //      SceneManager.LoadIngameScene("Upload Progress");
  //      //TODO Change the Main Bar Light to Yellow
  //      SetConsolePanelTextTo("Uploading Photo To Database");

		//ShowAd(); /// evil ad plug while uploading  <---- here ///

  //      byte[] bytes = previewImage.EncodeToJPG();
  //      WWWForm postForm = new WWWForm();
  //      postForm.AddBinaryData("theFile", bytes, ImageTitle.text+".jpg", "text/plain");
  //      postForm.AddField("meta", BuildMetaData());
  //      WWW upload = new WWW(uploadURL, postForm);
  //      yield return upload;
  //      if (upload.error == null)
  //      {
  //          SetConsolePanelTextTo("upload done :" + upload.text);
  //          //TODO Add Parse for Target ID (added target: TargetID)
  //          string targetID = ParseForTargetID(upload.text);
  //          Debug.Log("targetID = " + targetID);
  //          Debug.Log(upload.text);
            
  //          //TODO call menu bar to Blink the ImageUploadVuphoria light yellow
  //          ChangeMainBarLightYellow();
  //          //TODO Create a gameObject and Dont Destroy on Load - Have it run a check every 2 seconds on the target ID and send message when it is found
  //          //StartCoroutine(GetTargetIDInfo(targetID)); // TestingCode
  //          CreateTargetIDChecker(targetID);
  //          PlayerPrefs.SetString("ViewInstructions", "true");
  //      }
  //      else
  //      {
  //          //TODO Change Upload Light to Red if the target upload failed
  //          SetConsolePanelTextTo("Error during upload: " + upload.error);
  //          ChangeMainBarLightRed();
  //      }
  //  }

    string ParseForTargetID(string text)
    {
        string[] textSplit = text.Split(':'); // The format from the server is below
        //upload done :AppleButter.jpg /home/corearedorg/webapps/augmentourworld/Vuphoria/images/AppleButter.jpg {"name":"AppleButter","ContentType":"Object","objectName":"ButterFly"}\nSuccessfully added target: 4d36ddcc4d8c439aad95b6b34ece1f9e
        return textSplit[textSplit.Length - 1].Trim(' '); //Removes whitespace and returns target
    }

    private void ChangeMainBarLightYellow()
    {
        Debug.Log("Changing main bar to yellow");
        menuBarInstance.BlinkButtonPendingColor();
    }

    //private void CreateTargetIDChecker(string targetID)
    //{
    //    GameObject loadCheckManager = new GameObject("LoadCheckManager");
    //    CheckUploadProgress checkingProgress = loadCheckManager.AddComponent<CheckUploadProgress>();
    //    checkingProgress.GetCheckTargetIDInfo(targetID,menuBarInstance,mainURL);
    //    StartCoroutine(checkingProgress.PostToFacebook(uploadImageURLFBShare + ImageTitle.text + ".jpg"));
    //    DontDestroyOnLoad(loadCheckManager);
    //}

    private void ChangeMainBarLightRed()
    {
        menuBarInstance.ErrorButtonPending();
    }


    List<string> ParseReturnString(string IDTargetsRawData)
    {
        List<string> returnValue = new List<string>();
        returnValue.AddRange(IDTargetsRawData.Split('|'));
        return returnValue;
    }

    public void SetActiveObjectName(string name)
    {
        objectName = name;
    }

    //public void UploadFile()
    //{
    //    //TODO Add a check if URL or 3D Object was selected - If URL Check the URL text and then activate Required field - if 3D Object then Check that a 3D Object was selected from Dropdown
    //    if (CheckValidUploadData())
    //        StartCoroutine(UploadFileCo());
    //    else
    //        Debug.Log("Data not valid to submit upload");
    //}

    bool CheckValidUploadData()
    {

        switch(contentType)
        {
            case ContentTypeEnum.Video:
                if (CheckValidInput(URLVideo) && CheckValidInput(ImageTitle))
                {
                    videoURL = URLVideo.text;
                    return true;
                }
                break;
            case ContentTypeEnum.Website:
                if (CheckValidInput(URLWebsite) && CheckValidInput(ImageTitle))
                {
                    webURL = URLWebsite.text;
                    return true;
                }
                break;
            case ContentTypeEnum.Object:
                if (objectName != "")
                    return true;
                break;
            default:
                Debug.Log("a valid ContentTypeEnum option was not selected ");
                break;
        }
        return false;
    }

    bool CheckValidInput(UnityEngine.UI.InputField inputCheck)
    {
        if (inputCheck.text == "")
        {
            ActivateRequireField(inputCheck);
            return false;
        }
        return true;
    }

    void ActivateRequireField(UnityEngine.UI.InputField inputField)
    {
        inputField.placeholder.color = Color.red;
        //TODO if URL toggle selected and URL field has no text set placeholder for URL to red
    }

    //public void UploadCapturedTargetToServer(byte[] bytes)
    //{
    //    StartCoroutine(UploadTargetToServer(bytes));
    //}

    //public void GetAllImageTargetIDS()
    //{
    //    StartCoroutine(GetAllTargetIDs());
    //}

    //public void GetImageTargetInfo()
    //{
    //    StartCoroutine(GetTargetIDInfo());
    //}

    //public void GetImageTargetInfo(string targetID)
    //{
    //    StartCoroutine(GetTargetIDInfo(targetID));
    //}

    //public void GetImageTargetSummary()
    //{
    //    VWSCloudConnecterService.GetTargetIDSummary(targetIDDropdown.options[targetIDDropdown.value].text);
    //}

    //public void ConfirmationDeleteTargetID()
    //{
    //    ToggleAreYouSure(true);
    //}

    //public void DeleteTargetID()
    //{
    //    ToggleAreYouSure(false);
    //    StartCoroutine(DeleteTarget());
    //}

    //public void ConfirmationDeleteAllTargetIDs()
    //{
    //    ToggleAreYouSureAll(true);
    //}

    //public void DeleteAllTargetIDs()
    //{
    //    ToggleAreYouSureAll(false);
    //    StartCoroutine(DeleteAllTargets());
    //}

    void ToggleAreYouSureAll(bool OnOff)
    {
        AreYouSureWindows[0].SetActive(OnOff);
    }

    void ToggleAreYouSure(bool OnOff)
    {
        AreYouSureWindows[1].SetActive(OnOff);
    }

    public void DisableSureWindow()
    {
        foreach(GameObject gWindow in AreYouSureWindows)
        {
            gWindow.SetActive(false);
        }
    }

    public void UpdateImageTarget()
    {
        SceneManager.LoadIngameScene("UpdateImageTarget");
    }

    public void PreviewImage(Texture2D image,Texture2D mask)
    {        
        SetConsolePanelTextTo("Previewing Image, Cutting out Mask");
        SetPreviewImage(CutOutMask.CutOut(image, mask));
        SetConsolePanelTextTo("Mask Cutting Complete");
        SceneManager.LoadIngameScene("PreviewWindow");
    }

    public void PreviewImage(Texture2D image)
    {
        SetConsolePanelTextTo("Setting Preview Image");
        SetPreviewImage(image);
        SceneManager.LoadIngameScene("PreviewWindow");
    }

    public void SetPreviewImage(Texture2D image)
    {
        previewImage = image;
        previewImageDisplay.texture = image;
        previewImageDisplayChoose.texture = image;
    }
}