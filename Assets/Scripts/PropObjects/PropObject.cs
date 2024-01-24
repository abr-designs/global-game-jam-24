using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropObject : MonoBehaviour {

    [SerializeField] private PropObjectSO propObjectSO;

    public PropObjectSO GetPropObjectSO() {
        return propObjectSO;
    }

    //private void OnCollisionEnter(Collision collision) {
        
    //}
}
