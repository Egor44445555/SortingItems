using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GridManager : MonoBehaviour
{
    public static GridManager main;

    [SerializeField] GameObject shelfPrefab;
    [SerializeField] GameObject emptyShelfPrefab;
    [SerializeField] Transform itemsTransform;
    [SerializeField] List<int> shelvesArrayPosition;
    [SerializeField] List<GameObject> slotItemArray;

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
        for (var i = 0; i < 20; i++)
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

        foreach (GameObject item in slotItemArray)
        {
            if (availableSlots.Count == 0) break;
            
            int randomIndex = Random.Range(0, availableSlots.Count);
            Slot selectedSlot = availableSlots[randomIndex];
            
            GameObject itemObj = Instantiate(item, selectedSlot.transform.position, selectedSlot.transform.rotation, itemsTransform);
            Item itemComponent = itemObj.GetComponent<Item>();
            itemComponent.SetCurrentSlot(selectedSlot);
            selectedSlot.SetCurrentItem(itemComponent);            
            allItems.Add(itemComponent);            
            availableSlots.RemoveAt(randomIndex);
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

        if (allItems.Count <= 0)
        {
            print("End level");
        }
    }

    public List<Item> GetAllItems()
    {
        return allItems;
    }
}
