#ifndef BLEND_INCLUDED
#define BLEND_INCLUDED

float4 blend_burn(float4 base, float4 blend, float opacity)
{
    float4 result = 1.0 - (1.0 - blend) / base;
    return lerp(base, result, opacity);
}

float4 blend_darken(float4 base, float4 blend, float opacity)
{
    float4 result = min(blend, base);
    return lerp(base, result, opacity);
}

float4 blend_difference(float4 base, float4 blend, float opacity)
{
    float4 result = abs(blend - base);
    return lerp(base, result, opacity);
}

float4 blend_dodge(float4 base, float4 blend, float opacity)
{
    float4 result = base / (1.0 - blend);
    return lerp(base, result, opacity);
}

float4 blend_divide(float4 base, float4 blend, float opacity)
{
    float4 result = base / (blend + 0.000000000001);
    return lerp(base, result, opacity);
}

float4 blend_exclusion(float4 base, float4 blend, float opacity)
{
    float4 result = blend + base - (2.0 * blend * base);
    return lerp(base, result, opacity);
}

float4 blend_hardlight(float4 base, float4 blend, float opacity)
{
    float4 result1 = 1.0 - 2.0 * (1.0 - base) * (1.0 - blend);
    float4 result2 = 2.0 * base * blend;
    float4 zeroOrOne = step(blend, 0.5);

    float4 result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(blend, result, opacity);
}

float4 blend_hard_mix(float4 base, float4 blend, float opacity)
{
    float4 result = step(1.0 - base, blend);
    return lerp(base, result, opacity);
}

float4 blend_lighten(float4 base, float4 blend, float opacity)
{
    float4 result = max(blend, base);
    return lerp(base, result, opacity);
}

float4 blend_linear_burn(float4 base, float4 blend, float opacity)
{
    float4 result = base + blend - 1.0;
    return lerp(base, result, opacity);
}

float4 blend_linear_dodge(float4 base, float4 blend, float opacity)
{
    float4 result = base + blend;
    return lerp(base, result, opacity);
}

float4 blend_linear_light(float4 base, float4 blend, float opacity)
{
    float4 result = blend < 0.5 ? max(base + (2.0 * blend) - 1, 0) : min(base + 2.0 * (blend - 0.5), 1);
    return lerp(base, result, opacity);
}

float4 blend_linear_light_add_sub(float4 base, float4 blend, float opacity)
{
    float4 result = blend + 2.0 * base - 1.0;
    return lerp(base, result, opacity);
}

float4 blend_multiply(float4 base, float4 blend, float opacity)
{
    float4 result = base * blend;
    return lerp(base, result, opacity);
}

float4 blend_negation(float4 base, float4 blend, float opacity)
{
    float4 result = 1.0 - abs(1.0 - blend - base);
    return lerp(base, result, opacity);
}

float4 blend_overlay(float4 base, float4 blend, float opacity)
{
    float4 result1 = 1.0 - 2.0 * (1.0 - base) * (1.0 - blend);
    float4 result2 = 2.0 * base * blend;
    float4 zeroOrOne = step(base, 0.5);
    float4 result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(base, result, opacity);
}

float4 blend_pin_light(float4 base, float4 blend, float opacity)
{
    float4 check = step(0.5, blend);
    float4 result1 = check * max(2.0 * (base - 0.5), blend);
    float4 result = result1 + (1.0 - check) * min(2.0 * base, blend);
    return lerp(base, result, opacity);
}

float4 blend_screen(float4 base, float4 blend, float opacity)
{
    float4 result = 1.0 - (1.0 - blend) * (1.0 - base);
    return lerp(base, result, opacity);
}

float4 blend_soft_light(float4 base, float4 blend, float opacity)
{
    float4 result1 = 2.0 * base * blend + base * base * (1.0 - 2.0 * blend);
    float4 result2 = sqrt(base) * (2.0 * blend - 1.0) + 2.0 * base * (1.0 - blend);
    float4 zeroOrOne = step(0.5, blend);
    float4 result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(base, result, opacity);
}

float4 blend_subtract(float4 base, float4 blend, float opacity)
{
    float4 result = base - blend;
    return lerp(base, result, opacity);
}

float4 blend_vivid_light(float4 base, float4 blend, float opacity)
{
    float4 result1 = 1.0 - (1.0 - blend) / (2.0 * base);
    float4 result2 = blend / (2.0 * (1.0 - base));
    float4 zeroOrOne = step(0.5, base);
    float4 result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(base, result, opacity);
}

