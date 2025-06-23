using System;
using UnityEngine;

public class WireConnector : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private PlayerInputProcessor _inputProcessor;
    private LineRenderer _lineRenderer;
    private InGameUI _inGameUI;
    private IngameUIWirePointMark _wirePointMark;
    private PlayerBehavior _playerBehavior;
    
    private WIREMODE _wiremode;
    
    
    [Header("Wire-related members")]
    [SerializeField] private float wirePointDetectRadius;
    [SerializeField] private float minDistanceConst, maxDistanceConst, wireSwingForce;
    [SerializeField] private float wireAccel;

    public bool _isWiring;
    private bool _isGrounded => _playerBehavior.IsGrounded;
    
    private Collider[] _avilableWirePoints = new Collider[10];
    private Transform _currentWirePoint;
    
    public float distanceToPoint;
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _inputProcessor = GetComponent<PlayerInputProcessor>();
        _lineRenderer = GetComponent<LineRenderer>();
        _inGameUI = FindFirstObjectByType<InGameUI>();
        _wirePointMark = GetComponent<IngameUIWirePointMark>();
        _playerBehavior = GetComponent<PlayerBehavior>();
        
        _inputProcessor.shotEvent.AddListener(OnClick);
        _inputProcessor.releaseEvent.AddListener(OnRelease);
    }

    private void Update()
    {
        if (_currentWirePoint != null)
            RenderWire();
        
        
        if (!_isWiring)
        {
            ScanWirePoints();
        }
        else
        {
            RenderWire();
            MoveOnWire();
            OnWiringRotate();
            _wirePointMark.MakeMarkOff();
        }
    }

    private void FixedUpdate()
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
    
    private void MoveOnWire()
    {
        Debug.Log("HERER");
        var input = _inputProcessor.MoveInput.normalized;
        
        if (_playerBehavior.IsStunNow()) 
            input = Vector2.zero;
    
        Vector3 direction = new Vector3(input.x, 0, input.y);
        direction = Quaternion.AngleAxis(_playerBehavior.cameraObject.transform.rotation.eulerAngles.y, Vector3.up) * direction;
        
        var vecToPoint = _currentWirePoint.position - transform.position;
        var right = Vector3.Cross(Vector3.up, direction);
        var finalDir = Vector3.Cross(vecToPoint, right);
        if (Vector3.Dot(direction, finalDir) < 0)
            finalDir = -finalDir;
        
        _rigidbody.linearVelocity += finalDir.normalized * (wireAccel * Time.deltaTime);
    }

    private void RenderWire()
    {
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, _currentWirePoint.position);
    }

    private void OnClick()
    {
        if (_wiremode == WIREMODE.HOLD) TryConnectWire();
        else if (_wiremode == WIREMODE.TOGGLE) ToggleWireMode();
    }

    private void OnRelease()
    {
        if (_wiremode == WIREMODE.HOLD) StopWiring();
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

        if (_playerBehavior.IsStunNow()) return;
        if (point == null) return;

        GameManager.Instance.AudioManager.PlayOneShot(SFX.WIRE_RELEASE);

        // 와이어 액션 상태로 바꾼다.
        _currentWirePoint = point.transform;
        _isWiring = true;
        MakeActualLineConnection();

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

        _playerBehavior._isJustReleased = false;
        _playerBehavior._inputOnRelease = _inputProcessor.MoveInput.normalized;
    }

    private void OnWiringRotate()
    {
        var vecToPoint = _currentWirePoint.position - transform.position;

        transform.rotation = Quaternion.LookRotation(vecToPoint, Vector3.up);
    }

    private void ScanWirePoints()
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
        
        var pointAsArg = point == null ? null : point.transform;
        MakeVirtualLineConnection(pointAsArg);
        _currentWirePoint = pointAsArg;
    }
    
    // 현재 _availableWirePoints 배열에서 와이어 연결 가능한 포인트가 있는지 체크
    // 연결 가능한 포인트가 없다면 null 반환
    private Collider GetAvailableWirePoint()
    {
        if (_avilableWirePoints[0] == null) return null;

        Collider point = null;
        float minDistance = Mathf.Infinity;
        float tmpDistance = 0;
        
        // 일단 화면 z 좌표가 앞에 있는 것 중 가장 가까운 것을 골라보자
        // 모두 z 좌표가 화면 뒤에 있다면 랜덤한 포인트
        // null 은 일단 안 나옴
        for (int i = 0; i < _avilableWirePoints.Length; i++)
        {
            if (_avilableWirePoints[i] == null) continue;

            var viewportPoint = Camera.main.WorldToScreenPoint(_avilableWirePoints[i].transform.position);
            if (viewportPoint.z < 0 ||
                viewportPoint.x < 0 ||
                viewportPoint.y < 0 ||
                viewportPoint.x > Screen.width ||
                viewportPoint.y > Screen.height
               ) continue;
            
            if ((tmpDistance = Vector3.Distance(_avilableWirePoints[i].transform.position, transform.position)) < minDistance)
            {
                minDistance = tmpDistance;
                point = _avilableWirePoints[i];
            }
        }

        if (point == null) return null;
        
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
    
    private void MakeActualLineConnection()
    {
        SetLineRendererColor(Color.black);
        _lineRenderer.enabled = true;
    }

    private void MakeVirtualLineConnection(Transform point)
    {
        if (point == null)
        {
            _lineRenderer.enabled = false;
            _wirePointMark.MakeMarkOff();
            return;
        }

        SetLineRendererColor(Color.grey);
        _lineRenderer.enabled = true;
        
        _wirePointMark.MakeMarkOn();
        _wirePointMark.SetWireMarkImage(point.position);
    }

    private void SetLineRendererColor(Color color)
    {
        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;
    }
}
