using System.Collections.Generic;
using UnityEngine;

public enum FollowerAnimation { Auto, Keyboard }
public enum DialogBox { Confirm, Close, Opened }
public enum Switch { On, Off }
public enum State { Lock, Unlock }
public enum RefAxis { X, Y, Z }
public enum NodeType { Free, Smooth, Broken }
public enum User_Actions { None, Add, NormalEdit,  CoordinatesEdit }
  
public enum SpaceType { Local, World }
 
public enum KeyboardDirection { Forward,Backward,Left,Right,None}

[System.Serializable]
public class SPData
{
    public User_Actions User_Action = User_Actions.None;
    public KeyboardDirection KeyboardDirection = KeyboardDirection.None;

    public SplineSettings SplineSettings = new SplineSettings();
     public Projection Projection = new Projection();

    public static float KeyboardInputValue = 0;
    public int _NodeIndex;

    public SplinePlus SplinePlus;
    public KeyCode UpKey = KeyCode.UpArrow;
    public KeyCode DownKey = KeyCode.DownArrow;
 

    public List<Vector3> Vertices = new List<Vector3>();
    public List<Vector3> Tangents = new List<Vector3>();
    public List<Vector3> Normals = new List<Vector3>();

    public List<Node> Nodes = new List<Node>();
 
    public List<float> VerticesDistance = new List<float>();
 
    public float Length = 0;
    public float Distance = 0;
    public bool Close = false;

    public Node FirstNode
    {
        get
        {
            if (Nodes.Count > 0) return Nodes[0];
            else return null;
        }
    }
    public Node LastNode
    {
        get
        {
            if (Nodes.Count > 0) return Nodes[Nodes.Count - 1];
            else return null;
        }
    }
}

[System.Serializable]
public class SplineSettings
{
    public Switch InterpolateRotation = Switch.On;
    public Switch Helpers = Switch.Off;
    public Switch Gizmos = Switch.On;

    public bool Show_SecondaryHandles = true;
    public int Smoothness = 20;
    public float WeldDistance = 1.0f;
    public RefAxis ReferenceAxis = RefAxis.Y;


    public float HelperSize = 1;
    public float GizmosSize = 0.2f;
    public Color SimpleNodeColor = Color.green;
    public Color SharedNodeColor = Color.magenta;

    public NodeType NodeType = NodeType.Smooth;
}
 

[System.Serializable]
public class Projection
{
    public Switch HandlesProjection = Switch.Off;
    public Switch ShowRays = Switch.Off;
    public Switch ContinuosUpdate = Switch.Off;
    public Switch Projection_Normals = Switch.Off;
    public float RaysLength = 30;
    public float RaysOffset;
}
 

[System.Serializable]
public class Node
{
    public Transform Point;
    public Transform Point1;
    public Transform Point2;
 
    public NodeType _Type = NodeType.Smooth;

    public Vector3 Normal;
    public Vector3 Tangent;
    public float NormalAngle = 0;

    public bool HandlesFlipped = false;

    public int LocalIndex(SPData SPData )
    {
        for (int n = 0; n < SPData.Nodes.Count; n++)
        {
            if (SPData.Nodes[n].Point.gameObject == Point.gameObject)
            {
                return n;
            }
        }
        return -1;
    }

    public override bool Equals(object obj)
    {
        if (obj == null && (this.Point == null)) return true;
        if ((obj == null) || !(obj is Node)) return false;

        var node = ((Node)obj);
        return object.Equals(this.Point.gameObject, node.Point.gameObject);
    }

    public override int GetHashCode()
    {
        return Point.GetHashCode();
    }
}


