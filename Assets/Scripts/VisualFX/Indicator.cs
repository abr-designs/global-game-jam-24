using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Indicator : MonoBehaviour
{
    [SerializeField]
    private Transform outerCircle;
    [SerializeField]
    private Transform innerCircle;

    [SerializeField]
    private float pulseSpeed = 1f; // time scale

    [SerializeField]
    private float rotateSpeed = 180f; // speed in degrees per second

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        outerCircle.localScale = Vector3.one + (Vector3.one * ((Mathf.Sin(Time.time * pulseSpeed) + 1)*0.5f));
        outerCircle.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
        //transform.up = -Camera.main.transform.forward;
        transform.rotation = Quaternion.identity;
    }
}
