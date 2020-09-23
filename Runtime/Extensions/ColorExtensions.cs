namespace UniModules.UniGame.GraphicsTools.Runtime.Extensions
{
    using System;
    using UnityEngine;

    public static class ColorExtensions
    {
        private const float SRgbDitheringThreshold = 0.03928f;
        
        public static Color AdditiveSum(this Color source, Color other)
        {
            if(Math.Abs(source.a) < float.Epsilon && Math.Abs(other.a) < float.Epsilon)
                return Color.clear;
            if (Math.Abs(source.a) < float.Epsilon)
                return other;
            if (Math.Abs(other.a) < float.Epsilon)
                return source;

            var alpha = 1.0f - (1.0f - other.a) * (1.0f - source.a);
            var red   = other.r * other.a / alpha + source.r * source.a * (1.0f - other.a) / alpha;
            var green = other.g * other.a / alpha + source.g * source.a * (1.0f - other.a) / alpha;
            var blue  = other.b * other.a / alpha + source.b * source.a * (1.0f - other.a) / alpha;
            
            return new Color(red, green, blue, alpha);
        }
        
        public static float GetLuminance(this Color pixelColor)
        {
            return NonlinearTransformation(pixelColor.r) * 0.2126f + NonlinearTransformation(pixelColor.g) * 0.7152f + NonlinearTransformation(pixelColor.b) * 0.0722f;
        }
        
        public static float NonlinearTransformation(float channelValue)
        {
            if (channelValue <= SRgbDitheringThreshold) {
                return channelValue / 12.92f;
            }

            return Mathf.Pow((channelValue + 0.055f) / 1.055f, 2.4f);
        }
        
        // https://en.wikipedia.org/wiki/Color_difference
        public static float GetDifference(this Color a, Color b)
        {
            var lab1 = LABColor.ToLABColor(a);
            var lab2 = LABColor.ToLABColor(b);

            var c_star_1_ab       = Mathf.Sqrt(lab1.A * lab1.A + lab1.B * lab1.B);
            var c_star_2_ab       = Mathf.Sqrt(lab2.A * lab2.A + lab2.B * lab2.B);
            var c_star_average_ab = (c_star_1_ab + c_star_2_ab) * 0.5f;

            var c_star_average_ab_pot7 = c_star_average_ab * c_star_average_ab * c_star_average_ab;
            c_star_average_ab_pot7 *= c_star_average_ab_pot7 * c_star_average_ab;

            var G        = 0.5f * (1.0f - Mathf.Sqrt(c_star_average_ab_pot7 / (c_star_average_ab_pot7 + 6103515625f))); // 25^7
            var a1_prime = (1.0f + G) * lab1.A;
            var a2_prime = (1.0f + G) * lab2.A;

            var C_prime_1 = Mathf.Sqrt(a1_prime * a1_prime + lab1.B * lab1.B);
            var C_prime_2 = Mathf.Sqrt(a2_prime * a2_prime + lab2.B * lab2.B);

            var h_prime_1 = (Mathf.Atan2(lab1.B, a1_prime) * 180.0f / Mathf.PI + 360.0f) % 360.0f;
            var h_prime_2 = (Mathf.Atan2(lab2.B, a2_prime) * 180.0f / Mathf.PI + 360.0f) % 360.0f;

            var delta_L_prime = lab2.L - lab1.L;
            var delta_C_prime = C_prime_2 - C_prime_1;

            var   h_bar = Mathf.Abs(h_prime_1 - h_prime_2);
            float delta_h_prime;

            if (C_prime_1 * C_prime_2 == 0.0f)
                delta_h_prime = 0.0f;
            else {
                if (h_bar <= 180.0f) {
                    delta_h_prime = h_prime_2 - h_prime_1;
                }
                else if (h_bar > 180.0f && h_prime_2 <= h_prime_1) {
                    delta_h_prime = h_prime_2 - h_prime_1 + 360.0f;
                }
                else {
                    delta_h_prime = h_prime_2 - h_prime_1 - 360.0f;
                }
            }

            var delta_H_prime = 2.0f * Mathf.Sqrt(C_prime_1 * C_prime_2) * Mathf.Sin(delta_h_prime * Mathf.PI / 360.0f);

            var L_prime_average = (lab1.L + lab2.L) * 0.5f;
            var C_prime_average = (C_prime_1 + C_prime_2) * 0.5f;

            float h_prime_average;
            if (C_prime_1 * C_prime_2 == 0.0f)
                h_prime_average = 0.0f;
            else {
                if (h_bar <= 180.0f) {
                    h_prime_average = (h_prime_1 + h_prime_2) * 0.5f;
                }
                else if (h_bar > 180.0f && h_prime_1 + h_prime_2 < 360.0f) {
                    h_prime_average = (h_prime_1 + h_prime_2 + 360.0f) * 0.5f;
                }
                else {
                    h_prime_average = (h_prime_1 + h_prime_2 - 360.0f) * 0.5f;
                }
            }

            var L_prime_average_minus_50_square = L_prime_average - 50.0f;
            L_prime_average_minus_50_square *= L_prime_average_minus_50_square;

            var S_L = 1.0f + 0.015f * L_prime_average_minus_50_square / Mathf.Sqrt(20.0f + L_prime_average_minus_50_square);
            var S_C = 1.0f + 0.045f * C_prime_average;
            var T = 1.0f
                    - 0.17f * Mathf.Cos(Mathf.Deg2Rad * (h_prime_average - 30.0f))
                    + 0.24f * Mathf.Cos(Mathf.Deg2Rad * (h_prime_average * 2.0f))
                    + 0.32f * Mathf.Cos(Mathf.Deg2Rad * (h_prime_average * 3.0f + 6.0f))
                    - 0.2f * Mathf.Cos(Mathf.Deg2Rad * (h_prime_average * 4.0f - 63.0f));
            var S_H = 1.0f + 0.015f * T * C_prime_average;
            var h_prime_average_minus_275_div_25_square = (h_prime_average - 275.0f) / 25.0f;
            h_prime_average_minus_275_div_25_square *= h_prime_average_minus_275_div_25_square;
            var delta_theta = 30.0f * Mathf.Exp(-h_prime_average_minus_275_div_25_square);

            var C_prime_average_pot_7 = C_prime_average * C_prime_average * C_prime_average;
            C_prime_average_pot_7 *= C_prime_average_pot_7 * C_prime_average;
            
            var R_C = 2.0f * Mathf.Sqrt(C_prime_average_pot_7 / (C_prime_average_pot_7 + 6103515625f));
            var R_T = -Mathf.Sin(Mathf.Deg2Rad * (2.0f * delta_theta)) * R_C;

            var delta_L_prime_div_k_L_S_L = delta_L_prime / S_L;
            var delta_C_prime_div_k_C_S_C = delta_C_prime / S_C;
            var delta_H_prime_div_k_H_S_H = delta_H_prime / S_H;

            return Mathf.Sqrt(delta_L_prime_div_k_L_S_L * delta_L_prime_div_k_L_S_L
                                       + delta_C_prime_div_k_C_S_C * delta_C_prime_div_k_C_S_C
                                       + delta_H_prime_div_k_H_S_H * delta_H_prime_div_k_H_S_H
                                       + R_T * delta_C_prime_div_k_C_S_C * delta_H_prime_div_k_H_S_H);
        }
    }
}