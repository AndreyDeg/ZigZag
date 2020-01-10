using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Maze
{
    GameObject cube, capsule;
    bool[,] ground;
    List<GameObject> ltGround = new List<GameObject>();
    List<GameObject> ltBonus = new List<GameObject>();

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
            ground[x++, z] = true;
    }

    private void SetBonus(int x, int z)
    {
        GameObject clone = GameObject.Instantiate(capsule);
        clone.transform.position += new Vector3(x, 0, z);
        clone.SetActive(true);
        ltGround.Add(clone);
        ltBonus.Add(clone);
    }

    public void Generate()
    {
        foreach (var clone in ltGround)
            GameObject.Destroy(clone);
        ltGround.Clear();

        var r = new System.Random((int)DateTime.Now.Ticks); //TODO_deg
        ground = new bool[256, 256]; //TODO_deg

        for (int startX = -1; startX <= 1; startX++)
            for (int startZ = -1; startZ <= 1; startZ++)
                SetGround(startX, startZ);

        int p = 0;
        int x = 2;
        int z = 1;
        while (x < 250 && z < 250)
        {
            for (int i = r.Next(1, 5); i > 0; i--, p++)
            {
                if (p % 5 == p / 5 % 5)
                    SetBonus(x, z);
                SetGround(x++, z);
            }
            for (int i = r.Next(1, 5); i > 0; i--, p++)
            {
                if (p % 5 == p / 5 % 5)
                    SetBonus(x, z);
                SetGround(x, z++);
            }
        }
    }

    public bool Check(Vector3 pos)
    {
        int x = (int)Math.Round(pos.x);
        int z = (int)Math.Round(pos.z);

        ltGround.RemoveAll(clone => clone == null);
        foreach (var clone in ltGround)
        {
            var d = clone.transform.position.x + clone.transform.position.z - x - z;
            if (d < -2)
                clone.transform.position -= new Vector3(0, 10, 0) * Time.deltaTime;
            if (clone.transform.position.y < -10)
                GameObject.Destroy(clone);
        }

        ltBonus.RemoveAll(clone => clone == null);
        foreach (var clone in ltBonus)
        {
            var d = (clone.transform.position - pos).magnitude;
            if (d < (0.5+0.25)/2)
                GameObject.Destroy(clone);
        }

        return ground[x, z];
    }
}