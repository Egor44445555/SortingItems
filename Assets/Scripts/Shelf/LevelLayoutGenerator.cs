using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LevelLayoutGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float horizontalSpacing = 10f;
    [SerializeField] float verticalSpacing = 10f;
    [SerializeField] float startY = 0f;
    
    [Header("Prefabs")]
    [SerializeField] GameObject[] shelfPrefabs;
    
    [Header("Data")]
    [SerializeField] LevelData levelData;

    List<Slot> allSlots = new List<Slot>();
    
    void Start()
    {
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        if (levelData == null) return;

        foreach (Transform child in transform) Destroy(child.gameObject);
        allSlots.Clear();

        var shelvesByRow = levelData.shelvesConfig
            .GroupBy(s => s.rowIndex)
            .OrderBy(g => g.Key)
            .ToList();

        float currentY = startY;

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

                Slot[] slots = shelfObj.GetComponentsInChildren<Slot>();
                allSlots.AddRange(slots);
                
                if(GridManager.main != null)
                {
                    foreach(var slot in slots) GridManager.main.SetAllSlots(slot);
                }

                if (i < rowShelves.Count - 1)
                {
                    float currentW = shelfWidths[i];
                    float nextW = shelfWidths[i + 1];                    
                    float step = (currentW / 2f) + horizontalSpacing + (nextW / 2f);
                    
                    currentX += step;
                }
            }

            float rowHeight = shelfPrefabs[0].GetComponent<RectTransform>().rect.height; 
            currentY -= (rowHeight + verticalSpacing);
        }
    }

    GameObject GetShelfPrefabByType(int type)
    {
        int index = type - 1;
        if (index >= 0 && index < shelfPrefabs.Length)
            return shelfPrefabs[index];
        
        Debug.LogError($"Prefab for shelf type {type} not found!");
        return shelfPrefabs[shelfPrefabs.Length - 1]; // Fallback
    }
}
