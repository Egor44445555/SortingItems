using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class GridManager : MonoBehaviour
{
    public static GridManager main;

    [Header("Prefabs")]
    [SerializeField] GameObject shelfPrefab;
    [SerializeField] GameObject emptyShelfPrefab;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform itemsTransform;

    [Header("Level Configuration")]
    [SerializeField] LevelData currentLevelData;
    [SerializeField] List<Sprite> itemIconsDatabase;

    Transform parentTransform;
    List<Slot> allSlots = new List<Slot>();
    List<Item> allItems = new List<Item>();
    
    Shelf[] spawnedShelves = new Shelf[32];

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
        if (currentLevelData == null)
        {
            Debug.LogError("LevelData не назначена в GridManager!");
            return;
        }

        parentTransform = FindObjectOfType<LevelManager>().transform;
        
        // CreateGrid();        
        // CreateItemsFromData();
    }

    // void CreateGrid()
    // {
    //     spawnedShelves = new Shelf[32];
    //     allSlots.Clear();

    //     for (var i = 0; i < 32; i++)
    //     {
    //         Vector3 position = GetPositionForIndex(i);
            
    //         if (currentLevelData.activeShelfIndices.Contains(i))
    //         {
    //             GameObject shelfObj = Instantiate(shelfPrefab, position, Quaternion.identity, transform);
    //             Shelf shelfComponent = shelfObj.GetComponent<Shelf>();
    //             spawnedShelves[i] = shelfComponent;
    //         }
    //         else
    //         {
    //             Instantiate(emptyShelfPrefab, position, Quaternion.identity, transform);
    //         }
    //     }
    // }

    Vector3 GetPositionForIndex(int index)
    {
        int cols = 8;
        float xOffset = 1.2f;
        float yOffset = 1.5f;
        
        int x = index % cols;
        int y = index / cols;
        
        return new Vector3(x * xOffset, -y * yOffset, 0);
    }

    // void CreateItemsFromData()
    // {
    //     if (itemsTransform == null) 
    //     {
    //         itemsTransform = new GameObject("ItemsContainer").transform;
    //         itemsTransform.SetParent(transform);
    //     }

    //     foreach (var startItem in currentLevelData.startingItems)
    //     {
    //         if (startItem.shelfIndex < 0 || startItem.shelfIndex >= 32) continue;
    //         if (startItem.slotIndex < 0 || startItem.slotIndex > 2) continue;

    //         Shelf targetShelf = spawnedShelves[startItem.shelfIndex];
            
    //         if (targetShelf == null)continue;

    //         List<Slot> slots = targetShelf.GetInnerSlots();

    //         if (startItem.slotIndex >= slots.Count) continue;

    //         Slot targetSlot = slots[startItem.slotIndex];

    //         if (!targetSlot.IsEmpty())
    //         {
    //             Debug.Log($"Слот {startItem.shelfIndex}:{startItem.slotIndex} уже занят!");
    //             continue;
    //         }

    //         SpawnItemInSlot(targetSlot, startItem.image);
    //     }
    // }

    void SpawnItemInSlot(Slot slot, Sprite image)
    {
        GameObject itemObj = Instantiate(itemPrefab, slot.transform.position, slot.transform.rotation, itemsTransform);

        Item itemComponent = itemObj.GetComponent<Item>();
        itemComponent.Initialize(slot, image);
        allItems.Add(itemComponent);
    }

    public void SetAllSlots(Slot _slot)
    {
        if (!allSlots.Contains(_slot))
        {
            allSlots.Add(_slot);
        }
    }

    public List<Slot> GetAllSlots()
    {
        return allSlots;
    }

    public void RemoveItemInArray(Item _item)
    {
        if (allItems.Contains(_item))
        {
            allItems.Remove(_item);
        }

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