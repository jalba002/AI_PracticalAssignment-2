using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;

//external agent to store all the way points shared by the entities in the scene
public class GameGlobalBB : Singleton<GameGlobalBB>
{
    public GameObject[] wayPoints { get; private set; }

    void Awake()
    {
        wayPoints = GameObject.FindGameObjectsWithTag("WAYPOINT");
    }

    public GameObject GetRandomWanderPoint()
    {
        return wayPoints[UnityEngine.Random.Range(0, wayPoints.Length)];
    }
}
