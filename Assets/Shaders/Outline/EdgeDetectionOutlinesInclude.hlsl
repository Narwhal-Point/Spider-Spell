#ifndef SOBELOUTLINES_INCLUDED
#define SOBELOUTLINES_INCLUDED

// https://www.youtube.com/watch?v=RMt6DcaMxcE

/*
 * the sobel effect runs by sampling the texture around a point to see
 * if there are any large changes. Each sample is multiplied by a convolution
 * matrix weight for the x and y components separately. Each value is then
 * added together, and the final sobel value is the length of the resulting float2.
 * Higher values mean the algorithm detected more of an edge.
 */

// these are points to sample relative to the starting point
static float2 sobelSamplePoints[9] = {
    float2(-1, 1), float2(0, 1), float2(1, 1),
    float2(-1, 0), float2(0, 0), float2(1, 1),
    float2(-1, -1), float2(0, -1), float2(1, -1),
};

// weights for the x component
static float sobelXMatrix[9] = {
    1, 0, -1,
    2, 0, -2,
    1, 0, -1
};

// weights for the y component
static float sobelYMatrix[9] = {
    1, 2, 1,
    0, 0, 0,
    -1, -2, -1
};

// euler
// float ease_out_quad(float x) {
//     float t = x; float b = 0; float c = 1; float d = 1;
//     return -c *(t/=d)*(t-2) + b;
// }

// Find a way to sample the normals too. The URP Node URP_SAMPLE_BUFFER can do this. But how implement in this project???????


// this function runs the sobel algorithm over the depth texture
// thickness is the sample distance of the matrix.
void DepthSobel_float(float2 UV, float Thickness, out float Out)
{
    
    float2 sobel = 0;
    // unroll the loop to make it more efficient
    [unroll] for (int i = 0; i < 9; i++)
    {
        // ga van screen UV naar pixels (of andersom)
        // UV of sobelSamplePoints ombouwen naar zelfde coordinaatsysteem

        float depth = 400 * SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV + sobelSamplePoints[i] * Thickness);
        sobel += depth * float2(sobelXMatrix[i], sobelYMatrix[i]);
        
    }
    Out = length(sobel);
}


void ColorSobel_float(float2 UV, float Thickness, out float Out)
{
    float sobelR = 0;
    float sobelG = 0;
    float sobelB = 0;

    // unroll the loop to make it more efficient
    [unroll] for (int i = 0; i < 9; i++)
    {
        float3 rgb = SHADERGRAPH_SAMPLE_SCENE_COLOR(UV + sobelSamplePoints[i] * Thickness);

        float2 kernel = float2(sobelXMatrix[i], sobelYMatrix[i]);

        sobelR += rgb.r * kernel;
        sobelG += rgb.g * kernel;
        sobelB += rgb.b * kernel;
    }
    
    Out = max(length(sobelR), max(length(sobelG), length(sobelB)));
}

void NormalSobel_float(float2 UV, float Thickness, out float Out)
{
    float sobelX = 0;
    float sobelY = 0;
    float sobelZ = 0;

    // unroll the loop to make it more efficient
    [unroll] for (int i = 0; i < 9; i++)
    {
        float3 normal = SHADERGRAPH_SAMPLE_SCENE_NORMAL(UV + sobelSamplePoints[i] * Thickness);

        float2 kernel = float2(sobelXMatrix[i], sobelYMatrix[i]);

        sobelX += normal.x * kernel;
        sobelY += normal.y * kernel;
        sobelZ += normal.z * kernel;
    }
    
    Out = max(length(sobelX), max(length(sobelY), length(sobelZ)));
}

#endif