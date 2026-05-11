using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GridManager : MonoBehaviour
{
    public static GridManager main;

    [Header("Prefabs")]
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform itemsContainer;
    [SerializeField] GameObject[] shelfPrefabs;

    [Header("Settings")]
    [SerializeField] float horizontalSpacing = 10f;
    [SerializeField] float verticalSpacing = 10f;
    
    [Header("Level Configuration")]
    [SerializeField] LevelData[] levelData;

    Transform parentTransform;
    List<Slot> allSlots = new List<Slot>();
    List<Item> allItems = new List<Item>();
    List<Shelf> allShelves = new List<Shelf>();

    bool isDraggable = false;
    bool startCheckShelfs = false;
    int currentLevel = 0;
    Coroutine coroutine;

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

    public void Initialize()
    {
        parentTransform = FindObjectOfType<LevelManager>().transform;
        currentLevel = int.Parse(YGPlugin.main.GetPlayerLevel());

        if (currentLevel > levelData.Length - 1)
        {
            currentLevel = 0;
            YGPlugin.main.SetPlayerLevel(currentLevel.ToString());
        }

        GenerateLevel();
    }

    void Update()
    {
        if (startCheckShelfs && !isDraggable)
        {
            foreach (Shelf shelf in allShelves)
            {
                shelf.CheckEmptySlots();
            }
            startCheckShelfs = false;
        }
    }

    public void GenerateLevel()
    {
        if (levelData[currentLevel] == null) return;

        foreach (Transform child in transform) Destroy(child.gameObject);
        allSlots.Clear();

        var shelvesByRow = levelData[currentLevel].shelvesConfig
            .GroupBy(s => s.rowIndex)
            .OrderBy(g => g.Key)
            .ToList();
        float rowHeight = shelfPrefabs[0].GetComponent<RectTransform>().rect.height; 

        float currentY = (shelvesByRow.Count * rowHeight) / 2f - (verticalSpacing * shelvesByRow.Count);

        foreach (var rowGroup in shelvesByRow)
        {
            var rowShelves = rowGroup.ToList();
            float totalRowWidth = 0;
            List<float> shelfWidths = new List<float>();

            foreach (var shelfData in rowShelves)
            {
                GameObject prefab = GetShelfPrefabByType(shelfData.shelfType);                
                float width = prefab.GetComponent<RectTransform>().rect.width;
                
                shelfWidths.Add(width);
                totalRowWidth += width;
            }

            if (rowShelves.Count > 1)
            {
                totalRowWidth += horizontalSpacing * (rowShelves.Count - 1);
            }

            float currentX = 0f;

            if (shelfWidths.Count > 1)
            {
                currentX = -(totalRowWidth / 3f);
            }

            for (int i = 0; i < rowShelves.Count; i++)
            {
                var shelfData = rowShelves[i];
                GameObject prefab = GetShelfPrefabByType(shelfData.shelfType);                
                GameObject shelfObj = Instantiate(prefab, transform);                
                Vector3 pos = new Vector3(currentX, currentY, 0);

                shelfObj.transform.localPosition = pos;

                allShelves.Add(shelfObj.GetComponent<Shelf>());

                Slot[] slots = shelfObj.GetComponentsInChildren<Slot>();
                allSlots.AddRange(slots);
                
                foreach(var slot in slots) SetAllSlots(slot);

                if (i < rowShelves.Count - 1)
                {
                    float currentW = shelfWidths[i];
                    float nextW = shelfWidths[i + 1];                    
                    float step = (currentW / 2f) + horizontalSpacing + (nextW / 2f);
                    
                    currentX += step;
                }
            }
            
            currentY -= (rowHeight + verticalSpacing);
        }

        CreateItemsFromData();
    }

    GameObject GetShelfPrefabByType(int type)
    {
        int index = type - 1;
        if (index >= 0 && index < shelfPrefabs.Length)
            return shelfPrefabs[index];
        
        Debug.LogError($"Prefab for shelf type {type} not found!");
        return shelfPrefabs[shelfPrefabs.Length - 1]; // Fallback
    }

    void CreateItemsFromData()
    {
        foreach (var startItem in levelData[currentLevel].startingItems)
        {
            Shelf targetShelf = allShelves[startItem.shelfIndex];
            
            if (targetShelf == null) continue;

            List<Slot> slots = targetShelf.GetInnerSlots();

            if (startItem.slotIndex >= slots.Count) continue;

            Slot targetSlot = slots[startItem.slotIndex];

            if (!targetSlot.IsEmpty()) continue;

            if (startItem.behindItems.Count > 0)
            {
                foreach (var behindItem in startItem.behindItems)
                {
                    CreateItem(targetSlot, behindItem.image, true);
                }
            }

            CreateItem(targetSlot, startItem.image, false);
        }
    }

    void CreateItem(Slot slot, Sprite _image, bool _isBehind)
    {
        GameObject itemObj = Instantiate(itemPrefab, slot.GetComponent<RectTransform>().anchoredPosition, slot.transform.rotation, itemsContainer);
        Item itemComponent = itemObj.GetComponent<Item>();
        itemComponent.Initialize(slot, _image, _isBehind);
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

    public List<Item> GetAllItems()
    {
        return allItems;
    }

    public Transform GetWrapperTransform()
    {
        return parentTransform;
    }

    public Transform GetParentItemsContainer()
    {
        return itemsContainer;
    }

    public void RemoveItemInArray(Item _item)
    {
        if (allItems.Contains(_item))
        {
            allItems.Remove(_item);
        }

        if (allItems.Count <= 0 && UIManager.main != null)
        {
            Nextlevel();
            UIManager.main.EndLevel(currentLevel);
        }
    }

    public void Nextlevel()
    {
        currentLevel++;

        if (currentLevel > levelData.Length - 1)
        {
            currentLevel = 0;
        }

        YGPlugin.main.SetPlayerLevel(currentLevel.ToString());

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }        

        coroutine = StartCoroutine(ClearLevel());
    }

    IEnumerator ClearLevel()
    {
        yield return new WaitForSeconds(0.3f);

        UIManager.main.PlayFanfareEffect();
        
        yield return new WaitForSeconds(1f);

        float time = 0.3f * allShelves.Count;

        foreach (Shelf shelf in allShelves)
        {
            shelf.ClearLevel();
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(time);

        allShelves.Clear();
        allSlots.Clear();
        allItems.Clear();
        GenerateLevel();
    }

    public void SetDraggable(bool _draggable)
    {
        isDraggable = _draggable;
        startCheckShelfs = true;
    }

    public bool IsDraggable()
    {
        return isDraggable;
    }
}