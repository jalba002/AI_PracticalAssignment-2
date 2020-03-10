﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;

public class GameBlackboard : Singleton<GameBlackboard>
{
    public string exitTag;
    //public GameObject[] exitPoints;
    public GameObject[] wayPoints { get; private set; }

    void Awake()
    {
        //if (exitPoints.Length <= 0) exitPoints = GameObject.FindGameObjectsWithTag(exitTag);
        wayPoints = GameObject.FindGameObjectsWithTag("WAYPOINT");
    }

    public GameObject GetRandomWanderPoint()
    {
        return wayPoints[UnityEngine.Random.Range(0, wayPoints.Length)];
    }
}
