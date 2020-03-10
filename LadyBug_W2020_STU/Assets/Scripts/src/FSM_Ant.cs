using System;
using System.Collections;
using System.Collections.Generic;
using FSM;
using Pathfinding;
using Steerings;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Random = System.Random;

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
    private System.Random randomNumber = new System.Random(Guid.NewGuid().GetHashCode());

    private Path transportPath;
    private Path exitPath;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        transportingObject = GetComponentInChildren<Transform>().gameObject;

        Vector3 transportingPosition = AntBlackboard.Instance.wayPoints[randomNumber.Next(0, AntBlackboard.Instance.wayPoints.Length)].transform.position; //Select a random point from WAYPOINTS
        
        transportPath = seeker.StartPath(this.gameObject.transform.position, GeneratePoint(transportingPosition)); //Calculate first path.
        exitPath = seeker.StartPath(transportingPosition, transportingPosition); //Calculate second path.
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
                ChangeState(State.Transporting);
                break;
            case State.Transporting:
                //TODO If point reached, leave the transporting object and change to exit.
                break;
            case State.Exiting:
                //TODO if exitpoint reached, destroy this.
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ChangeState(State newState)
    {
        switch (currentState) //Exit logic
        {
            case State.Initial:
                break;
            case State.Transporting:
                pathFollowing.enabled = false;
                //TODO remove egg from child?
                break;
            case State.Exiting:
                pathFollowing.enabled = false;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        switch (newState) //Enter logic
        {
            case State.Initial:
                break;
            case State.Transporting:
                //pathFeeder.target = antBlackboard.;
                pathFollowing.enabled = true;
                pathFollowing.path = transportPath;
                break;
            case State.Exiting:
                //pathFeeder.target = antBlackboard.exitPoints[randomNumber.Next(0, antBlackboard.exitPoints.Length)];
                pathFollowing.enabled = true;
                pathFollowing.path = exitPath;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        currentState = newState;
    }

    Vector3 GeneratePoint(Vector3 worldPosition)
    {
        var pointGraph = AstarPath.active.data.FindGraph(g => g.name == "AntGraph");
        var node = pointGraph.GetNearest(worldPosition).node;
        Vector3 returnValue = (Vector3) node.position;

        return returnValue;
    }
}