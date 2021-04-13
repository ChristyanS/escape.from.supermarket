using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.IO;


[CustomEditor(typeof(SplinePlus))]
public class SplinePlusEditor : Editor
{
    public SPData SPData;

    public static GUIContent Banner;
    public static GUIContent Plus;
    public static GUIContent Minus;
    public static GUIContent Delete;
    public static GUIContent Return;

    public int ToolBarSelection = 0;

    [MenuItem("Tools/Spline plus", false, 0)]
    static void CreateSplinePlus()
    {
        var sPData = SplinePlusAPI.SplinePlus_Create(Vector3.zero);
        Selection.activeGameObject = sPData.SplinePlus.gameObject;
    }

    public void OnEnable()
    {
        var SplinePlus = (SplinePlus)target;
        SPData = SplinePlus.sPData;

        if (Banner == null) Banner = new GUIContent((Texture2D)EditorGUIUtility.Load(SplinePlusEditor.FindAssetPath("Banner.png")));
        if (Plus == null) Plus = new GUIContent((Texture2D)EditorGUIUtility.Load(SplinePlusEditor.FindAssetPath("Plus.png")));
        if (Minus == null) Minus = new GUIContent((Texture2D)EditorGUIUtility.Load(SplinePlusEditor.FindAssetPath("Minus.png")));
        if (Delete == null) Delete = new GUIContent((Texture2D)EditorGUIUtility.Load(SplinePlusEditor.FindAssetPath("Delete.png")));
        if (Return == null) Return = new GUIContent((Texture2D)EditorGUIUtility.Load(SplinePlusEditor.FindAssetPath("Return.png")));


        //Init node actions to selection
        SPData.User_Action = User_Actions.None;
        EditorUtility.SetDirty(SPData.SplinePlus);
    }

    public static string FindAssetPath(string assetName)
    {
        string[] res = System.IO.Directory.GetFiles(Application.dataPath, assetName, SearchOption.AllDirectories);
        if (res.Length == 0)
        {
            Debug.LogError("Asset " + assetName + " not found!!");
            return null;
        }

        var path = Regex.Split(res[0], "Assets", RegexOptions.None);
        return ("Assets" + path[1]);
    }

    public override void OnInspectorGUI()
    {
        // DrawDefaultInspector();
        var rect = EditorGUILayout.BeginHorizontal();
        var x = rect.x + (rect.width - Banner.image.width) * 0.5f;
        GUI.Label(new Rect(x, rect.y, Banner.image.width, Banner.image.height), Banner);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(Banner.image.height);
        ToolsBareEditor.Show(SPData);

        if (Event.current.commandName == "UndoRedoPerformed" || SPData.SplinePlus.gameObject.transform.hasChanged) //
        {
            if (SPData.SplinePlus.gameObject.transform.hasChanged) SPData.SplinePlus.gameObject.transform.hasChanged = false;
            SPData.Update();
        }
    }

    protected virtual void OnSceneGUI()
    {
        Selection.activeGameObject = SPData.SplinePlus.gameObject;

        if (SPData.Projection.ContinuosUpdate == Switch.On) SplineCreationClass.ProjectSpline(SPData);

        SceneViewDisplay.Display(SPData);
        Shortcuts();
        Repaint();
    }

    void Shortcuts()
    {
        var e = Event.current;
        if (e == null) return;

        // Set mode back to selection
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
        {
            Undo.RecordObject(SPData.SplinePlus, "Selection initialised");
            SPData.User_Action = User_Actions.None;
         }
 
        //focus on path point
        else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.C)
        {
            var selectedNode = SPData.Node_Selected();
            SceneView.lastActiveSceneView.LookAt(selectedNode.Point.position);
        }

        //Delete selected node
        else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Backspace)
        {
            var selectedNode = SPData.Node_Selected();
            Undo.RecordObject(SPData.SplinePlus, "node deleted");
            SplinePlusAPI.Node_Remove(SPData, selectedNode);
        }

        //Hide node handles
        else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.H)
        {
            Undo.RecordObject(SPData.SplinePlus, "node hide unhide handles");
            SPData.SplineSettings.Show_SecondaryHandles = !SPData.SplineSettings.Show_SecondaryHandles;
        }

        //Reverse sPData
        else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.R)
        {
            Undo.RecordObject(SPData.SplinePlus, "Reverse sPData");
            SplinePlusAPI.Spline_Reverse(SPData);
            SPData.Update( );
        }

        //Flip node handles
        else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.X)
        {
            Undo.RecordObject(SPData.SplinePlus, "Flip handles sPData");
            SplinePlusAPI.Node_FlipHandles(SPData,   SPData._NodeIndex);
            SPData.Update();
        }
    }
}


