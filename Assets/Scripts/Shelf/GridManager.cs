using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class GridManager : MonoBehaviour
{
    public static GridManager main;

    [SerializeField] GameObject shelfPrefab;
    [SerializeField] GameObject emptyShelfPrefab;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform itemsTransform;
    [SerializeField] List<int> shelvesArrayPosition;
    [SerializeField] List<SlotItemArray> slotItemArray;

    Transform parentTransform;
    List<Slot> allSlots = new List<Slot>();
    List<Item> allItems = new List<Item>();
    float timer = 0f;
    bool isCreateGrid = false;

    void Awake()
    {
        if (main == null)
        {
            main = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        parentTransform = FindObjectOfType<LevelManager>().transform;
        CreateGrid();
    }

    void Update()
    {
        if (!isCreateGrid)
        {
            timer += Time.deltaTime;
        }
        
        if (timer > 0.5f && !isCreateGrid)
        {
            CreateItems();
            isCreateGrid = true;
            timer = 0f;
        }
    }

    void CreateGrid()
    {
        for (var i = 0; i < 30; i++)
        {
            if (shelvesArrayPosition.Contains(i + 1))
            {
                GameObject shelfObj = Instantiate(shelfPrefab, transform);
                Shelf shelfComnponent = shelfObj.GetComponent<Shelf>();
            }
            else
            {
                Instantiate(emptyShelfPrefab, transform);
            }
        }
    }

    void CreateItems()
    {
        List<Slot> availableSlots = allSlots.Where(slot => slot.IsEmpty()).ToList();

        foreach (SlotItemArray item in slotItemArray)
        {
            for (int i = 0; item.amount > i; i++)
            {
                if (availableSlots.Count == 0) break;
            
                int randomIndex = Random.Range(0, availableSlots.Count);
                Slot selectedSlot = availableSlots[randomIndex];
                
                GameObject itemObj = Instantiate(itemPrefab, selectedSlot.transform.position, selectedSlot.transform.rotation, itemsTransform);
                itemObj.GetComponent<RectTransform>().sizeDelta = selectedSlot.GetComponent<RectTransform>().sizeDelta;
                itemObj.GetComponent<Image>().sprite = item.icon;
                
                Item itemComponent = itemObj.GetComponent<Item>();
                itemComponent.SetName(item.name);
                itemComponent.SetCurrentSlot(selectedSlot);
                selectedSlot.SetCurrentItem(itemComponent);
                allItems.Add(itemComponent);            
                availableSlots.RemoveAt(randomIndex);
            }
        }
    }

    public void SetAllSlots(Slot _slot)
    {
        allSlots.Add(_slot);
    }

    public List<Slot> GetAllSlots()
    {
        return allSlots;
    }

    public void RemoveItemInArray(Item _item)
    {
        allItems.Remove(_item);

        if (allItems.Count <= 0 && UIManager.main != null)
        {
            UIManager.main.EndLevel();
        }
    }

    public List<Item> GetAllItems()
    {
        return allItems;
    }

    public Transform GetWrapperTransform()
    {
        return parentTransform;
    }

    public Transform GetParentItemsTransform()
    {
        return itemsTransform;
    }
}
