using System;
using System.Diagnostics;
using Gameplay;
using UnityEngine;
using UnityEngine.Assertions;

public class ObjectInteractionController : MonoBehaviour
{
    public static event Action<InteractableObject> OnNewInteractableObject;
    
    [SerializeField]
    private LayerMask layerMask;

    [SerializeField, Min(0f)]
    private float interactionRadius;

    [SerializeField]
    public Transform playerRootTransform;
    
    [SerializeField, Min(0)]
    private float interactionRefreshTime = 0.05f;
    private float _refreshTimer;

    [SerializeField, Header("Debug")]
    private InteractableObject _closestReachable;
    [SerializeField]
    private Collider[] _colliders;
    [SerializeField]
    private int _lastCollisionCount;

    private void OnEnable()
    {
        GameplayController.OnLevelReady += OnLevelReady;
    }



    // Start is called before the first frame update
    private void Start()
    {
        _colliders = new Collider[5];
    }

    // Update is called once per frame
    private void Update()
    {
        //FIXME This is temp
        if (playerRootTransform == null)
            return;
        
        if (_refreshTimer < interactionRefreshTime)
        {
            _refreshTimer += Time.deltaTime;
            return;
        }

        _refreshTimer = 0f;
        _lastCollisionCount = Physics.OverlapSphereNonAlloc(playerRootTransform.position, interactionRadius, _colliders, layerMask.value);

        TryFindNewReachableInRange();
    }
    
    private void OnDisable()
    {
        GameplayController.OnLevelReady -= OnLevelReady;
    }
    
    //============================================================================================================//
    
    private void OnLevelReady()
    {
        var player = FindObjectOfType<WASDRagdollController>();
        Assert.IsNotNull(player, "Cannot find Player!!");
        
        player.transform.FindObjectWithName("KinematicRoot", out var root);
        
        Assert.IsNotNull(root, $"Cannot find KinematicRoot on {player.gameObject.name}");

        playerRootTransform = root;
    }
    
    private void TryFindNewReachableInRange()
    {
        if (_lastCollisionCount == 0)
        {
            SetReachableTarget(null);
            return;
        }

        var newClosest = FindClosestReachable();

        if (_closestReachable == newClosest)
            return;

        SetReachableTarget(newClosest);
    }

    private void SetReachableTarget(InteractableObject newClosest)
    {
        _closestReachable = newClosest;
            
        OnNewInteractableObject?.Invoke(_closestReachable);
    }
    
    private InteractableObject FindClosestReachable()
    {
        var playerPosition = playerRootTransform.position;

        var shortestDistance = float.MaxValue;
        var shortestIndex = -1;
        for (int i = 0; i < _lastCollisionCount; i++)
        {
            var collider = _colliders[i];
            if (collider == null)
                continue;

            var dir = (collider.transform.position - playerPosition);

            //Find the closest one
            var sqrDistance = dir.sqrMagnitude;
            if (sqrDistance > shortestDistance)
                continue;

            shortestDistance = sqrDistance;
            shortestIndex = i;
        }

        //IF we didn't find anything, then return default, otherwise try to return the interactable
        return shortestIndex < 0 ? default : _colliders[shortestIndex].GetComponent<InteractableObject>();
    }

    //============================================================================================================//

    [Conditional("UNITY_EDITOR")]

    private void OnDrawGizmos()
    {
        if (playerRootTransform == null)
            return;
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(playerRootTransform.position, interactionRadius);
    }
}
