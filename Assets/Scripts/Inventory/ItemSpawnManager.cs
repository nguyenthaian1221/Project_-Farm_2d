﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CLASS USED TO DROP AND DRAG ITEMS OUTSIDE OF INVENTORY - NOT USED YET

public class ItemSpawnManager : MonoBehaviour
{
    public static ItemSpawnManager instance;

    private void Awake()
    {
        instance = this;
    }

    // initiate object on the scene
    [SerializeField] GameObject pickUpItemPrefab;

    public void SpawnItem(Vector3 position, Item item, int count)
    {
        GameObject o = Instantiate(pickUpItemPrefab, position, Quaternion.identity);

    }
}