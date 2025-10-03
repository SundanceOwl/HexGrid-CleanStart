using System;

[System.Serializable]
public class HexData
{
    public int Q;
    public int R;

    // Nur der Index des Prefabs, der die Höhe/Terrain-Typ repräsentiert
    public int ElevationIndex;

    // Die unnötige float Height-Variable wird entfernt!
}
