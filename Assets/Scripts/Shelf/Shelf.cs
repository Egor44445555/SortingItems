using UnityEngine;
using System.Collections.Generic;

public class Shelf : MonoBehaviour
{
    [SerializeField] GameObject slotPrefab;

    List<Slot> innerSlots = new List<Slot>();
    
    void Start()
    {
        for (var i = 0; 3 > i; i++)
        {
            GameObject slot = Instantiate(slotPrefab, Vector3.zero, Quaternion.identity);
            slot.transform.SetParent(this.transform);
            innerSlots.Add(slot.GetComponent<Slot>());
        }
    }

    public List<Slot> GetInnerSlots()
    {
        return innerSlots;
    }
}