float4 blend_overwrite(float4 base, float4 blend, float opacity)
{
    return lerp(base, blend, opacity);
}

float3 blend_burn(float3 base, float3 blend, float opacity)
{
    float3 result = 1.0 - (1.0 - blend) / base;
    return lerp(base, result, opacity);
}

float3 blend_darken(float3 base, float3 blend, float opacity)
{
    float3 result = min(blend, base);
    return lerp(base, result, opacity);
}

float3 blend_difference(float3 base, float3 blend, float opacity)
{
    float3 result = abs(blend - base);
    return lerp(base, result, opacity);
}

float3 blend_dodge(float3 base, float3 blend, float opacity)
{
    float3 result = base / (1.0 - blend);
    return lerp(base, result, opacity);
}

float3 blend_divide(float3 base, float3 blend, float opacity)
{
    float3 result = base / (blend + 0.000000000001);
    return lerp(base, result, opacity);
}

float3 blend_exclusion(float3 base, float3 blend, float opacity)
{
    float3 result = blend + base - (2.0 * blend * base);
    return lerp(base, result, opacity);
}

float3 blend_hardlight(float3 base, float3 blend, float opacity)
{
    float3 result1 = 1.0 - 2.0 * (1.0 - base) * (1.0 - blend);
    float3 result2 = 2.0 * base * blend;
    float3 zeroOrOne = step(blend, 0.5);

    float3 result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(blend, result, opacity);
}

float3 blend_hard_mix(float3 base, float3 blend, float opacity)
{
    float3 result = step(1.0 - base, blend);
    return lerp(base, result, opacity);
}

float3 blend_lighten(float3 base, float3 blend, float opacity)
{
    float3 result = max(blend, base);
    return lerp(base, result, opacity);
}

float3 blend_linear_burn(float3 base, float3 blend, float opacity)
{
    float3 result = base + blend - 1.0;
    return lerp(base, result, opacity);
}

float3 blend_linear_dodge(float3 base, float3 blend, float opacity)
{
    float3 result = base + blend;
    return lerp(base, result, opacity);
}

float3 blend_linear_light(float3 base, float3 blend, float opacity)
{
    float3 result = blend < 0.5 ? max(base + (2.0 * blend) - 1, 0) : min(base + 2.0 * (blend - 0.5), 1);
    return lerp(base, result, opacity);
}

float3 blend_linear_light_add_sub(float3 base, float3 blend, float opacity)
{
    float3 result = blend + 2.0 * base - 1.0;
    return lerp(base, result, opacity);
}

float3 blend_multiply(float3 base, float3 blend, float opacity)
{
    float3 result = base * blend;
    return lerp(base, result, opacity);
}

float3 blend_negation(float3 base, float3 blend, float opacity)
{
    float3 result = 1.0 - abs(1.0 - blend - base);
    return lerp(base, result, opacity);
}

float3 blend_overlay(float3 base, float3 blend, float opacity)
{
    float3 result1 = 1.0 - 2.0 * (1.0 - base) * (1.0 - blend);
    float3 result2 = 2.0 * base * blend;
    float3 zeroOrOne = step(base, 0.5);
    float3 result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(base, result, opacity);
}

float3 blend_pin_light(float3 base, float3 blend, float opacity)
{
    float3 check = step(0.5, blend);
    float3 result1 = check * max(2.0 * (base - 0.5), blend);
    float3 result = result1 + (1.0 - check) * min(2.0 * base, blend);
    return lerp(base, result, opacity);
}

float3 blend_screen(float3 base, float3 blend, float opacity)
{
    float3 result = 1.0 - (1.0 - blend) * (1.0 - base);
    return lerp(base, result, opacity);
}

float3 blend_soft_light(float3 base, float3 blend, float opacity)
{
    float3 result1 = 2.0 * base * blend + base * base * (1.0 - 2.0 * blend);
    float3 result2 = sqrt(base) * (2.0 * blend - 1.0) + 2.0 * base * (1.0 - blend);
    float3 zeroOrOne = step(0.5, blend);
    float3 result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(base, result, opacity);
}

float3 blend_subtract(float3 base, float3 blend, float opacity)
{
    float3 result = base - blend;
    return lerp(base, result, opacity);
}

