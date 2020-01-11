using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Maze
{
    GameObject cube, capsule;
    int width;
    ICrystalGenerator crystalGenerator;

    int groundN;
    int groundX, groundZ;
    int mazeX, mazeZ;
    int mazeBlock = 16;
    bool[,] ground;
    
    List<GameObject> ltGround = new List<GameObject>();
    List<GameObject> ltCrystals = new List<GameObject>();

    public Maze(GameObject cube, GameObject capsule)
    {
        this.cube = cube;
        this.capsule = capsule;
        cube.SetActive(false);
        capsule.SetActive(false);
    }

    private void SetGround(int x, int z)
    {
        GameObject clone = GameObject.Instantiate(cube);
        clone.transform.position += new Vector3(x, 0, z);
        clone.SetActive(true);
        ltGround.Add(clone);
        if (x >= 0 && z >= 0)
            ground[x++ - mazeX, z - mazeZ] = true;
    }

    private void SetCrystal(float x, float z)
    {
        GameObject clone = GameObject.Instantiate(capsule);
        clone.transform.position += new Vector3(x, 0, z);
        clone.SetActive(true);
        ltGround.Add(clone);
        ltCrystals.Add(clone);
    }

    public void Generate(int width, ICrystalGenerator crystalGenerator)
    {
        this.width = width;
        this.crystalGenerator = crystalGenerator;

        mazeX = 0;
        mazeZ = 0;

        foreach (var clone in ltGround)
            GameObject.Destroy(clone);
        ltGround.Clear();

        ground = new bool[mazeBlock * 2, mazeBlock * 2];

        for (int startX = -1; startX <= 1; startX++)
            for (int startZ = -1; startZ <= 1; startZ++)
                SetGround(startX, startZ);

        groundN = 0;
        groundX = 1;
        groundZ = 1;
        GenerateGround();
    }

    private void GenerateGround()
    {
        while (groundX < mazeX + mazeBlock * 1.5 && groundZ < mazeZ + mazeBlock * 1.5)
        {
            for (int i = UnityEngine.Random.Range(1, 5); i > 0; i--)
            {
                groundZ++;
                if (crystalGenerator.Check(groundN++))
                    SetCrystal(groundX - (width - 1) / 2f, groundZ);
                for (int w = 0; w < width; w++)
                    SetGround(groundX - w, groundZ);
            }
            for (int i = UnityEngine.Random.Range(1, 5); i > 0; i--)
            {
                groundX++;
                if (crystalGenerator.Check(groundN++))
                    SetCrystal(groundX, groundZ - (width - 1) / 2f);
                for (int w = 0; w < width; w++)
                    SetGround(groundX, groundZ - w);
            }
        }
    }

    public bool Check(Vector3 pos)
    {
        int x = (int)Math.Round(pos.x);
        int z = (int)Math.Round(pos.z);

        if(x > mazeX + mazeBlock)
        {
            mazeX += mazeBlock;
            for (int mz = 0; mz < mazeBlock * 2; mz++)
            {
                for (int mx = 0; mx < mazeBlock; mx++)
                    ground[mx, mz] = ground[mx + mazeBlock, mz];
                for (int mx = mazeBlock; mx < mazeBlock*2; mx++)
                    ground[mx, mz] = false;
            }
            
            GenerateGround();
        }

        if (z > mazeZ + mazeBlock)
        {
            mazeZ += mazeBlock;
            for (int mx = 0; mx < mazeBlock * 2; mx++)
            {
                for (int mz = 0; mz < mazeBlock; mz++)
                    ground[mx, mz] = ground[mx, mz + mazeBlock];
                for (int mz = mazeBlock; mz < mazeBlock*2; mz++)
                    ground[mx, mz] = false;
            }

            GenerateGround();
        }

        ltGround.RemoveAll(clone => clone == null);
        foreach (var clone in ltGround)
        {
            var d = clone.transform.position.x + clone.transform.position.z - x - z;
            if (d < -2)
                clone.transform.position -= new Vector3(0, 10, 0) * Time.deltaTime;
            if (clone.transform.position.y < -10)
                GameObject.Destroy(clone);
        }

        ltCrystals.RemoveAll(clone => clone == null);
        foreach (var clone in ltCrystals)
        {
            var d = (clone.transform.position - pos).magnitude;
            if (d < (0.5 + 0.25) / 2)
                GameObject.Destroy(clone);
        }

        return ground[x - mazeX, z - mazeZ];
    }
}