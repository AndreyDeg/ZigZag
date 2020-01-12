using UnityEngine;

// Создает дорогу с поворотами, прямо или направо
public class MazeGenerator : ILevelGenerator
{
    int roadLen; //длинна дороги до поворота, рандом от 1 до roadLen
    int roadWidth; //ширина дороги
    int roadN; //общая длинна дороги
    int roadX, roadZ; //координаты куда проложена дорога

    ICrystalGenerator crystalGenerator;
    LevelData level;

    public MazeGenerator(int roadLen, int roadWidth)
    {
        this.roadLen = roadLen;
        this.roadWidth = roadWidth;
    }

    public void Generate(LevelData level, ICrystalGenerator crystalGenerator)
    {
        this.crystalGenerator = crystalGenerator;
        this.level = level;

        level.Clear(); //Чистим предыдущий уровень
        level.OnUpdate = GenerateRoad; //Подписываемся на достроение дороги

        //Стартовая площадка
        for (int startX = -1; startX <= 1; startX++)
            for (int startZ = -1; startZ <= 1; startZ++)
                level.SetGround(startX, startZ);

        roadN = 0;
        roadX = 1;
        roadZ = 1;
        GenerateRoad();
    }

    private void GenerateRoad()
    {
        while (level.CanSet(roadX + roadLen, roadZ + roadLen))
        {
            //Строим дорогу вперед
            for (int i = Random.Range(1, roadLen); i > 0; i--)
            {
                roadZ++;
                //Ставим кристалы
                if (crystalGenerator.Check(roadN++))
                    level.SetCrystal(roadX - Random.Range(0f, roadWidth - 1), roadZ);
                //Ставим дорогу
                for (int w = 0; w < roadWidth; w++)
                    level.SetGround(roadX - w, roadZ);
            }

            //Строим дорогу вправо
            for (int i = Random.Range(1, roadLen); i > 0; i--)
            {
                roadX++;
                if (crystalGenerator.Check(roadN++))
                    level.SetCrystal(roadX, roadZ - Random.Range(0f, roadWidth - 1));
                for (int w = 0; w < roadWidth; w++)
                    level.SetGround(roadX, roadZ - w);
            }
        }
    }
}