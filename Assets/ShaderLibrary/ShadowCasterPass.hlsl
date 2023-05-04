#ifndef SHADOW_CASTER_PASS_INCLUDE
#define SHADOW_CATSER_PASS_INCLUDE

#include "Common.hlsl"

TEXTURE2D(_BaseMap);
SAMPLER(sampler_BaseMap);

CBUFFER_START(UnityPerMaterial)
    float4 _BaseMap_ST;
    float4 _BaseColor;
CBUFFER_END

struct a2v
{
    float3 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float2 uv :TEXCOORD0;
};

struct v2f
{
    float4 positionCS:SV_POSITION;
    float3 normalWS :NORMAL1;
    float2 uv: TEXCOORD0;
};

v2f ShadowCasterPassVertex(a2v i)
{
    v2f o;
    o.positionCS = TransformObjectToHClip(i.positionOS);
    o.normalWS = TransformObjectToWorldNormal(i.normalOS);
    o.uv = i.uv;
}

void ShadowCasterPassFragment(v2f i)
{
    float4 base = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);
    float4 result = base * _BaseColor;
}
#endif