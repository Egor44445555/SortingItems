using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GridManager : MonoBehaviour
{
    public static GridManager main;

    [SerializeField] GameObject shelfPrefab;
    [SerializeField] GameObject emptyShelfPrefab;
    [SerializeField] Transform itemsTransform;
    [SerializeField] List<ShelfsItemArray> shelfsArray;

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
    }

    public List<Slot> GetAllSlots()
    {
        return allSlots;
    }
}
