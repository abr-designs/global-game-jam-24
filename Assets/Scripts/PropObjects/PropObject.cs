using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropObject : MonoBehaviour {

    private const string CONST_TAG_PLAYER = "Player";

    [SerializeField] private PropObjectSO propObjectSO;

    private bool initialCollision = false;

    public PropObjectSO GetPropObjectSO() {
        return propObjectSO;
    }

    private void OnCollisionEnter(Collision collision) {

        if (!initialCollision) {
            if(collision.gameObject.tag == CONST_TAG_PLAYER) {
                
                //Debug.Log($"Player initial collision with [{gameObject.name}]. Score points [{propObjectSO.collideScore}]");
                
                initialCollision = true;

                string pointsDescription = propObjectSO.objectName + "!";

                GameScoreManager.Instance.ScorePoints(pointsDescription, propObjectSO.collideScore);
            }
        }
    }
}
