using UnityEngine;
using UnityEditor;
 
public class SceneViewDisplay
{
    public static void Display(SPData sPData)
    {
        var selectedNode = sPData.Node_Selected();

        if (selectedNode != null)
        {
            DrawSelectedNodeGizmos(sPData, selectedNode);
            SceneViewUI(sPData, selectedNode);
        }
        else  // if no node is added then force node adding on user
        {
            EditorGUIUtility.AddCursorRect(Camera.current.pixelRect, MouseCursor.ArrowPlus);
            sPData.User_Action = User_Actions.Add;
        }
 
        if (EditorWindow.focusedWindow != null && EditorWindow.focusedWindow.titleContent.text == "Scene")
        {
 
              if (sPData.User_Action == User_Actions.NormalEdit)
            {
                var dialogBoxState = SplinePlusEditorAPI.SliderDialogBox_Normal(sPData, selectedNode, -180, 180, "Normal ", "Node Normal");
 
                if (dialogBoxState == DialogBox.Close || dialogBoxState == DialogBox.Confirm)
                {
                    sPData.User_Action = User_Actions.None;
                }
            }
            else if (sPData.User_Action == User_Actions.CoordinatesEdit)
            {
                var dialogBoxState = SplinePlusEditorAPI.CoordinatesDialogBox(sPData);
                if (dialogBoxState == DialogBox.Close || dialogBoxState == DialogBox.Confirm)
                {
                    sPData.User_Action = User_Actions.None;
                }
            }
       
            else if (sPData.User_Action == User_Actions.Add) SplinePlusEditorAPI.Node_Add(sPData);
 
            else SplinePlusEditorAPI.Node_Select(sPData);
        }
    }

    static void SceneViewUI(SPData sPData, Node selectedNode)
    {
        Handles.BeginGUI();
        if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
        {
            Vector2 mousePos = Event.current.mousePosition;
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Spline/Clear  "), false, Spline_Clear, sPData);
            menu.AddItem(new GUIContent("Spline/Reverse _r"), false, Spline_Reverse, sPData);
            if (sPData.Close)
            {
                menu.AddItem(new GUIContent("Spline/Type/Close"), true, Spline_Close, sPData);
                menu.AddItem(new GUIContent("Spline/Type/Open"), false, Spline_Open, sPData);
            }
            else
            {
                menu.AddItem(new GUIContent("Spline/Type/Close"), false, Spline_Close, sPData);
                menu.AddItem(new GUIContent("Spline/Type/Open"), true, Spline_Open, sPData);
            }

            menu.AddItem(new GUIContent("Node/Add "), false, Node_Add, sPData);
            menu.AddItem(new GUIContent("Node/Delete _DEL"), false, Node_Delete, sPData);

            menu.AddSeparator("Node/");

            menu.AddItem(new GUIContent("Node/Normal "), false, Node_Normal, sPData);
            menu.AddItem(new GUIContent("Node/Coordinates "), false, Node_Coordinates, sPData);

            menu.AddSeparator("Node/");
            if (sPData.SplineSettings.Show_SecondaryHandles) menu.AddItem(new GUIContent("Node/Handles/Hide _h"), false, Node_Hide_Unhide_Handles, sPData);
            else menu.AddItem(new GUIContent("Node/Handles/Unhide _h"), false, Node_Hide_Unhide_Handles, sPData);
            menu.AddItem(new GUIContent("Node/Handles/Flip _x"), false, Node_FlipHandles, sPData);

            if (selectedNode._Type == NodeType.Free)
            {
                menu.AddItem(new GUIContent("Node/Type/Free "), true, Node_Type_Free, sPData);
                menu.AddItem(new GUIContent("Node/Type/Smooth "), false, Node_Type_Smooth, sPData);
                menu.AddItem(new GUIContent("Node/Type/Broken "), false, Node_Type_Broken, sPData);
            }
            else if (selectedNode._Type == NodeType.Smooth)
            {
                menu.AddItem(new GUIContent("Node/Type/Free "), false, Node_Type_Free, sPData);
                menu.AddItem(new GUIContent("Node/Type/Smooth "), true, Node_Type_Smooth, sPData);
                menu.AddItem(new GUIContent("Node/Type/Broken "), false, Node_Type_Broken, sPData);
            }
            else if (selectedNode._Type == NodeType.Broken)
            {
                menu.AddItem(new GUIContent("Node/Type/Free "), false, Node_Type_Free, sPData);
                menu.AddItem(new GUIContent("Node/Type/Smooth "), false, Node_Type_Smooth, sPData);
                menu.AddItem(new GUIContent("Node/Type/Broken "), true, Node_Type_Broken, sPData);
            }

            menu.ShowAsContext();
        }
        //Debug Area
        var style = new GUIStyle();
        style.normal.textColor = Color.black;
        style.fontSize = 10;
        GUI.Label(new Rect(10, 5, 50, 20), "Node Index", style);
        style.normal.textColor = Color.green;

        GUI.Label(new Rect(80, 5, 50, 20), sPData._NodeIndex.ToString(), style);
 

        Handles.EndGUI();
    }

