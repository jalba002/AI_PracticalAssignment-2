using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using FSM;
using Pathfinding;
using Steerings;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Random = System.Random;

[RequireComponent(typeof(PathFollowing))]
[RequireComponent(typeof(Seeker))]

public class FSM_Ant : FiniteStateMachine
{
    public enum State
    {
        Initial,
        Transporting,
        Exiting
    }

    [Header("Required objects")] private GameObject transportingObject;

    [Header("Required steerings")] private Seeker seeker;
    private PathFollowing pathFollowing;

    [Header("State configuration")] private State currentState;
    private System.Random randomNumber;

    private Vector3 deliverPosition;
    private Path transportPath;
    private Path exitPath;

    void Start()
    {
        randomNumber = new System.Random(Guid.NewGuid().GetHashCode());

        seeker = GetComponent<Seeker>();
        pathFollowing = GetComponent<PathFollowing>();

        transportingObject = this.gameObject.GetComponentsInChildren<Transform>()[1].gameObject;

        deliverPosition = GameGlobalBB.Instance.wayPoints[randomNumber.Next(0, GameGlobalBB.Instance.wayPoints.Length)].transform.position; //Select a random point from WAYPOINTS

        StartCoroutine(CalculateAllPaths());
        pathFollowing.enabled = false;
    }

    public override void ReEnter()
    {
        base.ReEnter();
        ChangeState(State.Initial);
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Initial:
                if (transportPath != null)
                {
                    ChangeState(State.Transporting);
                }
                break;
            case State.Transporting:
                //TODO If point reached, leave the transporting object and change to exit.
                if (Vector3.Distance(this.gameObject.transform.position, deliverPosition) <= AntGlobalBB.Instance.objectReachedRadius && exitPath != null)
                {
                    ChangeState(State.Exiting);
                    break;
                }
                break;
            case State.Exiting:
                //TODO if exitpoint reached, destroy this.
                if (Vector3.Distance(this.gameObject.transform.position, (Vector3)exitPath.path[exitPath.path.Count - 1].position) <= AntGlobalBB.Instance.objectReachedRadius)
                {
                    Destroy(this.gameObject);
                    break;
                }
                break;
        }
    }

    private void ChangeState(State newState)
    {
        switch (currentState) //Exit logic
        {
            case State.Initial:
                break;
            case State.Transporting:
                //TODO remove egg from child?
                pathFollowing.enabled = false;
                transportingObject.transform.parent = null;
                GraphNode node = AstarPath.active.GetNearest(transportingObject.transform.position, NNConstraint.Default).node;
                transportingObject.transform.position = (Vector3)node.position;
                transportingObject.tag = transportingObject.name.ToString().Equals("egg") ? "EGG" : "SEED";
                transportingObject = null;
                break;
            case State.Exiting:
                pathFollowing.enabled = false;
                break;
        }

        switch (newState) //Enter logic
        {
            case State.Initial:
                break;
            case State.Transporting:
                pathFollowing.wayPointReachedRadius = AntGlobalBB.Instance.objectReachedRadius;
                pathFollowing.currentWaypointIndex = 0;
                pathFollowing.path = transportPath;
                pathFollowing.enabled = true;
                break;
            case State.Exiting:
                pathFollowing.wayPointReachedRadius = AntGlobalBB.Instance.objectReachedRadius;
                pathFollowing.currentWaypointIndex = 0;
                pathFollowing.path = exitPath;
                pathFollowing.enabled = true;
                break;
        }

        currentState = newState;
    }

    Vector3 GeneratePoint(Vector3 worldPosition)
    {
        var pointGraph = AstarPath.active.data.FindGraph(g => g.name == "AntGraph");
        var node = pointGraph.GetNearest(worldPosition).node;
        Vector3 returnValue = (Vector3)node.position;
        return returnValue;
    }

    IEnumerator CalculateAllPaths()
    {
        transportPath = seeker.StartPath(this.gameObject.transform.position, GeneratePoint(deliverPosition)); //Calculate first path.
        yield return seeker.IsDone();
        exitPath = seeker.StartPath(GeneratePoint(deliverPosition), AntGlobalBB.Instance.exitPoints[randomNumber.Next(0, AntGlobalBB.Instance.exitPoints.Length - 1)].transform.position);
        yield return seeker.IsDone();
    }
}