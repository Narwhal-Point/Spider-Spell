// used https://www.youtube.com/watch?v=GGTTHOpUQDE&t=221s.

#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

// found the correct include. Needs to be disabled for the shader to work.
// #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

void MainLight_float(float3 WorldPos, out float3 Direction, out float ShadowAtten)
{
    #if SHADERGRAPH_PREVIEW
    Direction = float3(0.5, 0.5, 0);
    ShadowAtten = 1;
    #else
    
    float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
    Light mainLight = GetMainLight(shadowCoord);
    // light direction
    Direction = mainLight.direction;
    // shadow
    ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();
    float shadowStrength = GetMainLightShadowStrength();
    ShadowAtten = SampleShadowmap(shadowCoord,
                                  TEXTURE2D_ARGS(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture),
                                  shadowSamplingData, shadowStrength, false);
    #endif
}

void MoreLights_float(float3 WorldPos, float3 Normal, float LightIntensity, out float3 Color)
{
    // initialize variables
    Color = float3(0, 0, 0);
    float maxDistance = 0;
    
    #ifndef SHADERGRAPH_PREVIEW
    // loop through all the available lights
    int pixelLightCount = GetAdditionalLightsCount();
    for (int i = 0; i < pixelLightCount; ++i)
    {
        // retrieve light data for index i
        Light light = GetAdditionalLight(i, WorldPos);
        
        // if within range of light
        if (light.distanceAttenuation > 0)
        {
            if (light.distanceAttenuation > maxDistance)
            {
                // set color to closest light source multiplied by the step of the dot product of the light direction and the normal
                Color = light.color * (clamp(dot(light.direction,Normal),0,1) > 0);
                // register distance of closest light source
                maxDistance = light.distanceAttenuation; 
            }
        }
    }
    // multiply color by light intensity
    Color *= LightIntensity; 

    #endif
}

#endif
