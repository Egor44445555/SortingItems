using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLevel", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    [System.Serializable]
    public class ShelfConfig
    {
        public int shelfType;
        public int rowIndex;
    }

    [System.Serializable]
    public class ItemSpawn
    {
        public Texture2D image;
        public int shelfIndex;
        public int slotIndex;
        public List<ItemSpawnBehind> behindItems;
    }

    [System.Serializable]
    public class ItemSpawnBehind
    {
        public Texture2D image;
    }

    public List<ShelfConfig> shelvesConfig;
    public List<ItemSpawn> startingItems;
}
