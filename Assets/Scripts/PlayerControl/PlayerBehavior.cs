using System;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerBehavior : MonoBehaviour
{
    /********************************************************************************/
    [Header("Player Properties")]
    # region player properties
    
    [Header("Movement")]
    [SerializeField] private float acceleration;
    [SerializeField] private float deaccel;
    [SerializeField] private float dynamicDeaccel;
    
    [Header("Jump & Air")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    [SerializeField] private float airDeaccel;
    [SerializeField] private int jumpCount;

    
    [Header("Speed Limits")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float airMaxSpeed;

    
    [Header("Ground Check")]
    [SerializeField] private Transform feetTransform;
    [SerializeField] private float groundCheckDistance;
    
    [Header("Wire-related members")]
    [SerializeField] private float wirePointDetectRadius;
    [SerializeField] private float minDistanceConst, maxDistanceConst, wireSwingForce;
    [SerializeField] private float wireAccel;
    # endregion
    
    /********************************************************************************/
    [Header("external Properties")]
    # region external properties

    [SerializeField] public GameObject cameraObject;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wirePointLayer;
    
    # endregion 

    /********************************************************************************/
    [Header("Player Object Components")]
    # region Component_Refs
    
    private Rigidbody _rigidbody;
    private PlayerInputProcessor _inputProcessor;
    private LineRenderer _lineRenderer;
    private Animator _animator;
    private IngameUIWirePointMark _wirePointMark;
    private WireConnector _wireConnector;

    # endregion
    
    /********************************************************************************/
    private Vector3 _velocity;
    private bool _isGrounded;
    private int _jumpCount = 0;

    public bool _isWiring => _wireConnector._isWiring;
    private Collider[] _avilableWirePoints = new Collider[10];
    private Transform _currentWirePoint;

    private GroundFriction _currentGroundOn;
    
    private bool _isStun;
    private float _stunTimeElapsed;

    public Vector2 _inputOnRelease;
    public bool _isJustReleased;
    private InGameUI _inGameUI;

    private WIREMODE _wiremode;
    
    public Transform respawnPos;

    public float distanceToPoint;

    private bool _respawn;

    public TMP_Text text;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, wirePointDetectRadius);
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _inputProcessor = GetComponent<PlayerInputProcessor>();
        _lineRenderer = GetComponent<LineRenderer>();
        _animator = GetComponentInChildren<Animator>();
        _inGameUI = FindFirstObjectByType<InGameUI>();
        _wirePointMark = GetComponent<IngameUIWirePointMark>();
        _wireConnector = GetComponent<WireConnector>();

        _inputProcessor.jumpEvent.AddListener(Jump);
        /*_inputProcessor.shotEvent.AddListener(OnClick);
        _inputProcessor.releaseEvent.AddListener(OnRelease);*/
        _inputProcessor.respawnEvent.AddListener(RespawnButton);

        _lineRenderer.enabled = false;
        float sensitivity = GameManager.Instance.DataManager.sensitivity;
        var orbital = cameraObject.GetComponent<CinemachineInputAxisController>();
        foreach (var c in orbital.Controllers)
        {
            if(c.Name == "Look Orbit X")
            {
                c.Input.Gain = sensitivity;
            }else if(c.Name == "Look Orbit Y")
            {
                c.Input.Gain = -sensitivity;
            }
        }

        _wiremode = GameManager.Instance.DataManager.wiremode;
        Debug.Log("wire mode:" + _wiremode);
    }
    
    public bool IsWiring => _isWiring;
    public bool IsGrounded => _isGrounded;

    private void Update()
    {
        Vector3 xzVelocity = _rigidbody.linearVelocity;
        xzVelocity.y = 0;
        
        GroundCheck();
        StunCheck();
        
        
        if (!_isWiring)
        {
            DetectInputChange();
            Move();
        }
        AnimationUpdate();
    }

    private void FixedUpdate()
    {
        if (_respawn)
        {
            Respawn();
            _respawn = false;
        }
    }

    private void GroundCheck()
    {
        if (Physics.Raycast(feetTransform.position, -Vector3.up, out RaycastHit hit, groundCheckDistance, groundLayer))
        {
            if (!_isGrounded)
                _jumpCount = 0;
            
            _isGrounded = true;
            
            _currentGroundOn = hit.collider.GetComponent<GroundFriction>();
        }
        else
        {
            _isGrounded = false;

            _currentGroundOn = null;
        }
    }

    private void DetectInputChange()
    {
        var input = _inputProcessor.MoveInput.normalized;

        if (input != _inputOnRelease)
            _isJustReleased = false;
    }

    private void Move()
    {
        var input = _inputProcessor.MoveInput.normalized;
        
        if (IsStunNow()) 
            input = Vector2.zero;
        
        // 인풋의 카메라 시점 방향 결정
        Vector3 direction = new Vector3(input.x, 0, input.y);
        direction = Quaternion.AngleAxis(cameraObject.transform.rotation.eulerAngles.y, Vector3.up) * direction;
    
        // 인풋을 이용하여 가속
        // 방금 릴리즈 했다면 가속 X -> 인풋이 바뀌기 전까지는 조작 X
        if (!_isJustReleased)
            _rigidbody.linearVelocity += direction * (acceleration * Time.deltaTime);
        
        Vector3 velWithoutY = _rigidbody.linearVelocity;
        velWithoutY.y = 0;

        // 현재 밟고 있는 지형의 friction 구하기
        float groundFriction = _currentGroundOn != null ? _currentGroundOn.friction : 0.1f;
        
        // 공중에서의 감속 계수 결정
        // 땅에 있을 때는 1로 하여 곱했을 때 아무 효과 없도록 함
        // 공중에 있을 때는, 기존 maxSpeed 보다 느린 경우는 땅과 동일하게 감속이 되도록 함
        //                 maxSpeed 보다 빠르고 airMaxSpeed 보다 느린 경우 감속을 더 적게 함
        //                 airMaxSpeed 보다 빠른 경우 1로 결정하여 땅과 동일하게 감속이 되도록 함 
        float airCof = _isGrounded ? 1 :
             (_rigidbody.linearVelocity.magnitude > maxSpeed
                ? (_rigidbody.linearVelocity.magnitude > airMaxSpeed ? 1 : airDeaccel)
                : 1f);
        airCof = _isJustReleased ? 0 : airCof;

        if (IsStunNow())
            airCof = airDeaccel / 2f;

        if (input.magnitude < 0.1f)
        {
            if (_isGrounded) 
                _rigidbody.AddForce(-velWithoutY * (airCof * groundFriction * deaccel * Time.deltaTime));
        }
        else
        {
            _rigidbody.AddForce(-velWithoutY * (airCof * dynamicDeaccel * Time.deltaTime));
        }
    }
    

    private void StunCheck()
    {
        if (!_isStun)
        {
            GetComponent<MeshRenderer>().materials[0].color = Color.white;
            return;
        }

        GetComponent<MeshRenderer>().materials[0].color = Color.red;
        _stunTimeElapsed -= Time.deltaTime;
        if (_stunTimeElapsed <= 0 && _isGrounded)
        {
            _isStun = false;
            _animator.SetTrigger("RecoverTrig");
        }
    }

    public bool IsStunNow()
    {
        if (_isStun) return true;
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Recover") ||
            _animator.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
        {
            return true;
        }

        return false;
    }
    
    private void Jump()
    {
        if (IsStunNow() || _inGameUI.GetPaused()) return;
        if (_isWiring) return;
        
        if (_isGrounded || _jumpCount < jumpCount)
        {
            if (_jumpCount != 0) 
                _animator.SetTrigger("DoubleJump");
            
            _jumpCount++;
            
            GameManager.Instance.AudioManager.PlayOneShot(SFX.JUMP);
            
            _rigidbody.linearVelocity -= _rigidbody.linearVelocity.y * Vector3.up;
            _rigidbody.AddForce(Vector3.up * (_jumpCount == 0 ? jumpForce : doubleJumpForce), ForceMode.Impulse);
        }
    }

    public void GetHit(Vector3 knockback, float stunTime)
    {
        if (_isStun) return; 
        
        _isStun = true;
        _stunTimeElapsed = stunTime;

        if (_isWiring)
        {
            _wireConnector.ToggleWireMode();
        }

        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.AddForce(knockback, ForceMode.Impulse);
        
        _animator.SetTrigger("HitTrig");

        GameManager.Instance.AudioManager.PlayOneShot(SFX.HIT);
    }

    private void RenderWire()
    {
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, _currentWirePoint.position);
    }

    private void AnimationUpdate()
    {
        if (!_isWiring && !IsStunNow())
            transform.rotation = Quaternion.AngleAxis(cameraObject.transform.rotation.eulerAngles.y, Vector3.up);
        
        _animator.SetFloat("Speed_f", _rigidbody.linearVelocity.magnitude);
        _animator.SetFloat("Speed_y", _rigidbody.linearVelocity.y);
        _animator.SetBool("IsGrounded", _isGrounded);
        _animator.SetBool("Wire_b", _isWiring);
    }
    
    private void Respawn()
    {
        if (_isWiring) _wireConnector.StopWiring();
        _rigidbody.linearVelocity = Vector3.zero;
        transform.position = respawnPos.position;
        Debug.Log(respawnPos.position);
    }

    private void RespawnButton()
    {
        _respawn = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Respawn"))
        {
            Respawn();
        }
    }
}
