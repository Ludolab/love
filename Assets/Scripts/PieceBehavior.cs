﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using UnityEngine.Networking;

public class PieceBehavior : NetworkBehaviour
{
    [SerializeField]
    float speed = 2f;

    Vector3 anchor;
    public GameObject matchedGrid;
    public Collider col;
    GameObject player;

    Vector3 stopPos;

    bool isMatch;
    public void SetIsMatch(bool _isMatch)
    {
        isMatch = _isMatch;
    }

    public bool GetIsMatch()
    {
        return isMatch;
    }

    public bool isInteracting;

    bool isAbsorbed;
    public void SetIsAbsorbed(bool _isAbsorbed)
    {
        isAbsorbed = _isAbsorbed;
    }

    public bool GetIsAbsorbed()
    {
        return isAbsorbed;
    }

    bool enableMatch;
    public bool GetEnableMatch()
    {
        return enableMatch;
    }

    AudioSource audioSource;
    [SerializeField]
    AudioClip matchSound;

    [HideInInspector]
    public bool isSelected;

    void Start()
    {
        anchor = GameSingleton.instance.anchor;
        col = GetComponent<Collider>();
        player = GameObject.FindGameObjectWithTag("MainCamera");
        audioSource = GetComponent<AudioSource>();
        isSelected = false;
    }

    void Update()
    {
        if (isMatch)
        {
            Match();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (player.GetComponent<PlayerBehaviorNetworking>().GetIsSnapped() && isSelected)
        {
            if ((this.gameObject.tag == "piece1" && other.gameObject.tag == "grid1")
            || (this.gameObject.tag == "piece2" && other.gameObject.tag == "grid2")
            || (this.gameObject.tag == "piece3" && other.gameObject.tag == "grid3")
            || (this.gameObject.tag == "piece4" && other.gameObject.tag == "grid4"))
            {
                if (matchedGrid && matchedGrid != other.gameObject)
                {
                    matchedGrid.GetComponent<WallGrid>().isHovering = false;
                    matchedGrid.GetComponent<WallGrid>().triggerHover = false;
                    matchedGrid = null;
                }

                matchedGrid = other.gameObject;
                enableMatch = true;
                matchedGrid.GetComponent<WallGrid>().isHovering = false;
                matchedGrid.GetComponent<WallGrid>().triggerHover = true;
                Debug.Log("can be matched with " + matchedGrid);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (matchedGrid && !enableMatch)
        {
            enableMatch = true;
        }
        if (matchedGrid && !matchedGrid.GetComponent<WallGrid>().triggerHover)
        {
            matchedGrid.GetComponent<WallGrid>().isHovering = false;
            matchedGrid.GetComponent<WallGrid>().triggerHover = true;
        }
        Debug.Log("on trigger stay: " + matchedGrid);
    }

    void OnTriggerExit(Collider other)
    {
        if (matchedGrid && matchedGrid.GetComponent<WallGrid>().triggerHover)
        {
            enableMatch = false;
            matchedGrid.GetComponent<WallGrid>().isHovering = false;
            matchedGrid.GetComponent<WallGrid>().triggerHover = false;
            //matchedGrid = null;
        }
    }

    void Float()
    {
        transform.RotateAround(anchor, Vector3.up, Time.deltaTime * speed);
    }

    void Stop()
    {
        speed = 0f;
    }

    void Match()
    {
        StartCoroutine(Absorb());
        matchedGrid.GetComponent<WallGrid>().triggerHover = false;
        //this.GetComponent<PieceHover>().Final();
    }

    IEnumerator Absorb()
    {
        isMatch = false;
        transform.parent = null;
        isAbsorbed = true;
        matchedGrid.GetComponent<WallGrid>().hasPiece = true;

        speed = 0f;
        float lerpTime = 0f;
        float absorbSpeed = 0.5f;

        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(matchSound);
        }

        while (true)
        {
            lerpTime += Time.deltaTime * absorbSpeed;

            if (lerpTime >= 1f)
            {
                yield break;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, matchedGrid.transform.position, lerpTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, lerpTime);
            }
            yield return null;
        }
    }


}