float3 blend_vivid_light(float3 base, float3 blend, float opacity)
{
    float3 result1 = 1.0 - (1.0 - blend) / (2.0 * base);
    float3 result2 = blend / (2.0 * (1.0 - base));
    float3 zeroOrOne = step(0.5, base);
    float3 result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(base, result, opacity);
}

float3 blend_overwrite(float3 base, float3 blend, float opacity)
{
    return lerp(base, blend, opacity);
}

float2 blend_burn(float2 base, float2 blend, float opacity)
{
    float2 result = 1.0 - (1.0 - blend) / base;
    return lerp(base, result, opacity);
}

float2 blend_darken(float2 base, float2 blend, float opacity)
{
    float2 result = min(blend, base);
    return lerp(base, result, opacity);
}

float2 blend_difference(float2 base, float2 blend, float opacity)
{
    float2 result = abs(blend - base);
    return lerp(base, result, opacity);
}

float2 blend_dodge(float2 base, float2 blend, float opacity)
{
    float2 result = base / (1.0 - blend);
    return lerp(base, result, opacity);
}

float2 blend_divide(float2 base, float2 blend, float opacity)
{
    float2 result = base / (blend + 0.000000000001);
    return lerp(base, result, opacity);
}

float2 blend_exclusion(float2 base, float2 blend, float opacity)
{
    float2 result = blend + base - (2.0 * blend * base);
    return lerp(base, result, opacity);
}

float2 blend_hardlight(float2 base, float2 blend, float opacity)
{
    float2 result1 = 1.0 - 2.0 * (1.0 - base) * (1.0 - blend);
    float2 result2 = 2.0 * base * blend;
    float2 zeroOrOne = step(blend, 0.5);

    float2 result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(blend, result, opacity);
}

float2 blend_hard_mix(float2 base, float2 blend, float opacity)
{
    float2 result = step(1.0 - base, blend);
    return lerp(base, result, opacity);
}

float2 blend_lighten(float2 base, float2 blend, float opacity)
{
    float2 result = max(blend, base);
    return lerp(base, result, opacity);
}

float2 blend_linear_burn(float2 base, float2 blend, float opacity)
{
    float2 result = base + blend - 1.0;
    return lerp(base, result, opacity);
}

float2 blend_linear_dodge(float2 base, float2 blend, float opacity)
{
    float2 result = base + blend;
    return lerp(base, result, opacity);
}

float2 blend_linear_light(float2 base, float2 blend, float opacity)
{
    float2 result = blend < 0.5 ? max(base + (2.0 * blend) - 1, 0) : min(base + 2.0 * (blend - 0.5), 1);
    return lerp(base, result, opacity);
}

float2 blend_linear_light_add_sub(float2 base, float2 blend, float opacity)
{
    float2 result = blend + 2.0 * base - 1.0;
    return lerp(base, result, opacity);
}

float2 blend_multiply(float2 base, float2 blend, float opacity)
{
    float2 result = base * blend;
    return lerp(base, result, opacity);
}

float2 blend_negation(float2 base, float2 blend, float opacity)
{
    float2 result = 1.0 - abs(1.0 - blend - base);
    return lerp(base, result, opacity);
}

float2 blend_overlay(float2 base, float2 blend, float opacity)
{
    float2 result1 = 1.0 - 2.0 * (1.0 - base) * (1.0 - blend);
    float2 result2 = 2.0 * base * blend;
    float2 zeroOrOne = step(base, 0.5);
    float2 result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(base, result, opacity);
}

float2 blend_pin_light(float2 base, float2 blend, float opacity)
{
    float2 check = step(0.5, blend);
    float2 result1 = check * max(2.0 * (base - 0.5), blend);
    float2 result = result1 + (1.0 - check) * min(2.0 * base, blend);
    return lerp(base, result, opacity);
}

float2 blend_screen(float2 base, float2 blend, float opacity)
{
    float2 result = 1.0 - (1.0 - blend) * (1.0 - base);
    return lerp(base, result, opacity);
}

float2 blend_soft_light(float2 base, float2 blend, float opacity)
{
    float2 result1 = 2.0 * base * blend + base * base * (1.0 - 2.0 * blend);
    float2 result2 = sqrt(base) * (2.0 * blend - 1.0) + 2.0 * base * (1.0 - blend);
    float2 zeroOrOne = step(0.5, blend);
    float2 result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(base, result, opacity);
}

float2 blend_subtract(float2 base, float2 blend, float opacity)
{
    float2 result = base - blend;
    return lerp(base, result, opacity);
}

