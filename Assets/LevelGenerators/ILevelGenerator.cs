using System.Collections.Generic;
using UnityEngine;

public interface ILevelGenerator
{
    void Clear();
    IEnumerable<GroundData> Generate(int maxX, int maxZ);
}