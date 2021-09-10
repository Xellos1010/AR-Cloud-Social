using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleOutput : SingletonBehaviour<ConsoleOutput>
{
    public TMPro.TextMeshProUGUI consoleTextObject;
    public void SetConsolePanelTextTo(string text)
    {
        if (consoleTextObject != null)
            consoleTextObject.text = text;
        else
            Debug.Log(text);
    }

    public void AppendTextToConsole(string text)
    {
        Debug.Log("Appending to Panel Text");
        if (consoleTextObject != null)
            consoleTextObject.text += "\n" + text;
        else
            Debug.Log(text);
    }
}
