#ifndef INTERPOLATION_INCLUDED
#define INTERPOLATION_INCLUDED

float bilinear_interpolation(float x, float y, float q11, float q12, float q21, float q22, float2 xRange, float2 yRange)
{
    float x1 = xRange.x;
    float x2 = xRange.y;

    float y1 = yRange.x;
    float y2 = yRange.y;
                
    float r1 = (x2 - x) / (x2 - x1) * q11 + (x - x1) / (x2 - x1) * q21;
    float r2 = (x2 - x) / (x2 - x1) * q12 + (x - x1) / (x2 - x1) * q22;

    float p = (y2 - y) / (y2 - y1) * r1 + (y - y1) / (y2 - y1) * r2;

    return p;
}

#endif