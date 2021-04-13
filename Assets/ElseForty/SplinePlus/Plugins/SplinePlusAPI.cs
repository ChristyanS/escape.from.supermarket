using System.Collections.Generic;
using UnityEngine;

public static class SplinePlusAPI
{
    public delegate void BranchDeleted(int branchKey);
    public static event BranchDeleted Branch_Deleted;

    #region Spline Plus

    public static SPData SplinePlus_Create(Vector3 position)
    {
        var NewSpline = new GameObject("SplinePlus");
        NewSpline.transform.position = position;
        var splinePlus = NewSpline.AddComponent<SplinePlus>();

        var sPData = splinePlus.sPData;
        sPData.SplinePlus = splinePlus;
        return sPData;
    }



    public static void SplinePlus_Set_Smoothness(SPData sPData, int smoothness)
    {
        sPData.SplineSettings.Smoothness = smoothness;
        sPData.Update();
    }

    public static int SplinePlus_Get_Smoothness(SPData sPData)
    {
        return sPData.SplineSettings.Smoothness;
    }

    public static void SplinePlus_Set_Reference_Axis(SPData sPData, RefAxis axis)
    {
        sPData.SplineSettings.ReferenceAxis = axis;
        sPData.Update();
    }

    public static RefAxis SplinePlus_Get_Reference_Axis(SPData sPData)
    {
        return sPData.SplineSettings.ReferenceAxis;
    }

    public static GameObject AddMeshHolder(SPData sPData, string modifierName)
    {
        var holder = new GameObject(modifierName);
        holder.transform.parent = sPData.SplinePlus.transform;

        holder.transform.localPosition = Vector3.zero;

        if (modifierName != "DeformedMesh")
        {
            holder.AddComponent<MeshRenderer>();
            holder.AddComponent<MeshFilter>();
        }

        return holder;
    }
    #endregion

    #region Node
    public static Node Node_Selected(this SPData sPData)
    {
        var nodeIndex = sPData._NodeIndex;
        if (nodeIndex >= sPData.Nodes.Count || nodeIndex < 0) return null;
        var node = sPData.Nodes[nodeIndex];
        return node;
    }

    public static Node Node_Create(SPData sPData, Vector3 nodePos, SpaceType spaceType)
    {
        for (int n = 0; n < sPData.Nodes.Count; n++)//shared nodes
        {
            if (Vector3.Distance(sPData.Nodes[n].Point.transform.position, nodePos) < Vector3.kEpsilon)
            {
                return sPData.Nodes[n];
            }
        }

        var node = new Node();
        var point = new GameObject("p");
        var point1 = new GameObject("p1");
        var point2 = new GameObject("p2");

        point.transform.parent = sPData.SplinePlus.gameObject.transform;
        point1.transform.parent = point.transform;
        point2.transform.parent = point.transform;

        point.hideFlags = HideFlags.HideInHierarchy;

        node.Point = point.transform;
        node.Point1 = point1.transform;
        node.Point2 = point2.transform;

        if (spaceType == SpaceType.Local) node.Point.localPosition = nodePos;
        else if (spaceType == SpaceType.World) node.Point.localPosition = nodePos;

        node.Point1.localPosition = Vector3.zero;
        node.Point2.localPosition = Vector3.zero;

        return node;
    }

    public static void Node_Set_Position(SPData sPData, Node node, Vector3 nodePos, SpaceType spaceType)
    {
        if (spaceType == SpaceType.Local) node.Point.localPosition = nodePos;
        else if (spaceType == SpaceType.World) node.Point.position = nodePos;

        sPData.Update();
    }

    public static Vector3 Node_Get_Position(Node node, SpaceType spaceType)
    {
        return (spaceType == SpaceType.Local) ? node.Point.localPosition : node.Point.position;
    }

    public static void Node_Handles_Set_Position(SPData sPData, Node node, Vector3 point1Pos, Vector3 point2Pos, SpaceType spaceType)
    {
        node.Point1.localPosition = node.Point2.localPosition = Vector3.zero;

        if (spaceType == SpaceType.Local)
        {
            node.Point1.localPosition = point1Pos;
            node.Point2.localPosition = point2Pos;
        }
        else
        {
            node.Point1.position = point1Pos;
            node.Point2.position = point2Pos;
        }

        sPData.Update();
    }

    public static Vector3 Node_Get_Point1_Position(Node node, SpaceType spaceType)
    {
        return (spaceType == SpaceType.Local) ? node.Point1.localPosition : node.Point1.position;
    }

