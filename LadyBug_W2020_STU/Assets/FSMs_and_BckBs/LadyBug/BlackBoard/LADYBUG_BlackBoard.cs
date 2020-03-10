using UnityEngine;
using System.Collections;

public class LADYBUG_BlackBoard : MonoBehaviour
{
    [Header("LadyBug Patrolling FSM parameters")]
    public float detectableEggRadius = 50.0f;
    public float wanderReachedRadius = 2.0f;

    void Awake()
    {

    }
}