
#if UNITY_EDITOR
using UnityEditor;

public class CustomBaseManagerEditor : Editor
{
    //public bool enableDefaultInspector = false;
    public override void OnInspectorGUI()
    {
        EditorUtilities.DrawUILine(UnityEngine.Color.white);
        //enableDefaultInspector = EditorGUILayout.Toggle("Toggle to view Inspector raw", enableDefaultInspector);
        //if (enableDefaultInspector)
        base.OnInspectorGUI();
    }
}
#endif
