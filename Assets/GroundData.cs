using UnityEngine;

public class GroundData
{
    public Vector3 Position;
    public int? NumberLenght;
    public int WidthX, WidthZ;

    public GroundData(int x, int z, int? numberLenght, int widthX, int widthZ)
    {
        Position = new Vector3(x, 0, z);
        NumberLenght = numberLenght;
        WidthX = widthX;
        WidthZ = widthZ;
    }
}