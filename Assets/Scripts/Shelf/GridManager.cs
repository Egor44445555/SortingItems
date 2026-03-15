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
        allSlots = FindObjectsByType<Slot>(FindObjectsSortMode.None).ToList();

        CreateGrid();
    }

    void CreateGrid()
    {
        for (var i = 0; i < 20; i++)
        {
            if (shelfsArrayPosition.Contains(i + 1))
            {
                Instantiate(shelfPrefab, transform);
            }
            else
            {
                Instantiate(emptyShelfPrefab, transform);
            }
        }
        
        foreach (var item in slotItemArray)
        {
            GameObject itemObj = Instantiate(item, itemsTransform);
            // itemObj.GetComponent<Item>();
        }
    }

    public List<Slot> GetAllSlots()
    {
        return allSlots;
    }
}
