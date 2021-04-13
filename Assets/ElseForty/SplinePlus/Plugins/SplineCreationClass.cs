using UnityEngine;

public static class SplineCreationClass
{
    public delegate void OnUpdate();
    public static event OnUpdate Update_Spline;

    public static void Update(this SPData sPData)
    {
        sPData.Vertices.Clear();
        sPData.Tangents.Clear();
        sPData.Normals.Clear();

        sPData.VerticesDistance.Clear();

        if (sPData.Nodes.Count >= 2)
        {
            sPData.Length = 0;
            for (int j = 0; j < sPData.Nodes.Count - 1; j++)
            {
                if (j == 0) CubicBezier(sPData, sPData.Nodes[j + 1], sPData.Nodes[j]);
                else CubicBezier(sPData, sPData.Nodes[j + 1], sPData.Nodes[j]);
            }
            if (sPData.Close) CubicBezier(sPData, sPData.Nodes[0], sPData.Nodes[sPData.Nodes.Count - 1]);
            if (Update_Spline != null) Update_Spline();
        }
    }

    static void CubicBezier(SPData sPData, Node pointA, Node pointB)
    {
        Vector3 _pointA1 = pointA.Point1.position;
        Vector3 _pointB2 = pointB.Point2.position;

        Vector3 vertex = Vector3.zero;
        Vector3 tangent = (_pointB2 - _pointA1).normalized;
        Vector3 normal = sPData.SplinePlus.transform.up;

        float speed = 0;
        float t = 0;
        int smoothness = sPData.SplineSettings.Smoothness;

        if (pointA._Type == NodeType.Free)
        {
            _pointA1 = Vector3.Lerp(pointB.Point.position, pointA.Point.position, 0.5f);
            smoothness = 2;
        }
        else if (pointA._Type == NodeType.Smooth) pointA.Point1.localPosition = -pointA.Point2.localPosition;

        if (pointB._Type == NodeType.Free)
        {
            _pointB2 = Vector3.Lerp(pointB.Point.position, pointA.Point.position, 0.5f);
            smoothness = 2;
        }
        else if (pointB._Type == NodeType.Smooth) pointB.Point1.localPosition = -pointB.Point2.localPosition;

        for (int i = 0; i < smoothness; i++)
        {
            t = Mathf.InverseLerp(0, smoothness - 1, i);
            vertex = CalculateCubicBezier(t, pointB.Point.position, _pointB2,
                                  _pointA1, pointA.Point.position);


            if ((sPData.Vertices.Count > 1) && Vector3.Distance(vertex, sPData.Vertices[sPData.Vertices.Count - 1]) <= float.Epsilon)
            {
                continue;
            }


            tangent = CalculateTangent(t, pointB.Point.position, _pointB2,
                                                      _pointA1, pointA.Point.position);

            //Avoid null tangent when node point1 and point2 overlap with point
            if (tangent == Vector3.zero) tangent = (pointA.Point.position - pointB.Point.position).normalized;

            var angle = Mathf.Lerp(pointB.NormalAngle, pointA.NormalAngle, t);

            if (sPData.Projection.Projection_Normals == Switch.Off)
            {
                Vector3 dir = sPData.SplinePlus.transform.up;
                if (sPData.SplineSettings.ReferenceAxis == RefAxis.X) dir = sPData.SplinePlus.transform.right;
                else if (sPData.SplineSettings.ReferenceAxis == RefAxis.Y) dir = sPData.SplinePlus.transform.up;
                else if (sPData.SplineSettings.ReferenceAxis == RefAxis.Z) dir = sPData.SplinePlus.transform.forward;
                Vector3 biNormal = Vector3.Cross(dir, tangent);
                normal = Quaternion.AngleAxis(angle, tangent) * Vector3.Cross(tangent, biNormal).normalized;
            }
            else
            {
                normal = Vector3.Lerp(pointB.Normal, pointA.Normal, t);
            }

            sPData.Vertices.Add(vertex);
            if (sPData.Vertices.Count > 1)
            {
                sPData.Length += Vector3.Distance(sPData.Vertices[sPData.Vertices.Count - 2],
               vertex);
            }

            sPData.Normals.Add(normal);
            sPData.Tangents.Add(tangent);

            sPData.VerticesDistance.Add(sPData.Length);

            // catch node data
            if (i == 0)
            {
                pointB.Normal = normal;
                pointB.Tangent = tangent;
            }

            // catch node data
            if (i == (smoothness - 1))
            {
                pointA.Normal = normal;
                pointA.Tangent = tangent;
            }
        }
    }

    public static void ProjectSpline(SPData sPData)
    {
        for (int i = 0; i < sPData.Nodes.Count; i++)
        {
            RaycastHit Hit, Hit1;
            var origin = sPData.Nodes[i].Point.position + Vector3.up * sPData.Projection.RaysLength;
            if (Physics.Raycast(origin, -Vector3.up, out Hit, sPData.Projection.RaysLength * 2))
            {
                sPData.Nodes[i].Point.position = Hit.point + Vector3.up * sPData.Projection.RaysOffset;
                if (sPData.Projection.Projection_Normals == Switch.On) sPData.Nodes[i].Normal = Hit.normal;
            }
            if (sPData.Projection.HandlesProjection == Switch.On)
            {
                if (sPData.Nodes[i]._Type == NodeType.Smooth)
                {
                    origin = sPData.Nodes[i].Point1.position + Vector3.up * sPData.Projection.RaysLength;
                    if (Physics.Raycast(origin, -Vector3.up, out Hit1, sPData.Projection.RaysLength * 2))
                    {
                        sPData.Nodes[i].Point1.position = Hit1.point + Vector3.up * sPData.Projection.RaysOffset;
                        sPData.Nodes[i].Point2.localPosition = -sPData.Nodes[i].Point1.localPosition;
                    }
                }
                else if (sPData.Nodes[i]._Type == NodeType.Broken)
                {
                    origin = sPData.Nodes[i].Point1.position + Vector3.up * sPData.Projection.RaysLength;

                    if (Physics.Raycast(origin, -Vector3.up, out Hit1, sPData.Projection.RaysLength * 2))
                    {
                        sPData.Nodes[i].Point1.position = Hit1.point + Vector3.up * sPData.Projection.RaysOffset;
                    }

                    origin = sPData.Nodes[i].Point2.position + Vector3.up * sPData.Projection.RaysLength;

                    if (Physics.Raycast(origin, -Vector3.up, out Hit1, sPData.Projection.RaysLength * 2))
                    {
                        sPData.Nodes[i].Point2.position = Hit1.point + Vector3.up * sPData.Projection.RaysOffset;
                    }
                }
            }
        }

        sPData.Update();
    }

    static private Vector3 CalculateCubicBezier(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        var u = 1 - t;
        var uu = u * u;
        var uuu = u * uu;
        var tt = t * t;
        var ttt = t * tt;
        var p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }

    static private Vector3 CalculateTangent(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        var tt = t * t;
        var u = (1 - t);
        var p = -u * u * p0;
        p += (3 * tt - 4 * t + 1) * p1;
        p += (-3 * tt + 2 * t) * p2;
        p += tt * p3;

        return p.normalized;
    }
}
