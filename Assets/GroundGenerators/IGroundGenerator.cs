using UnityEngine;

public interface IGroundGenerator
{
    void Generate(ICrystalGenerator crystalGenerator, GameObject cube, GameObject capsule);
    bool Check(Vector3 pos);
    int Points { get; }
}