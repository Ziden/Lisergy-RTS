using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PartySlotButton : MonoBehaviour, IDeselectHandler
{
    public Action DeselectCallback;

    public void OnDeselect(BaseEventData eventData)
    {
        DeselectCallback?.Invoke();
    }
}
