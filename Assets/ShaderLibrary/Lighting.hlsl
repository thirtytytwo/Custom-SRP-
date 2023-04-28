#ifndef CUSTOM_LIGHTING_INCLUDE
#define CUSTOM_LIGHTING_INCLUDE

half3 GetLighting(Surface surface, Light light);
half3 IncomingLight(Surface surface, Light light);


half3 GetLighting(Surface surface)
{
    half3 result = 0.0;
    for(int i =0; i < GetLightCount(); i++)
    {
        result += GetLighting(surface, GetDirectionalLight(i));
    }
    return result;
};

half3 GetLighting(Surface surface, Light light)
{
    return IncomingLight(surface, light);
};

half3 IncomingLight(Surface surface, Light light)
{
    return half3((dot(surface.normal, light.direction) * 0.5 + 0.5) * light.color);
};
#endif