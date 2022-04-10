using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

[Serializable]
public class VRButtonEvent : UnityEvent { }

public class VRButton : MonoBehaviour
{
    public VRButtonEvent delayedEvent = new VRButtonEvent();

    public float interactionDelay = 1;
    public bool triggerActive=true;
    public void PointerEnter()
    {
        triggerActive=true;
        StartCoroutine(WaitToInterract());
    }
    public void PointerExit(){
        triggerActive=false;
        Debug.Log("will not invoked");
    }


    public IEnumerator WaitToInterract()
    {
        yield return new WaitForSeconds(interactionDelay);
        Debug.Log(triggerActive);
        if(triggerActive){   
            Debug.Log("Action Triggered.");
            delayedEvent.Invoke();
        }
        else{
            Debug.Log("Not Triggered");
        }
    }


}
