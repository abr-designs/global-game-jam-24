using System;
using System.Collections.Generic;
using Audio;
using Audio.SoundFX;
using UnityEngine;
using UnityEngine.Serialization;
using Levels;
using System.Linq;
using VisualFX;

public enum PropImpactType {
    DEFAULT = 1,
    PLAYER = 2,
    THROWN = 3,
    EXPLOSION = 4
}

public enum PropScoreType {
    IMPACT = 1,
    DESTROY = 2,
    EVENT = 3
}

[DisallowMultipleComponent]
public class PropObject : MonoBehaviour
{
    private const string PLAYER_TAG = "Player";

    public static event Action<string, int, Vector3> OnPointsScored;

    public event Action<Collision> OnImpulse;

    [SerializeField] 
    public PropObjectSO propObjectSO;

    [SerializeField]
    private float scoreForceThreshold = 0f;
    [SerializeField]
    private PropScoreType scoreType = PropScoreType.IMPACT;
    public PropScoreType GetScoreType => scoreType;

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
    private bool _isScored;
    // Track whether object was triggered for kinematic
    private bool _isTriggered;

    private Rigidbody _rigidbody;

    //Unity Functions
    //============================================================================================================//

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {

        Debug.Log($"{name} hit {collision.collider.name} impulse {collision.impulse.magnitude}");

        // Ignore debris collisions
        if(collision.gameObject.CompareTag("Debris") == true)
            return;

        PropImpactType impactType = PropImpactType.DEFAULT;
        // Player will always trigger a prop
        if (collision.gameObject.CompareTag(PLAYER_TAG) == true)
        {
            impactType = PropImpactType.PLAYER;
        }  

        OnImpulse?.Invoke(collision);

        ApplyImpact(impactType, collision.impulse);

    }

    //============================================================================================================//

    private void SetPropObjectAsKinematic(PropObject targetPropObject, Vector3 impulse)
    {
        if(targetPropObject._isTriggered)
            return;

        Debug.Log($"Setting prop to non-kinematic {name} rigidbody {targetPropObject._rigidbody}");

        targetPropObject._isTriggered = true;

        if (targetPropObject._rigidbody.isKinematic)
        {
            targetPropObject._rigidbody.isKinematic = false;
        }

        if (targetPropObject.parentPropObject != null)
        {
            PropObject parentPropObject = targetPropObject.parentPropObject;
            SetPropObjectAsKinematic(parentPropObject, impulse);
            // Clear parent
            targetPropObject.parentPropObject = null;
        }
        
        // Toggle all child objects and unparent
        foreach (PropObject child in targetPropObject.childPropObjects)
        {
            Debug.Log($"Set prop child {child}");
            if(child == null)
                continue;

            SetPropObjectAsKinematic(child, impulse);

            // Unparent the transform
            child.transform.parent = transform.parent;
        }

        // Clear all children
        targetPropObject.childPropObjects.Clear();    

    }

    public void ApplyImpact(PropImpactType impactType, Vector3 impulse) 
    {
        SetPropObjectAsKinematic(this, impulse);
        SwapShatteredMesh(impulse);
        if(scoreType == PropScoreType.IMPACT)
            TriggerScore();
    }

    private void SwapShatteredMesh(Vector3 impulse)
    {
        // Prevent re-entry when multiple collisions happen at once
        if(_isShattered || swapOutMeshPrefab == null)
            return;

        // check for shatter force
        if(impulse.magnitude < shatterForceThreshold)
            return;

        Debug.Log($"Prop {name} was shattered");
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

        if(scoreType == PropScoreType.DESTROY)
        {
            TriggerScore();
        }

        // remove the container
        Destroy(newMeshGameObject);
        Destroy(gameObject);
   
    }

    public void TriggerScore() {

        if(_isScored)
            return;
        
        _isScored = true;

        string pointsDescription = propObjectSO.objectName + "!";

        var score = 0;
        if (LevelLoader.CurrentLevelController.avoidObjects.Any(x => x == propObjectSO))
            score = propObjectSO.kingPenalty;
        else
            score = propObjectSO.collideScore;            

        OnPointsScored?.Invoke(pointsDescription, score, transform.position);
        SFX.IMPACT.PlaySoundAtLocation(transform.position);

    }

    //============================================================================================================//
}
