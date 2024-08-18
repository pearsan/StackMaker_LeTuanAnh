using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    [SerializeField]private bool passed = false;
    public bool IsPass
    {
        get { return passed; }   
    }
    [SerializeField] private GameObject stack;
    
    public void Pass()
    {
        if (!passed)
        {
            stack.SetActive(true);
            passed = true;
        }
    }
}
