using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugHelper : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField]
    private bool useSceneView = false;

    private void Start()
    {
        if (useSceneView)
        {
            EventManager.Instance.TriggerEvent("UnlockCursor");
            UnityEditor.EditorWindow.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
        }
        else
        {
            EventManager.Instance.TriggerEvent("LockCursor");
        }
    }
#endif
}
