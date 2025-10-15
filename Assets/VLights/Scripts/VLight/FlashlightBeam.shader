Shader "Custom/FlashlightBeamURP"
{
    Properties
    {
        [HDR] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        _Alpha("Alpha", Range(0,1)) = 0.2
    }

    SubShader
    {
        Tags { 
            "RenderPipeline"="UniversalPipeline" 
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        Blend One One      // additive blending → tembus fog
        ZWrite Off          // jangan tulis depth biar gak potong kabut
        Cull Back           // hanya render sisi depan

        Pass
        {
            Name "BeamPass"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
                float _Alpha;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
                color.a = _Alpha;
                return color;
            }
            ENDHLSL
        }
    }
}
