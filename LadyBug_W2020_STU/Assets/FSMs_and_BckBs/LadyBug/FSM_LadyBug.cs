using System;
using System.Collections;
using System.Collections.Generic;
using FSM;
using Pathfinding;
using Steerings;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace FSM
{
    [RequireComponent(typeof(Seeker))]
    [RequireComponent(typeof(PathFollowing))]
    [RequireComponent(typeof(PathFeeder))]
    [RequireComponent(typeof(LADYBUG_BlackBoard))]

    public class FSM_LadyBug : FiniteStateMachine
    {
        public enum State { Initial, Wandering };

        [Header("Required steerings")] private PathFeeder pathFeeder;
        private LADYBUG_BlackBoard blackBoard;

        [Header("State configuration")] private State currentState = State.Initial;

        GameObject wanderPoint;

        void Start()
        {
            pathFeeder = GetComponent<PathFeeder>();
            blackBoard = GetComponent<LADYBUG_BlackBoard>();

            pathFeeder.enabled = false;
            pathFeeder.target = null;
            wanderPoint = GameBlackboard.Instance.GetRandomWanderPoint();
        }

        public override void Exit()
        {
            pathFeeder.enabled = false;
            base.Exit();
        }

        public override void ReEnter()
        {
            currentState = State.Initial;
            base.ReEnter();
        }

        void Update()
        {
            switch (currentState)
            {
                case State.Initial:
                    ChangeState(State.Wandering);
                    break;
                case State.Wandering:
                    if (SensingUtils.DistanceToTarget(gameObject, wanderPoint) < blackBoard.wanderReachedRadius)
                    {
                        wanderPoint = GameBlackboard.Instance.GetRandomWanderPoint();
                        pathFeeder.target = wanderPoint;
                        break;
                    }
                    break;
            }
        }

        private void ChangeState(State newState)
        {
            // exit logic
            switch (currentState)
            {
                case State.Wandering:
                    pathFeeder.enabled = false;
                    pathFeeder.target = null;
                    break;
            }

            // enter logic
            switch (newState)
            {
                case State.Wandering:
                    pathFeeder.target = wanderPoint;
                    pathFeeder.enabled = true;
                    break;
            }

            currentState = newState;
        }
    }
}