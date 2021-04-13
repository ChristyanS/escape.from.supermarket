using UnityEngine;
using UnityEditor;


public class FollowerWindow : EditorWindow
{
    SerializedProperty follower;
    Follower _follower;
    SPData sPData;

    public Vector2 ScrolPos = new Vector2(0, 0);
    public GUIContent Banner;

    public void Show(SPData _sPData, SerializedProperty __followerSP, Follower __follower)
    {
        sPData = _sPData;
        follower = __followerSP;
        _follower = __follower;


    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);


        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.ObjectField(follower.FindPropertyRelative("FollowerGO"), typeof(GameObject), new GUIContent("Follower"));
        if (EditorGUI.EndChangeCheck())
        {
            follower.serializedObject.ApplyModifiedProperties();
            SimpleFollowerAnim.Follow(sPData, _follower);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUI.BeginChangeCheck();
        var Distance = EditorGUILayout.FloatField("Distance", follower.FindPropertyRelative("Distance").floatValue);
        if (EditorGUI.EndChangeCheck())
        {
            var branchDist = sPData.Length;
            if (Distance < 0) Distance = 0;
            else if (Distance > branchDist) Distance = branchDist;

            follower.FindPropertyRelative("Distance").floatValue = Distance;
            follower.serializedObject.ApplyModifiedProperties();
            SimpleFollowerAnim.Follow(sPData, _follower);
        }


        EditorGUILayout.BeginHorizontal();
        var position = EditorGUILayout.Vector3Field("Position", follower.FindPropertyRelative("Position").vector3Value);
        if (follower.FindPropertyRelative("Position").vector3Value != position)
        {
            follower.FindPropertyRelative("Position").vector3Value = position;
            follower.serializedObject.ApplyModifiedProperties();
            SimpleFollowerAnim.Follow(sPData, _follower);
        }

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(follower.FindPropertyRelative("SpaceType"), new GUIContent(""), GUILayout.Width(60));
        if (EditorGUI.EndChangeCheck())
        {
            follower.serializedObject.ApplyModifiedProperties();
            SimpleFollowerAnim.Follow(sPData, _follower);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        var rotation = EditorGUILayout.Vector3Field("Rotation", follower.FindPropertyRelative("Rotation").vector3Value);
        if (follower.FindPropertyRelative("Rotation").vector3Value != rotation)
        {
            follower.FindPropertyRelative("Rotation").vector3Value = rotation;
            follower.serializedObject.ApplyModifiedProperties();
            SimpleFollowerAnim.Follow(sPData, _follower);
        }

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(follower.FindPropertyRelative("LockRotation"), new GUIContent(""), GUILayout.Width(60));
        if (EditorGUI.EndChangeCheck())
        {
            follower.serializedObject.ApplyModifiedProperties();
            SimpleFollowerAnim.Follow(sPData, _follower);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        var elements = new string[] { "Animation" };
        _follower.ToolBareSelection = GUILayout.Toolbar(_follower.ToolBareSelection, elements);
        switch (_follower.ToolBareSelection)
        {
            case 0:
                Animation();
                break;
        }
    }

    void Animation()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(follower.FindPropertyRelative("Animation"), new GUIContent(""), GUILayout.Width(40));
        if (EditorGUI.EndChangeCheck())
        {
            follower.serializedObject.ApplyModifiedProperties();
            SimpleFollowerAnim.Follow(sPData, _follower);

        }

        EditorGUI.BeginChangeCheck();
        var speed = EditorGUILayout.FloatField("Speed", follower.FindPropertyRelative("Speed").floatValue);
        if (EditorGUI.EndChangeCheck())
        {
            if (speed < 0) speed = 0;

            follower.FindPropertyRelative("Speed").floatValue = speed;
            follower.serializedObject.ApplyModifiedProperties();
        }


        EditorGUI.BeginChangeCheck();
        var Accel_Time = EditorGUILayout.FloatField("Full Speed Time", follower.FindPropertyRelative("TimeToReachFullSpeed").floatValue);
        if (EditorGUI.EndChangeCheck())
        {
            if (Accel_Time < 0) Accel_Time = 0;
            follower.FindPropertyRelative("TimeToReachFullSpeed").floatValue = Accel_Time;
            follower.serializedObject.ApplyModifiedProperties();
        }

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(
            follower.FindPropertyRelative("_FollowerAnimation"), new GUIContent("Animation type"));
        if (EditorGUI.EndChangeCheck())
        {
            follower.serializedObject.ApplyModifiedProperties();
        }

        GUI.enabled = (follower.FindPropertyRelative("_FollowerAnimation").enumValueIndex == (int)FollowerAnimation.Keyboard) ? true : false;

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(follower.FindPropertyRelative("FlipDirection"), new GUIContent("Flip Direction"));
        if (EditorGUI.EndChangeCheck())
        {
            follower.serializedObject.ApplyModifiedProperties();
            SimpleFollowerAnim.Follow(sPData, _follower);
        }


        GUI.enabled = true;

        EditorGUILayout.EndVertical();
    }
}
