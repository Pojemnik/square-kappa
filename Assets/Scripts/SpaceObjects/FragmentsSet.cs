using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FragmentsSet", menuName = "ScriptableObjects/FragmentsSet")]
public class FragmentsSet : ScriptableObject
{
    public List<GameObject> framgents;

    public GameObject GetRandomFragment()
    {
        int index = Random.Range(0, framgents.Count - 1);
        return framgents[index];
    }
}
