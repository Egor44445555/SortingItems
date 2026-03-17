using UnityEngine;
using System.Collections.Generic;

public class Shelf : MonoBehaviour
{
    [SerializeField] GameObject slotPrefab;

    List<Slot> innerSlots = new List<Slot>();
    
    void Start()
    {
        for (int i = 0; 3 > i; i++)
        {
            GameObject slot = Instantiate(slotPrefab, Vector3.zero, Quaternion.identity, transform);
            Slot slotComponent = slot.GetComponent<Slot>();
            slotComponent.SetShelf(this);
            innerSlots.Add(slotComponent);

            if (GridManager.main != null)
            {
                GridManager.main.SetAllSlots(slotComponent);
            }
        }
    }

    public List<Slot> GetInnerSlots()
    {
        return innerSlots;
    }

    public void CheckInnerSlots()
    {        
        Item item1 = innerSlots[0].GetCurrentItem();
        Item item2 = innerSlots[1].GetCurrentItem();
        Item item3 = innerSlots[2].GetCurrentItem();
        
        if (item1 == null || item2 == null || item3 == null)
        {
            OnSlotsMatched?.Invoke(false, null);
            return;
        }
        
        bool allItemsMatch = item1.GetNameItem() == item2.GetNameItem() && item2.GetNameItem() == item3.GetNameItem();
        
        if (allItemsMatch)
        {
            OnSlotsMatched?.Invoke(true, item1);
            ClearInnerSlots();
        }
        else
        {
            OnSlotsMatched?.Invoke(false, null);
        }
    }

    public event System.Action<bool, Item> OnSlotsMatched;

    public void ClearInnerSlots()
    {
        UIManager.main.PlayCollectedEffectEffect();

        foreach (var slot in innerSlots)
        {
            Item itemComponent = slot.GetCurrentItem().GetComponent<Item>();
            GridManager.main.RemoveItemInArray(itemComponent);
            itemComponent.DestroyItem();
            slot.RemoveCurrentItem();
        }
    }
}
