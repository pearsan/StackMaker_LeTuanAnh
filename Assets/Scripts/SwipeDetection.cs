using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwipeDetection : MonoBehaviour
{
    public static SwipeDetection Instance;

    public delegate void Swipe(Player.Direction direction);

    public event Swipe SwipePerformed;

    [SerializeField] private InputAction position, press;

    [SerializeField] private float swipeResistance;

    private Vector2 initialPos;

    private Vector2 currentPos => position.ReadValue<Vector2>();

    private void Awake()
    {
        if (Instance == null)
        { 
            Instance = this; 
        }
        else 
        {
            Destroy(gameObject);
        }
        position.Enable();
        press.Enable();
        press.performed += _ => { initialPos = currentPos; };
        press.canceled += _ => DectecSwipe();
    }

    private void DectecSwipe()
    {
        Vector2 delta = currentPos - initialPos;
        Player.Direction swipeDirection = Player.Direction.None;
        if (delta.magnitude > swipeResistance)
        {
            delta.Normalize();
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                if (delta.x > 0)
                {
                    swipeDirection = Player.Direction.Right;
                }
                else
                {
                    swipeDirection = Player.Direction.Left;
                }
            }
            else 
            {
                if (delta.y > 0)
                {
                    swipeDirection = Player.Direction.Forward;
                }
                else
                {
                    swipeDirection = Player.Direction.Backward;
                }
            }
        }
        if (swipeDirection != Player.Direction.None && SwipePerformed != null)
        {
            SwipePerformed(swipeDirection);
        }
    }
}