float2 blend_vivid_light(float2 base, float2 blend, float opacity)
{
    float2 result1 = 1.0 - (1.0 - blend) / (2.0 * base);
    float2 result2 = blend / (2.0 * (1.0 - base));
    float2 zeroOrOne = step(0.5, base);
    float2 result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(base, result, opacity);
}

float2 blend_overwrite(float2 base, float2 blend, float opacity)
{
    return lerp(base, blend, opacity);
}

float blend_burn(float base, float blend, float opacity)
{
    float result = 1.0 - (1.0 - blend) / base;
    return lerp(base, result, opacity);
}

float blend_darken(float base, float blend, float opacity)
{
    float result = min(blend, base);
    return lerp(base, result, opacity);
}

float blend_difference(float base, float blend, float opacity)
{
    float result = abs(blend - base);
    return lerp(base, result, opacity);
}

float blend_dodge(float base, float blend, float opacity)
{
    float result = base / (1.0 - blend);
    return lerp(base, result, opacity);
}

float blend_divide(float base, float blend, float opacity)
{
    float result = base / (blend + 0.000000000001);
    return lerp(base, result, opacity);
}

float blend_exclusion(float base, float blend, float opacity)
{
    float result = blend + base - (2.0 * blend * base);
    return lerp(base, result, opacity);
}

float blend_hardlight(float base, float blend, float opacity)
{
    float result1 = 1.0 - 2.0 * (1.0 - base) * (1.0 - blend);
    float result2 = 2.0 * base * blend;
    float zeroOrOne = step(blend, 0.5);

    float result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(blend, result, opacity);
}

float blend_hard_mix(float base, float blend, float opacity)
{
    float result = step(1.0 - base, blend);
    return lerp(base, result, opacity);
}

float blend_lighten(float base, float blend, float opacity)
{
    float result = max(blend, base);
    return lerp(base, result, opacity);
}

float blend_linear_burn(float base, float blend, float opacity)
{
    float result = base + blend - 1.0;
    return lerp(base, result, opacity);
}

float blend_linear_dodge(float base, float blend, float opacity)
{
    float result = base + blend;
    return lerp(base, result, opacity);
}

float blend_linear_light(float base, float blend, float opacity)
{
    float result = blend < 0.5 ? max(base + (2.0 * blend) - 1, 0) : min(base + 2.0 * (blend - 0.5), 1);
    return lerp(base, result, opacity);
}

float blend_linear_light_add_sub(float base, float blend, float opacity)
{
    float result = blend + 2.0 * base - 1.0;
    return lerp(base, result, opacity);
}

float blend_multiply(float base, float blend, float opacity)
{
    float result = base * blend;
    return lerp(base, result, opacity);
}

float blend_negation(float base, float blend, float opacity)
{
    float result = 1.0 - abs(1.0 - blend - base);
    return lerp(base, result, opacity);
}

float blend_overlay(float base, float blend, float opacity)
{
    float result1 = 1.0 - 2.0 * (1.0 - base) * (1.0 - blend);
    float result2 = 2.0 * base * blend;
    float zeroOrOne = step(base, 0.5);
    float result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(base, result, opacity);
}

float blend_pin_light(float base, float blend, float opacity)
{
    float check = step(0.5, blend);
    float result1 = check * max(2.0 * (base - 0.5), blend);
    float result = result1 + (1.0 - check) * min(2.0 * base, blend);
    return lerp(base, result, opacity);
}

float blend_screen(float base, float blend, float opacity)
{
    float result = 1.0 - (1.0 - blend) * (1.0 - base);
    return lerp(base, result, opacity);
}

float blend_soft_light(float base, float blend, float opacity)
{
    float result1 = 2.0 * base * blend + base * base * (1.0 - 2.0 * blend);
    float result2 = sqrt(base) * (2.0 * blend - 1.0) + 2.0 * base * (1.0 - blend);
    float zeroOrOne = step(0.5, blend);
    float result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(base, result, opacity);
}

float blend_subtract(float base, float blend, float opacity)
{
    float result = base - blend;
    return lerp(base, result, opacity);
}

float blend_vivid_light(float base, float blend, float opacity)
{
    float result1 = 1.0 - (1.0 - blend) / (2.0 * base);
    float result2 = blend / (2.0 * (1.0 - base));
    float zeroOrOne = step(0.5, base);
    float result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(base, result, opacity);
}

float blend_overwrite(float base, float blend, float opacity)
{
    return lerp(base, blend, opacity);
}

