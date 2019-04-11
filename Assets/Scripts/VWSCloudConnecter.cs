using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Advertisements;
using UnityEngine.Networking;
using UnityEditor;

enum phpModeSelet
{
    None,
    AddTarget,
    GetTargetInfo,
    GetTargetSummary,
    GetAllTargets,
    UpdateTarget,
    DeleteTarget,
    DeleteAllTargets
}

public class VWSCloudConnecter : MonoBehaviour {

    public static VWSCloudConnecter instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<VWSCloudConnecter>();
            return _instance;
        }
    }
    private static VWSCloudConnecter _instance;

    string uploadURL = "http://evanmccall.info/phpVuf/imageupload.php";
    string mainURL = "http://evanmccall.info/phpVuf/main.php";
    public ContentTypeEnum contentType;
    string webURL;
    string videoURL;
    string objectName;

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
    public UnityEngine.UI.Text PanelText;
    public UnityEngine.UI.Dropdown targetIDDropdown;
    public UnityEngine.UI.RawImage CapturePreview;
    public Dictionary<string, string> targetNameToID;
    public SetTargetInfo targetInfoArea;
    public GameObject[] AreYouSureWindows;

    void Awake()
    {
        _instance = this;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Manage"))
            GetAllImageTargetIDS();
    }

    private void OnEnable()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Manage"))
            targetIDDropdown.onValueChanged.AddListener(delegate {
                DropdownValueChanged(targetIDDropdown);
            });
    }

    private void OnDisable()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Manage"))
            targetIDDropdown.onValueChanged.RemoveAllListeners();
    }

    //Ouput the new value of the Dropdown into Text
    void DropdownValueChanged(UnityEngine.UI.Dropdown change)
    {
        //Populate Info On Target;
        Debug.Log(change.value);
        StartCoroutine(GetTargetSummaryByID(targetIDs[change.value]));
    }

    public void ShowAd()
    {
        /*
        if (Advertisement.IsReady())
        {
            Advertisement.Show();
        }
        */
    }


    void WriteToPanel(string text)
    {
        if (PanelText != null)
            PanelText.text = text;
        else
            Debug.Log(text);
    }

    void AppendToPanelText(string text)
    {
        Debug.Log("Appending to Panel Text");
        if (PanelText != null)
            PanelText.text += "\n" + text;
        else
            Debug.Log(text);
    }

    void SetDropdownItems()
    {
        Debug.Log("Setting Dropdown Informations");
        if (dropDownLoading != null)
        {
            dropDownLoading.SetActive(false);
        }
        targetIDDropdown.AddOptions(targetIDs);
        StartCoroutine(GetTargetSummaryByID(targetIDs[0]));
    }

    void UpdateDropdownItems()
    {
        targetIDDropdown.ClearOptions();
        SetDropdownItems();
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
    
    IEnumerator UploadFileCo()
    {
        StaticPanelManager.LoadIngameScene("Upload Progress");
        //TODO Change the Main Bar Light to Yellow
        WriteToPanel("Uploading Photo To Database");

		ShowAd(); /// evil ad plug while uploading

        byte[] bytes = previewImage.EncodeToJPG();
        WWWForm postForm = new WWWForm();
        postForm.AddBinaryData("theFile", bytes, ImageTitle.text+".jpg", "text/plain");
        postForm.AddField("meta", BuildMetaData());
        using (UnityWebRequest upload = UnityWebRequest.Post(uploadURL, postForm))
        {
            yield return upload.SendWebRequest();
            if (upload.isNetworkError || upload.isHttpError)
            {
                Debug.Log(upload.error);
                //TODO Change Upload Light to Red if the target upload failed
                WriteToPanel("Error during upload: " + upload.error);
                ChangeMainBarLightRed();
            }
            else
            {
                //WriteToPanel("upload done :" + upload.downloadHandler.text);
                //TODO Add Parse for Target ID (added target: TargetID)
                Debug.Log(upload.downloadHandler.text);
                string targetID = ParseForTargetID(upload.downloadHandler.text);
                Debug.Log("targetID = " + targetID);
                ChangeMainBarLightYellow();
                CreateTargetIDChecker(targetID);
                PlayerPrefs.SetString("ViewInstructions", "true");
            }
        }
    }

    string ParseForTargetID(string text)
    {
        string[] textSplit = text.Split(':'); // The format from the server is below
        //upload done :AppleButter.jpg /home/corearedorg/webapps/augmentourworld/Vuphoria/images/AppleButter.jpg {"name":"AppleButter","ContentType":"Object","objectName":"ButterFly"}\nSuccessfully added target: 4d36ddcc4d8c439aad95b6b34ece1f9e
        return textSplit[textSplit.Length - 1].Trim(' '); //Removes whitespace and returns target
    }

    private void ChangeMainBarLightYellow()
    {
        Debug.Log("Changing main bar to yellow");
        MenuBarStateManager.instance.BlinkButtonPendingColor();
    }

    private void CreateTargetIDChecker(string targetID)
    {
        GameObject loadCheckManager = new GameObject("LoadCheckManager");
        CheckUploadProgress checkingProgress = loadCheckManager.AddComponent<CheckUploadProgress>();
        checkingProgress.GetCheckTargetIDInfo(targetID, MenuBarStateManager.instance, mainURL);
        DontDestroyOnLoad(loadCheckManager);
    }

    private void ChangeMainBarLightRed()
    {
        MenuBarStateManager.instance.ErrorButtonPending();
    }

    IEnumerator UploadFileCo(UnityEngine.UI.InputField inputStatus)
    {
        inputStatus.text = "Starting Upload...\n";
        byte[] bytes = previewImage.EncodeToJPG();
        WWWForm postForm = new WWWForm();
        postForm.AddBinaryData("theFile", bytes, ImageTitle.text + ".jpg", "text/plain");
        postForm.AddField("meta", "www.augmentourworld.com");
        WWW upload = new WWW(uploadURL, postForm);
        inputStatus.text += "Submitting to the Server \n";
        yield return upload;
        if (upload.error == null)
        {
            inputStatus.text += "upload done :" + upload.text;
            yield return true;
        }
        else
        {
            inputStatus.text = "Error during upload: " + upload.error;
            yield return false;
        }
    }

    IEnumerator UploadTargetToServer(byte[] bytes)
    {
        WWWForm postForm = new WWWForm();
        postForm.AddBinaryData("theFile", bytes, previewImage.name + ".jpg", "text/plain");
        postForm.AddField("meta", BuildMetaData());//"www.augmentourworld.com");
        WWW upload = new WWW(uploadURL, postForm);
        yield return upload;
        if (upload.error == null)
            WriteToPanel("upload done :" + upload.text);
        else
            WriteToPanel("Error during upload: " + upload.error);
    }

    IEnumerator GetAllTargetIDs()
    {
        Debug.Log("Getting all Target ID's");
        WWWForm postForm = new WWWForm();
        postForm.AddField("select", phpModeSelet.GetAllTargets.ToString());
        using (UnityWebRequest getTargetIDs = UnityWebRequest.Post(mainURL, postForm))
        {
            yield return getTargetIDs.SendWebRequest();
            if (getTargetIDs.isNetworkError || getTargetIDs.isHttpError)
            {
                Debug.Log(getTargetIDs.error);
                WriteToPanel("Error during Getting Target ID's: " + getTargetIDs.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
                Debug.Log("getTargetIDs.text = " + getTargetIDs.downloadHandler.text);
                WriteToPanel("Getting Target ID's done :" + getTargetIDs.downloadHandler.text);
                targetIDs = ParseReturnString(getTargetIDs.downloadHandler.text);
                for (int i = targetIDs.Count - 1; i > 0; i--)
                {
                    if (String.IsNullOrEmpty(targetIDs[i]))
                    {
                        targetIDs.RemoveAt(i);
                    }
                }
                AppendToPanelText(targetIDs.ToString());
                SetDropdownItems();
            }
        }
    }

    IEnumerator GetTargetIDSummary()
    {
        if (targetIDDropdown.options.Count > 0)
        {
            string targetID = targetIDDropdown.options[targetIDDropdown.value].text;
            WWWForm postForm = new WWWForm();
            postForm.AddField("select", phpModeSelet.GetTargetSummary.ToString());
            postForm.AddField("targetID", targetID);
            WWW getTargetIDs = new WWW(mainURL, postForm);
            yield return getTargetIDs;
            if (getTargetIDs.error == null)
            {
                WriteToPanel("Getting Target ID's done :" + getTargetIDs.text);
                jsonTargetSummary = new JSONObject(getTargetIDs.text);
                Debug.Log(jsonTargetSummary);
                StaticPanelManager.LoadIngameScene("DisplayText");
            }
            else
                WriteToPanel("Error during Getting Target ID's: " + getTargetIDs.error);
        }
        else
        {
            WriteToPanel("Select an Image Target from the Dropdown Menu.\nUse Get All Image ID's to update Dropdown options");
        }

    }

    internal IEnumerator GetTargetSummaryByID(string target)
    {
        yield return GetTargetIDInfo(target);
        //Debug.Log(jsonTargetSummary.Print(true));
        targetInfoArea.SetTargetInfoText(jsonTargetSummary["target_record"]["name"].ToString(), jsonTargetSummary["target_record"]["tracking_rating"].ToString(), jsonTargetSummary["target_record"]["width"].ToString());
    }

    IEnumerator GetTargetIDInfo(string targetID)
    {
        Debug.Log("Getting Info on Target ID " + targetID);
        WWWForm postForm = new WWWForm();
        postForm.AddField("select", phpModeSelet.GetTargetInfo.ToString());
        postForm.AddField("targetID", targetID);
        using (UnityWebRequest www = UnityWebRequest.Post(mainURL, postForm))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                jsonTargetSummary = new JSONObject(www.downloadHandler.text);
                /*Debug.Log("Form upload complete!");
                Debug.Log("getTargetIDs.text = " + www.downloadHandler.text);
                
                Debug.Log(jsonTargetSummary.Print(true));
                Debug.Log(jsonTargetSummary["target_record"]["name"]);*/
            }
        }
    }

    IEnumerator GetTargetIDInfo()
    {
        if (targetIDDropdown.options.Count > 0)
        {
            string targetID = targetIDDropdown.options[targetIDDropdown.value].text;
            WWWForm postForm = new WWWForm();
            postForm.AddField("select", phpModeSelet.GetTargetInfo.ToString());
            postForm.AddField("targetID", targetID);
            WWW getTargetIDs = new WWW(mainURL, postForm);
            yield return getTargetIDs;
            if (getTargetIDs.error == null)
            {
                WriteToPanel("Getting Target ID's done :" + getTargetIDs.text);
                jsonTargetSummary = new JSONObject(getTargetIDs.text);
                Debug.Log(jsonTargetSummary);
                StaticPanelManager.LoadIngameScene("DisplayText");
            }
            else
                WriteToPanel("Error during Getting Target ID's: " + getTargetIDs.error);
            }
        else
        {
            WriteToPanel("Select an Image Target from the Dropdown Menu.\nUse Get All Image ID's to update Dropdown options");
        }
    }

    IEnumerator DeleteTarget()
    {
        if (targetIDDropdown.options.Count > 0)
        {
            string targetID = targetIDDropdown.options[targetIDDropdown.value].text;
            WWWForm postForm = new WWWForm();
            postForm.AddField("select", phpModeSelet.DeleteTarget.ToString());
            postForm.AddField("targetID", targetID);
            WWW deleteTargetID = new WWW(mainURL, postForm);
            yield return deleteTargetID;
            if (deleteTargetID.error == null)
            {
                WriteToPanel("Getting Target ID's done :" + deleteTargetID.text);
                jsonTargetSummary = new JSONObject(deleteTargetID.text);
                Debug.Log(jsonTargetSummary);
                RemoveDropdownItem(targetIDDropdown.value);
            }
            else
                WriteToPanel("Error during Getting Target ID's: " + deleteTargetID.error);
        }
        else
        {
            WriteToPanel("Select an Image Target from the Dropdown Menu.\nUse Get All Image ID's to update Dropdown options");
        }
    }

    IEnumerator DeleteAllTargets()
    {
        WWWForm postForm = new WWWForm();
        postForm.AddField("select", phpModeSelet.DeleteAllTargets.ToString());
        WWW deleteTargetIDs = new WWW(mainURL, postForm);
        yield return deleteTargetIDs;
        if (deleteTargetIDs.error == null)
        {
            WriteToPanel("DeletingTarget all ID's done :" + deleteTargetIDs.text);
        }
        else
            WriteToPanel("Error during Deleting all Target ID's: " + deleteTargetIDs.error);
    }

    IEnumerator UpdateTargetInfo()
    {
        if (targetIDDropdown.options.Count > 0)
        {
            string targetID = targetIDDropdown.options[targetIDDropdown.value].text;
            WWWForm postForm = new WWWForm();            
            postForm.AddField("targetID", targetID);
            postForm.AddField("name", targetID);
            postForm.AddField("image", targetID);
            postForm.AddField("width", targetID);
            postForm.AddField("", targetID);
            postForm.AddField("targetID", targetID);
            WWW getTargetIDs = new WWW(mainURL, postForm);
            yield return getTargetIDs;
            if (getTargetIDs.error == null)
            {
                WriteToPanel("Getting Target ID's done :" + getTargetIDs.text);
                jsonTargetSummary = new JSONObject(getTargetIDs.text);
                Debug.Log(jsonTargetSummary);
            }
            else
                WriteToPanel("Error during Getting Target ID's: " + getTargetIDs.error);
        }
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

    public void UploadFile()
    {
        //TODO Add a check if URL or 3D Object was selected - If URL Check the URL text and then activate Required field - if 3D Object then Check that a 3D Object was selected from Dropdown
        if (CheckValidUploadData())
            StartCoroutine(UploadFileCo());
        else
            Debug.Log("Data not valid to submit upload");
    }

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

    public void UploadCapturedTargetToServer(byte[] bytes)
    {
        StartCoroutine(UploadTargetToServer(bytes));
    }

    public void GetAllImageTargetIDS()
    {
        StartCoroutine(GetAllTargetIDs());
    }

    public void GetImageTargetInfo()
    {
        StartCoroutine(GetTargetIDInfo());
    }

    public void GetImageTargetInfo(string targetID)
    {
        StartCoroutine(GetTargetIDInfo(targetID));
    }

    public void GetImageTargetSummary()
    {
        StartCoroutine(GetTargetIDSummary());
    }

    public void ConfirmationDeleteTargetID()
    {
        ToggleAreYouSure(true);
    }

    public void DeleteTargetID()
    {
        ToggleAreYouSure(false);
        StartCoroutine(DeleteTarget());
    }

    public void ConfirmationDeleteAllTargetIDs()
    {
        ToggleAreYouSureAll(true);
    }

    public void DeleteAllTargetIDs()
    {
        ToggleAreYouSureAll(false);
        StartCoroutine(DeleteAllTargets());
    }

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
        StaticPanelManager.LoadIngameScene("UpdateImageTarget");
    }

    public void PreviewImage(Texture2D image,Texture2D mask)
    {        
        WriteToPanel("Previewing Image, Cutting out Mask");
        SetPreviewImage(CutOutMask.CutOut(image, mask));
        WriteToPanel("Mask Cutting Complete");
        StaticPanelManager.LoadIngameScene("PreviewWindow");
    }

    public void PreviewImage(Texture2D image)
    {
        WriteToPanel("Setting Preview Image");
        SetPreviewImage(image);
        StaticPanelManager.LoadIngameScene("PreviewWindow");
    }

    public void SetPreviewImage(Texture2D image)
    {
        previewImage = image;
        previewImageDisplay.texture = image;
        previewImageDisplayChoose.texture = image;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(VWSCloudConnecter))]
public class VWSCloudConnecterEditor:Editor
{
    VWSCloudConnecter _target;

    private void OnEnable()
    {
        _target = (VWSCloudConnecter)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Get Info on TargetID"))
            _target.GetTargetSummaryByID("de2f2b6ae98343aa88825388741594f1");
        if (GUILayout.Button("Upload Image Target"))
        {
            _target.UploadFile();
        }
    }
}
#endif