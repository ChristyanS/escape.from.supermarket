using UnityEngine;
using UnityEditor;
using System;

public class ToolsBareEditor
{
    public static void Show(SPData sPData)
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));

        var c = EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Edit", "ToolbarButton"))
        {

            GenericMenu menu = new GenericMenu();
 
            menu.AddItem(new GUIContent("Settings"), false, Settings, sPData);
            menu.AddItem(new GUIContent("Snap To Grid"), false, SnapToGrid, sPData);


            c.y += 5;
            menu.DropDown(c);
        }
        EditorGUILayout.EndHorizontal();

        c = EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Modifiers", "ToolbarButton"))
        {
            GenericMenu menu = new GenericMenu();
            menu.AddDisabledItem(new GUIContent("Animation"));
            menu.AddItem(new GUIContent("Simple Followers"), false, AddModifier, new object[] { sPData, "SimpleFollowersClass" });
           
            c.y += 5;
            menu.DropDown(c);
        }
        EditorGUILayout.EndHorizontal();

        c = EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Help", "ToolbarButton" ))
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Documentation"), false, Documentation);

            c.y += 5;
            menu.DropDown(c);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndHorizontal();
    }

    #region Items menu methods

    static void SnapToGrid(object obj)
    {
        SPData sPData = (SPData)obj;
        Undo.RecordObject(sPData.SplinePlus, "Spline snaped");
        SplinePlusEditorAPI.Snap(sPData);
    }
 
    static void Settings(object obj)
    {
        SPData sPData = (SPData)obj;
        SettingsWindow settingsWindow = (SettingsWindow)EditorWindow.GetWindow(typeof(SettingsWindow), true, "Settings", true);
        settingsWindow.Show(sPData);
    }

    static void AddModifier(object objs)
    {
        var o = (object[])objs;
        var modifierName = (string)o[1];
        var sPData = (SPData)o[0];

        var myType = Type.GetType(modifierName + ", Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
        if (myType == null)
        {
            EditorUtility.DisplayDialog("Error", modifierName + " modifier not found!", "Okey");
        }
        else
        {
            sPData.SplinePlus.gameObject.AddComponent(myType);
        }
    }

    static void Documentation()
    {
        Application.OpenURL("https://elseforty.github.io/unity/");
    }
    #endregion
}
