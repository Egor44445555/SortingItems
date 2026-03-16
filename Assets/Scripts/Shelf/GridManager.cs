using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GridManager : MonoBehaviour
{
    public static GridManager main;

    [SerializeField] GameObject shelfPrefab;
    [SerializeField] GameObject emptyShelfPrefab;
    [SerializeField] Transform itemsTransform;
    [SerializeField] List<int> shelfsArrayPosition;
    [SerializeField] List<GameObject> slotItemArray;

    List<Slot> allSlots = new List<Slot>();
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
            if (shelfsArrayPosition.Contains(i + 1))
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
        foreach (var item in slotItemArray)
        {
            GameObject itemObj = Instantiate(item, itemsTransform);
            Item itemComponent = itemObj.GetComponent<Item>();
            itemComponent.FindEmptySlot();
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
}
