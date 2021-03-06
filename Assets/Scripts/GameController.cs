﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GoogleARCore;
using TMPro;

public class GameController : NetworkBehaviour {

    [SerializeField] PlaneController planeController;

    [SerializeField] GameObject[] piecesPrefab;
    [SerializeField] int piecesNum;

    [SerializeField] GameObject wallPrefab;

    // wall height
    [SerializeField] float height = 1.2f;
    bool didSpawn;

    // when add new objects, use unity editor to add prefabs
    [SerializeField] GameObject[] spawnablesPrefab;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip spawnSound;

    void Start ()
    {
        if (!isServer)
        {
            this.enabled = false;
            Debug.Log("not server");
            return;
        }

        Debug.Log("is server");
    }
	
    void Update ()
    {
        if (planeController.GetHasPlaneFound() && !didSpawn)
        {
            // server spawns pieces and wall
            SpawnPieces();
            SpawnWall();
            //Spawnables();
            didSpawn = true;
        }
	}

    void SpawnPieces()
    {
        // spawn pieces
        for (int i = 0; i < piecesNum; i++)
        {
            float yRange = Random.Range(0, 1.5f);
            float xRange = Random.Range(-2f, 2f);
            float zRange = Random.Range(-2f, 2f);
            int index = i % piecesPrefab.Length;
            GameObject pieces = Instantiate(piecesPrefab[index], GameSingleton.instance.anchor + new Vector3(xRange, yRange, zRange), Random.rotation);

            NetworkServer.Spawn(pieces);
            // store piece list
            GameSingleton.instance.spawnedPieces.Add(pieces);
            Debug.Log("spawn");
        }

        // store piece num info
        GameSingleton.instance.PieceNum(piecesNum);
        // allow snapping interaction
        GameSingleton.instance.AllowSnap(true);
    }

    void SpawnWall()
    {
        GameObject wall = Instantiate(wallPrefab, GameSingleton.instance.anchor + new Vector3(0f, height, 0f), Quaternion.identity);
        NetworkServer.Spawn(wall);

        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(spawnSound);
        }

        Debug.Log("wall");
    }

    // when add new spawn objects, use this function
    void Spawnables()
    {
        for (int i = 0; i < spawnablesPrefab.Length; i++)
        {
            GameObject spawnables = Instantiate(spawnablesPrefab[i]);
            NetworkServer.Spawn(spawnables);
        }
    }
}
