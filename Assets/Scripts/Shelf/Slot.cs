using UnityEngine;

public class Slot : MonoBehaviour
{
    Item currentItem;
    RectTransform rectTransform;
    Shelf shelf;

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
