Shader "Custom/StencilMask_URP"
{
   SubShader
    {
        Tags { 
            "RenderType" = "Opaque" 
            "Queue" = "Geometry-1"
            "RenderPipeline" = "UniversalPipeline"
        }
        
        Cull Off
        ZWrite On
        ZTest LEqual  // Убрали Always, теперь учитываем глубину
        
        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace
        }
        
        ColorMask 0
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag() : SV_Target
            {
                return half4(0,0,0,0);
            }
            ENDHLSL
        }
    }
}