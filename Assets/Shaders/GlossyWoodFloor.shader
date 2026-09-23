Shader "Museum/Glossy Wood Mirror Floor"
{
    Properties
    {
        [Header(Wood Texture Settings)]
        _BaseMap ("Wood Floor Texture", 2D) = "white" {}
        _BaseColor ("Wood Tint Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _BumpMap ("Normal / Bump Map", 2D) = "bump" {}
        _BumpScale ("Normal Strength", Range(0, 2)) = 0.25
        _TileScale ("Plank Tile Size (meters)", Float) = 2.5
        _WoodRotation ("Plank Rotation (deg)", Range(0, 360)) = 0.0
        [Toggle] _UseWorldSpaceUV ("Use World Space UV", Float) = 1.0

        [Header(Mirror Reflection Settings)]
        _Smoothness ("Mirror Smoothness", Range(0.8, 1.0)) = 0.985
        _Metallic ("Metallic / Reflectivity", Range(0.0, 0.5)) = 0.12
        _MirrorBoost ("Mirror Reflection Intensity", Range(0.5, 3.0)) = 1.7
        [Toggle(_SPECULARHIGHLIGHTS_OFF)] _SpecularHighlights ("Disable Moving Light Glints", Float) = 0.0
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
                float4 _BaseMap_ST;
                float4 _BumpMap_ST;
                float _BumpScale;
                float _TileScale;
                float _WoodRotation;
                float _UseWorldSpaceUV;
                half _Metallic;
                half _Smoothness;
                half _MirrorBoost;
            CBUFFER_END

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_BumpMap);
            SAMPLER(sampler_BumpMap);

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float3 positionWS   : TEXCOORD0;
                half3 normalWS      : TEXCOORD1;
                half4 tangentWS     : TEXCOORD2;
                float2 uv           : TEXCOORD3;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                output.positionCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.normalWS = normalInputs.normalWS;
                output.tangentWS = half4(normalInputs.tangentWS, input.tangentOS.w);
                output.uv = input.uv;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // 1. Calculate UV coordinates (World-Space XZ or Mesh UV)
                float2 floorUV;
                if (_UseWorldSpaceUV > 0.5)
                {
                    float rad = radians(_WoodRotation);
                    float s = sin(rad);
                    float c = cos(rad);
                    float2x2 rot = float2x2(c, -s, s, c);
                    floorUV = mul(rot, input.positionWS.xz) / max(_TileScale, 0.01);
                }
                else
                {
                    floorUV = input.uv * _BaseMap_ST.xy + _BaseMap_ST.zw;
                }

                // 2. Sample BaseMap (Wood Albedo)
                half4 albedoTex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, floorUV);
                half3 woodAlbedo = albedoTex.rgb * _BaseColor.rgb;

                // 3. Normal mapping (subtle wood grain relief beneath smooth mirror varnish)
                half3 normalWS = NormalizeNormalPerPixel(input.normalWS);
                half4 bumpSample = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, floorUV);
                half3 normalTS = UnpackNormalScale(bumpSample, _BumpScale);

                // Build TBN matrix with fallback for flat geometry
                half3 tangentWS = input.tangentWS.xyz;
                if (dot(tangentWS, tangentWS) < 0.001)
                {
                    tangentWS = half3(1, 0, 0);
                }
                half3 bitangentWS = cross(normalWS, tangentWS) * (abs(input.tangentWS.w) > 0.01 ? input.tangentWS.w : 1.0);
                half3x3 tangentToWorld = half3x3(tangentWS, bitangentWS, normalWS);
                // For a mirror floor, surface coat is smooth, so we blend normal with up vector
                half3 blendedNormalWS = SafeNormalize(mul(normalTS, tangentToWorld));
                // Coat normal is practically flat up for clear mirror reflection
                half3 coatNormalWS = lerp(normalWS, blendedNormalWS, 0.15);

                // 4. Setup PBR Surface Data
                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = woodAlbedo;
                surfaceData.metallic = _Metallic;
                surfaceData.smoothness = _Smoothness;
                surfaceData.normalTS = half3(0, 0, 1);
                surfaceData.occlusion = 1.0;
                surfaceData.alpha = 1.0;

                // 5. Setup Input Data for URP Lighting
                InputData inputData = (InputData)0;
                inputData.positionWS = input.positionWS;
                inputData.normalWS = coatNormalWS;
                inputData.viewDirectionWS = SafeNormalize(GetWorldSpaceViewDir(input.positionWS));
                inputData.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                inputData.bakedGI = SampleSH(coatNormalWS);
                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
                inputData.shadowMask = half4(1, 1, 1, 1);

                // 6. Compute PBR Lighting (URP already applies reflection probes + box projection)
                half4 finalColor = UniversalFragmentPBR(inputData, surfaceData);

                // Extra env brightness without a second box-projection pass.
                // GlossyEnvironmentReflection uses URP's probe + box projection once.
                if (_MirrorBoost > 1.01)
                {
                    half3 reflectVector = reflect(-inputData.viewDirectionWS, coatNormalWS);
                    half roughness = PerceptualSmoothnessToPerceptualRoughness(_Smoothness);
                    half3 envColor = GlossyEnvironmentReflection(reflectVector, inputData.positionWS, roughness, 1.0, inputData.normalizedScreenSpaceUV);
                    half ndotv = saturate(dot(coatNormalWS, inputData.viewDirectionWS));
                    half fresnel = Pow4(1.0 - ndotv);
                    half extra = (_MirrorBoost - 1.0) * lerp(0.35, 1.15, fresnel);
                    finalColor.rgb += envColor * extra;
                }

                return finalColor;
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