    #region Menu items Methods
    static void Node_Type_Free(object obj)
    {
        SPData sPData = (SPData)obj;
        Undo.RecordObject(sPData.SplinePlus, "Node type changed to free");
        var node = sPData.Node_Selected();
        SplinePlusAPI.Node_Set_Type(sPData, node, NodeType.Free);
    }

    static void Node_Type_Smooth(object obj)
    {
        SPData sPData = (SPData)obj;
        Undo.RecordObject(sPData.SplinePlus, "Node type changed to smooth");
        var node = sPData.Node_Selected();
        SplinePlusAPI.Node_Set_Type(sPData, node, NodeType.Smooth);
    }

    static void Node_Type_Broken(object obj)
    {
        SPData sPData = (SPData)obj;
        Undo.RecordObject(sPData.SplinePlus, "Node type changed to broken");
        var selectedNode = sPData.Node_Selected();
        SplinePlusAPI.Node_Set_Type(sPData, selectedNode, NodeType.Broken);
    }

    static void Node_Add(object obj)
    {
        SPData sPData = (SPData)obj;
        sPData.User_Action = User_Actions.Add;
    }

    static void Node_Hide_Unhide_Handles(object obj)
    {
        SPData sPData = (SPData)obj;
        Undo.RecordObject(sPData.SplinePlus, "Hide handles");
        sPData.SplineSettings.Show_SecondaryHandles = !sPData.SplineSettings.Show_SecondaryHandles;
    }

    static void Node_Normal(object obj)
    {
        SPData sPData = (SPData)obj;
        sPData.User_Action = User_Actions.NormalEdit;
    }

    static void Node_Coordinates(object obj)
    {
        SPData sPData = (SPData)obj;
        sPData.User_Action = User_Actions.CoordinatesEdit;
    }

    static void Node_FlipHandles(object obj)
    {
        SPData sPData = (SPData)obj;
        Undo.RecordObject(sPData.SplinePlus, "handles Fliped");
        SplinePlusAPI.Node_FlipHandles(sPData,  sPData._NodeIndex);
    }

    static void Node_Delete(object obj)
    {
        SPData sPData = (SPData)obj;
        Undo.RecordObject(sPData.SplinePlus, "node deleted");
        SplinePlusAPI.Node_Remove(sPData, sPData.Node_Selected());
    }

    static void Spline_Clear(object obj)
    {
        SPData sPData = (SPData)obj;
        Undo.RecordObject(sPData.SplinePlus, "sPData Reversed");
        SplinePlusAPI.Spline_Clear(sPData);
        if (sPData.Close) sPData.Close = false;
    }

    static void Spline_Reverse(object obj)
    {
        SPData sPData = (SPData)obj;
        Undo.RecordObject(sPData.SplinePlus, "sPData Reversed");
        SplinePlusAPI.Spline_Reverse(sPData);
    }

    static void Spline_Close(object obj)
    {
        SPData sPData = (SPData)obj;
        Undo.RecordObject(sPData.SplinePlus, "sPData closed");

        sPData.Close = true;
        sPData.Update();
    }

    static void Spline_Open(object obj)
    {
        SPData sPData = (SPData)obj;
        Undo.RecordObject(sPData.SplinePlus, "sPData Open");
        sPData.Close = false;
        sPData.Update( );
    }
    #endregion

    static void DrawSelectedNodeGizmos(SPData sPData, Node selectedNode)
    {
        var Point = selectedNode.Point;
        var Point1 = selectedNode.Point1;
        var Point2 = selectedNode.Point2;

        Vector3 p;

        EditorGUI.BeginChangeCheck();
        p = Handles.PositionHandle(Point.position, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(Point.transform, "Position point changed");
            Point.position = p;
            sPData.Update();
        }

        if (sPData.SplineSettings.Show_SecondaryHandles && selectedNode._Type != NodeType.Free)
        {
            Vector3 pos, pos2;
            EditorGUI.BeginChangeCheck();

            pos = Handles.PositionHandle(Point1.position, Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(Point1.transform, "Position point1 changed");
                Undo.RecordObject(Point2.transform, "Position point2 changed");
                Point1.position = pos;
                if (selectedNode._Type == NodeType.Smooth) Point2.localPosition = -Point1.localPosition;
                sPData.Update();
            }
            EditorGUI.BeginChangeCheck();
            pos2 = Handles.PositionHandle(Point2.position, Quaternion.Euler(0, 180f, 0));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(Point1.transform, "Position point1 changed");
                Undo.RecordObject(Point2.transform, "Position point2 changed");
                Point2.position = pos2;
                if (selectedNode._Type == NodeType.Smooth) Point1.localPosition = -Point2.localPosition;
                sPData.Update();
            }
            NodeHandlesGizmos(Point, Point1, Point2);
        }
    }

    static void NodeHandlesGizmos(Transform Point, Transform Point1, Transform Point2)
    {
        Handles.color = Color.yellow;
        Handles.DrawLine(Point.position, Point1.position);
        Handles.DrawLine(Point.position, Point2.position);
        Handles.color = Color.white;
    }
}
