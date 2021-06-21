using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugHelper : MonoBehaviour
{
#if UNITY_EDITOR
    public bool useSceneView = false;

    private void Awake()
    {
        if (useSceneView)
        {
            Cursor.lockState = CursorLockMode.None;
            UnityEditor.EditorWindow.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
        }
    }
#endif
}
