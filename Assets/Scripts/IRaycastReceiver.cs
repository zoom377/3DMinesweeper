using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IRaycastReceiver
{
    public void RaycastEnter();
    public void RaycastExit();
}
