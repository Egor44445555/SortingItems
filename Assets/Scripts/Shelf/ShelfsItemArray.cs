using System;
using UnityEngine;

[Serializable]
public class ShelfsItemArray
{
    public bool empty = false;    

    public ShelfsItemArray(bool _empty, SlotItemArray[] _slotItemArray)
    {
        empty = _empty;   
    }
}
