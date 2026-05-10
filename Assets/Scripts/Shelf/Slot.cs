using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Slot : MonoBehaviour
{
    Item currentItem;
    RectTransform rectTransform;
    Shelf shelf;
    public Queue<Item> behindItems = new Queue<Item>();

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public RectTransform GetRectTransform()
    {
        return rectTransform;
    }

    public Item GetCurrentItem()
    {
        return currentItem;
    }

    public void SetCurrentItem(Item _item)
    {
        currentItem = _item;
        shelf.CheckInnerSlots();
    }

    public void SetBehindItem(Item _item)
    {
        behindItems.Enqueue(_item);
    }

    public void CheckBehindItems()
    {
        if (behindItems.Count > 0)
        {
            currentItem = behindItems.Dequeue();
            currentItem.transform.SetParent(GridManager.main.GetWrapperTransform());
            currentItem.transform.SetParent(GridManager.main.GetParentItemsContainer());            
            currentItem.EnableDraggable(true);
            currentItem.GetComponent<Image>().color = new Color(255, 255, 255, 1);
            currentItem.AnimationPlay("Placed", true);
        }
    }

    public int GetBehindItemsCount()
    {
        return behindItems.Count;
    }

    public bool IsEmpty()
    {
        return currentItem == null;
    }

    public void RemoveCurrentItem()
    {
        currentItem = null;
        shelf.CheckInnerSlots();
    }

    public void SetShelf(Shelf _shelf)
    {
        shelf = _shelf;
    }
}
