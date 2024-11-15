using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oathstring.ProjectRoad.Utility
{
    public static class Check
    {
        //Only For Project Road
        public static Vector3 ResultVector3ToPositive(Vector3 edge)
        {
            Vector3 checkResult = edge / 1.001f;
            Vector3 result = checkResult - edge;
            Vector3 convertToPositiveResultLeft = new(System.MathF.Abs(result.x),
                System.MathF.Abs(result.x), System.MathF.Abs(result.x));
            Vector3 allresult = edge + convertToPositiveResultLeft;
            return allresult;
        }

        public static Vector3 ResultVector3(Vector3 edge)
        {
            Vector3 checkResultRight = edge / 1.001f;
            Vector3 resultRight = checkResultRight - edge;
            Vector3 allresult = edge + resultRight;
            return allresult;
        }
    }
}
