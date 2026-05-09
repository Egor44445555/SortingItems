using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    [Tooltip("Индексы полок (0-31), которые будут активны на уровне")]
    public List<int> activeShelfIndices = new List<int>();
    public List<StartingItem> startingItems = new List<StartingItem>();
}

[System.Serializable]
public class StartingItem
{
    public int shelfIndex;
    public int slotIndex;
    public Sprite image;
}