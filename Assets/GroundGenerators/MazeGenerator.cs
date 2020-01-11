using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MazeGenerator : IGroundGenerator
{
    int roadWidth;
    int roadLen = 5;
    ICrystalGenerator crystalGenerator;
    Ground ground;

    int groundN;
    int groundX, groundZ;

    public MazeGenerator(int roadLen, int roadWidth)
    {
        this.roadLen = roadLen;
        this.roadWidth = roadWidth;
    }

    public void Generate(ICrystalGenerator crystalGenerator, Ground ground)
    {
        this.crystalGenerator = crystalGenerator;
        this.ground = ground;

        ground.Clear();
        ground.OnUpdate = CheckGenerate;

        for (int startX = -1; startX <= 1; startX++)
            for (int startZ = -1; startZ <= 1; startZ++)
                ground.SetGround(startX, startZ);

        groundN = 0;
        groundX = 1;
        groundZ = 1;
        CheckGenerate();
    }

    private void CheckGenerate()
    {
        while (ground.CanSet(groundX + roadLen, groundZ + roadLen))
        {
            for (int i = UnityEngine.Random.Range(1, roadLen); i > 0; i--)
            {
                groundZ++;
                if (crystalGenerator.Check(groundN++))
                    ground.SetCrystal(groundX - (roadWidth - 1) / 2f, groundZ);
                for (int w = 0; w < roadWidth; w++)
                    ground.SetGround(groundX - w, groundZ);
            }
            for (int i = UnityEngine.Random.Range(1, roadLen); i > 0; i--)
            {
                groundX++;
                if (crystalGenerator.Check(groundN++))
                    ground.SetCrystal(groundX, groundZ - (roadWidth - 1) / 2f);
                for (int w = 0; w < roadWidth; w++)
                    ground.SetGround(groundX, groundZ - w);
            }
        }
    }
}