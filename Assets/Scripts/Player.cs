using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform stackHolder;
    [SerializeField] private Transform raycastPoint;
    [SerializeField] float raycastDistance = 20f;
    private LayerMask wallLayer;
    private LayerMask pathLayer;
    private LayerMask bridgeLayer;
    private LayerMask finishLayer;
    [SerializeField] private Transform model;
    [SerializeField] private Level level;
    [SerializeField] private float moveSpeed;
    private int stackCount = 0;

    private string currentAnimName;
    public enum Direction
    {
        None,
        Forward,
        Backward,
        Left,
        Right,
    }

    private Direction direction;
    private Vector3 moveDirection;

    private void Awake()
    {

    }

    public void OnInit(Vector3 position, LayerMask wallLayer, LayerMask pathLayer, LayerMask bridgeLayer, LayerMask finishLayer)
    {
        direction = Direction.None;
        SwipeDetection.Instance.SwipePerformed += HandleSwipe;
        transform.position = position;
        this.wallLayer = wallLayer;
        this.pathLayer = pathLayer;
        this.bridgeLayer = bridgeLayer;
        this.finishLayer = finishLayer;
        AddStack();
    }

    private void HandleSwipe(Direction direction)
    {
        StartCoroutine(SetMove(direction));
    }

    private void FixedUpdate()
    {
        if (direction != Direction.None) 
        {
            Move(direction);
        }
    }

    private void Move(Direction direction)
    {
        if (GameManager.GetInstance().IsState(GameState.Setting))
        {
            return;
        }
        transform.Translate(moveDirection * moveSpeed * Time.fixedDeltaTime);
        CheckCollision();

        RaycastHit hit;
        if (Physics.Raycast(raycastPoint.position, moveDirection, out hit, raycastDistance, wallLayer))
        {

            // Change the color of the raycast to red
            Debug.DrawRay(raycastPoint.position, hit.point - raycastPoint.position, Color.red);
        }
        else
        {
            // Draw the green raycast
            Debug.DrawRay(raycastPoint.position, moveDirection * raycastDistance, Color.green);
        }
    }

    private Vector3 GetMoveVector(Direction direction)
    {
        Vector3 moveDirection = Vector3.zero;

        switch (direction)
        {
            case Direction.Forward:
                moveDirection = raycastPoint.forward;
                break;
            case Direction.Backward:
                moveDirection = -raycastPoint.forward;
                break;
            case Direction.Left:
                moveDirection = -raycastPoint.right;
                break;
            case Direction.Right:
                moveDirection = raycastPoint.right;
                break;

        }
        return moveDirection;
    }

    private IEnumerator SetMove(Direction direction)
    {
        if (this.direction == Direction.None)
        {
            yield return null;
            this.direction = direction;
            moveDirection = GetMoveVector(direction);

            moveDirection.y = 0f;
            moveDirection.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            model.DORotateQuaternion(targetRotation, 1f / 5f)
                .SetEase(Ease.Linear)
                .SetUpdate(true);
        }
    }
    
    protected void ChangeAnim(string animName)
    {
        if (currentAnimName != animName)
        {
            animator.ResetTrigger(animName);
            currentAnimName = animName;
            animator.SetTrigger(currentAnimName);
        }
    }

    private void CheckCollision()
    {
        RaycastHit hit;
        Vector3 moveDirection = GetMoveVector(direction);

        if (Physics.Raycast(raycastPoint.position, moveDirection, out hit, raycastDistance))
        {
            Vector3 hitPos = hit.point;
            GameObject hitObj = hit.transform.gameObject;
            // Check if the raycast hit a wall
            if ((1 << hitObj.layer) == wallLayer)
            {
                direction = Direction.None;
                transform.position = hitPos - 0.5f * moveDirection;
                ChangeAnim("idle");
            } else if ((1 << hitObj.layer) == bridgeLayer)
            {
                Bridge bridge;
                if (!level.passedBridge.ContainsKey(hitObj))
                {
                    bridge = hitObj.GetComponent<Bridge>();
                    level.passedBridge.Add(hitObj, bridge);
                }
                else 
                {
                    bridge = level.passedBridge[hitObj];
                }

                if (stackCount > 0 && !bridge.IsPass) 
                {
                    bridge.Pass();
                    RemoveStack();
                }

                if (stackCount <= 0 && !bridge.IsPass)
                {
                    direction = Direction.None;
                    transform.position = hitPos - 0.5f * moveDirection;
                    ChangeAnim("idle");
                }
            } else if ((1 << hitObj.layer) == pathLayer)
            {
                Path path;
                if (!level.passedPath.ContainsKey(hitObj))
                {
                    ChangeAnim("jump");
                    path = hitObj.GetComponent<Path>();
                    level.passedPath.Add(hitObj, path);
                    path.Pass();
                    AddStack();
                    level.UpdateScore();
                }
            }
            else if ((1 << hitObj.layer) == finishLayer)
            {
                direction = Direction.None;
                ChangeAnim("finish");
                level.OnFinish();
            }
        }
    }

    private void AddStack()
    {
        stackCount++;
        GameObject newBrick = level.pool.GetPooledObject().gameObject;
        newBrick.SetActive(true);
        newBrick.transform.parent = stackHolder.transform;

        float yPosition = 0.15f + 0.3f * (stackCount - 1);
        newBrick.transform.localPosition = new Vector3(0f, yPosition, 0f);

        newBrick.transform.SetSiblingIndex(0);

        model.transform.localPosition = new Vector3(0, 0.3f * stackCount, 0);
    }

    private void RemoveStack()
    {
        if (stackCount > 0)
        {
            stackCount--;

            // Get the first child (the top brick in the stack)
            Stack topBrick = stackHolder.transform.GetChild(0).GetComponent<Stack>();

            // Return the brick to the pool
            level.pool.ReturnObject(topBrick);

            // Move the player's model transform down by 0.3 units
            model.Translate(Vector3.down * 0.3f);
        }
    }

    public void OnDespawn()
    {
        SwipeDetection.Instance.SwipePerformed -= HandleSwipe;
    }
}
