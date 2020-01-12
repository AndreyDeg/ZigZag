using System;
using System.Collections.Generic;
using UnityEngine;

//Игровой уровень
//Имеет размеры mazeSize*mazeSize
public class LevelData
{
    GameObject cube, capsule;

    //Размер и позиция уровня
    int mazeSize = 32;
    int mazeX, mazeZ;

    //Карта земли, если true, то в этой точке есть земля
    bool[,] ground;

    //Объекты на уровне, земля и кристаллы
    List<GameObject> ltObjects = new List<GameObject>();
    //Отдельный список для кристаллов
    List<GameObject> ltCrystals = new List<GameObject>();

    public Action OnUpdate;

    //Очки, заработанные за кристалы
    public int Score { get; private set; }

    public LevelData(GameObject cube, GameObject capsule)
    {
        this.cube = cube;
        this.capsule = capsule;
        cube.SetActive(false);
        capsule.SetActive(false);
    }

    //Проверка границы уровня
    public bool CanSet(int x, int z)
    {
        return x < mazeX + mazeSize && z < mazeZ + mazeSize;
    }

    //Установить землю
    public void SetGround(int x, int z)
    {
        GameObject clone = GameObject.Instantiate(cube);
        clone.transform.position += new Vector3(x, 0, z);
        clone.SetActive(true);
        ltObjects.Add(clone);
        if (x >= 0 && z >= 0)
            ground[x - mazeX, z - mazeZ] = true;
    }

    private float lastCrystalX, lastCrystalZ;

    //Установить кристалл
    //Кристал ставится выше или правее предыдущего кристала, чтобы игрок их не пропустил
    public void SetCrystal(float x, float z)
    {
        lastCrystalX = x = Math.Max(x, lastCrystalX);
        lastCrystalZ = z = Math.Max(z, lastCrystalZ);
        GameObject clone = GameObject.Instantiate(capsule);
        clone.transform.position += new Vector3(x, 0, z);
        clone.SetActive(true);
        ltObjects.Add(clone);
        ltCrystals.Add(clone);
    }

    //Очистка уровня
    public void Clear()
    {
        Score = 0;
        lastCrystalX = 0;
        lastCrystalZ = 0;

        mazeX = 0;
        mazeZ = 0;

        foreach (var clone in ltObjects)
            GameObject.Destroy(clone);
        ltObjects.Clear();

        ground = new bool[mazeSize, mazeSize];
    }

    //Сдвиг уровня и достройка дороги
    private void Shift(int shiftX, int shiftZ)
    {
        mazeX += shiftX;
        mazeZ += shiftZ;
        for (int mx = 0; mx < mazeSize; mx++)
            for (int mz = 0; mz < mazeSize; mz++)
                if (mx + shiftX < mazeSize && mz + shiftZ < mazeSize)
                    ground[mx, mz] = ground[mx + shiftX, mz + shiftZ];
                else
                    ground[mx, mz] = false;

        OnUpdate(); //Достроить дорогу
    }

    //Проверка, что в данной позиции есть дорога
    public bool Check(Vector3 pos)
    {
        int x = (int)Math.Round(pos.x);
        int z = (int)Math.Round(pos.z);

        //Если игрок проехал четверть уровня, то сдвинуть уровень и построить дорогу дальше
        if (x - mazeX > mazeSize / 4 || z - mazeZ > mazeSize / 4)
            Shift(x - mazeX, z - mazeZ);

        //Удаление блоков позади игрока
        ltObjects.RemoveAll(clone => clone == null);
        foreach (var clone in ltObjects)
        {
            var d = clone.transform.position.x + clone.transform.position.z - pos.x - pos.z;
            if (d < -2)
                clone.transform.position -= new Vector3(0, 10, 0) * Time.deltaTime;
            if (clone.transform.position.y < -10)
                GameObject.Destroy(clone);
        }

        //Подбор кристаллов
        ltCrystals.RemoveAll(clone => clone == null);
        foreach (var clone in ltCrystals)
        {
            var d = (clone.transform.position - pos).magnitude;
            if (d < (0.5 + 0.25) / 2)
            {
                Score++;
                GameObject.Destroy(clone);
            }
        }

        return ground[x - mazeX, z - mazeZ];
    }
}