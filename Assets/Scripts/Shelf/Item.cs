using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

public class Item : MonoBehaviour
{
    [SerializeField] string name = "";

    Slot currentSlot;
    RectTransform rectTransform;
    Canvas canvas;
    
    
    void Start()
    {
        canvas = LevelManager.main.GetCanvas();
        rectTransform = GetComponent<RectTransform>();
    }

    public void FindEmptySlot()
    {
        var emptySlots = GridManager.main.GetAllSlots().Where(slot => slot.IsEmpty()).ToArray();
        
        if (emptySlots.Length == 0 && canvas == null) return;

        Slot nearestSlot = null;
        float minDistance = float.MaxValue;
        Vector2 mousePosition = Input.mousePosition;

        foreach (Slot slot in emptySlots)
        {
            Vector2 slotScreenPosition = Camera.main.WorldToScreenPoint(
                slot.GetRectTransform().position
            );
            
            float distance = Vector2.Distance(mousePosition, slot.GetRectTransform().position);
            
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestSlot = slot;
            }
        }

        if (nearestSlot != null)
        {
            SetCurrentSlot(nearestSlot);
            nearestSlot.SetCurrentItem(this);
        }
    }

    public void SetCurrentSlot(Slot _slot)
    {
        currentSlot = _slot;
        rectTransform.position = _slot.GetRectTransform().position;
    }

    public Slot GetCurrentSlot()
    {
        return currentSlot;
    }

    public void RemoveCurrentSlot()
    {
        currentSlot.RemoveCurrentItem();
        currentSlot = null;
    }
}
