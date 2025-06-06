using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class TestPlayerBehavior : MonoBehaviour
{
    /********************************************************************************/
    [Header("Player Properties")]
    # region player properties
    
    [Header("Movement")]
    [SerializeField] public float acceleration;
    [SerializeField] public float deaccel;
    [SerializeField] public float dynamicDeaccel;
    
    [Header("Jump & Air")]
    [SerializeField] public float jumpForce;
    [SerializeField] public float doubleJumpForce;
    [SerializeField] public float airDeaccel;
    [SerializeField] public int jumpCount;

    
    [Header("Speed Limits")]
    [SerializeField] public float maxSpeed;
    [SerializeField] public float airMaxSpeed;

    
    [Header("Ground Check")]
    [SerializeField] public Transform feetTransform;
    [SerializeField] public float groundCheckDistance;
    
    [Header("Wire-related members")]
    [SerializeField] public float wirePointDetectRadius;
    [SerializeField] public float minDistanceConst, maxDistanceConst, wireSwingForce;
    [SerializeField] public float wireAccel;
    # endregion
    
    /********************************************************************************/
    [Header("external Properties")]
    # region external properties

    [SerializeField] public GameObject cameraObject;
    [SerializeField] public LayerMask groundLayer;
    [SerializeField] public LayerMask wirePointLayer;
    
    # endregion 

    /********************************************************************************/
    [Header("Player Object Components")]
    # region Component_Refs
    
    public Rigidbody _rigidbody;
    public PlayerInputProcessor _inputProcessor;
    public LineRenderer _lineRenderer;
    public Animator _animator;

    # endregion
    
    /********************************************************************************/
    public Vector3 _velocity;
    public bool _isGrounded;
    public int _jumpCount = 0;

    public bool _isWiring = false;
    public Collider[] _avilableWirePoints = new Collider[10];
    public Transform _currentWirePoint;

    public GroundFriction _currentGroundOn;
    
    public bool _isStun;
    public float _stunTimeElapsed;

    public Vector2 _inputOnRelease;
    public bool _isJustReleased;
    public InGameUI _inGameUI;

    public Vector2 testInput; 
    
    public Transform respawnPos;

    public float distanceToPoint;

    public TMP_Text text;

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, wirePointDetectRadius);
    }

    public void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _inputProcessor = GetComponent<PlayerInputProcessor>();
        _lineRenderer = GetComponent<LineRenderer>();
        _animator = GetComponentInChildren<Animator>();
        _inGameUI = FindFirstObjectByType<InGameUI>();

        _inputProcessor.jumpEvent.AddListener(Jump);
        _inputProcessor.shotEvent.AddListener(TryConnectWire);
        _inputProcessor.releaseEvent.AddListener(StopWiring);

        _lineRenderer.enabled = false;
    }
    
    public void Update()
    {
        Vector3 xzVelocity = _rigidbody.linearVelocity;
        xzVelocity.y = 0;
        GroundCheck();
        StunCheck();
        
        if (_currentWirePoint != null)
            Debug.Log(Vector3.Distance(_currentWirePoint.position, transform.position));
        
        if (!_isWiring)
        {
            DetectInputChange();
            ScanWirePoints();
            Move();
        }
        else
        {
            RenderWire();
            MoveOnWire();
            OnWiringRotate();
            
            if (_isGrounded) StopWiring();
        }

        AnimationUpdate();
    }

    public void FixedUpdate()
    {
        // 와이어 액션 동안 와이어의 길이를 유지해줌
        if (_isWiring)
        {
            var vecToPoint = _currentWirePoint.position - transform.position;
            _rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, vecToPoint);
            
            var dist = Vector3.Distance(_currentWirePoint.position, transform.position);
            
            _rigidbody.MovePosition(transform.position + vecToPoint.normalized * (dist-distanceToPoint));
        }
    }

    public void GroundCheck()
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

    public void DetectInputChange()
    {
        var input = testInput.normalized;

        if (input != _inputOnRelease)
            _isJustReleased = false;
    }

    public void Move()
    {
        var input = testInput.normalized;
        
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

    public void MoveOnWire()
    {
        var input = testInput.normalized;
        
        if (IsStunNow()) 
            input = Vector2.zero;
    
        Vector3 direction = new Vector3(input.x, 0, input.y);
        direction = Quaternion.AngleAxis(cameraObject.transform.rotation.eulerAngles.y, Vector3.up) * direction;
        
        var vecToPoint = _currentWirePoint.position - transform.position;
        var right = Vector3.Cross(Vector3.up, direction);
        var finalDir = Vector3.Cross(vecToPoint, right);
        if (Vector3.Dot(direction, finalDir) < 0)
            finalDir = -finalDir;
        
        _rigidbody.linearVelocity += finalDir.normalized * (wireAccel * Time.deltaTime);
    }

    public void StunCheck()
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
    
    public void Jump()
    {
        if (IsStunNow() || _inGameUI.GetPaused()) return;
        
        if (_isGrounded || _jumpCount < jumpCount)
        {
            if (_jumpCount != 0) 
                _animator.SetTrigger("DoubleJump");
            
            _jumpCount++;
            
            _rigidbody.linearVelocity -= _rigidbody.linearVelocity.y * Vector3.up;
            _rigidbody.AddForce(Vector3.up * (_jumpCount == 0 ? jumpForce : doubleJumpForce), ForceMode.Impulse);
        }
    }

    public void ToggleWireMode()
    {
        if (_isWiring)
        {
            StopWiring();
        }
        else
        {
            TryConnectWire();
        }
    }
    public void TryConnectWire()
    {
        if (_isWiring || _isGrounded || _inGameUI.GetPaused())
            return;
        
        var point = GetAvailableWirePoint();

        if (IsStunNow()) return;
        if (point == null) return;

        GameManager.Instance.AudioManager.PlayOneShot(SFX.WIRE_RELEASE);

        // 와이어 액션 상태로 바꾼다.
        _currentWirePoint = point.transform;
        _lineRenderer.enabled = true;
        _isWiring = true;

        // 와이어 포인트에 플레이어를 연결 시켜준다.
        var sprjt = _currentWirePoint.GetComponent<SpringJoint>();
        
        var minDistance = Vector3.Distance(point.transform.position, transform.position);
        
        sprjt.connectedBody = _rigidbody;
        sprjt.minDistance = minDistance / minDistanceConst;
        sprjt.maxDistance = minDistance / maxDistanceConst;

        distanceToPoint = minDistance;
    }
    
    public void StopWiring()
    {
        if (!_isWiring || _inGameUI.GetPaused())
            return;
        _currentWirePoint.GetComponent<SpringJoint>().connectedBody = null;
        
        transform.rotation = Quaternion.identity;

        _currentWirePoint = null;
        _lineRenderer.enabled = false;
        _isWiring = false;

        _isJustReleased = true;
        _inputOnRelease = testInput.normalized;
    }

    public void OnWiringRotate()
    {
        var vecToPoint = _currentWirePoint.position - transform.position;

        transform.rotation = Quaternion.LookRotation(vecToPoint, Vector3.up);
    }

    public void ScanWirePoints()
    {
        foreach (var i in _avilableWirePoints)
        {
            if (i != null)
                i.gameObject.GetComponent<MeshRenderer>().materials[0].color = Color.white;
        }
        
        int hit = Physics.OverlapSphereNonAlloc(transform.position, wirePointDetectRadius, _avilableWirePoints,
            LayerMask.GetMask("WirePoint"));

        if (hit == 0)
        {
            _avilableWirePoints = new Collider[10];
        }


        var point = GetAvailableWirePoint();
        if (point != null) point.gameObject.GetComponent<MeshRenderer>().materials[0].color = Color.yellow;
    }
    
    // 현재 _availableWirePoints 배열에서 와이어 연결 가능한 포인트가 있는지 체크
    // 연결 가능한 포인트가 없다면 null 반환
    public Collider GetAvailableWirePoint()
    {
        if (_avilableWirePoints[0] == null) return null;
        
        var point = _avilableWirePoints[0];
        float minDistance = Vector3.Distance(_avilableWirePoints[0].transform.position, transform.position);
        float tmpDistance = 0;
        
        // 일단 화면 z 좌표가 앞에 있는 것 중 가장 가까운 것을 골라보자
        // 모두 z 좌표가 화면 뒤에 있다면 랜덤한 포인트
        // null 은 일단 안 나옴
        for (int i = 0; i < _avilableWirePoints.Length; i++)
        {
            if (_avilableWirePoints[i] == null) continue;

            if (Camera.main.WorldToScreenPoint(point.transform.position).z < 0)
            {
                minDistance = Vector3.Distance(_avilableWirePoints[i].transform.position, transform.position);
                point = _avilableWirePoints[i];
                continue;
            }
            if ((tmpDistance = Vector3.Distance(_avilableWirePoints[i].transform.position, transform.position)) < minDistance)
            {
                minDistance = tmpDistance;
                point = _avilableWirePoints[i];
            }
        }

        var vecToPoint = point.transform.position - transform.position;
        var dotValue = Vector3.Dot(vecToPoint, transform.forward);

        // 내적 값이 음수, 다시 말해 진행 방향 뒤에 있다면 null
        if (dotValue < 0) return null;
        
        // 화면 밖에 있다면... return null
        var viewportPonint = Camera.main.WorldToScreenPoint(point.transform.position);
        if (viewportPonint.z < 0 ||
            viewportPonint.x < 0 ||
            viewportPonint.y < 0 ||
            viewportPonint.x > Screen.width ||
            viewportPonint.y > Screen.height
            )
            return null;
        
        return point;
    }

    public void GetHit(Vector3 knockback, float stunTime)
    {
        _isStun = true;
        _stunTimeElapsed = stunTime;

        if (_isWiring)
        {
            ToggleWireMode();
        }

        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.AddForce(knockback, ForceMode.Impulse);
        
        _animator.SetTrigger("HitTrig");

        GameManager.Instance.AudioManager.PlayOneShot(SFX.HIT);
    }

    public void RenderWire()
    {
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, _currentWirePoint.position);
    }

    public void AnimationUpdate()
    {
        if (!_isWiring && !IsStunNow())
            transform.rotation = Quaternion.AngleAxis(cameraObject.transform.rotation.eulerAngles.y, Vector3.up);
        
        _animator.SetFloat("Speed_f", _rigidbody.linearVelocity.magnitude);
        _animator.SetFloat("Speed_y", _rigidbody.linearVelocity.y);
        _animator.SetBool("IsGrounded", _isGrounded);
        _animator.SetBool("Wire_b", _isWiring);
    }
    

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Respawn"))
        {
            _rigidbody.linearVelocity = Vector3.zero;
            transform.position = respawnPos.position;
        }
    }
}