    public static Vector3 Node_Get_Point2_Position(Node node, SpaceType spaceType)
    {
        return (spaceType == SpaceType.Local) ? node.Point2.localPosition : node.Point2.position;
    }

    public static void Node_Remove(SPData sPData, Node nodeToRemove)
    {
        for (int r = 0; r < sPData.Nodes.Count; r++)
        {
            if (sPData.Nodes[r].Equals(nodeToRemove))
            {
                sPData.Nodes.RemoveAt(r);
            }
        }

        if (sPData.Nodes.Count > 0)
        {
            sPData._NodeIndex = 0;
        }
        else
        {
            sPData._NodeIndex = -1;
        }
        sPData.Update();

#if UNITY_EDITOR
        UnityEditor.Undo.DestroyObjectImmediate(nodeToRemove.Point.gameObject);
#else
        Object.Destroy(nodeToRemove.Point.gameObject);
#endif
    }

    public static void Node_Set_Type(SPData sPData, Node node, NodeType newNodeType)
    {
        if (node.Equals(null)) return;
        node._Type = newNodeType;
        sPData.Update();
    }

    public static NodeType Node_Get_Type(Node node)
    {
        return node._Type;
    }

    public static void Node_Set_Normal(SPData sPData, Node node, float NormalFactor)
    {
        if (node.Equals(null)) return;
        if (NormalFactor < -180) NormalFactor = -180;
        else if (NormalFactor > 180) NormalFactor = 180;

        node.NormalAngle = (int)NormalFactor;
        sPData.Update();
    }

    public static float Node_Get_Normal(Node node)
    {
        if (node.Equals(null)) return 0;

        return node.NormalAngle;
    }

    public static Node Node_Duplicate(SPData sPData, Node originNode)
    {
        var node = new Node();
        var point = new GameObject("p");
        var point1 = new GameObject("p1");
        var point2 = new GameObject("p2");


        point.transform.parent = sPData.SplinePlus.gameObject.transform;
        point1.transform.parent = point.transform;
        point2.transform.parent = point.transform;

        point.transform.position = originNode.Point.position;
        point1.transform.localPosition = originNode.Point1.transform.localPosition;
        point2.transform.localPosition = originNode.Point2.transform.localPosition;

        point.hideFlags = HideFlags.HideInHierarchy;

        node.Point = point.transform;
        node.Point1 = point1.transform;
        node.Point2 = point2.transform;

        return node;
    }

    public static void Node_FlipHandles(SPData sPData, int _NodeIndex)
    {
        if (_NodeIndex < 0 ||
           sPData.Nodes.Count <= _NodeIndex) return;

        var node = sPData.Nodes[_NodeIndex];

        Transform point1 = node.Point1;
        Transform point2 = node.Point2;

        node.Point1 = point2;
        node.Point2 = point1;

        node.HandlesFlipped = !node.HandlesFlipped;
        sPData.Nodes[_NodeIndex] = node;

        sPData.Update();
    }

    public static void Node_Auto_Smooth_Handles(SPData sPData, int nodeIndex)
    {
        Node node0 = null, node1 = null, node2 = null;

        //Smooth midlle nodes 
        if (nodeIndex != 0 && nodeIndex != sPData.Nodes.Count - 1)
        {
            node0 = sPData.Nodes[nodeIndex - 1];
            node1 = sPData.Nodes[nodeIndex];
            node2 = sPData.Nodes[nodeIndex + 1];
        }
        //Smooth other nodes
        else if (sPData.Nodes.Count > 2)
        {
            if (nodeIndex == 0)
            {
                node0 = sPData.Nodes[nodeIndex];
                node1 = sPData.Nodes[nodeIndex + 1];
                node2 = sPData.Nodes[nodeIndex + 2];
            }
            else
            {
                node0 = sPData.Nodes[nodeIndex - 2];
                node1 = sPData.Nodes[nodeIndex - 1];
                node2 = sPData.Nodes[nodeIndex];
            }
        }
        else return;

        var tan = Vector3.Normalize(node0.Point.position - node2.Point.position);
        var dist = Vector3.Distance(node0.Point.position, node2.Point.position) * 0.15f;
        var handlePos = dist * tan;

        Node_Handles_Set_Position(sPData, node1, handlePos, -handlePos, SpaceType.Local);

        sPData.Update();
    }
    #endregion

    #region Branches

