using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Shelf : MonoBehaviour
{
    [SerializeField] GameObject slotPrefab;
    [SerializeField] int slotCount = 3;

    List<Slot> innerSlots = new List<Slot>();

    Image image;
    bool checkSlots = false;
    
    void Awake()
    {
        image = GetComponent<Image>();

        for (int i = 0; slotCount > i; i++)
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

    public void CheckEmptySlots()
    {
        int countEmptySlot = 0;
        
        foreach (Slot slot in innerSlots)
        {
            if (slot.IsEmpty())
            {
                countEmptySlot++;
            }
        }

        if (countEmptySlot == slotCount)
        {
            foreach (Slot slot in innerSlots)
            {
                slot.CheckBehindItems();
            }
        }
    }

    public void CheckInnerSlots()
    {
        checkSlots = true;        

        if (slotCount == 3)
        {
            Item item1 = innerSlots[0].GetCurrentItem();
            Item item2 = innerSlots[1].GetCurrentItem();
            Item item3 = innerSlots[2].GetCurrentItem();
            
            if (item1 == null || item2 == null || item3 == null)
            {
                return;
            }
            
            bool allItemsMatch = item1.GetNameItem() == item2.GetNameItem() && item2.GetNameItem() == item3.GetNameItem();
            
            if (allItemsMatch)
            {
                ClearInnerSlots();
            }
        }
    }

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
