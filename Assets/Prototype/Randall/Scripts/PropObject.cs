using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using Audio.SoundFX;
using UnityEngine;

public class PropObject : MonoBehaviour
{
    private const string PLAYER_TAG = "Player";

    public static event Action<string, int, Vector3> OnPointsScored;

    [SerializeField] private PropObjectSO propObjectSO;

    [Header("Related Prop Objects")] [SerializeField]
    private PropObject parentPropObject;

    [SerializeField] private List<PropObject> childPropObjectList;

    [Header("Destructable Prop Objects")] [SerializeField]
    private float shatterForceThreshold;

    [SerializeField] private GameObject swapOutMeshPrefab;
    [SerializeField] private Vector3 swapOutTransformOffset;

    private bool initialCollision = false;

    private Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public PropObjectSO GetPropObjectSO()
    {
        return propObjectSO;
    }

    public Rigidbody GetRigidbody()
    {
        return rigidbody;
    }

    public PropObject GetParentPropObject()
    {
        return parentPropObject;
    }

    public List<PropObject> GetChildPropObjectList()
    {
        return childPropObjectList;
    }

    public GameObject GetSwapOutMeshPrefab()
    {
        return swapOutMeshPrefab;
    }

    public float GetShatterForceThreshold()
    {
        return shatterForceThreshold;
    }

    private void OnCollisionEnter(Collision collision)
    {

        // check if a prop object is colliding with another prop object
        // also make sure they are not both kinematic
        /*if(collision.gameObject.layer == gameObject.layer)
        {
            var other = collision.gameObject.GetComponent<Rigidbody>();
            var mine = gameObject.GetComponent<Rigidbody>();


            if(other.isKinematic || mine.isKinematic)
            {
                SetPropObjectAsKinematic(this, collision.impulse);
            }
        }*/

        // check with player collision
        if (!initialCollision)
        {
            if (collision.gameObject.tag == PLAYER_TAG)
            {

                initialCollision = true;

                string pointsDescription = propObjectSO.objectName + "!";

                OnPointsScored?.Invoke(pointsDescription, propObjectSO.collideScore, transform.position);
                SFX.IMPACT.PlaySoundAtLocation(transform.position);


                //Debug.Log($"Impulse: {collision.impulse}, Magnitude: {collision.impulse.magnitude}");
                //Debug.Log($"Player initial collision with [{gameObject.name}]. Score points [{propObjectSO.collideScore}]");
                SetPropObjectAsKinematic(this, collision.impulse);
            }
        }
        else
        {

            // check for shatter only
            //if (collision.gameObject.tag == CONST_TAG_PLAYER) {
            // check for shatter force
            if (this.GetSwapOutMeshPrefab() != null)
            {
                if (collision.impulse.magnitude > this.GetShatterForceThreshold())
                {
                    SwapShatteredMesh(collision.impulse);
                }
            }
            //}
        }
    }

    public void SetPropObjectAsKinematic(PropObject kinematicPropObject, Vector3 impulse)
    {

        if (kinematicPropObject.GetParentPropObject() != null)
        {

            PropObject parentPropObject = kinematicPropObject.GetParentPropObject();
            parentPropObject.SetPropObjectAsKinematic(parentPropObject, impulse);

        }
        else
        {

            Rigidbody kinematicRigidbody = kinematicPropObject.GetRigidbody();

            if (kinematicRigidbody.isKinematic)
            {
                kinematicRigidbody.isKinematic = false;
                //rigidbody.WakeUp();
                //rigidbody.AddForce(impulse, ForceMode.Impulse);
            }

            foreach (PropObject child in kinematicPropObject.GetChildPropObjectList())
            {
                child.GetRigidbody().isKinematic = false;
                //child.GetRigidbody().WakeUp();
            }
        }

        // check for shatter force
        if (kinematicPropObject.GetSwapOutMeshPrefab() != null)
        {
            if (impulse.magnitude > kinematicPropObject.GetShatterForceThreshold())
            {
                SwapShatteredMesh(impulse);
            }
        }

    }

    private void SwapShatteredMesh(Vector3 impulse)
    {

        Transform parentTransform = transform.parent;

        GameObject newMeshGameObject = Instantiate(swapOutMeshPrefab,
            transform.position + swapOutTransformOffset, transform.rotation, parentTransform);

        // remove children in shard container
        foreach (Transform child in newMeshGameObject.transform)
        {

            child.parent = parentTransform;

            //PropObject childPropObject = child.GetComponent<PropObject>();
            //childPropObject.GetRigidbody().AddForce(impulse, ForceMode.Impulse);
            child.GetComponent<Rigidbody>().AddForce(impulse, ForceMode.Impulse);
        }

        // remove the container
        Destroy(newMeshGameObject);
        Destroy(gameObject);

        // apply impulse force to shards
    }
}
