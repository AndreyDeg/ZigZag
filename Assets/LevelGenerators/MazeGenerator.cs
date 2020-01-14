using System.Collections.Generic;
using UnityEngine;

// Создает дорогу с поворотами, прямо или направо
public class MazeGenerator : ILevelGenerator
{
    int roadLen; //длинна дороги до поворота, рандом от 1 до roadLen
    int roadWidth; //ширина дороги
    int roadN; //общая длинна дороги
    int roadX, roadZ; //координаты куда проложена дорога

    public MazeGenerator(int roadLen, int roadWidth)
    {
        this.roadLen = roadLen;
        this.roadWidth = roadWidth;
    }

    public void Clear()
    {
        roadN = 0;
        roadX = 0;
        roadZ = 0;
    }

    public IEnumerable<GroundData> Generate(int maxX, int maxZ)
    {
        if (roadX == 0 && roadZ == 0)
        {
            roadX = 1;
            roadZ = 1;

            //Стартовая площадка
            yield return new GroundData(roadX, roadZ, null, 3, 3);
        }

        while (roadX + roadLen < maxX && roadZ + roadLen < maxZ)
        {
            //Строим дорогу вперед
            for (int i = Random.Range(1, roadLen); i > 0; i--)
                yield return new GroundData(roadX, ++roadZ, roadN++, roadWidth, 1);

            //Строим дорогу вправо
            for (int i = Random.Range(1, roadLen); i > 0; i--)
                yield return new GroundData(++roadX, roadZ, roadN++, 1, roadWidth);
        }
    }
}