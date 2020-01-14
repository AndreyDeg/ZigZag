using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

//Игровой уровень
//Имеет размеры mazeSize*mazeSize
public class LevelData
{
    GameObjectPull groundPull;
    GameObjectPull crystalPull;

    ILevelGenerator levelGenerator;
    ICrystalGenerator crystalGenerator;

    //Размер и позиция уровня
    int mazeSize = 32;
    int mazeX, mazeZ;

    //Карта земли, если true, то в этой точке есть земля
    bool[,] ground;

    //Список блоков земли
    List<GameObject> ltGrounds = new List<GameObject>();
    //Список кристаллов
    List<GameObject> ltCrystals = new List<GameObject>();

    //Очки, заработанные за кристалы
    public int Score { get; private set; }

    public LevelData(GameObjectPull groundPull, GameObjectPull crystalPull)
    {
        this.groundPull = groundPull;
        this.crystalPull = crystalPull;
    }

    //Установить землю
    private void SetGround(Vector3 position)
    {
        var cube = groundPull.Get();
        cube.transform.position += position;
        ltGrounds.Add(cube);

        int x = (int)Math.Round(position.x);
        int z = (int)Math.Round(position.z);
        if (x >= 0 && z >= 0)
            ground[x - mazeX, z - mazeZ] = true;
    }

    private float lastCrystalX, lastCrystalZ;

    //Установить кристалл
    //Кристал ставится выше или правее предыдущего кристала, чтобы игрок их не пропустил
    private void SetCrystal(Vector3 position)
    {
        var crustal = crystalPull.Get();
        lastCrystalX = Math.Max(position.x, lastCrystalX);
        lastCrystalZ = Math.Max(position.z, lastCrystalZ);
        crustal.transform.position += new Vector3(lastCrystalX, 0, lastCrystalZ);
        ltCrystals.Add(crustal);
    }

    //Очистка уровня
    private void Clear()
    {
        Score = 0;
        lastCrystalX = 0;
        lastCrystalZ = 0;

        mazeX = 0;
        mazeZ = 0;

        foreach (var clone in ltGrounds)
            groundPull.Put(clone);
        ltGrounds.Clear();
        foreach (var clone in ltCrystals)
            crystalPull.Put(clone);
        ltCrystals.Clear();

        ground = new bool[mazeSize, mazeSize];
    }

    public void Generate(ILevelGenerator levelGenerator, ICrystalGenerator crystalGenerator)
    {
        this.levelGenerator = levelGenerator;
        this.crystalGenerator = crystalGenerator;

        //Чистим предыдущий уровень
        Clear();
        levelGenerator.Clear();

        GenerateNext();
    }

    private void GenerateNext()
    {
        foreach (var ground in levelGenerator.Generate(mazeX + mazeSize, mazeZ + mazeSize))
        {
            for (int x = 0; x < ground.WidthX; x++)
                for (int z = 0; z < ground.WidthZ; z++)
                    SetGround(ground.Position - new Vector3(x, 0, z));

            if (ground.NumberLenght.HasValue && crystalGenerator.Check(ground.NumberLenght.Value))
                SetCrystal(ground.Position - new Vector3(Random.Range(0f, ground.WidthX - 1), 0, Random.Range(0f, ground.WidthZ - 1)));
        }
    }

    //Сдвиг уровня
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
    }

    //Проверка, что в данной позиции есть земля
    public bool CheckGround(Vector3 pos)
    {
        int x = (int)Math.Round(pos.x);
        int z = (int)Math.Round(pos.z);

        //Если игрок проехал четверть уровня, то сдвинуть уровень и построить дорогу дальше
        if (x - mazeX > mazeSize / 4 || z - mazeZ > mazeSize / 4)
        {
            Shift(x - mazeX, z - mazeZ);
            GenerateNext();
        }

        //Удаление блоков позади игрока
        foreach (var clone in ltGrounds)
            if (CheckFall(clone, pos))
                groundPull.Put(clone);
        ltGrounds.RemoveAll(clone => !clone.activeInHierarchy);

        //Подбор кристаллов
        foreach (var clone in ltCrystals)
            if (CheckFall(clone, pos) || CheckLoot(clone, pos))
                crystalPull.Put(clone);
        ltCrystals.RemoveAll(clone => !clone.activeInHierarchy);

        return ground[x - mazeX, z - mazeZ];
    }

    private bool CheckFall(GameObject gameObject, Vector3 pos)
    {
        var d = gameObject.transform.position.x + gameObject.transform.position.z - pos.x - pos.z;
        if (d < -2)
            gameObject.transform.position -= new Vector3(0, 10, 0) * Time.deltaTime;

        return gameObject.transform.position.y < -10;
    }

    private bool CheckLoot(GameObject gameObject, Vector3 pos)
    {
        var d = (gameObject.transform.position - pos).magnitude;
        if (d < (0.5 + 0.25) / 2)
        {
            Score++;
            return true;
        }

        return false;
    }
}