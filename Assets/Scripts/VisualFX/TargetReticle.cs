using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ReticleType {
    None = 0,
    Sphere = 1,
    Crosshair = 2
}

public class TargetReticle : MonoBehaviour
{
    [SerializeField]
    private LayerMask targetMask;

    [SerializeField]
    private Transform outerCircle;
    [SerializeField]
    private Transform innerCircle;

    [SerializeField]
    private float pulseSpeed = 1f;

    [SerializeField]
    private Transform sphere;

    [SerializeField]
    private ReticleType reticleType = ReticleType.Sphere;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        var screenPointToRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(screenPointToRay, out RaycastHit raycastHit, 100f, targetMask))
        {
            transform.position = raycastHit.point;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, raycastHit.normal);
        }
        */
        
        if(reticleType == ReticleType.Crosshair)
        {
            outerCircle.localScale = Vector3.one + (Vector3.one * ((Mathf.Sin(Time.time * pulseSpeed) + 1)*0.5f));
        }
    }

    private void OnValidate() 
    {
        if(reticleType == ReticleType.None)
        {
            sphere.gameObject.SetActive(false);
            innerCircle.gameObject.SetActive(false);
            outerCircle.gameObject.SetActive(false);
        }else if(reticleType == ReticleType.Sphere)
        {
            sphere.gameObject.SetActive(true);
            innerCircle.gameObject.SetActive(false);
            outerCircle.gameObject.SetActive(false);
        } else if(reticleType == ReticleType.Crosshair)
        {
            sphere.gameObject.SetActive(false);
            innerCircle.gameObject.SetActive(true);
            outerCircle.gameObject.SetActive(true);
        }
    }
}