half4 blend_burn(half4 base, half4 blend, float opacity)
{
    half4 result = 1.0 - (1.0 - blend) / base;
    return lerp(base, result, opacity);
}

half4 blend_darken(half4 base, half4 blend, float opacity)
{
    half4 result = min(blend, base);
    return lerp(base, result, opacity);
}

half4 blend_difference(half4 base, half4 blend, float opacity)
{
    half4 result = abs(blend - base);
    return lerp(base, result, opacity);
}

half4 blend_dodge(half4 base, half4 blend, float opacity)
{
    half4 result = base / (1.0 - blend);
    return lerp(base, result, opacity);
}

half4 blend_divide(half4 base, half4 blend, float opacity)
{
    half4 result = base / (blend + 0.000000000001);
    return lerp(base, result, opacity);
}

half4 blend_exclusion(half4 base, half4 blend, float opacity)
{
    half4 result = blend + base - (2.0 * blend * base);
    return lerp(base, result, opacity);
}

half4 blend_hardlight(half4 base, half4 blend, float opacity)
{
    half4 result1 = 1.0 - 2.0 * (1.0 - base) * (1.0 - blend);
    half4 result2 = 2.0 * base * blend;
    half4 zeroOrOne = step(blend, 0.5);

    half4 result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(blend, result, opacity);
}

half4 blend_hard_mix(half4 base, half4 blend, float opacity)
{
    half4 result = step(1.0 - base, blend);
    return lerp(base, result, opacity);
}

half4 blend_lighten(half4 base, half4 blend, float opacity)
{
    half4 result = max(blend, base);
    return lerp(base, result, opacity);
}

half4 blend_linear_burn(half4 base, half4 blend, float opacity)
{
    half4 result = base + blend - 1.0;
    return lerp(base, result, opacity);
}

half4 blend_linear_dodge(half4 base, half4 blend, float opacity)
{
    half4 result = base + blend;
    return lerp(base, result, opacity);
}

half4 blend_linear_light(half4 base, half4 blend, float opacity)
{
    half4 result = blend < 0.5 ? max(base + (2.0 * blend) - 1, 0) : min(base + 2.0 * (blend - 0.5), 1);
    return lerp(base, result, opacity);
}

half4 blend_linear_light_add_sub(half4 base, half4 blend, float opacity)
{
    half4 result = blend + 2.0 * base - 1.0;
    return lerp(base, result, opacity);
}

half4 blend_multiply(half4 base, half4 blend, float opacity)
{
    half4 result = base * blend;
    return lerp(base, result, opacity);
}

half4 blend_negation(half4 base, half4 blend, float opacity)
{
    half4 result = 1.0 - abs(1.0 - blend - base);
    return lerp(base, result, opacity);
}

half4 blend_overlay(half4 base, half4 blend, float opacity)
{
    half4 result1 = 1.0 - 2.0 * (1.0 - base) * (1.0 - blend);
    half4 result2 = 2.0 * base * blend;
    half4 zeroOrOne = step(base, 0.5);
    half4 result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(base, result, opacity);
}

half4 blend_pin_light(half4 base, half4 blend, float opacity)
{
    half4 check = step(0.5, blend);
    half4 result1 = check * max(2.0 * (base - 0.5), blend);
    half4 result = result1 + (1.0 - check) * min(2.0 * base, blend);
    return lerp(base, result, opacity);
}

half4 blend_screen(half4 base, half4 blend, float opacity)
{
    half4 result = 1.0 - (1.0 - blend) * (1.0 - base);
    return lerp(base, result, opacity);
}

half4 blend_soft_light(half4 base, half4 blend, float opacity)
{
    half4 result1 = 2.0 * base * blend + base * base * (1.0 - 2.0 * blend);
    half4 result2 = sqrt(base) * (2.0 * blend - 1.0) + 2.0 * base * (1.0 - blend);
    half4 zeroOrOne = step(0.5, blend);
    half4 result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(base, result, opacity);
}

half4 blend_subtract(half4 base, half4 blend, float opacity)
{
    half4 result = base - blend;
    return lerp(base, result, opacity);
}

half4 blend_vivid_light(half4 base, half4 blend, float opacity)
{
    half4 result1 = 1.0 - (1.0 - blend) / (2.0 * base);
    half4 result2 = blend / (2.0 * (1.0 - base));
    half4 zeroOrOne = step(0.5, base);
    half4 result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(base, result, opacity);
}

