#ifndef CUSTOM_LIGHT_INCLUDE
#define CUSTOM_LIGHT_INCLUDE
#define MAX_COUNT 4
struct Light
{
    half3 color;
    float3 direction;
};

CBUFFER_START(_CustomLight)
    half3 _DirectionalLightColor[MAX_COUNT];
    float3 _DirectionalLightDirection[MAX_COUNT];
    int _DirectionalLightCount;
CBUFFER_END

int GetLightCount()
{
    return _DirectionalLightCount;
}
Light GetDirectionalLight(int index)
{
    Light light;
    light.color = _DirectionalLightColor[index].rgb;
    light.direction = _DirectionalLightDirection[index].xyz;
    return light;
}

#endif