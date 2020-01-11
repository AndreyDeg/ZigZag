using UnityEngine;

public interface IGroundGenerator
{
    void Generate(ICrystalGenerator crystalGenerator, Ground ground);
}