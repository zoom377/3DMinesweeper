using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : MonoBehaviour
{
    public int Width, Height, Depth, Percentage;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
