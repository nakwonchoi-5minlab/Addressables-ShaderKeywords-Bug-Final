Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" "Queue" = "Geometry+0" }
        LOD 100

        Pass
        {
            Name "Universal Forward"
            Tags { "LightMode" = "UniversalForward" }
            Cull [_Cull]
            ZTest LEqual


            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN

            struct appdata
            {
                float4 color : COLOR0;
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 color : COLOR0;
                float4 positionCS : SV_POSITION;
                
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.color = v.color;
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.positionCS = TransformObjectToHClip(v.vertex.xyz);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // sample the texture
                half4 col = tex2D(_MainTex, i.uv);
                
                #if defined(_MAIN_LIGHT_SHADOWS)
                    col = half4(1.0, 0, 0, 1.0);
                #elif defined(_MAIN_LIGHT_SHADOWS_CASCADE)
                    col = half4(0, 1.0, 0, 1.0);
                #endif

                return col;
            }
            ENDHLSL
        }
    }
}
