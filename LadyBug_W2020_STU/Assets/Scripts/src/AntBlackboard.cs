using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;

public class AntBlackboard : Singleton<AntBlackboard>
{
    public string exitTag;
    public GameObject[] exitPoints;
    public GameObject[] wayPoints { get; private set; }
    void Awake()
    {
        if (exitPoints.Length <= 0) exitPoints = GameObject.FindGameObjectsWithTag(exitTag);
        wayPoints = GameObject.FindGameObjectsWithTag("WAYPOINT");
    }

}
