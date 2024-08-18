using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public StackPool pool;

    [SerializeField] private Player player;
    private Transform playerTransform;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Vector3 cameraOffset;
    
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask bridgeLayer;
    [SerializeField] private LayerMask pathLayer;
    [SerializeField] private LayerMask finishLayer;

    [SerializeField] private float smoothTime;
    [SerializeField] private Path spawnPath;
    [SerializeField] private Transform finishPos;
    private Vector3 velocity = Vector3.zero;

    public Dictionary<GameObject, Bridge> passedBridge;
    public Dictionary<GameObject, Path> passedPath;

    private int score = 0;

    public void OnInit()
    {
        passedBridge = new Dictionary<GameObject, Bridge>();
        passedPath = new Dictionary<GameObject, Path>();
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
        playerTransform = player.transform;
        player.OnInit(spawnPath.transform.position, wallLayer,pathLayer, bridgeLayer, finishLayer);
        spawnPath.Pass();
        score = 0;
        UIManager.GetInstance().UpdateScore(score);
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = playerTransform.position + cameraOffset;

        // Smoothly move the camera towards the target position
        cameraTransform.position = targetPosition;
    }

    public void OnFinish()
    {
        player.transform.position = finishPos.position;
        player.OnDespawn();
        GameManager.GetInstance().OnFinish();
    }

    public void OnDespawn()
    {
        player.OnDespawn();
        score = 0;
        UIManager.GetInstance().UpdateScore(score);
    }

    public void UpdateScore()
    {
        score = score + 1;
        UIManager.GetInstance().UpdateScore(score);
    }
}
