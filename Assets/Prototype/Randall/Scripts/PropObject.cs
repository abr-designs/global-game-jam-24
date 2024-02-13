using System;
using System.Collections.Generic;
using Audio;
using Audio.SoundFX;
using UnityEngine;
using UnityEngine.Serialization;
using Levels;
using System.Linq;
using VisualFX;

[DisallowMultipleComponent]
public class PropObject : MonoBehaviour
{
    private const string PLAYER_TAG = "Player";

    public static event Action<string, int, Vector3> OnPointsScored;

    public event Action<Collision> OnImpulse;

    [SerializeField] 
    public PropObjectSO propObjectSO;

    [SerializeField, Header("Related Prop Objects")]
    private PropObject parentPropObject;

    [FormerlySerializedAs("childPropObjectList")] [SerializeField] 
    private List<PropObject> childPropObjects;

    [SerializeField, Header("Destructable Prop Objects")]
    private float shatterForceThreshold;

    [SerializeField]
    private VFX[] shatterVFX;
    [SerializeField]
    private SFX shatterSFX = SFX.NONE;

    [SerializeField] 
    private GameObject swapOutMeshPrefab;
    [SerializeField] 
    private Vector3 swapOutTransformOffset;

    private bool _isShattered;
    // Track whether the object has been scored
    private bool _isTriggered;

    private Rigidbody _rigidbody;

    //Unity Functions
    //============================================================================================================//

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {

        Debug.Log($"{name} hit {collision.collider.name} impulse {collision.impulse.magnitude}");

        // Player will always trigger a prop
        if (collision.gameObject.CompareTag(PLAYER_TAG) == true)
        {
            TriggerProp(collision.impulse);
        }  

        var impulse = collision.impulse.magnitude;
        
        OnImpulse?.Invoke(collision);

        // check for shatter only
        //if (collision.gameObject.tag == CONST_TAG_PLAYER) {
        // check for shatter force
        if (swapOutMeshPrefab == null) 
            return;
        
        if (impulse > shatterForceThreshold)
        {
            TriggerProp(collision.impulse);
            SwapShatteredMesh(collision.impulse);
        }

    }

    //============================================================================================================//

    private void SetPropObjectAsKinematic(PropObject targetPropObject, Vector3 impulse)
    {
        if (targetPropObject.parentPropObject != null)
        {
            PropObject parentPropObject = targetPropObject.parentPropObject;
            parentPropObject.SetPropObjectAsKinematic(parentPropObject, impulse);
        }
        else
        {

            if (targetPropObject._rigidbody.isKinematic)
            {
                targetPropObject._rigidbody.isKinematic = false;
            }

            foreach (PropObject child in targetPropObject.childPropObjects)
            {
                if(child == null)
                    continue;
                child._rigidbody.isKinematic = false;

                // Unparent the transform
                child.transform.parent = transform.parent;
            }

        }

        // check for shatter force
        if (targetPropObject.swapOutMeshPrefab != null)
        {
            if (impulse.magnitude > targetPropObject.shatterForceThreshold)
            {
                SwapShatteredMesh(impulse);
            }
        }

    }

    public void TriggerProp(Vector3 impulse)
    {
        Debug.Log($"Trigger prop {gameObject.name} ");

        if(_isTriggered) 
            return;

        _isTriggered = true;

        string pointsDescription = propObjectSO.objectName + "!";

        var score = 0;
        if (LevelLoader.CurrentLevelController.avoidObjects.Any(x => x == propObjectSO))
            score = propObjectSO.kingPenalty;
        else
            score = propObjectSO.collideScore;            

        OnPointsScored?.Invoke(pointsDescription, score, transform.position);
        SFX.IMPACT.PlaySoundAtLocation(transform.position);

        SetPropObjectAsKinematic(this,impulse);

    }

    private void SwapShatteredMesh(Vector3 impulse)
    {
        Debug.Log($"Prop {name} was shattered");
        // Prevent re-entry when multiple collisions happen at once
        if(_isShattered)
            return;
        _isShattered = true;

        Transform parentTransform = transform.parent;

        GameObject newMeshGameObject = Instantiate(swapOutMeshPrefab,
            transform.position + swapOutTransformOffset, transform.rotation, parentTransform);

        // remove children in shard container
        foreach (Transform child in newMeshGameObject.transform)
        {
            child.parent = parentTransform;

            // apply impulse force to shards
            child.GetComponent<Rigidbody>().AddForce(impulse, ForceMode.Impulse);
        }

        // Play any VFX/SFX if they are set
        if(shatterVFX.Length > 0)
        {
            foreach(VFX v in shatterVFX)
            {
                v.PlayAtLocation(transform.position);
            }
        }
        shatterSFX.PlaySoundAtLocation(transform.position);

        // remove the container
        Destroy(newMeshGameObject);
        Destroy(gameObject);
   
    }

    //============================================================================================================//
}
