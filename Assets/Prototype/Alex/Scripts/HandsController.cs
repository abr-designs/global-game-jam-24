using Prototype.Alex.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

public class HandsController : MonoBehaviour
{
    [SerializeField]
    private Transform leftHand;
    [SerializeField]
    private Rigidbody closestToLeftHand;
    [SerializeField]
    private Transform rightHand;
    [SerializeField]
    private Rigidbody closestToRightHand;
    [SerializeField]
    private KeyCode interactKeyCode;
    [SerializeField]
    private float force;
    private InteractableObject _holdingObject;
    [FormerlySerializedAs("playerpos")] [SerializeField]
    private Transform playerRootTransform;
    //============================================================================================================//
    private void OnEnable()
    {
        ObjectInteractionController.OnNewInteractableObject += OnNewInteractableObject;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(interactKeyCode))
            ToggleObject();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ThrowObject();
        }
    }

    private void OnDisable()
    {
        ObjectInteractionController.OnNewInteractableObject -= OnNewInteractableObject;
    }

    //============================================================================================================//
    private void ThrowObject()
    {
        if (_holdingObject)
        {
            _holdingObject.Throw(leftHand.position, force);
            _holdingObject = null;
            return;
        }

        if (_objectInRange == null)
            return;


        //_holdingObject = _objectInRange;
        _objectInRange.Push(playerRootTransform.forward, force);
    }

    private void ToggleObject()
    {
        if (_holdingObject)
        {
            _holdingObject.Drop();
            _holdingObject = null;
            return;
        }

        if (_objectInRange == null)
            return;


        _holdingObject = _objectInRange;
        //_holdingObject.Pickup(leftHand.position, closestToLeftHand);
        _holdingObject.Pickup(leftHand.position, leftHand);
    }

    //============================================================================================================//

    private InteractableObject _objectInRange;
    private void OnNewInteractableObject(InteractableObject objectInRange)
    {
        _objectInRange = objectInRange;
    }
}
