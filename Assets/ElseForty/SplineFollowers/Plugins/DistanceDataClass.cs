using UnityEngine;

public static class DistanceDataClass
{
    public static DistanceData DataExtraction(SPData sPData, Follower follower, bool isForward, bool flipDirection)
    {
        int f = 1;
 
        if (flipDirection) f = follower.Reverse ? -1 : 1;
        else
        {
            if (isForward) f = follower.Reverse ? -1 : 1;
            else f = follower.Reverse ? 1 : -1;
        }

        for (int i = sPData.VerticesDistance.Count - 2; i >= 0; i--)
        {
            if (follower.Distance >= sPData.VerticesDistance[i])
            {
                follower.DistanceData.Index = i;
                break;
            }
        }

        var a = follower.DistanceData.Index;
        var b = a + 1;

        var vertexA = sPData.Vertices[a];
        var vertexB = sPData.Vertices[b];

        var tangentA = sPData.Tangents[a];
        var tangentB = sPData.Tangents[b];

        var normalA = sPData.Normals[a];
        var normalB = sPData.Normals[b];

        var vertexDistanceA = sPData.VerticesDistance[a];
        var vertexDistanceB = sPData.VerticesDistance[b];

        var EdgeDistance = Mathf.InverseLerp(vertexDistanceA, vertexDistanceB, follower.Distance);

        follower.DistanceData.Position = Vector3.Lerp(vertexA, vertexB, EdgeDistance);

        if (sPData.SplineSettings.InterpolateRotation == Switch.On)
        {
            Quaternion FirstNodeRot = Quaternion.LookRotation(f * tangentA, normalA);
            Quaternion SecondNodeRot = Quaternion.LookRotation(f * tangentB, normalB);

            follower.DistanceData.Rotation = Quaternion.Lerp(FirstNodeRot, SecondNodeRot, EdgeDistance);
        }
        else
        {
            var dir = vertexB - vertexA;
            follower.DistanceData.Rotation = Quaternion.LookRotation(f * dir, normalA);
        }
        return follower.DistanceData;
    }
}
