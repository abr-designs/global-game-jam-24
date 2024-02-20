using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeCircle : MonoBehaviour
{
    [SerializeField]
    private Transform outerRing;

    [SerializeField]
    private Transform timerCircle;

    private Vector3 _outerStartScale;
    private Vector3 _innerStartScale;

    [SerializeField]
    private AnimationCurve outerScaleSpeed;
    
    [SerializeField]
    private AnimationCurve innerScaleSpeed;
    private float _outerScale;
    private float _innerScale;

    private float _targetTime = 1f;
    private float _timer = 0f;

    private void Start()
    {
        _outerStartScale = outerRing.localScale;
        _innerStartScale = timerCircle.localScale;
    }

    private void Update()
    {
        if(transform.position.y <= 0.02f)
            transform.position = new Vector3(transform.position.x, 0.02f, transform.position.z);
        transform.rotation = Quaternion.LookRotation(Vector3.forward);

        _timer += Time.deltaTime;
        _outerScale = outerScaleSpeed.Evaluate(_timer);
        _innerScale = innerScaleSpeed.Evaluate(_timer/_targetTime);
        //Debug.Log($"{_innerScale}");

        if(_timer < 1f)
            outerRing.localScale = _outerStartScale * _outerScale;
        if(_timer < _targetTime)
            timerCircle.localScale = _innerStartScale * _innerScale;

    }

    public void SetTimer(float seconds)
    {
        _targetTime = seconds;
    }

    
}
