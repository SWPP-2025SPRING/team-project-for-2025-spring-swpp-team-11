using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DemoPlayerController : MonoBehaviour
{
    public float cameraRotateSpeed;
    public float movementSpeed;

    public float jumpForce;
    
    public float groundCheckDistance;
    
    public DemoCameraMovement demoCameraMovement;

    public LayerMask groundLayer;

    private Vector3 _moveDirection;
    private Vector2 _lookVector;
    
    private bool _isGrounded;
    private bool _doubleJump;

    private bool _ropeAction;
    
    private Rigidbody _rigidbody;
    private Animator _animator;
    private LineRenderer _lineRenderer;
    
    public SpringJoint spring;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
        _lineRenderer = GetComponent<LineRenderer>();
    }
    
    private void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out var hit, groundCheckDistance, groundLayer))
        {
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }
        
        _rigidbody.MovePosition(transform.position + Quaternion.Euler(0, demoCameraMovement.yAngle, 0) *  _moveDirection * (movementSpeed * Time.fixedDeltaTime));
        transform.rotation = Quaternion.Euler(0, demoCameraMovement.yAngle, 0);
        
        _animator.SetBool("isMoving", _moveDirection.magnitude > 0.01f);
        _animator.SetBool("isGrounded", _isGrounded);

        if (_ropeAction)
        {
            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, spring.transform.position);
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, Vector3.down * groundCheckDistance);
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        var tmp = context.ReadValue<Vector2>();
        _moveDirection.x = tmp.x;
        _moveDirection.z = tmp.y;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (_isGrounded && context.started)
        {
            _isGrounded = false;
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _doubleJump = true;
            
            _animator.SetTrigger("jump");
        } 
        else if (_doubleJump && _rigidbody.linearVelocity.y < 0.1f && context.started)
        {
            _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _doubleJump = false;
            
            _animator.SetTrigger("flip");
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _lookVector = context.ReadValue<Vector2>();
        demoCameraMovement.UpdateAngle(_lookVector.x, _lookVector.y);
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (spring.connectedBody != _rigidbody)
        {
            spring.connectedBody = _rigidbody;
            _lineRenderer.enabled = true;
            
            _lineRenderer.positionCount = 2;
            
            _lineRenderer.startColor = Color.black;
            _ropeAction = true;
        }
        else
        {
            spring.connectedBody = null;
            _lineRenderer.enabled = false;
        }
    }
}
