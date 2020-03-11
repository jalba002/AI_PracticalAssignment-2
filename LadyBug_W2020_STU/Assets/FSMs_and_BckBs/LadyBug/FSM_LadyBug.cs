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
        public enum State { Initial, Wandering, ReachingSeed, TransportingSeed, ReachingEgg, TransportingEgg };

        [Header("Required steerings")] private PathFeeder pathFeeder;
        private LADYBUG_BlackBoard blackBoard;

        [Header("State configuration")] public State currentState = State.Initial;

        private GameObject wanderPoint;
        private GameObject storePoint;
        private GameObject hatchingPoint;
        private GameObject seed;
        private GameObject egg;
        private GameObject otherEgg;

        private int ID; // 1 = egg ; 2 = seed
        private bool seedDropped; //seed dropped by lady bug

        void Start()
        {
            pathFeeder = GetComponent<PathFeeder>();
            blackBoard = GetComponent<LADYBUG_BlackBoard>();

            pathFeeder.enabled = false;
            pathFeeder.target = null;
            storePoint = blackBoard.GetRandomStorePoint();
            hatchingPoint = blackBoard.GetRandomHatchingPoint();
            seedDropped = false;

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

                    egg = SensingUtils.FindInstanceWithinRadius(gameObject, "EGG", blackBoard.eggDetectedRadius);
                    if (egg != null)
                    {
                        ChangeState(State.ReachingEgg);
                        break;
                    }

                    if (egg == null)
                    {
                        egg = SensingUtils.FindRandomInstanceWithinRadius(gameObject, "EGG", blackBoard.eggRandomDetectedRadius);
                        if (egg != null)
                        {
                            ChangeState(State.ReachingEgg);
                            break;
                        }

                        seed = SensingUtils.FindInstanceWithinRadius(gameObject, "SEED", blackBoard.seedDetectedRadius);
                        if (seed != null)
                        {
                            ChangeState(State.ReachingSeed);
                            break;
                        }

                        if (seed == null)
                        {
                            seed = SensingUtils.FindRandomInstanceWithinRadius(gameObject, "SEED", blackBoard.seedRandomDetectedRadius);
                            if (seed != null)
                                ChangeState(State.ReachingSeed);
                            break;
                        }
                    }
                    break;
                case State.ReachingSeed:

                    egg = SensingUtils.FindInstanceWithinRadius(gameObject, "EGG", blackBoard.eggLastChanceRadius);
                    if (egg != null)
                    {
                        ChangeState(State.ReachingEgg);
                        break;
                    }

                    if (seed.tag != "SEED")
                    {
                        ChangeState(State.Wandering);
                        break;
                    }

                    if (SensingUtils.DistanceToTarget(gameObject, seed) < blackBoard.seedReachedRadius)
                    {
                        ChangeState(State.TransportingSeed);
                        break;
                    }
                    break;
                case State.TransportingSeed:

                    egg = SensingUtils.FindInstanceWithinRadius(gameObject, "EGG", blackBoard.eggLastChanceRadius);
                    if (egg != null)
                    {
                        seedDropped = true;
                        ChangeState(State.ReachingEgg);
                        break;
                    }

                    if (SensingUtils.DistanceToTarget(gameObject, storePoint) < blackBoard.storeReachedRadius)
                    {
                        ChangeState(State.Wandering);
                        break;
                    }
                    break;
                case State.ReachingEgg:

                    if (egg != null)
                    {
                        if (egg.tag != "EGG")
                        {
                            ChangeState(State.Wandering);
                            break;
                        }

                        otherEgg = SensingUtils.FindInstanceWithinRadius(gameObject, "EGG", blackBoard.eggDetectedRadius);
                        if (FindEgg(egg, otherEgg))
                        {
                            egg = null;
                            break;
                        }
                    }
                    else
                    {
                        if (otherEgg.tag != "EGG")
                        {
                            ChangeState(State.Wandering);
                            break;
                        }

                        egg = SensingUtils.FindInstanceWithinRadius(gameObject, "EGG", blackBoard.eggDetectedRadius);
                        if (FindEgg(otherEgg, egg))
                        {
                            otherEgg = null;
                            break;
                        }
                    }

                    if (SensingUtils.DistanceToTarget(gameObject, pathFeeder.target) < blackBoard.eggReachedRadius)
                    {
                        ChangeState(State.TransportingEgg);
                        break;
                    }
                    break;
                case State.TransportingEgg:
                    if (SensingUtils.DistanceToTarget(gameObject, hatchingPoint) < blackBoard.hatchingReachedRadius)
                    {
                        ChangeState(State.Wandering);
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
                case State.ReachingSeed:
                    pathFeeder.enabled = false;
                    pathFeeder.target = null;
                    break;
                case State.TransportingSeed:
                    if (seedDropped)
                        seed.tag = "SEED";
                    else
                        seed.tag = "SEED_ON_STOREPOINT";
                    seedDropped = false;
                    seed.transform.parent = null;
                    pathFeeder.enabled = false;
                    pathFeeder.target = null;
                    storePoint = blackBoard.GetRandomStorePoint();
                    break;
                case State.ReachingEgg:
                    pathFeeder.enabled = false;
                    pathFeeder.target = null;
                    break;
                case State.TransportingEgg:
                    egg.tag = "EGG_ON_HATCHINGPOINT";
                    egg.transform.parent = null;
                    pathFeeder.enabled = false;
                    pathFeeder.target = null;
                    hatchingPoint = blackBoard.GetRandomHatchingPoint();
                    break;
            }

            // enter logic
            switch (newState)
            {
                case State.Wandering:
                    wanderPoint = GameBlackboard.Instance.GetRandomWanderPoint();
                    pathFeeder.target = wanderPoint;
                    pathFeeder.enabled = true;
                    break;
                case State.ReachingSeed:
                    pathFeeder.target = seed;
                    pathFeeder.enabled = true;
                    break;
                case State.TransportingSeed:
                    pathFeeder.target = storePoint;
                    pathFeeder.enabled = true;
                    seed.tag = "SEED_ON_LADYBUG";
                    seed.transform.parent = gameObject.transform;
                    break;
                case State.ReachingEgg:
                    SetFeederTarget(egg);
                    pathFeeder.enabled = true;
                    break;
                case State.TransportingEgg:
                    pathFeeder.target = hatchingPoint;
                    pathFeeder.enabled = true;
                    egg.tag = "EGG_ON_LADYBUG";
                    egg.transform.parent = gameObject.transform;
                    break;
            }
            currentState = newState;
        }

        bool FindEgg(GameObject currentEgg, GameObject newEgg)
        {
            if (newEgg != null && newEgg != currentEgg && SensingUtils.DistanceToTarget(gameObject, newEgg) < SensingUtils.DistanceToTarget(gameObject, currentEgg))
            {
                SetFeederTarget(newEgg);
                return true;
            }
            return false;
        }

        void SetFeederTarget(GameObject target)
        {
            pathFeeder.target = target;
        }
    }
}