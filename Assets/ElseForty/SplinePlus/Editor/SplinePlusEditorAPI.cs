using UnityEngine;
using UnityEditor;
using System;
 
public class SplinePlusEditorAPI
{
    public static Node Node_Closest_To_MousePosition(SPData sPData)
    {
        float dist = float.MaxValue;
        Node closestNode = null;
      
            for (int r = 0; r < sPData.Nodes.Count; r++)
            {
                var node = sPData.Nodes[r];
                // find closest node to mouse position
                var newDist = Vector2.Distance(HandleUtility.WorldToGUIPoint(node.Point.position), Event.current.mousePosition);
                if (newDist < dist)
                {
                    dist = newDist;
                    closestNode = node;
                }
            }
      
        return closestNode;
    }

    public static void Node_Select(SPData sPData)
    {
        if (Event.current.type == EventType.MouseDown && Event.current.clickCount == 2)
        {
            Undo.RecordObject(sPData.SplinePlus, "Selection Changed");
            float dist = float.MaxValue;
             int cachIndex = 0;
          
                for (int r = 0; r < sPData.Nodes.Count; r++)
                {
                    var node = sPData.Nodes[r];

                    // find closest node to mouse position
                    var newDist = Vector2.Distance(HandleUtility.WorldToGUIPoint(node.Point.position), Event.current.mousePosition);
                    if (newDist < dist)
                    {
                        dist = newDist;
    
                        cachIndex = r;
                    }
                }
            // delete last selected sPData if it has only one node
 
            sPData._NodeIndex = cachIndex;
        }
    }
    public static void Node_Add(SPData sPData)
    {
        SceneView.currentDrawingSceneView.Repaint();
        EditorGUIUtility.AddCursorRect(Camera.current.pixelRect, MouseCursor.ArrowPlus);

        float dist = float.MaxValue;
        Vector3 vert = Vector3.zero;
        int index = 0;

        // find closest vertex to mouse position
        for (int r = 0; r < sPData.Vertices.Count; r++)
        {
            var vertex = sPData.Vertices[r];
            var newDist = Vector2.Distance(HandleUtility.WorldToGUIPoint(vertex), Event.current.mousePosition);
            if (newDist < dist)
            {
                dist = newDist;
                vert = vertex;
                index = r;
            }
        }

        // Add node to sPData when click is performed
        Action<Vector3> Add_Node_Click = delegate (Vector3 pos)
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                Undo.RecordObject(sPData.SplinePlus, "Node added");
                Func<string, Vector3, Quaternion, Transform, Transform> CreatePoint = delegate (string name, Vector3 Pos, Quaternion Rot, Transform Parent)
                {
                    var Obj = new GameObject(name);
                    Obj.hideFlags = HideFlags.HideInHierarchy;
                    Obj.transform.position = Pos;
                    Obj.transform.rotation = Rot;
                    Obj.transform.parent = Parent;
                    return Obj.transform;
                };

                var node = new Node();

                node._Type = sPData.SplineSettings.NodeType;

                node.Point = CreatePoint("p", pos, Quaternion.identity, sPData.SplinePlus.gameObject.transform);
                node.Point1 = CreatePoint("p1", node.Point.position, Quaternion.identity, node.Point);
                node.Point2 = CreatePoint("p2", node.Point.position, Quaternion.identity, node.Point);


                if (index == 0)
                {
                    SplinePlusAPI.Add_Node_Beginning(sPData,  node);
                }
                else if (index == sPData.Vertices.Count - 1)
                {
                    SplinePlusAPI.Add_Node_End(sPData,   node);
                }
                else
                {
                    var v = Mathf.InverseLerp(0, sPData.Vertices.Count - 1, index);

                    int nodeTargetIndex = (sPData.Close) ? (int)Mathf.Lerp(0, sPData.Nodes.Count, v) : (int)Mathf.Lerp(0, sPData.Nodes.Count - 1, v);
                    nodeTargetIndex++;

                    SplinePlusAPI.Add_Node_At_Index(sPData,   node, nodeTargetIndex);
                }
            }
        };

        // convert mouse position to vector3 world space
        Func<Vector2, Vector3> MousePos_To_Vector3 = delegate (Vector2 mousePosition)
        {
            float distance = 0.0f;
            RaycastHit Hit;
            Ray ray = new Ray();
            Plane m_Plane;
            Vector3 hitPoint = Vector3.zero;

            ray = HandleUtility.GUIPointToWorldRay(mousePosition);

            if (Camera.current.transform.eulerAngles == new Vector3(0, 0, 0) || Camera.current.transform.eulerAngles == new Vector3(0, 180, 0))
                m_Plane = new Plane(Vector3.forward, Vector3.zero);
            else if (Camera.current.transform.eulerAngles == new Vector3(90, 0, 0) || Camera.current.transform.eulerAngles == new Vector3(270, 0, 0))
                m_Plane = new Plane(Vector3.up, Vector3.zero);
            else if (Camera.current.transform.eulerAngles == new Vector3(0, 90, 0) || Camera.current.transform.eulerAngles == new Vector3(0, 270, 0))
                m_Plane = new Plane(Vector3.right, Vector3.zero);
            else m_Plane = new Plane(Vector3.up, Vector3.zero);

            if (Physics.Raycast(ray, out Hit)) hitPoint = Hit.point;
            else if (m_Plane.Raycast(ray, out distance)) hitPoint = ray.GetPoint(distance);

            return hitPoint;
        };

 

        // First node added to a newly created sPData
        if (sPData.Nodes.Count == 0)
        {
            var hitPoint = MousePos_To_Vector3(Event.current.mousePosition);
            Handles.color = Color.yellow;
            Handles.SphereHandleCap(0, hitPoint, Quaternion.identity, sPData.SplineSettings.GizmosSize * 2, EventType.Repaint);

            Add_Node_Click(hitPoint);
        }
        // node added to the beginning of the sPData
        else if (index == 0)
        {
            var pos = MousePos_To_Vector3(Event.current.mousePosition);
            var node = sPData.Nodes[0];
       
                Preview(sPData, node.Point.position, pos);
                Add_Node_Click(pos);
            }  
        // node added to the end of the sPData
        else if (index == sPData.Vertices.Count - 1)
        {
            var pos = MousePos_To_Vector3(Event.current.mousePosition);
            var node = sPData.Nodes[sPData.Nodes.Count - 1];
     
                Preview(sPData, node.Point.position, pos);
                Add_Node_Click(pos);
         
        }
        // node added in the middle of the sPData
        else
        {
            Handles.color = Color.yellow;
            Handles.SphereHandleCap(0, vert, Quaternion.identity, sPData.SplineSettings.GizmosSize * 2, EventType.Repaint);

            Add_Node_Click(vert);
        }
    }

    // create gizmos to preview nodes adding on sPData forks
    static void Preview(SPData sPData, Vector3 start, Vector3 end)
    {
        Handles.color = Color.yellow;
        Handles.SphereHandleCap(0, end, Quaternion.identity, sPData.SplineSettings.GizmosSize * 2, EventType.Repaint);
        Handles.DrawBezier(start, end, start, end, Color.yellow, null, 1.0f);
        SceneView.currentDrawingSceneView.Repaint();
    }

    public static DialogBox ValueDialogBox(SPData sPData, ref float value, float min, float max, string valueName, string title)
    {
        GUI.BeginGroup(new Rect(5, SceneView.lastActiveSceneView.position.height - 75, 250, 200));
        GUI.Box(new Rect(0, 0, 250, 50), "");
        GUI.Box(new Rect(0, 0, 250, 20), title);

        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
        {
            GUI.EndGroup();
            return DialogBox.Confirm;
        }

        else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
        {
            GUI.EndGroup();
            return DialogBox.Close;
        }

        EditorGUI.BeginChangeCheck();
        var val = EditorGUI.FloatField(new Rect(10, 25, 235, 20), valueName, value);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(sPData.SplinePlus, "Valuechanged");
            if (val < min) val = min;
            else if (val > max) val = max;
            value = val;
            sPData.Update();
        }

        GUI.EndGroup();
        return DialogBox.Opened;
    }

    public static DialogBox CoordinatesDialogBox(SPData sPData)
    {
        var selectedNode = sPData.Node_Selected();
        GUI.BeginGroup(new Rect(5, SceneView.lastActiveSceneView.position.height - 175, 300, 350));
        GUI.Box(new Rect(0, 0, 300, 150), "");
        GUI.Box(new Rect(0, 0, 300, 20), "Node Coordinates");

        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
        {
            GUI.EndGroup();
            return DialogBox.Confirm;
        }

        else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
        {
            GUI.EndGroup();
            return DialogBox.Close;
        }

        EditorGUI.BeginChangeCheck();
        var point = EditorGUI.Vector3Field(new Rect(10, 25, 280, 20), "Point", selectedNode.Point.position);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(selectedNode.Point.transform, "Position point changed");
            Undo.RecordObject(sPData.SplinePlus, "Valuechanged");
            selectedNode.Point.position = point;
            sPData.Update();
        }

        EditorGUI.BeginChangeCheck();
        var point1 = EditorGUI.Vector3Field(new Rect(10, 65, 280, 20), "Point 1", sPData.Node_Selected().Point1.position);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(selectedNode.Point1.transform, "Position point1 changed");
            Undo.RecordObject(selectedNode.Point2.transform, "Position point2 changed");
            Undo.RecordObject(sPData.SplinePlus, "Valuechanged");
            selectedNode.Point1.position = point1;
            if (selectedNode._Type == NodeType.Smooth) selectedNode.Point2.localPosition = -selectedNode.Point1.localPosition;
            sPData.Update();
        }

        EditorGUI.BeginChangeCheck();
        var point2 = EditorGUI.Vector3Field(new Rect(10, 105, 280, 20), "Point 2", selectedNode.Point2.position);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(selectedNode.Point1.transform, "Position point1 changed");
            Undo.RecordObject(selectedNode.Point2.transform, "Position point2 changed");
            Undo.RecordObject(sPData.SplinePlus, "Valuechanged");
            selectedNode.Point2.position = point2;
            if (selectedNode._Type == NodeType.Smooth) selectedNode.Point1.localPosition = -selectedNode.Point2.localPosition;

            sPData.Update();
        }

        GUI.EndGroup();
        return DialogBox.Opened;
    }

    public static DialogBox SliderDialogBox(SPData sPData, ref float value, float min, float max, string valueName, string title)
    {
        GUI.BeginGroup(new Rect(5, SceneView.lastActiveSceneView.position.height - 75, 250, 200));
        GUI.Box(new Rect(0, 0, 250, 50), "");
        GUI.Box(new Rect(0, 0, 250, 20), title);

        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
        {
            GUI.EndGroup();
            return DialogBox.Confirm;
        }

        else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
        {
            GUI.EndGroup();
            return DialogBox.Close;
        }


        EditorGUI.BeginChangeCheck();
        var val = EditorGUI.Slider(new Rect(10, 25, 235, 20), value, min, max);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(sPData.SplinePlus, "Valuechanged");
            value = val;
            sPData.Update();
        }

        GUI.EndGroup();
        return DialogBox.Opened;
    }


    public static DialogBox SliderDialogBox_Normal(SPData sPData, Node selectedNode, float min, float max, string valueName, string title)
    {
        GUI.BeginGroup(new Rect(5, SceneView.lastActiveSceneView.position.height - 75, 250, 200));
        GUI.Box(new Rect(0, 0, 250, 50), "");
        GUI.Box(new Rect(0, 0, 250, 20), title);

        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
        {
            GUI.EndGroup();
            return DialogBox.Confirm;
        }

        else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
        {
            GUI.EndGroup();
            return DialogBox.Close;
        }


        EditorGUI.BeginChangeCheck();
        var normalAngle = EditorGUI.Slider(new Rect(10, 25, 235, 20), selectedNode.NormalAngle, min, max);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(sPData.SplinePlus, "Valuechanged");
            selectedNode.NormalAngle = normalAngle;
 
            sPData.Update();
        }

        GUI.EndGroup();
        return DialogBox.Opened;
    }

    public static void Snap(SPData sPData)
    {
        var cam = SceneView.GetAllSceneCameras()[0];
        
            for (int n = 0; n < sPData.Nodes.Count; n++)
            {
                //3D Grid
                if (cam.transform.eulerAngles == new Vector3(0, 0, 0) || cam.transform.eulerAngles == new Vector3(0, 180, 0))
                {
                    sPData.Nodes[n].Point.position = new Vector3(sPData.Nodes[n].Point.position.x,
                        sPData.Nodes[n].Point.position.y, 0);
                    sPData.Nodes[n].Point1.position = new Vector3(sPData.Nodes[n].Point1.position.x,
                        sPData.Nodes[n].Point1.position.y, 0);
                }
                //2D Grid
                else
                {
                    sPData.Nodes[n].Point.position = new Vector3(sPData.Nodes[n].Point.position.x, 0,
                        sPData.Nodes[n].Point.position.z);
                    sPData.Nodes[n].Point1.position = new Vector3(sPData.Nodes[n].Point1.position.x, 0,
                        sPData.Nodes[n].Point1.position.z);
                }
                sPData.Nodes[n].Point2.localPosition = -sPData.Nodes[n].Point1.localPosition;
            }
        
        sPData.Update();
    }
}
