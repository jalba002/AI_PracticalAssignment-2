using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;

public class AntGlobalBB : Singleton<AntGlobalBB>
{
    [Header("Ant FSM Global parameters")]
    public string exitTag;
    public GameObject[] exitPoints;

    public float objectReachedRadius = 2f;

    void Awake()
    {
        if (exitPoints.Length <= 0) exitPoints = GameObject.FindGameObjectsWithTag(exitTag);
    }
}
