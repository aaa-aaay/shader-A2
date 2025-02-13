Shader "Custom Post-Processing/UnderwaterEffect"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _WaterColor("Water Color", Color) = (0, 0.4, 0.6, 1)
        _DistortionStrength("Distortion Strength", Range(0, 1)) = 0.05
        _FogIntensity("Fog Intensity", Range(0, 1)) = 0.5
    }

    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        // Render settings for URP
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM
            // Vertex and fragment shader functions
            #pragma vertex vert
            #pragma fragment frag

            // Include UnityCG.cginc (Old Unity Shader Library)
            #include "UnityCG.cginc"

            // Texture and parameters
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _WaterColor;
            float _DistortionStrength;
            float _FogIntensity;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                // Apply wave distortion effect
                float wave = sin(o.uv.y * 8.0 + _Time.y * 2.0) * 0.002 * _DistortionStrength;
                o.uv += float2(wave, wave * 0.5);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.uv);

                // Apply fog effect
                float depth = i.uv.y;
                float fogFactor = saturate(1.0 - depth * _FogIntensity);
                color.rgb = lerp(color.rgb, _WaterColor.rgb, fogFactor * 0.5);

                return color;
            }
            ENDHLSL
        }
    }
}
