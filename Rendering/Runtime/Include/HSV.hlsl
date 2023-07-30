#ifndef HSV_INCLUDED
#define HSV_INCLUDED

#define MIN_HSV_RANGE(range) float3(range.minH, range.minS, range.minV)
#define MAX_HSV_RANGE(range) float3(range.maxH, range.maxS, range.maxV)

#define MIN_V(range) min(range.minV, range.maxV)
#define MAX_V(range) max(range.minV, range.maxV)

#define MIN_S(range) min(range.minS, range.maxS)
#define MAX_S(range) max(range.minS, range.maxS)

#define MIN_H(range) min(range.minH, range.maxH)
#define MAX_H(range) max(range.minH, range.maxH)

#define IS_V_INCLUDE(hsv, range) (IS_V_INVERSE(range) ? hsv.z >= range.maxV && hsv.z <= range.minV : hsv.z >= range.minV && hsv.z <= range.maxV)
#define IS_S_INCLUDE(hsv, range) (IS_S_INVERSE(range) ? hsv.y >= range.maxS && hsv.y <= range.minS : hsv.y >= range.minS && hsv.y <= range.maxS)
#define IS_H_INCLUDE(hsv, range) (!IS_H_INVERSE(range) ? hsv.x >= range.minH && hsv.x <= range.maxH : hsv.x >= range.minH && hsv.x <= 1.0 || hsv.x >= 0.0 && hsv.x <= range.maxH)

#define IS_RANGE_INCLUDE(hsv, range) (IS_V_INCLUDE(hsv, range) && IS_S_INCLUDE(hsv, range) && IS_H_INCLUDE(hsv, range))

#define IS_LINEAR_RANGES_CROSS(min, max) (max.x >= min.y && min.x <= max.y || max.y >= min.x && min.y <= max.x)

#define IS_V_CROSS(rangeA, rangeB) IS_LINEAR_RANGES_CROSS(float2(MIN_V(rangeA), MIN_V(rangeB)), float2(MAX_V(rangeA), MAX_V(rangeB)))
#define IS_S_CROSS(rangeA, rangeB) IS_LINEAR_RANGES_CROSS(float2(MIN_S(rangeA), MIN_S(rangeB)), float2(MAX_S(rangeA), MAX_S(rangeB)))
#define IS_H_CROSS(rangeA, rangeB) is_h_cross(rangeA, rangeB)

#define IS_HSV_RANGES_CROSS(rangeA, rangeB) (IS_H_CROSS(rangeA, rangeB) && IS_S_CROSS(rangeA, rangeB) && IS_V_CROSS(rangeA, rangeB))

#define TRY_EXCLUDE_RANGES(hsv, rangeA, rangeB, excludeResult) exclude_ranges(hsv, rangeA, rangeB, excludeResult)

#define PACK_HSV(hsvMin, hsvMax) pack(hsvMin, hsvMax)

#define IS_H_INVERSE(range) (range.minH > range.maxH)
#define IS_S_INVERSE(range) (range.minS > range.maxS)
#define IS_V_INVERSE(range) (range.minV > range.maxV)

#define EMPTY_RANGE PACK_HSV(float3(0, 0, 0), float3(0, 0, 0))

#define H_WEIGHT(hsv, excludeRange, range) calculate_weight(hsv.x, excludeRange.minH, excludeRange.maxH, range.minH, range.maxH)
#define S_WEIGHT(hsv, excludeRange, range) calculate_weight(hsv.y, excludeRange.minS, excludeRange.maxS, range.minS, range.maxS)
#define V_WEIGHT(hsv, excludeRange, range) calculate_weight(hsv.z, excludeRange.minV, excludeRange.maxV, range.minV, range.maxV)

struct HsvRange
{
    float minH;
    float minS;
    float minV;

    float maxH;
    float maxS;
    float maxV;
};

HsvRange pack(float3 min, float3 max)
{
    HsvRange range;
    
    range.minH = min.x;
    range.minS = min.y;
    range.minV = min.z;
    
    range.maxH = max.x;
    range.maxS = max.y;
    range.maxV = max.z;
    
    return range;
}

