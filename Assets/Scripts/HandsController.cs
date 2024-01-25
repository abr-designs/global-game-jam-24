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

    private InteractableObject _holdingObject;

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
    }
    
    private void OnDisable()
    {
        throw new NotImplementedException();
    }
    
    //============================================================================================================//

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
        _holdingObject.Pickup(leftHand.position, closestToLeftHand);
    }
    
    //============================================================================================================//

    private InteractableObject _objectInRange;
    private void OnNewInteractableObject(InteractableObject objectInRange)
    {
        _objectInRange = objectInRange;
    }
}
