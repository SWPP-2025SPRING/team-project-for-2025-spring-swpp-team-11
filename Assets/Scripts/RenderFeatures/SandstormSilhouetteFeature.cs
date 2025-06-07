using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SandstormSilhouetteFeature : ScriptableRendererFeature
{
    // ▶ 이 static 플래그가 true일 때만 패스가 작동합니다.
    public static bool IsActive = false;

    class SandstormRenderPass : ScriptableRenderPass
    {
        const string _profilerTag = "SandstormSilhouettePass";

        readonly Material _material;
        readonly int      _tempColorTexId;

        public SandstormRenderPass(Material mat, RenderPassEvent evt)
        {
            _material = mat;
            renderPassEvent = evt;
            _tempColorTexId = Shader.PropertyToID("_TemporaryColorTexture");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_material == null)
                return;

            var cmd = CommandBufferPool.Get(_profilerTag);

            // 1) 카메라 타겟과 같은 크기로 임시 RT 생성 (깊이 버퍼 없이)
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;
            cmd.GetTemporaryRT(_tempColorTexId, desc, FilterMode.Bilinear);

            // 2) Execute() 내부에서만 cameraColorTargetHandle 호출
            RTHandle cameraHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;
            RenderTargetIdentifier cameraColor = cameraHandle;

            // 3) 임시 RT를 RenderTargetIdentifier로 변환
            var tempRT = new RenderTargetIdentifier(_tempColorTexId);

            // 4) Blit: 카메라 컬러 → 임시 RT
            cmd.Blit(cameraColor, tempRT);

            // 5) Blit: 임시 RT → 카메라 컬러 (머티리얼 적용)
            cmd.Blit(tempRT, cameraColor, _material);

            // 6) 임시 RT 해제
            cmd.ReleaseTemporaryRT(_tempColorTexId);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    [System.Serializable]
    public struct SandstormSettings
    {
        public Material        sandstormMaterial;
        public RenderPassEvent renderPassEvent;
    }

    public SandstormSettings settings;
    private SandstormRenderPass _scriptablePass;

    public override void Create()
    {
        if (settings.sandstormMaterial == null)
        {
            Debug.LogWarning("SandstormSilhouetteFeature: 머티리얼이 할당되지 않았습니다.");
            return;
        }

        _scriptablePass = new SandstormRenderPass(
            settings.sandstormMaterial,
            settings.renderPassEvent
        );
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (_scriptablePass == null || settings.sandstormMaterial == null)
            return;

        // ▶ IsActive가 true여야만 패스를 등록하도록 합니다.
        if (IsActive)
        {
            renderer.EnqueuePass(_scriptablePass);
        }
    }
}