bool is_h_cross(HsvRange rangeA, HsvRange rangeB)
{
    // normal case
    if(!IS_H_INVERSE(rangeA) && !IS_H_INVERSE(rangeB))
        return IS_LINEAR_RANGES_CROSS(float2(rangeA.minH, rangeB.minH), float2(rangeA.maxH, rangeB.maxH));

    // rangeA is inverse, rangeB is normal
    if(IS_H_INVERSE(rangeA) && !IS_H_INVERSE(rangeB))
        return IS_LINEAR_RANGES_CROSS(float2(rangeA.minH, rangeB.minH), float2(1.0, rangeB.maxH))
                || IS_LINEAR_RANGES_CROSS(float2(0.0, rangeB.minH), float2(rangeA.maxH, rangeB.maxH));

    // rangeA is normal, rangeB is inverse
    if(!IS_H_INVERSE(rangeA) && IS_H_INVERSE(rangeB))
        return IS_LINEAR_RANGES_CROSS(float2(rangeA.minH, rangeB.minH), float2(rangeA.maxH, 1.0))
                || IS_LINEAR_RANGES_CROSS(float2(rangeA.minH, 0.0), float2(rangeA.maxH, rangeB.maxH));

    // rangeA is inverse, rangeB is inverse
    if(IS_H_INVERSE(rangeA) && IS_H_INVERSE(rangeB))
        return IS_LINEAR_RANGES_CROSS(float2(rangeA.minH, rangeB.minH), float2(1.0, 1.0))
                || IS_LINEAR_RANGES_CROSS(float2(rangeA.minH, 0.0), float2(1.0, rangeB.maxH))
                || IS_LINEAR_RANGES_CROSS(float2(0.0, rangeB.minH), float2(rangeA.maxH, 1.0))
                || IS_LINEAR_RANGES_CROSS(float2(0.0, 0.0), float2(rangeA.maxH, rangeB.maxH));

    return false;
}

bool exclude_ranges(float3 hsv, HsvRange rangeA, HsvRange rangeB, out HsvRange excludeResult)
{
    if(!IS_HSV_RANGES_CROSS(rangeA, rangeB))
    {
        excludeResult = EMPTY_RANGE;
        return false;
    }
    
    excludeResult.minV = max(MIN_V(rangeA), MIN_V(rangeB));
    excludeResult.maxV = min(MAX_V(rangeA), MAX_V(rangeB));

    excludeResult.minS = max(MIN_S(rangeA), MIN_S(rangeB));
    excludeResult.maxS = min(MAX_S(rangeA), MAX_S(rangeB));

    // rangeA is normal, rangeB is normal
    if(!IS_H_INVERSE(rangeA) && !IS_H_INVERSE(rangeB))
    {
        excludeResult.minH = max(rangeA.minH, rangeB.minH);
        excludeResult.maxH = min(rangeA.maxH, rangeB.maxH);

        return IS_RANGE_INCLUDE(hsv, excludeResult);
    }

    // rangeA is inverse, rangeB is normal
    if(IS_H_INVERSE(rangeA) && !IS_H_INVERSE(rangeB))
    {
        if(IS_LINEAR_RANGES_CROSS(float2(rangeA.minH, rangeB.minH), float2(1.0, rangeB.maxH)))
        {
            excludeResult.minH = max(rangeA.minH, rangeB.minH);
            excludeResult.maxH = min(1.0, rangeB.maxH);

            if(IS_RANGE_INCLUDE(hsv, excludeResult))
                return true;
        }

        if(IS_LINEAR_RANGES_CROSS(float2(0.0, rangeB.minH), float2(rangeA.maxH, rangeB.maxH)))
        {
            excludeResult.minH = max(0.0, rangeB.minH);
            excludeResult.maxH = min(rangeA.maxH, rangeB.maxH);

            if(IS_RANGE_INCLUDE(hsv, excludeResult))
                return true;
        }
    }
    
    // rangeA is normal, rangeB is inverse
    if(!IS_H_INVERSE(rangeA) && IS_H_INVERSE(rangeB))
    {
        if(IS_LINEAR_RANGES_CROSS(float2(rangeA.minH, rangeB.minH), float2(rangeA.maxH, 1.0)))
        {
            excludeResult.minH = max(rangeA.minH, rangeB.minH);
            excludeResult.maxH = min(rangeA.maxH, 1.0);

            if(IS_RANGE_INCLUDE(hsv, excludeResult))
                return true;
        }

        if(IS_LINEAR_RANGES_CROSS(float2(rangeA.minH, 0.0), float2(rangeA.maxH, rangeB.maxH)))
        {
            excludeResult.minH = max(rangeA.minH, 0.0);
            excludeResult.maxH = min(rangeA.maxH, rangeB.maxH);

            if(IS_RANGE_INCLUDE(hsv, excludeResult))
                return true;
        }
    }

    // rangeA is inverse, rangeB is inverse
    if(IS_H_INVERSE(rangeA) && IS_H_INVERSE(rangeB))
    {
        if(IS_LINEAR_RANGES_CROSS(float2(rangeA.minH, rangeB.minH), float2(1.0, 1.0)))
        {
            excludeResult.minH = max(rangeA.minH, rangeB.minH);
            excludeResult.maxH = 1.0;

            if(IS_LINEAR_RANGES_CROSS(float2(0.0, 0.0), float2(rangeA.maxH, rangeB.maxH)))
            {
                excludeResult.minH = max(excludeResult.minH, 0.0);
                excludeResult.maxH = min(excludeResult.maxH, min(rangeA.maxH, rangeB.maxH));
            }

            if(IS_RANGE_INCLUDE(hsv, excludeResult))
                return true;
        }

        if(IS_LINEAR_RANGES_CROSS(float2(rangeA.minH, 0.0), float2(1.0, rangeB.maxH)))
        {
            excludeResult.minH = max(rangeA.minH, 0.0);
            excludeResult.maxH = min(1.0, rangeB.maxH);

            if(IS_RANGE_INCLUDE(hsv, excludeResult))
                return true;
        }

        if(IS_LINEAR_RANGES_CROSS(float2(0.0, rangeB.minH), float2(rangeA.maxH, 1.0)))
        {
            excludeResult.minH = max(0.0, rangeB.minH);
            excludeResult.maxH = min(rangeA.maxH, 1.0);

            if(IS_RANGE_INCLUDE(hsv, excludeResult))
                return true;
        }
    }

    excludeResult = EMPTY_RANGE;
    
    return false;
}

