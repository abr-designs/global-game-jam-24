using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PropObjectSO : ScriptableObject {

    public Transform prefab;
    public string objectName;
    public int collideScore;
    public int kingPenalty;

}
