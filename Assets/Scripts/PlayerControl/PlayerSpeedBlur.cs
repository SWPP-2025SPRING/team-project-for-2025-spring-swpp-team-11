using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerSpeedBlur : MonoBehaviour
{
    [Header("Speed → Blur 매핑")]
    [Tooltip("속도가 이 값 이상일 때부터 Blur가 시작됩니다.")]
    public float minSpeedThreshold = 2f;

    [Tooltip("속도가 이 값이면 BlurIntensity가 1이 됩니다.")]
    public float maxSpeedThreshold = 10f;

    [Tooltip("Blur 강도가 감소하는 딜레이(초)")]
    public float blurFadeOutTime = 0.5f;

    private Rigidbody _rb;
    private float _currentBlur = 0f;
    private float _targetBlur = 0f;
    private float _fadeTimer = 0f;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        // 초기값
        SpeedBlurFeature.CurrentBlurIntensity = 0f;
    }

    void Update()
    {
        // 1) 현재 속도 크기 계산 (2D가 아닌 3D 속도로 가정)
        float speed = _rb.linearVelocity.magnitude;

        // 2) 속도를 0~1 사이 BlurIntensity로 매핑
        float mapped = Mathf.InverseLerp(minSpeedThreshold, maxSpeedThreshold, speed);
        // Mathf.InverseLerp: speed≤minSpeedThreshold → 0, speed≥maxSpeedThreshold → 1,
        // 그 사이 값은 0~1 비율로 선형 매핑

        // 3) mapped가 (_targetBlur)보다 크면 즉시 그 값으로 셋, 작아지면 서서히 감소
        if (mapped > _targetBlur)
        {
            _targetBlur = mapped;
            _fadeTimer = 0f;
        }
        else
        {
            // 속도가 감소할 때, 곧바로 0으로 떨어지는 게 아니라 
            // blurFadeOutTime 초 동안 선형으로 감소하도록 처리
            _fadeTimer += Time.deltaTime;
            float t = Mathf.Clamp01(_fadeTimer / blurFadeOutTime);
            _targetBlur = Mathf.Lerp(_targetBlur, mapped, t);
        }

        // 4) 최종 blur 값을 CurrentBlurIntensity에 적용
        _currentBlur = Mathf.Clamp01(_targetBlur);
        SpeedBlurFeature.CurrentBlurIntensity = _currentBlur;
    }
}
