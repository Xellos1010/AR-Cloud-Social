using UnityEngine;
using UnityEditor;
using System.Collections;

public class ButtonIndicatorStateManager : MonoBehaviour {

    public bool pendingActive = false;
    UnityEngine.UI.Image lightImage
    {
        get
        {
            if (_lightImage == null)
                _lightImage = GetComponentInChildren<UnityEngine.UI.Image>();
            return _lightImage;
        }
    }
    UnityEngine.UI.Image _lightImage;

    public void ActivatePendingState()
    {
        Debug.Log("Activating Pending State for " + gameObject.name);
        ActivatePendingState(MenuBarStateManager.instance.pendingButtonColor, MenuBarStateManager.instance.inactiveButtonColor, MenuBarStateManager.instance.pendingColorBlinkDuration);
    }

    public void ActivatePendingState(Color pendingColor, Color inActiveColor, float pendingBlinkDuration)
    {
        Debug.Log("Activating Pending State for " + gameObject.name);
        pendingActive = true;
        StartCoroutine(FlashPending(pendingColor, inActiveColor,pendingBlinkDuration));
    }

    public void DeactivatePendingState()
    {
        pendingActive = false;
        StopAllCoroutines();
    }

    public void FlashPendingColor(Color pendingColor, Color inActiveColor, float pendingBlinkDuration)
    {
        StartCoroutine(FlashPending(pendingColor, inActiveColor, pendingBlinkDuration));
    }

    IEnumerator FlashPending(Color pendingColor, Color inActiveColor, float pendingBlinkDuration)
    {
        Debug.Log("Flashing Pending Color");
        ChangeColor(pendingColor);
        yield return new WaitForSeconds(pendingBlinkDuration);
        ChangeColor(inActiveColor);
        yield return new WaitForSeconds(pendingBlinkDuration);
        StartCoroutine(FlashPending(pendingColor, inActiveColor, pendingBlinkDuration));
    }

    public void StopFlashing()
    {
        StopAllCoroutines();
    }    

    public void ActivateActiveColor()
    {
        ChangeColor(MenuBarStateManager.instance.activeButtonColor);
    }

    public void ChangeColor(Color color)
    {
        lightImage.color = color;
        //TODO Fade Color
    }
}

#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(ButtonIndicatorStateManager))]
public class ButtonIndicatorStateManagerEditor : Editor
{
    ButtonIndicatorStateManager _target;
    private void OnEnable()
    {
        _target = (ButtonIndicatorStateManager)target;
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ButtonIndicatorStateManager targetScript = (ButtonIndicatorStateManager)target;
        if (GUILayout.Button("Set State To Pending"))
        {
            targetScript.ActivatePendingState();
        }
        if (GUILayout.Button("Stop Pending"))
        {
            targetScript.DeactivatePendingState();
        }
        if (GUILayout.Button("Set Active State"))
        {
            targetScript.ActivateActiveColor();
        }
        if(GUILayout.Button("Set MenuLoader Button Active to this"))
            {
            MenuBarStateManager.instance.SetActiveButton(_target.gameObject);
        }
    }
}
#endif