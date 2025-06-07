using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SpeedBlurFeature : ScriptableRendererFeature
{
    // 현재 프레임의 블러 강도를 외부에서 동적으로 설정할 수 있도록
    public static float CurrentBlurIntensity = 0f;

    class SpeedBlurPass : ScriptableRenderPass
    {
        const string _profilerTag = "SpeedBlurPass";
        readonly Material _material;
        readonly int      _tempRTId;
        readonly int      _intensityID;

        public SpeedBlurPass(Material mat, RenderPassEvent evt)
        {
            _material = mat;
            renderPassEvent = evt;
            // 임시 렌더 텍스처용 ID
            _tempRTId = Shader.PropertyToID("_SpeedBlurTempRT");
            // 셰이더의 _BlurIntensity 프로퍼티 ID
            _intensityID = Shader.PropertyToID("_BlurIntensity");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_material == null)
                return;

            // CurrentBlurIntensity 값이 거의 0이면 패스를 건너뜁니다.
            if (CurrentBlurIntensity <= 0.001f)
                return;

            // 머티리얼에 블러 강도 설정
            _material.SetFloat(_intensityID, CurrentBlurIntensity);

            var cmd = CommandBufferPool.Get(_profilerTag);

            // 1) 카메라 타겟과 동일한 크기로 임시 RT 생성 (깊이 버퍼 없이)
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;
            cmd.GetTemporaryRT(_tempRTId, desc, FilterMode.Bilinear);

            // 2) Execute() 내부에서 cameraColorTargetHandle 호출
            RTHandle cameraHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;
            RenderTargetIdentifier cameraColor = cameraHandle;

            // 3) 임시 RT를 RenderTargetIdentifier로 변환
            var tempRT = new RenderTargetIdentifier(_tempRTId);

            // 4) 카메라 컬러 → 임시 RT
            cmd.Blit(cameraColor, tempRT);

            // 5) 임시 RT → 카메라 컬러 (SpeedBlur 머티리얼 적용)
            cmd.Blit(tempRT, cameraColor, _material);

            // 6) 임시 RT 해제
            cmd.ReleaseTemporaryRT(_tempRTId);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    [System.Serializable]
    public struct SpeedBlurSettings
    {
        public Material        speedBlurMaterial;
        public RenderPassEvent renderPassEvent;
    }

    public SpeedBlurSettings settings;
    private SpeedBlurPass _scriptablePass;

    public override void Create()
    {
        if (settings.speedBlurMaterial == null)
        {
            Debug.LogWarning("SpeedBlurFeature: speedBlurMaterial이 할당되지 않았습니다.");
            return;
        }

        // SpeedBlurPass 생성 시점: 언제 이 블릿을 적용할지 설정
        _scriptablePass = new SpeedBlurPass(settings.speedBlurMaterial, settings.renderPassEvent);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (_scriptablePass == null || settings.speedBlurMaterial == null)
            return;

        // 렌더러에 패스 등록 (블러 강도가 0이면 Execute() 내에서 스킵됩니다)
        renderer.EnqueuePass(_scriptablePass);
    }
}
