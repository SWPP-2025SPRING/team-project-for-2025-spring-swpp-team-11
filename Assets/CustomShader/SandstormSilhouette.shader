Shader "Custom/SandstormSilhouette_FixFlip"
{
    Properties
    {
        _Threshold    ("Threshold",   Range(0,1)) = 0.5
        _SandColor    ("Sand Color",  Color)      = (0.8,0.7,0.5,1)
        _NoiseTex     ("NoiseTex",    2D)         = "white" {}
        _NoiseSpeed   ("Noise Speed", Float)      = 0.2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Name "SandstormSilhouettePass"
            ZTest Always Cull Off ZWrite Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_CameraColorTexture);
            SAMPLER(sampler_CameraColorTexture);
            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            float4  _SandColor;
            float   _Threshold;
            float   _NoiseSpeed;

            struct Attributes
            {
                float2 positionOS : POSITION;  // (0~1, 0~1)로 가정
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                // 1) (0~1 범위) → (-1~+1 범위) 클립스크린 좌표
                float2 pos = IN.positionOS * 2 - 1;
                // 2) Metal 등에서 화면 Y축이 뒤집히는 현상을 보정하기 위해 Y만 반전
                pos.y = -pos.y;
                OUT.positionHCS = float4(pos, 0, 1);
                OUT.uv = IN.uv;
                return OUT;
            }

            float4 Frag(Varyings IN) : SV_Target
            {
                // 3) 화면 컬러 샘플링 (그러면 이미 UV는 올바른 방향임)
                float4 sceneCol = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, IN.uv);

                // 4) 그레이스케일 계산
                float luminance = dot(sceneCol.rgb, float3(0.299, 0.587, 0.114));

                // 5) 노이즈 샘플링 (UV를 시간에 따라 움직이기)
                float2 noiseUV = IN.uv + _Time.y * _NoiseSpeed;
                float noiseVal = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, noiseUV).r;

                // 6) 그레이 + 노이즈 합성
                float combined = lerp(luminance, noiseVal, 0.5);

                // 7) 임계값 비교로 실루엣 마스크 생성
                float mask = step(_Threshold, combined);

                // 8) 마스크 값에 따라 검은색 실루엣 또는 모래색 출력
                float3 finalRGB = lerp(float3(0,0,0), _SandColor.rgb, mask);
                return float4(finalRGB, 1.0);
            }
            ENDHLSL
        }
    }
}