half4 blend_overwrite(half4 base, half4 blend, float opacity)
{
    return lerp(base, blend, opacity);
}

half3 blend_burn(half3 base, half3 blend, float opacity)
{
    half3 result = 1.0 - (1.0 - blend) / base;
    return lerp(base, result, opacity);
}

half3 blend_darken(half3 base, half3 blend, float opacity)
{
    half3 result = min(blend, base);
    return lerp(base, result, opacity);
}

half3 blend_difference(half3 base, half3 blend, float opacity)
{
    half3 result = abs(blend - base);
    return lerp(base, result, opacity);
}

half3 blend_dodge(half3 base, half3 blend, float opacity)
{
    half3 result = base / (1.0 - blend);
    return lerp(base, result, opacity);
}

half3 blend_divide(half3 base, half3 blend, float opacity)
{
    half3 result = base / (blend + 0.000000000001);
    return lerp(base, result, opacity);
}

half3 blend_exclusion(half3 base, half3 blend, float opacity)
{
    half3 result = blend + base - (2.0 * blend * base);
    return lerp(base, result, opacity);
}

half3 blend_hardlight(half3 base, half3 blend, float opacity)
{
    half3 result1 = 1.0 - 2.0 * (1.0 - base) * (1.0 - blend);
    half3 result2 = 2.0 * base * blend;
    half3 zeroOrOne = step(blend, 0.5);

    half3 result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(blend, result, opacity);
}

half3 blend_hard_mix(half3 base, half3 blend, float opacity)
{
    half3 result = step(1.0 - base, blend);
    return lerp(base, result, opacity);
}

half3 blend_lighten(half3 base, half3 blend, float opacity)
{
    half3 result = max(blend, base);
    return lerp(base, result, opacity);
}

half3 blend_linear_burn(half3 base, half3 blend, float opacity)
{
    half3 result = base + blend - 1.0;
    return lerp(base, result, opacity);
}

half3 blend_linear_dodge(half3 base, half3 blend, float opacity)
{
    half3 result = base + blend;
    return lerp(base, result, opacity);
}

half3 blend_linear_light(half3 base, half3 blend, float opacity)
{
    half3 result = blend < 0.5 ? max(base + (2.0 * blend) - 1, 0) : min(base + 2.0 * (blend - 0.5), 1);
    return lerp(base, result, opacity);
}

half3 blend_linear_light_add_sub(half3 base, half3 blend, float opacity)
{
    half3 result = blend + 2.0 * base - 1.0;
    return lerp(base, result, opacity);
}

half3 blend_multiply(half3 base, half3 blend, float opacity)
{
    half3 result = base * blend;
    return lerp(base, result, opacity);
}

half3 blend_negation(half3 base, half3 blend, float opacity)
{
    half3 result = 1.0 - abs(1.0 - blend - base);
    return lerp(base, result, opacity);
}

half3 blend_overlay(half3 base, half3 blend, float opacity)
{
    half3 result1 = 1.0 - 2.0 * (1.0 - base) * (1.0 - blend);
    half3 result2 = 2.0 * base * blend;
    half3 zeroOrOne = step(base, 0.5);
    half3 result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(base, result, opacity);
}

half3 blend_pin_light(half3 base, half3 blend, float opacity)
{
    half3 check = step(0.5, blend);
    half3 result1 = check * max(2.0 * (base - 0.5), blend);
    half3 result = result1 + (1.0 - check) * min(2.0 * base, blend);
    return lerp(base, result, opacity);
}

half3 blend_screen(half3 base, half3 blend, float opacity)
{
    half3 result = 1.0 - (1.0 - blend) * (1.0 - base);
    return lerp(base, result, opacity);
}

half3 blend_soft_light(half3 base, half3 blend, float opacity)
{
    half3 result1 = 2.0 * base * blend + base * base * (1.0 - 2.0 * blend);
    half3 result2 = sqrt(base) * (2.0 * blend - 1.0) + 2.0 * base * (1.0 - blend);
    half3 zeroOrOne = step(0.5, blend);
    half3 result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(base, result, opacity);
}

half3 blend_subtract(half3 base, half3 blend, float opacity)
{
    half3 result = base - blend;
    return lerp(base, result, opacity);
}

half3 blend_vivid_light(half3 base, half3 blend, float opacity)
{
    half3 result1 = 1.0 - (1.0 - blend) / (2.0 * base);
    half3 result2 = blend / (2.0 * (1.0 - base));
    half3 zeroOrOne = step(0.5, base);
    half3 result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(base, result, opacity);
}

