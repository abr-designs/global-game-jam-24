using GameInput;
using UnityEngine;

public class MouseRagdollController : MonoBehaviour
{
    private static readonly int SpeedAnimator = Animator.StringToHash("Speed");
    
    [SerializeField]
    private Animator puppeteerAnimator;

    [SerializeField]
    private Rigidbody root;

    private Vector2 _inputDirections;
    private bool _hasInput;

    private bool _ragdollActive;

    [SerializeField]
    private LayerMask groundMask;

    private float _groundHeightOffset;
    private float _heightOffGround;

    [SerializeField]
    private float mult = 10f;

    private bool _leftMousePressed;

    //============================================================================================================//

    private void OnEnable()
    {
        PuppetRagdoll.OnRagdollActive += OnRagdollActive;
        GameInputDelegator.OnLeftClick += OnLeftClick;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_ragdollActive)
            return;


        if (_leftMousePressed == false)
        {
            puppeteerAnimator.SetFloat(SpeedAnimator, 0f);

            return;
        }
        
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out var raycastHit, 100, groundMask.value) == false)
            return;

        var dif = root.transform.position - raycastHit.point;

        var speed = dif.magnitude * mult;

        root.transform.forward = Vector3.ProjectOnPlane(-dif.normalized, Vector3.up);
        root.transform.position = Vector3.MoveTowards(root.transform.position,
            raycastHit.point + (Vector3.up * _groundHeightOffset), speed * Time.deltaTime);
        
        
        puppeteerAnimator.SetFloat(SpeedAnimator, speed);
    }
    
    private void OnDisable()
    {
        PuppetRagdoll.OnRagdollActive -= OnRagdollActive;
        GameInputDelegator.OnLeftClick -= OnLeftClick;
    }

    //============================================================================================================//
    
    private void OnRagdollActive(bool ragdollActive)
    {
        _ragdollActive = ragdollActive;
    }

    //============================================================================================================//

    private void OnLeftClick(bool pressed)
    {
        _leftMousePressed = pressed;
    }
    
    //============================================================================================================//
}