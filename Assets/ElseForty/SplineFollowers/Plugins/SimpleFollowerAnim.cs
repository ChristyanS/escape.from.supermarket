using System.Collections.Generic;
using UnityEngine;

public class SimpleFollowerAnim
{
    public static void Follow(SPData SPData, Follower follower)
    {
        if (follower.Animation == Switch.Off || follower.FollowerGO == null) return;
 
        AnimationType(SPData, follower);
    }

    public static void Follow(SPData SPData, List<Follower> followers)
    {
        for (int i = 0; i < followers.Count; i++)
        {
            Follow(SPData, followers[i]);
        }
    }

    public static void AnimationType(SPData SPData, Follower follower)
    {
        var flipDir = (follower.FlipDirection == Switch.On) ? true : false;
        flipDir = (follower._FollowerAnimation == FollowerAnimation.Auto) ? false : flipDir;
        follower.DistanceData = DistanceDataClass.DataExtraction(SPData, follower, follower.IsForward, flipDir);


        if (follower._FollowerAnimation == FollowerAnimation.Keyboard) KeyboardAnimationType(SPData, follower);
        else AutoAnimated( follower);

        //transform assign
        TransformFollower(follower);
        if (DistanceCheck(SPData, follower))
        {
            var delta = follower.Distance - SPData.Length;
            follower.Distance = delta;
        }
    }

    static void AutoAnimated(  Follower follower)
    {
        if (Application.isPlaying  )
        {
            float Accel_Time = 1;
            if (follower.TimeToReachFullSpeed != 0)
            {
                if (follower.Time < follower.TimeToReachFullSpeed) follower.Time += Time.deltaTime;
                Accel_Time = Mathf.InverseLerp(0, follower.TimeToReachFullSpeed, follower.Time);
            }


            if (follower.Reverse) follower.Distance -= follower.Speed * Accel_Time  * Time.deltaTime;
            else follower.Distance += follower.Speed  * Accel_Time * Time.deltaTime;
        }
    }

    public static void KeyboardAnimationType(SPData sPData, Follower follower)
    {
        if (Input.GetKey(sPData.UpKey) )
        {
          
            sPData.KeyboardDirection =  KeyboardDirection.Forward;

            InputGravity(sPData,follower);
        }
 
        else if (Input.GetKey(sPData.DownKey) )
        {
   
            sPData.KeyboardDirection = KeyboardDirection.Backward;

            InputGravity(sPData,follower);
        }
 
        if (!Input.GetKey(sPData.UpKey) && !Input.GetKey(sPData.DownKey))
        {
     
            sPData.KeyboardDirection = KeyboardDirection.None;

            InputGravity(sPData,follower);
        }
 
        if (Application.isPlaying )
        {
 
            if (follower.IsForward)
            {
                if (follower.Reverse) follower.Distance -= SPData.KeyboardInputValue * follower.Speed *Time.deltaTime;
                else follower.Distance += SPData.KeyboardInputValue * follower.Speed *  Time.deltaTime;
            }
            else
            {
                if (follower.Reverse) follower.Distance += SPData.KeyboardInputValue * follower.Speed * Time.deltaTime;
                else follower.Distance -= SPData.KeyboardInputValue * follower.Speed *   Time.deltaTime;
            }
        }
    }
 
    public static bool DistanceCheck(SPData SPData, Follower follower)
    {
        var pathLength = SPData.Length;
        if (!follower.Reverse)
        {
            return (follower.Distance >= pathLength) ? true : false;
        }
        else
        {
            return (follower.Distance <= 0) ? true : false;
        }
    }

    public static void InputGravity(SPData sPData,Follower follower )
    {
        if (sPData.KeyboardDirection == KeyboardDirection.Forward)
        {
            if (!follower.IsForward && follower.Time >= 0)
            {
                follower.Reverse = !follower.Reverse;
                follower.IsForward = true;
            }
            if (follower.TimeToReachFullSpeed == 0)
            {
                SPData.KeyboardInputValue = 1;
                return;
            }


            if (follower.Time < 0) follower.Time += Time.deltaTime ;
            else if (follower.Time < follower.TimeToReachFullSpeed) follower.Time += Time.deltaTime;
        }
        else if (sPData.KeyboardDirection == KeyboardDirection.Backward)
        {
            if (follower.IsForward && follower.Time <= 0)
            {
                follower.Reverse = !follower.Reverse;
                follower.IsForward = false;
            }
            if (follower.TimeToReachFullSpeed == 0)
            {
                SPData.KeyboardInputValue = -1;
                return;
            }

            if (follower.Time > 0) follower.Time -= Time.deltaTime ;
            else if (follower.Time > -follower.TimeToReachFullSpeed) follower.Time -= Time.deltaTime;
        }
        else
        {
            if (follower.TimeToReachFullSpeed == 0)
            {
                SPData.KeyboardInputValue = 0;
                return;
            }

            if (follower.IsForward && follower.Time > 0)
            {
                follower.Time -= Time.deltaTime;
                if (follower.Time < 0) follower.Time = 0;
            }
            else if (!follower.IsForward && follower.Time < 0)
            {
                follower.Time += Time.deltaTime;
                if (follower.Time > 0) follower.Time = 0;
            }
        }
        SPData.KeyboardInputValue = Mathf.Lerp(-1, 1, Mathf.InverseLerp(-follower.TimeToReachFullSpeed,
            follower.TimeToReachFullSpeed, follower.Time));
    }

    public static void TransformFollower(Follower follower)
    {
        //transform assign
        follower.FollowerGO.transform.position = follower.DistanceData.Position;
        if (follower.SpaceType == SpaceType.Local) follower.FollowerGO.transform.Translate(follower.Position);
        else follower.FollowerGO.transform.position += follower.Position;

        if (follower.LockRotation == Switch.On) follower.FollowerGO.transform.rotation = Quaternion.Euler(follower.Rotation);
        else follower.FollowerGO.transform.rotation = follower.DistanceData.Rotation * Quaternion.Euler(follower.Rotation);
    }
 }
