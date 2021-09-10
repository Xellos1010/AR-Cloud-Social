using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

/* Example:
var getRequest = UnityWebRequest.Get("http://www.google.com");
await getRequest.SendWebRequest();
var result = getRequest.downloadHandler.text;
*/
public enum phpModeSelet
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

public class VWSCloudConnecterService
{
    private static string uploadURL = "http://www.evanmccallvr.com/TakeOver/imageupload.php/";
    private static string mainURL = "http://www.evanmccallvr.com/TakeOver/main.php/";
    private static string unityPostGetURL = "http://www.evanmccallvr.com/TakeOver/unityphppostget.php/";
    private static string uploadImageURLFBShare = "http://www.augmentourworld.com/Vuphoria/images/";

    public void SendPostRequestTest(MonoBehaviour callingObject, string Message = "")
    {
        callingObject.StartCoroutine(SendPRIE(Message));
    }

    public void SendGetRequestTest(MonoBehaviour callingObject, string Message = "")
    {
        callingObject.StartCoroutine(SendGRIE(Message));
    }

    IEnumerator SendPRIE(string Message)
    {
        WWWForm form = new WWWForm();
        form.AddField("unitypost", Message);
        using (UnityWebRequest www = UnityWebRequest.Post(unityPostGetURL, form))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string responseText = www.downloadHandler.text;
                Debug.Log("Response Text from the server = " + responseText);
            }
        }
    }

    internal async Task SendGRTask(string Message)
    {
        using (UnityWebRequest www = UnityWebRequest.Get($"{unityPostGetURL}?unityget=" + Message))
        {
            await www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string responseText = www.downloadHandler.text;
                Debug.Log("Response Text from the server = " + responseText);
            }
        }
    }

    internal async Task SendPRTask(string Message)
    {
        WWWForm form = new WWWForm();
        form.AddField("unitypost", Message);
        using (UnityWebRequest www = UnityWebRequest.Post(unityPostGetURL, form))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            await www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string responseText = www.downloadHandler.text;
                Debug.Log("Response Text from the server = " + responseText);
            }
        }
    }

    IEnumerator SendGRIE(string Message)
    {
        using (UnityWebRequest www = UnityWebRequest.Get($"{unityPostGetURL}?unityget=" + Message))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string responseText = www.downloadHandler.text;
                Debug.Log("Response Text from the server = " + responseText);
            }
        }
    }

    public async Task<string> GetTargetIDSummary(string targetID)
    {
        if (targetID != null && targetID != "")
        {
            WWWForm form = new WWWForm();
            form.AddField("select", phpModeSelet.GetTargetSummary.ToString());
            form.AddField("targetID", targetID);
            using (UnityWebRequest www = UnityWebRequest.Post(mainURL,form))
            {
                www.downloadHandler = new DownloadHandlerBuffer();
                await www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.ConnectionError )
                {
                    Debug.Log(www.error);
                    return www.error;
                }
                else
                {
                    string responseText = www.downloadHandler.text;
                    Debug.Log("Response Text from the server = " + responseText);
                    return responseText;
                }
            }
        }
        else
        {
            return "Please set a proper targetID";
        }
    }

    //[TestMethod]
    //public void IsPalindrome_ForPalindromeString_ReturnsTrue()
    //{
    //    // In the Arrange phase, we create and set up a system under test.
    //    // A system under test could be a method, a single object, or a graph of connected objects.
    //    // It is OK to have an empty Arrange phase, for example if we are testing a static method -
    //    // in this case SUT already exists in a static form and we don't have to initialize anything explicitly.
    //    PalindromeDetector detector = new PalindromeDetector();

    //    // The Act phase is where we poke the system under test, usually by invoking a method.
    //    // If this method returns something back to us, we want to collect the result to ensure it was correct.
    //    // Or, if method doesn't return anything, we want to check whether it produced the expected side effects.
    //    bool isPalindrome = detector.IsPalindrome("kayak");

    //    // The Assert phase makes our unit test pass or fail.
    //    // Here we check that the method's behavior is consistent with expectations.
    //    Assert.IsTrue(isPalindrome);
    //}

    //public static Task GetTargetIDInfo(string targetID)
    //{
    //    WWWForm postForm = new WWWForm();
    //    postForm.AddField("select", phpModeSelet.GetTargetInfo.ToString());
    //    postForm.AddField("targetID", targetID);
    //    WWW getTargetIDs = new WWW(mainURL, postForm);
    //    Debug.Log("Getting target ID Info. targetID = " + targetID + " url = " + getTargetIDs.url);
    //    yield return getTargetIDs;
    //    if (getTargetIDs.error == null)
    //    {
    //        Debug.Log("getTargetIDs.text = " + getTargetIDs.text);
    //        jsonTargetSummary = new JSONObject(getTargetIDs.text);

    //    }
    //}

    //public static Task GetTargetIDInfo()
    //{
    //    if (targetIDDropdown.options.Count > 0)
    //    {
    //        string targetID = targetIDDropdown.options[targetIDDropdown.value].text;
    //        WWWForm postForm = new WWWForm();
    //        postForm.AddField("select", phpModeSelet.GetTargetInfo.ToString());
    //        postForm.AddField("targetID", targetID);
    //        WWW getTargetIDs = new WWW(mainURL, postForm);
    //        yield return getTargetIDs;
    //        if (getTargetIDs.error == null)
    //        {
    //            SetConsolePanelTextTo("Getting Target ID's done :" + getTargetIDs.text);
    //            jsonTargetSummary = new JSONObject(getTargetIDs.text);
    //            Debug.Log(jsonTargetSummary);
    //            SceneManager.LoadIngameScene("DisplayText");
    //        }
    //        else
    //            SetConsolePanelTextTo("Error during Getting Target ID's: " + getTargetIDs.error);
    //    }
    //    else
    //    {
    //        SetConsolePanelTextTo("Select an Image Target from the Dropdown Menu.\nUse Get All Image ID's to update Dropdown options");
    //    }
    //}

    //public static Task DeleteTarget()
    //{
    //    if (targetIDDropdown.options.Count > 0)
    //    {
    //        string targetID = targetIDDropdown.options[targetIDDropdown.value].text;
    //        WWWForm postForm = new WWWForm();
    //        postForm.AddField("select", phpModeSelet.DeleteTarget.ToString());
    //        postForm.AddField("targetID", targetID);
    //        WWW deleteTargetID = new WWW(mainURL, postForm);
    //        yield return deleteTargetID;
    //        if (deleteTargetID.error == null)
    //        {
    //            SetConsolePanelTextTo("Getting Target ID's done :" + deleteTargetID.text);
    //            jsonTargetSummary = new JSONObject(deleteTargetID.text);
    //            Debug.Log(jsonTargetSummary);
    //            RemoveDropdownItem(targetIDDropdown.value);
    //        }
    //        else
    //            SetConsolePanelTextTo("Error during Getting Target ID's: " + deleteTargetID.error);
    //    }
    //    else
    //    {
    //        SetConsolePanelTextTo("Select an Image Target from the Dropdown Menu.\nUse Get All Image ID's to update Dropdown options");
    //    }
    //}

    //public static Task DeleteAllTargets()
    //{
    //    WWWForm postForm = new WWWForm();
    //    postForm.AddField("select", phpModeSelet.DeleteAllTargets.ToString());
    //    WWW deleteTargetIDs = new WWW(mainURL, postForm);
    //    yield return deleteTargetIDs;
    //    if (deleteTargetIDs.error == null)
    //    {
    //        SetConsolePanelTextTo("DeletingTarget all ID's done :" + deleteTargetIDs.text);
    //    }
    //    else
    //        SetConsolePanelTextTo("Error during Deleting all Target ID's: " + deleteTargetIDs.error);
    //}

    //public static Task UpdateTargetInfo()
    //{
    //    if (targetIDDropdown.options.Count > 0)
    //    {
    //        string targetID = targetIDDropdown.options[targetIDDropdown.value].text;
    //        WWWForm postForm = new WWWForm();
    //        postForm.AddField("targetID", targetID);
    //        postForm.AddField("name", targetID);
    //        postForm.AddField("image", targetID);
    //        postForm.AddField("width", targetID);
    //        postForm.AddField("", targetID);
    //        postForm.AddField("targetID", targetID);
    //        WWW getTargetIDs = new WWW(mainURL, postForm);
    //        yield return getTargetIDs;
    //        if (getTargetIDs.error == null)
    //        {
    //            SetConsolePanelTextTo("Getting Target ID's done :" + getTargetIDs.text);
    //            jsonTargetSummary = new JSONObject(getTargetIDs.text);
    //            Debug.Log(jsonTargetSummary);
    //        }
    //        else
    //            SetConsolePanelTextTo("Error during Getting Target ID's: " + getTargetIDs.error);
    //    }
    //}


    //IEnumerator UploadFileCo(UnityEngine.UI.InputField inputStatus)
    //{
    //    inputStatus.text = "Starting Upload...\n";
    //    byte[] bytes = previewImage.EncodeToJPG();
    //    WWWForm postForm = new WWWForm();
    //    postForm.AddBinaryData("theFile", bytes, ImageTitle.text + ".jpg", "text/plain");
    //    postForm.AddField("meta", "www.augmentourworld.com");
    //    WWW upload = new WWW(uploadURL, postForm);
    //    inputStatus.text += "Submitting to the Server \n";
    //    yield return upload;
    //    if (upload.error == null)
    //    {
    //        inputStatus.text += "upload done :" + upload.text;
    //        yield return true;
    //    }
    //    else
    //    {
    //        inputStatus.text = "Error during upload: " + upload.error;
    //        yield return false;
    //    }
    //}

    //IEnumerator UploadTargetToServer(byte[] bytes)
    //{
    //    WWWForm postForm = new WWWForm();
    //    postForm.AddBinaryData("theFile", bytes, previewImage.name + ".jpg", "text/plain");
    //    postForm.AddField("meta", BuildMetaData());//"www.augmentourworld.com");
    //    WWW upload = new WWW(uploadURL, postForm);
    //    yield return upload;
    //    if (upload.error == null)
    //        SetConsolePanelTextTo("upload done :" + upload.text);
    //    else
    //        SetConsolePanelTextTo("Error during upload: " + upload.error);
    //}

    //IEnumerator GetAllTargetIDs()
    //{
    //    Debug.Log("Getting all Target ID's");
    //    WWWForm postForm = new WWWForm();
    //    postForm.AddField("select", phpModeSelet.GetAllTargets.ToString());
    //    WWW getTargetIDs = new WWW(mainURL, postForm);
    //    yield return getTargetIDs;
    //    if (getTargetIDs.error == null)
    //    {
    //        SetConsolePanelTextTo("Getting Target ID's done :" + getTargetIDs.text);
    //        targetIDs = ParseReturnString(getTargetIDs.text);
    //        for (int i = targetIDs.Count - 1; i > 0; i--)
    //        {
    //            if (String.IsNullOrEmpty(targetIDs[i]))
    //            {
    //                targetIDs.RemoveAt(i);
    //            }
    //        }
    //        AppendTextToConsole(targetIDs.ToString());
    //        SetTargetIdsToDropdown();
    //    }
    //    else
    //        SetConsolePanelTextTo("Error during Getting Target ID's: " + getTargetIDs.error);
    //}
}
