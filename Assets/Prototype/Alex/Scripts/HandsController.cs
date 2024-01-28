using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField]
    private Transform playerpos;
    //============================================================================================================//
    private void OnEnable()
    {
        ObjectInteractionController.OnNewInteractableObject += OnNewInteractableObject;
    }



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(interactKeyCode))
            ToggleObject();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ThrowObject();
        }
        //Input.GetKeyDown(k)
        // if (Input.GetKeyDown(k))
        // {
        //     return;
        // }
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
        _objectInRange.Swat(playerpos.forward, force);
    }

    private void ToggleObject()
    {
        Debug.Log("Toggled");
        if (_holdingObject)
        {
            _holdingObject.Drop();
            _holdingObject = null;
            return;
        }

        if (_objectInRange == null)
            return;


        _holdingObject = _objectInRange;
        _holdingObject.Pickup(leftHand.position, closestToLeftHand);
    }

    //============================================================================================================//

    private InteractableObject _objectInRange;
    private void OnNewInteractableObject(InteractableObject objectInRange)
    {
        _objectInRange = objectInRange;
    }
}
