using System.Collections.Generic;
using UnityEngine;

namespace FishingRodSystem
{
    public static class BezierCurve
    {
        /// <summary>
        /// Updates the positions of the vein segments
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="D"></param>
        /// <param name="positionsList"></param>
        public static void GetBezierCurve(Vector3 A, Vector3 B, Vector3 C, Vector3 D, List<Vector3> positionsList)
        {
            float lineResolution = 0.1f;

            positionsList.Clear();

            float t = 0;

            while (t <= 1f)
            {
                // Finds the coordinates between the control points with a Bezier curve
                Vector3 newPos = DeCasteljausAlgorithm(A, B, C, D, t);

                positionsList.Add(newPos);

                // Which t position are we at?
                t += lineResolution;
            }

            positionsList.Add(D);
        }

        /// <summary>
        /// The De Casteljau's Algorithm
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="D"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        static Vector3 DeCasteljausAlgorithm(Vector3 A, Vector3 B, Vector3 C, Vector3 D, float t)
        {
            // To make it faster
            float oneMinusT = 1f - t;

            // Layer 1
            Vector3 Q = oneMinusT * A + t * B;
            Vector3 R = oneMinusT * B + t * C;
            Vector3 S = oneMinusT * C + t * D;

            // Layer 2
            Vector3 P = oneMinusT * Q + t * R;
            Vector3 T = oneMinusT * R + t * S;

            // Final interpolated position
            Vector3 U = oneMinusT * P + t * T;

            return U;
        }
    }
}