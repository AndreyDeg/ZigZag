using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Ground
{
    GameObject cube, capsule;

    public int mazeX, mazeZ;
    int mazeBlock = 32;
    bool[,] ground;

    List<GameObject> ltGround = new List<GameObject>();
    List<GameObject> ltCrystals = new List<GameObject>();

    public Action OnUpdate;

    public int Points { get; private set; }

    public Ground(GameObject cube, GameObject capsule)
    {
        this.cube = cube;
        this.capsule = capsule;
        cube.SetActive(false);
        capsule.SetActive(false);
    }

    public bool CanSet(int x, int z)
    {
        return x < mazeX + mazeBlock && z < mazeZ + mazeBlock;
    }

    public void SetGround(int x, int z)
    {
        GameObject clone = GameObject.Instantiate(cube);
        clone.transform.position += new Vector3(x, 0, z);
        clone.SetActive(true);
        ltGround.Add(clone);
        if (x >= 0 && z >= 0)
            ground[x - mazeX, z - mazeZ] = true;
    }

    private float lastCrystalX, lastCrystalZ;

    public void SetCrystal(float x, float z)
    {
        x = Math.Max(x, lastCrystalX);
        z = Math.Max(z, lastCrystalZ);
        lastCrystalX = x;
        lastCrystalZ = z;
        GameObject clone = GameObject.Instantiate(capsule);
        clone.transform.position += new Vector3(x, 0, z);
        clone.SetActive(true);
        ltGround.Add(clone);
        ltCrystals.Add(clone);
    }

    public void Clear()
    {
        Points = 0;
        lastCrystalX = 0;
        lastCrystalZ = 0;

        mazeX = 0;
        mazeZ = 0;

        foreach (var clone in ltGround)
            GameObject.Destroy(clone);
        ltGround.Clear();

        ground = new bool[mazeBlock, mazeBlock];
    }


    private void Shift(int shiftX, int shiftZ)
    {
        mazeX += shiftX;
        mazeZ += shiftZ;
        for (int mx = 0; mx < mazeBlock; mx++)
            for (int mz = 0; mz < mazeBlock; mz++)
                if (mx + shiftX < mazeBlock && mz + shiftZ < mazeBlock)
                    ground[mx, mz] = ground[mx + shiftX, mz + shiftZ];
                else
                    ground[mx, mz] = false;

        OnUpdate();
    }

    public bool Check(Vector3 pos)
    {
        int x = (int)Math.Round(pos.x);
        int z = (int)Math.Round(pos.z);

        if (x - mazeX > mazeBlock / 2 || z - mazeZ > mazeBlock / 2)
            Shift(x - mazeX, z - mazeZ);

        ltGround.RemoveAll(clone => clone == null);
        foreach (var clone in ltGround)
        {
            var d = clone.transform.position.x + clone.transform.position.z - pos.x - pos.z;
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
            {
                Points++;
                GameObject.Destroy(clone);
            }
        }

        return ground[x - mazeX, z - mazeZ];
    }
}