using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Pathfinding;
using Steerings;
using UnityEngine;
using Random = System.Random;

public class AntSpawner : MonoBehaviour
{
    [Header("Resources")] public GameObject seedAnt;
    public GameObject eggAnt;

    [Header("Spawning settings")] private byte seedChance = 80;

    private float spawnTimer = 15f;
    private float currentTimer;

    [Header("Random properties")] System.Random randomNumber = new System.Random(Guid.NewGuid().GetHashCode());

    void Start()
    {
        // get the prefabs
        //Can be loaded from inspector or automatically
        try
        {
            if (seedAnt == null) seedAnt = Resources.Load<GameObject>("SEED_ANT");
            if (eggAnt == null) eggAnt = Resources.Load<GameObject>("EGG_ANT");
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

        currentTimer = 1f;
    }

    void Update()
    {
        if (SpawnAnt())
        {
            currentTimer = spawnTimer + randomNumber.Next(-10, 10);
        }
        else if(currentTimer > 0)
        {
            currentTimer -= Time.deltaTime;
        }
    }

    private bool SpawnAnt()
    {
        if (currentTimer <= 0f)
        {
            Instantiate(randomNumber.Next(0, 101) < seedChance ? seedAnt : eggAnt,
                this.gameObject.transform.position,
                Quaternion.identity);
            return true;
        }

        return false;
    }
}