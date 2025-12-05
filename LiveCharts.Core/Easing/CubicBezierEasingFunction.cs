// this function is inspired on
// https://github.com/gre/bezier-easing/blob/master/src/index.js

using System;

namespace LiveChartsCore.Easing
{
    /// <summary>
    /// 三次贝塞尔缓动函数，提供高度可定制的缓动曲线
    /// 使用四个控制点定义速度曲线，与CSS的cubic-bezier相同
    /// </summary>
    public static class CubicBezierEasingFunction
    {
        // 牛顿迭代法的参数
        private static readonly float NEWTON_ITERATIONS = 4f;
        private static readonly float NEWTON_MIN_SLOPE = 0.001f;
        private static readonly float SUBDIVISION_PRECISION = 0.0000001f;
        private static readonly float SUBDIVISION_MAX_ITERATIONS = 10f;

        // 样条表大小和采样步长
        private static readonly int kSplineTableSize = 11;
        private static readonly float kSampleStepSize = 1.0f / (kSplineTableSize - 1.0f);

        /// <summary>
        /// 构建贝塞尔缓动函数
        /// </summary>
        /// <param name="mX1">第一个控制点的X坐标（0-1）</param>
        /// <param name="mY1">第一个控制点的Y坐标</param>
        /// <param name="mX2">第二个控制点的X坐标（0-1）</param>
        /// <param name="mY2">第二个控制点的Y坐标</param>
        /// <returns>缓动函数，输入0-1，输出0-1</returns>
        public static Func<float, float> BuildBezierEasingFunction(float mX1, float mY1, float mX2, float mY2)
        {
            // 验证输入参数
            if (!(0 <= mX1 && mX1 <= 1 && 0 <= mX2 && mX2 <= 1))
            {
                throw new Exception("Bezier x values must be in [0, 1] range 贝塞尔曲线的X值必须在[0, 1]范围内");
            }

            // 如果是线性缓动（控制点在对角线上）
            if (mX1 == mY1 && mX2 == mY2)
            {
                return LinearEasing;
            }

            // Precompute samples table
            // 预计算采样表
            var sampleValues = new float[kSplineTableSize];
            for (var i = 0; i < kSplineTableSize; ++i)
            {
                sampleValues[i] = CalcBezier(i * kSampleStepSize, mX1, mX2);
            }

            // 内部函数：根据X值查找对应的t值
            float getTForX(float aX)
            {
                var intervalStart = 0.0f;
                var currentSample = 1;
                var lastSample = kSplineTableSize - 1;

                // 在采样表中查找包含aX的区间
                for (; currentSample != lastSample && sampleValues[currentSample] <= aX; ++currentSample)
                {
                    intervalStart += kSampleStepSize;
                }
                --currentSample;

                // Interpolate to provide an initial guess for t
                // 插值提供t的初始猜测值
                var dist = (aX - sampleValues[currentSample]) / (sampleValues[currentSample + 1] - sampleValues[currentSample]);
                var guessForT = intervalStart + dist * kSampleStepSize;

                var initialSlope = GetSlope(guessForT, mX1, mX2);
                if (initialSlope >= NEWTON_MIN_SLOPE)
                {
                    return NewtonRaphsonIterate(aX, guessForT, mX1, mX2);
                }
                else if (initialSlope == 0.0f)
                {
                    return guessForT;
                }
                else
                {
                    return BinarySubdivide(aX, intervalStart, intervalStart + kSampleStepSize, mX1, mX2);
                }
            }

            // 返回缓动函数
            return (t) =>
            {
                // Because JavaScript number are imprecise, we should guarantee the extremes are right.
                // 因为JavaScript数字不精确，我们应该保证极端情况是正确的
                //if (t == 0f || t == 1f)
                //{
                //    return t;
                //}
                return CalcBezier(getTForX(t), mY1, mY2);
            };
        }

        // 三次贝塞尔曲线的系数计算
        private static float A(float aA1, float aA2)
        { return 1.0f - 3.0f * aA2 + 3.0f * aA1; }

        private static float B(float aA1, float aA2)
        { return 3.0f * aA2 - 6.0f * aA1; }

        private static float C(float aA1)
        { return 3.0f * aA1; }

        // 计算贝塞尔曲线在t时刻的值
        private static float CalcBezier(float aT, float aA1, float aA2)
        { return ((A(aA1, aA2) * aT + B(aA1, aA2)) * aT + C(aA1)) * aT; }

        // 计算贝塞尔曲线在t时刻的斜率（导数）
        private static float GetSlope(float aT, float aA1, float aA2)
        { return 3.0f * A(aA1, aA2) * aT * aT + 2.0f * B(aA1, aA2) * aT + C(aA1); }

        // 二分查找法：在区间[aA, aB]中查找使贝塞尔曲线值为aX的t值
        private static float BinarySubdivide(float aX, float aA, float aB, float mX1, float mX2)
        {
            float currentX;
            float currentT;
            var i = 0;
            do
            {
                currentT = aA + (aB - aA) / 2.0f;
                currentX = CalcBezier(currentT, mX1, mX2) - aX;
                if (currentX > 0.0)
                {
                    aB = currentT;
                }
                else
                {
                    aA = currentT;
                }
            } while (Math.Abs(currentX) > SUBDIVISION_PRECISION && ++i < SUBDIVISION_MAX_ITERATIONS);
            return currentT;
        }

        // 牛顿-拉弗森迭代法：使用切线快速逼近解
        private static float NewtonRaphsonIterate(float aX, float aGuessT, float mX1, float mX2)
        {
            for (var i = 0; i < NEWTON_ITERATIONS; ++i)
            {
                var currentSlope = GetSlope(aGuessT, mX1, mX2);
                if (currentSlope == 0.0f)
                {
                    return aGuessT;
                }
                var currentX = CalcBezier(aGuessT, mX1, mX2) - aX;
                aGuessT -= currentX / currentSlope;
            }
            return aGuessT;
        }

        // 线性缓动函数
        private static float LinearEasing(float x)
        {
            return x;
        }
    }
}