using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    [SerializeField] private GameObject stack;
    private bool passed = false;
    public bool IsPassed {  get { return passed; } }

    public void Pass()
    {
        if (!passed) 
        {
            passed = true;
            stack.SetActive(false);
        }
    }
}
