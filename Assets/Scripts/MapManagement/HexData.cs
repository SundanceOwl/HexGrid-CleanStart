using System;

[System.Serializable]
public class HexData
{
    public int Q;
    public int R;

    // Nur der Index des Prefabs, der die H�he/Terrain-Typ repr�sentiert
    public int ElevationIndex;

    // Die unn�tige float Height-Variable wird entfernt!
}
