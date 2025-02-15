Shader "Custom Post-Processing/UnderwaterEffect"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _WaterColor("Water Color", Color) = (0, 0.4, 0.6, 1)
        _DistortionStrength("Distortion Strength", Range(0, 0.1)) = 0.05
        _FogIntensity("Fog Intensity", Range(0, 2)) = 0.5
        _CausticTex("Caustic Texture", 2D) = "black" {} // Optional caustics
        _CausticStrength("Caustic Strength", Range(0, 1)) = 0.2
        _ChromaticAmount("Chromatic Aberration", Range(0, 0.01)) = 0.005
    }

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _CausticTex;
            float4 _MainTex_ST;
            float4 _WaterColor;
            float _DistortionStrength;
            float _FogIntensity;
            float _CausticStrength;
            float _ChromaticAmount;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float depth : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                //  sine wave for disortion
                float wave1 = sin(o.uv.y * 20.0 + _Time.y * 5.0) * 0.02 * _DistortionStrength;
                float wave2 = sin(o.uv.x * 15.0 + _Time.y * 3.0) * 0.015 * _DistortionStrength;
                float waveDistortion = wave1 + wave2;

                o.uv += float2(waveDistortion, waveDistortion * 0.7);

                o.depth = o.vertex.z / o.vertex.w;  

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Apply chromatic affect
                float2 chromaticOffset = i.uv * _ChromaticAmount;
                fixed4 colorR = tex2D(_MainTex, i.uv + chromaticOffset);
                fixed4 colorG = tex2D(_MainTex, i.uv);
                fixed4 colorB = tex2D(_MainTex, i.uv - chromaticOffset);
                fixed4 color = fixed4(colorR.r, colorG.g, colorB.b, 1);

                // animated textre
                float2 causticUV = i.uv + float2(_Time.y * 0.1, _Time.y * 0.05);
                float caustic = tex2D(_CausticTex, causticUV).r * _CausticStrength;
                color.rgb += caustic;

                // Improved fog with realistic color absorption
                float fogFactor = exp(-i.depth * _FogIntensity);
                float3 fogColor = _WaterColor.rgb * float3(0.8, 0.9, 1.0); // Water absorbs red/green
                color.rgb = lerp(fogColor, color.rgb, fogFactor);

                return color;
            }
            ENDHLSL
        }
    }
}
