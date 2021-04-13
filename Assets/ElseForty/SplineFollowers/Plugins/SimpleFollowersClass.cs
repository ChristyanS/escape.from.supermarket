using System.Collections.Generic;
using UnityEngine;

public class SimpleFollowersClass : MonoBehaviour
{
    public List<Follower> Followers = new List<Follower>();
    public SPData SPData;
    public int _FollowerIndex = 0;

    private void OnEnable()
    {
        SplineCreationClass.Update_Spline += UpdateSpline;
        SplinePlusAPI.Branch_Deleted += Branch_Deleted;
    }
    private void OnDisable()
    {
        SplineCreationClass.Update_Spline -= UpdateSpline;
        SplinePlusAPI.Branch_Deleted -= Branch_Deleted;
    }

    void UpdateSpline( )
    {
        SimpleFollowerAnim.Follow(SPData, Followers);
    }

    public void Branch_Deleted(int branchKey)
    {
        SimpleFollowerAnim.Follow(SPData, Followers);
    }
 
    private void Start()
    {
        SPData = GetComponent<SplinePlus>().sPData;

        SPData.Update();
        _FollowerIndex = 0;
        for (int i = 0; i < Followers.Count; i++)
        {
             Followers[i].Time = 0;
        }
    }
    
    private void Update()
    {
        SimpleFollowerAnim.Follow(SPData, Followers);
    }
}

[System.Serializable]
public class Follower
{
    public GameObject FollowerGO;

    public float Distance = 0;
    public float TimeToReachFullSpeed = 0;
    public float Time = 0;

    public FollowerAnimation _FollowerAnimation = FollowerAnimation.Auto;

    public Vector3 Position;
    public Vector3 Rotation;

    public float Speed = 2.5f;

    public bool IsForward = true;
    public bool Reverse = false;
    public Switch LockRotation = Switch.Off;
    public SpaceType SpaceType = SpaceType.Local;
    public Switch FlipDirection = Switch.On;
    public Switch Animation = Switch.On;

    public DistanceData DistanceData = new DistanceData();

    public int ToolBareSelection = 0;
}
 
[System.Serializable]
public struct DistanceData
{
    public int Index;
    public Vector3 Position;
    public Quaternion Rotation;
}

 