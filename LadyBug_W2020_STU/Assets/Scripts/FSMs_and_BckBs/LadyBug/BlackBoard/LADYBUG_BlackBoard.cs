using UnityEngine;
using System.Collections;

public class LADYBUG_BlackBoard : MonoBehaviour
{
    [Header("LadyBug FSM parameters")]
    public float eggDetectedRadius = 50.0f;
    public float eggReachedRadius = 2.0f;
    public float eggRandomDetectedRadius = 180.0f;
    public float eggLastChanceRadius = 25.0f;
    public float seedDetectedRadius = 80.0f;
    public float seedRandomDetectedRadius = 125.0f;
    public float seedReachedRadius = 2.0f;
    public float wanderReachedRadius = 2.0f;
    public float storeReachedRadius = 5.0f;
    public float hatchingReachedRadius = 5.0f;

    [Header("LadyBug FSM store and hatching points")]
    public GameObject[] storePoints;
    public GameObject[] hatchingPoints;

    void Awake()
    {
        storePoints = GameObject.FindGameObjectsWithTag("STOREPOINT");
        hatchingPoints = GameObject.FindGameObjectsWithTag("HATCHINGPOINT");
    }

    public GameObject GetRandomStorePoint()
    {
        return storePoints[Random.Range(0, storePoints.Length)];
    }

    public GameObject GetRandomHatchingPoint()
    {
        return hatchingPoints[Random.Range(0, hatchingPoints.Length)];
    }
}