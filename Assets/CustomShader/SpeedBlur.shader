Shader "Custom/SpeedBlur"
{
    Properties
    {
        _MainTex       ("Scene Color (ReadOnly)",  2D)    = "white" {}
        _BlurIntensity ("Blur Intensity (0~1)",    Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        // 이 Pass는 풀스크린 블릿 전용이므로 ZTest/ZWrite 비활성화
        Pass
        {
            Name "SpeedBlurPass"
            ZTest Always Cull Off ZWrite Off

            Stencil
            {
                    Ref 1           
                    Comp NotEqual 
                    Pass Keep
            }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float _BlurIntensity;    // 0(블러 없음) ~ 1(최대 블러)
            float4 _MainTex_TexelSize; // x=1/width, y=1/height, z=width, w=height

            struct Attributes
            {
                float2 positionOS : POSITION;
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
                // (0~1) → (-1~+1) 클립 좌표로 변환
                float2 pos = IN.positionOS * 2 - 1;
                pos.y   = -pos.y;
                OUT.positionHCS = float4(pos, 0, 1);
                OUT.uv = IN.uv;
                return OUT;
            }

            // 3×3 평균 블러
            float4 Frag(Varyings IN) : SV_Target
            {
                // 블러 강도가 거의 0이면 그냥 원본을 리턴
                if (_BlurIntensity <= 0.001)
                    return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                // 주변 9개 샘플을 합산
                float2 texel = float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y);
                float2 uv = IN.uv;

                float4 sum = float4(0,0,0,0);
                sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + texel * float2(-1, -1));
                sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + texel * float2( 0, -1));
                sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + texel * float2( 1, -1));

                sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + texel * float2(-1,  0));
                sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + texel * float2( 0,  0));
                sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + texel * float2( 1,  0));

                sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + texel * float2(-1,  1));
                sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + texel * float2( 0,  1));
                sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + texel * float2( 1,  1));

                // 평균값 계산 (9로 나눔)
                float4 blurColor = sum / 9.0;

                // 블러 강도에 따라 원본과 블러를 섞음
                float4 original = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                return lerp(original, blurColor, _BlurIntensity);
            }
            ENDHLSL
        }
    }
}