half3 blend_overwrite(half3 base, half3 blend, float opacity)
{
    return lerp(base, blend, opacity);
}

half2 blend_burn(half2 base, half2 blend, float opacity)
{
    half2 result = 1.0 - (1.0 - blend) / base;
    return lerp(base, result, opacity);
}

half2 blend_darken(half2 base, half2 blend, float opacity)
{
    half2 result = min(blend, base);
    return lerp(base, result, opacity);
}

half2 blend_difference(half2 base, half2 blend, float opacity)
{
    half2 result = abs(blend - base);
    return lerp(base, result, opacity);
}

half2 blend_dodge(half2 base, half2 blend, float opacity)
{
    half2 result = base / (1.0 - blend);
    return lerp(base, result, opacity);
}

half2 blend_divide(half2 base, half2 blend, float opacity)
{
    half2 result = base / (blend + 0.000000000001);
    return lerp(base, result, opacity);
}

half2 blend_exclusion(half2 base, half2 blend, float opacity)
{
    half2 result = blend + base - (2.0 * blend * base);
    return lerp(base, result, opacity);
}

half2 blend_hardlight(half2 base, half2 blend, float opacity)
{
    half2 result1 = 1.0 - 2.0 * (1.0 - base) * (1.0 - blend);
    half2 result2 = 2.0 * base * blend;
    half2 zeroOrOne = step(blend, 0.5);

    half2 result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(blend, result, opacity);
}

half2 blend_hard_mix(half2 base, half2 blend, float opacity)
{
    half2 result = step(1.0 - base, blend);
    return lerp(base, result, opacity);
}

half2 blend_lighten(half2 base, half2 blend, float opacity)
{
    half2 result = max(blend, base);
    return lerp(base, result, opacity);
}

half2 blend_linear_burn(half2 base, half2 blend, float opacity)
{
    half2 result = base + blend - 1.0;
    return lerp(base, result, opacity);
}

half2 blend_linear_dodge(half2 base, half2 blend, float opacity)
{
    half2 result = base + blend;
    return lerp(base, result, opacity);
}

half2 blend_linear_light(half2 base, half2 blend, float opacity)
{
    half2 result = blend < 0.5 ? max(base + (2.0 * blend) - 1, 0) : min(base + 2.0 * (blend - 0.5), 1);
    return lerp(base, result, opacity);
}

half2 blend_linear_light_add_sub(half2 base, half2 blend, float opacity)
{
    half2 result = blend + 2.0 * base - 1.0;
    return lerp(base, result, opacity);
}

half2 blend_multiply(half2 base, half2 blend, float opacity)
{
    half2 result = base * blend;
    return lerp(base, result, opacity);
}

half2 blend_negation(half2 base, half2 blend, float opacity)
{
    half2 result = 1.0 - abs(1.0 - blend - base);
    return lerp(base, result, opacity);
}

half2 blend_overlay(half2 base, half2 blend, float opacity)
{
    half2 result1 = 1.0 - 2.0 * (1.0 - base) * (1.0 - blend);
    half2 result2 = 2.0 * base * blend;
    half2 zeroOrOne = step(base, 0.5);
    half2 result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(base, result, opacity);
}

half2 blend_pin_light(half2 base, half2 blend, float opacity)
{
    half2 check = step(0.5, blend);
    half2 result1 = check * max(2.0 * (base - 0.5), blend);
    half2 result = result1 + (1.0 - check) * min(2.0 * base, blend);
    return lerp(base, result, opacity);
}

half2 blend_screen(half2 base, half2 blend, float opacity)
{
    half2 result = 1.0 - (1.0 - blend) * (1.0 - base);
    return lerp(base, result, opacity);
}

half2 blend_soft_light(half2 base, half2 blend, float opacity)
{
    half2 result1 = 2.0 * base * blend + base * base * (1.0 - 2.0 * blend);
    half2 result2 = sqrt(base) * (2.0 * blend - 1.0) + 2.0 * base * (1.0 - blend);
    half2 zeroOrOne = step(0.5, blend);
    half2 result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(base, result, opacity);
}

half2 blend_subtract(half2 base, half2 blend, float opacity)
{
    half2 result = base - blend;
    return lerp(base, result, opacity);
}

