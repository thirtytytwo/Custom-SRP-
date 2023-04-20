#ifndef CUSTOM_UNLIT_PASS_INCLUDE
#define CUSTOM_UNLIT_PASS_INCLUDE
#include "Common.hlsl"

/*CBUFFER_START(UnityPerMaterial) //SRPBatch
half4 _BaseColor;
CBUFFER_END*/

UNITY_INSTANCING_BUFFER_START(UnityPerMaterial) //GPUInstancing
UNITY_DEFINE_INSTANCED_PROP(half4, _BaseColor)
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
    float2 uv: TEXCOORD1;
    UNITY_VERTEX_INPUT_INSTANCE_ID//GPUInstancing
};
v2f UnlitPassVertex(a2v i)
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
half4 UnlitPassFragment(v2f i):SV_TARGET
{
    #pragma region GPUInstancing
    UNITY_SETUP_INSTANCE_ID(i);
    return UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseColor);
    #pragma endregion 
}
#endif