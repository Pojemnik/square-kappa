using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathBehaviour : MonoBehaviour
{
    [System.Serializable]
    public struct ForceData
    {
        public Vector3 force;
        public float time;
    }

    public List<ForceData> initList;
    public List<ForceData> forceList;

    private List<ForceData> currentList;
    private new Rigidbody rigidbody;
    private List<ForceData>.Enumerator currentForceData;
    private float timeCounter;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        timeCounter = 0;
        if(forceList == null || forceList.Count == 0)
        {
            throw new System.Exception("No forces in force list!");
        }
        currentList = forceList;
        if(initList != null && initList.Count > 0)
        {
            currentList = initList;
        }
        currentForceData = currentList.GetEnumerator();
    }

    void FixedUpdate()
    {
        timeCounter += Time.fixedDeltaTime;
        if(timeCounter > currentForceData.Current.time)
        {
            timeCounter -= currentForceData.Current.time;
            if(!currentForceData.MoveNext())
            {
                currentList = forceList;
                currentForceData = currentList.GetEnumerator();
            }
        }
        rigidbody.AddForce(currentForceData.Current.force * Time.fixedDeltaTime);
    }
}