float calculate_weight(float value, float excludeMin, float excludeMax, float rangeMin, float rangeMax)
{
    float distance = abs(excludeMax - excludeMin);
    if(excludeMin > excludeMax)
        distance = abs(1.0 - excludeMin + excludeMax);

    float delta = 0.0;
    if(rangeMin < excludeMin || rangeMin > excludeMin)
    {
        delta = max(excludeMin > excludeMax && value >= excludeMin && value <= 1.0 ? abs(1.0 - value + excludeMax) : abs(excludeMax - value), delta);
    }

    if(rangeMax > excludeMax || rangeMax < excludeMax)
    {
        delta = excludeMin > excludeMax ? max(value >= excludeMin && value <= 1.0 ? abs(value - excludeMin) : abs(1.0 - excludeMin + value), delta) : max(abs(excludeMin - value), delta);
    }

    if(rangeMin == excludeMin && rangeMax == excludeMax)
    {
        float delta1 = excludeMin > excludeMax && value >= excludeMin && value <= 1.0 ? abs(1.0 - value + excludeMax) : abs(excludeMax - value);
        float delta2 = excludeMin > excludeMax ? value >= excludeMin && value <= 1.0 ? abs(value - excludeMin) : abs(1.0 - excludeMin + value) : abs(excludeMin - value);

        delta = min(delta1, delta2);
    }

    return delta / distance;
}

float calculate_weight_over_range(float3 hsv, HsvRange excludeRange, HsvRange range)
{
    float hWeight = H_WEIGHT(hsv, excludeRange, range);
    float sWeight = S_WEIGHT(hsv, excludeRange, range);
    float vWeight = V_WEIGHT(hsv, excludeRange, range);

    float aWeight = hWeight * sWeight * vWeight;
    float bWeight = (1.0 - hWeight) * (1.0 - sWeight) * (1.0 - vWeight);

    if(aWeight == 0.0 && bWeight == 0.0)
    {
        aWeight = FLT_EPS;
        bWeight = FLT_EPS;
    }

    return aWeight / (aWeight + bWeight);
}

float calculate_weight(float3 hsv, HsvRange rangeA, HsvRange rangeB)
{
    float weightA = 0.0;

    if(IS_RANGE_INCLUDE(hsv, rangeA))
    {
        weightA = 1.0;
    }

    if(IS_RANGE_INCLUDE(hsv, rangeB))
    {
        weightA = 0.0;
    }

    HsvRange excludeRange;
    if(TRY_EXCLUDE_RANGES(hsv, rangeA, rangeB, excludeRange))
    {
        weightA = calculate_weight_over_range(hsv, excludeRange, rangeA);
    }
                
    return weightA;
}

// hsv to cartesian space
float3 hsv_to_linear_space(float3 hsv)
{
    hsv.x = hsv.x * TWO_PI;

    float h = sin(hsv.x) * hsv.y * hsv.z;
    float s = cos(hsv.x) * hsv.y * hsv.z;
    float v = hsv.z;

    return float3(h, v, -s);
}

#endif