    public static void Add_Node_Beginning(SPData sPData, Node nodeToAdd)
    {
        if (sPData.Nodes.Contains(nodeToAdd)) return;
        sPData.Nodes.Insert(0, nodeToAdd);

        sPData._NodeIndex = 0;

        Node_Auto_Smooth_Handles(sPData, 0);
        sPData.Update();
    }

    public static void Add_Node_End(SPData sPData, Node nodeToAdd)
    {
        if (sPData.Nodes.Contains(nodeToAdd)) return;
        sPData.Nodes.Add(nodeToAdd);

        sPData._NodeIndex = sPData.Nodes.Count - 1;

        Node_Auto_Smooth_Handles(sPData, sPData._NodeIndex);
        sPData.Update();
    }

    public static void Add_Node_At_Index(SPData sPData, Node nodeToAdd, int index)
    {
        if (index >= sPData.Nodes.Count && index < 0) return;
        if (sPData.Nodes.Contains(nodeToAdd)) return;


        sPData.Nodes.Insert(index, nodeToAdd);

        sPData._NodeIndex = index;

        Node_Auto_Smooth_Handles(sPData, index);
        sPData.Update();
    }

    public static List<Node> Spline_Get_All_Nodes(SPData sPData)
    {
        return sPData.Nodes;
    }

    public static void Spline_Clear(SPData sPData)
    {
        sPData.Nodes.Clear();
        sPData.Update();
    }

    public static void Spline_Reverse(SPData sPData)
    {
        sPData.Nodes.Reverse();

        for (int i = 0; i < sPData.Nodes.Count; i++)
        {
            Node_FlipHandles(sPData, i);
        }
        sPData.Update();
    }

    public static void Close(SPData sPData, bool close)
    {
        sPData.Close = close;
    }

    public static float Spline_Get_Length(SPData sPData)
    {
        return sPData.Length;
    }

    public static List<Vector3> Spline_Get_Vertices(SPData sPData)
    {
        return sPData.Vertices;
    }

    public static List<Vector3> Spline_Get_Normals(SPData sPData)
    {
        return sPData.Normals;
    }

    public static List<Vector3> Spline_Get_Tangents(SPData sPData)
    {
        return sPData.Tangents;
    }
    #endregion

    #region Followers

    public static Follower Follower_Create(SPData sPData)
    {
        var simpleFollowersClass = sPData.SplinePlus.gameObject.GetComponent<SimpleFollowersClass>();
        if (simpleFollowersClass == null)
        {
            simpleFollowersClass = sPData.SplinePlus.gameObject.AddComponent<SimpleFollowersClass>();
        }

        Follower follower = new Follower();
        simpleFollowersClass.Followers.Add(follower);
        return follower;
    }

    public static void Follower_Delete(SPData sPData, Follower follower)
    {
        var simpleFollowersClass = sPData.SplinePlus.gameObject.GetComponent<SimpleFollowersClass>();
        if (simpleFollowersClass == null)
        {
            sPData.SplinePlus.gameObject.AddComponent<SimpleFollowersClass>();
            return;
        }

        simpleFollowersClass.Followers.Remove(follower);
    }

    public static void Follower_Set_GameObject(Follower follower, GameObject followerGo)
    {
        follower.FollowerGO = followerGo;
    }

    public static GameObject Follower_Get_GameObject(Follower follower)
    {
        return follower.FollowerGO;
    }

    public static void Follower_Set_Speed(Follower follower, float speed)
    {
        follower.Speed = speed;
    }

    public static float Follower_Get_Speed(Follower follower)
    {
        return follower.Speed;
    }

    public static void Follower_Set_Distance(Follower follower, float Distance)
    {
        follower.Distance = Distance;
    }

    public static float Follower_Get_Distance(Follower follower)
    {
        return follower.Distance;
    }

    public static void Follower_Set_AnimationState(Follower follower, Switch value)
    {
        follower.Animation = value;
    }

    public static Switch Follower_Get_AnimationState(Follower follower)
    {
        return follower.Animation;
    }

    public static void Follower_Set_AnimationType(Follower follower, FollowerAnimation followerAnimation)
    {
        follower._FollowerAnimation = followerAnimation;
    }

    public static FollowerAnimation Follower_Get_AnimationType(Follower follower)
    {
        return follower._FollowerAnimation;
    }
    #endregion


    #region Others
    public static Vector2 Vector2_Rotate_Around_Pivot(Vector2 aPoint, float aDegree)
    {
        float rad = aDegree * Mathf.Deg2Rad;
        Quaternion rot = Quaternion.Euler(0, 0, aDegree);

        return rot * aPoint;
    }
    #endregion
}
