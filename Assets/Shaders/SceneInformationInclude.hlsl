#ifndef SCENEDEPTH
#define SCENEDEPTH

void SampleSceneDepth_float(float4 UV, out float Out)
{
    Out = SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV);
}

void SampleSceneNormal_float(float4 UV, out float3 Out)
{
    Out = SHADERGRAPH_SAMPLE_SCENE_NORMAL(UV);
}

void SampleSceneColor_float(float4 UV, out float3 Out)
{
    Out = SHADERGRAPH_SAMPLE_SCENE_COLOR(UV);
}
#endif