half2 blend_vivid_light(half2 base, half2 blend, float opacity)
{
    half2 result1 = 1.0 - (1.0 - blend) / (2.0 * base);
    half2 result2 = blend / (2.0 * (1.0 - base));
    half2 zeroOrOne = step(0.5, base);
    half2 result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(base, result, opacity);
}

half2 blend_overwrite(half2 base, half2 blend, float opacity)
{
    return lerp(base, blend, opacity);
}

half blend_burn(half base, half blend, float opacity)
{
    half result = 1.0 - (1.0 - blend) / base;
    return lerp(base, result, opacity);
}

half blend_darken(half base, half blend, float opacity)
{
    half result = min(blend, base);
    return lerp(base, result, opacity);
}

half blend_difference(half base, half blend, float opacity)
{
    half result = abs(blend - base);
    return lerp(base, result, opacity);
}

half blend_dodge(half base, half blend, float opacity)
{
    half result = base / (1.0 - blend);
    return lerp(base, result, opacity);
}

half blend_divide(half base, half blend, float opacity)
{
    half result = base / (blend + 0.000000000001);
    return lerp(base, result, opacity);
}

half blend_exclusion(half base, half blend, float opacity)
{
    half result = blend + base - (2.0 * blend * base);
    return lerp(base, result, opacity);
}

half blend_hardlight(half base, half blend, float opacity)
{
    half result1 = 1.0 - 2.0 * (1.0 - base) * (1.0 - blend);
    half result2 = 2.0 * base * blend;
    half zeroOrOne = step(blend, 0.5);

    half result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(blend, result, opacity);
}

half blend_hard_mix(half base, half blend, float opacity)
{
    half result = step(1.0 - base, blend);
    return lerp(base, result, opacity);
}

half blend_lighten(half base, half blend, float opacity)
{
    half result = max(blend, base);
    return lerp(base, result, opacity);
}

half blend_linear_burn(half base, half blend, float opacity)
{
    half result = base + blend - 1.0;
    return lerp(base, result, opacity);
}

half blend_linear_dodge(half base, half blend, float opacity)
{
    half result = base + blend;
    return lerp(base, result, opacity);
}

half blend_linear_light(half base, half blend, float opacity)
{
    half result = blend < 0.5 ? max(base + (2.0 * blend) - 1, 0) : min(base + 2.0 * (blend - 0.5), 1);
    return lerp(base, result, opacity);
}

half blend_linear_light_add_sub(half base, half blend, float opacity)
{
    half result = blend + 2.0 * base - 1.0;
    return lerp(base, result, opacity);
}

half blend_multiply(half base, half blend, float opacity)
{
    half result = base * blend;
    return lerp(base, result, opacity);
}

half blend_negation(half base, half blend, float opacity)
{
    half result = 1.0 - abs(1.0 - blend - base);
    return lerp(base, result, opacity);
}

half blend_overlay(half base, half blend, float opacity)
{
    half result1 = 1.0 - 2.0 * (1.0 - base) * (1.0 - blend);
    half result2 = 2.0 * base * blend;
    half zeroOrOne = step(base, 0.5);
    half result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(base, result, opacity);
}

half blend_pin_light(half base, half blend, float opacity)
{
    half check = step(0.5, blend);
    half result1 = check * max(2.0 * (base - 0.5), blend);
    half result = result1 + (1.0 - check) * min(2.0 * base, blend);
    return lerp(base, result, opacity);
}

half blend_screen(half base, half blend, float opacity)
{
    half result = 1.0 - (1.0 - blend) * (1.0 - base);
    return lerp(base, result, opacity);
}

half blend_soft_light(half base, half blend, float opacity)
{
    half result1 = 2.0 * base * blend + base * base * (1.0 - 2.0 * blend);
    half result2 = sqrt(base) * (2.0 * blend - 1.0) + 2.0 * base * (1.0 - blend);
    half zeroOrOne = step(0.5, blend);
    half result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(base, result, opacity);
}

half blend_subtract(half base, half blend, float opacity)
{
    half result = base - blend;
    return lerp(base, result, opacity);
}

half blend_vivid_light(half base, half blend, float opacity)
{
    half result1 = 1.0 - (1.0 - blend) / (2.0 * base);
    half result2 = blend / (2.0 * (1.0 - base));
    half zeroOrOne = step(0.5, base);
    half result = result2 * zeroOrOne + (1.0 - zeroOrOne) * result1;
    return lerp(base, result, opacity);
}

half blend_overwrite(half base, half blend, float opacity)
{
    return lerp(base, blend, opacity);
}

#endif