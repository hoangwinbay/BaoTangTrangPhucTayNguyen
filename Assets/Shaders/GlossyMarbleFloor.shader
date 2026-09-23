Shader "Museum/Glossy Marble Floor"
{
    Properties
    {
        _BaseColor ("Marble Color", Color) = (0.84, 0.86, 0.88, 1)
        _VeinColor ("Vein Color", Color) = (0.25, 0.29, 0.34, 1)
        _GroutColor ("Grout Color", Color) = (0.10, 0.12, 0.15, 1)
        _TileSize ("Tile Size (meters)", Float) = 1.2
        _GroutWidth ("Grout Width", Range(0.001, 0.08)) = 0.018
        _Metallic ("Metallic", Range(0, 1)) = 0.05
        _Smoothness ("Mirror Smoothness", Range(0, 1)) = 0.9
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fog
            #pragma multi_compile _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile _ _REFLECTION_PROBE_BOX_PROJECTION
            #pragma multi_compile _ _SPECULARHIGHLIGHTS_OFF

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _VeinColor;
                half4 _GroutColor;
                float _TileSize;
                float _GroutWidth;
                half _Metallic;
                half _Smoothness;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                half3 normalWS : TEXCOORD1;
            };

            float Hash21(float2 p)
            {
                p = frac(p * float2(123.34, 345.45));
                p += dot(p, p + 34.345);
                return frac(p.x * p.y);
            }

            float Noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(Hash21(i), Hash21(i + float2(1, 0)), f.x),
                            lerp(Hash21(i + float2(0, 1)), Hash21(i + float2(1, 1)), f.x), f.y);
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);
                output.positionCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.normalWS = normalInputs.normalWS;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float2 marbleUV = input.positionWS.xz / max(_TileSize, 0.01);
                float2 cellUV = frac(marbleUV);
                float edge = min(min(cellUV.x, cellUV.y), min(1.0 - cellUV.x, 1.0 - cellUV.y));
                float grout = 1.0 - smoothstep(_GroutWidth, _GroutWidth + 0.01, edge);

                float2 veinUV = marbleUV * 2.4;
                float distortion = Noise(veinUV * 1.8) * 2.2 + Noise(veinUV * 5.1) * 0.35;
                float veinLine = abs(sin((veinUV.x * 1.25 + veinUV.y * 0.55 + distortion) * 3.14159));
                float vein = 1.0 - smoothstep(0.70, 0.92, veinLine);
                half3 marble = lerp(_BaseColor.rgb, _VeinColor.rgb, vein * 0.42);
                marble = lerp(marble, _GroutColor.rgb, grout);

                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = marble;
                surfaceData.metallic = _Metallic;
                surfaceData.smoothness = _Smoothness;
                surfaceData.normalTS = half3(0, 0, 1);
                surfaceData.occlusion = 1;
                surfaceData.alpha = 1;

                InputData inputData = (InputData)0;
                inputData.positionWS = input.positionWS;
                inputData.normalWS = NormalizeNormalPerPixel(input.normalWS);
                inputData.viewDirectionWS = SafeNormalize(GetWorldSpaceViewDir(input.positionWS));
                inputData.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                inputData.bakedGI = SampleSH(inputData.normalWS);
                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
                inputData.shadowMask = half4(1, 1, 1, 1);

                return UniversalFragmentPBR(inputData, surfaceData);
            }
            ENDHLSL
        }
    }
}
