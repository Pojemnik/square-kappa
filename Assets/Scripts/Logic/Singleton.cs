using UnityEngine;

/// <summary>
/// Inherit from this base class to create a singleton.
/// e.g. public class MyClassName : Singleton<MyClassName> {}
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // Check to see if we're about to be destroyed.
    private static bool m_ShuttingDown = false;
    private static object m_Lock = new object();
    private static T m_Instance;

    /// <summary>
    /// Access singleton instance through this propriety.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (m_ShuttingDown)
            {
                //Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                //   "' already destroyed. Returning null.");
                return null;
            }
            lock (m_Lock)
            {
                if (m_Instance == null)
                {
                    m_Instance = FindObjectOfType<T>();
                    if (m_Instance == null)
                    {
                        Debug.LogErrorFormat("No exisitng sigleton of type {0}", typeof(T));
                    }
                }
                return m_Instance;
            }
        }
    }

    protected static void RegisterInstance(T instance)
    {
        if (m_Instance != null && m_Instance != instance)
        {
            Debug.LogErrorFormat("Instance of singleton {0} registered more than once", typeof(T));
            return;
        }
        m_Instance = instance;
        m_ShuttingDown = false;
    }


    private void OnApplicationQuit()
    {
        m_ShuttingDown = true;
    }


    private void OnDestroy()
    {
        m_ShuttingDown = true;
    }
}