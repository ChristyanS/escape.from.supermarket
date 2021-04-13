using UnityEngine;

public class SplinePlus : MonoBehaviour
{
    public SPData sPData = new SPData();
    public static SPData CopiedSPData;

    public SplinePlus()
    {
        sPData.SplinePlus = this;
    }

    private void Start()
    {
        sPData.Update();
    }

    public void Update()
    {
        if (sPData.Projection.ContinuosUpdate == Switch.On) SplineCreationClass.ProjectSpline(sPData);
    }

    #region Gizmos

    private void OnDrawGizmosSelected()
    {
        Draw_Spline_Line();
        Draw_Spline_Nodes_Gizmos();
    }
    void Draw_Spline_Nodes_Gizmos()
    {

        if (sPData.SplineSettings.Gizmos == Switch.On)
        {
            for (int i = 0; i < sPData.Nodes.Count; i++)
            {
                var nodePos = sPData.Nodes[i].Point.position;
                Gizmos.color = sPData.SplineSettings.SimpleNodeColor;
                Gizmos.DrawSphere(nodePos, sPData.SplineSettings.GizmosSize);
            }
        }
    }

    void Draw_Spline_Line()
    {
        if (sPData.Vertices.Count < 2) return;

        for (int z = 0; z < sPData.Vertices.Count; z++)
        {
            if (z > 0)
            {
                var a = sPData.Vertices[z - 1];
                var b = sPData.Vertices[z];

                Gizmos.color = Color.green;
                Gizmos.DrawLine(a, b);

            }

            if (sPData.SplineSettings.Helpers == Switch.On)
            {
                var c = sPData.Vertices[z];

                var n = sPData.Normals[z] * sPData.SplineSettings.HelperSize;
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(c, c + n);

                var t = sPData.Tangents[z] * sPData.SplineSettings.HelperSize;
                Gizmos.color = Color.red;
                Gizmos.DrawLine(c, c + t);
            }
        }
    }
    #endregion
}