using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPanelFadeOnEnable : MonoBehaviour
{
    private void OnEnable()
    {
        StaticPanelManager.AlphaManagerCanvasGroup = GetComponent<CanvasGroup>();
    }
}
