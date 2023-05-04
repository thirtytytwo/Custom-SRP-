#ifndef CUSTOM_LIT_PASS_INCLUDE
#define CUSTOM_LIT_PASS_INCLUDE
#include "Common.hlsl"
#include "Surface.hlsl"
#include "Light.hlsl"
#include "Lighting.hlsl"

/*CBUFFER_START(UnityPerMaterial) //SRPBatch
half4 _BaseColor;
CBUFFER_END*/
TEXTURE2D(_BaseMap);
SAMPLER(sampler_BaseMap);

UNITY_INSTANCING_BUFFER_START(UnityPerMaterial) //GPUInstancing
    UNITY_DEFINE_INSTANCED_PROP(half4, _BaseColor)
    UNITY_DEFINE_INSTANCED_PROP(float4, _BaseMap_ST)
UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)
struct a2v
{
    float3 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float2 uv :TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID//GPUInstancing
};

struct v2f
{
    float4 positionCS:SV_POSITION;
    float3 normalWS :NORMAL1;
    float2 uv: TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID//GPUInstancing
};
v2f LitPassVertex(a2v i)
{
    v2f o;
    #pragma region GPUInstancing
    UNITY_SETUP_INSTANCE_ID(i);
    UNITY_TRANSFER_INSTANCE_ID(i, o);
    #pragma endregion 
    o.positionCS = TransformObjectToHClip(i.positionOS);
    o.normalWS = TransformObjectToWorldNormal(i.normalOS);
    o.uv = i.uv;
    return o;
};
half4 LitPassFragment(v2f i):SV_TARGET
{
    #pragma region GPUInstancing
    UNITY_SETUP_INSTANCE_ID(i);
    half3 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);
    half4 baseColor = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseColor);
    #pragma endregion
    Surface surface;
    surface.normal = normalize(i.normalWS);
    surface.color = baseColor.rgb;
    surface.alpha = baseMap.r;
    return half4(GetLighting(surface) * baseColor.rgb, 1.0f);
}
#endif