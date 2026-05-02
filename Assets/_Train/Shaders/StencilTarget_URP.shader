Shader "Custom/StencilTarget_URP"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { 
            "RenderType" = "Opaque" 
            "Queue" = "Geometry"
            "RenderPipeline" = "UniversalPipeline"
        }
        
        Stencil
        {
            Ref 1
            Comp Equal    // Рисуем только там, где трафарет = 1
        }
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag() : SV_Target
            {
                return _BaseColor;
            }
            ENDHLSL
        }
    }
}