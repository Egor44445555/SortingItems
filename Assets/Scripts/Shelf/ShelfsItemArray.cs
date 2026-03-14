using System;
using UnityEngine;

[Serializable]
public class ShelfsItemArray
{
    public Vector2 position = new Vector2(0, 0);

    public ShelfsItemArray(Vector2 _position)
    {
        position = _position;
    }
}
