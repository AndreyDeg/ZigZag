using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public float Speed = 2f;
    public CrystalGenerateType CrystalGenerate = CrystalGenerateType.Random;
    public GameDifficultyType GameDifficulty = GameDifficultyType.Medium;

    public GameObject sphere;
    public GameObject cube;
    public GameObject capsule;

    readonly Dictionary<CrystalGenerateType, ICrystalGenerator> CrystalGenerators= new Dictionary<CrystalGenerateType, ICrystalGenerator>
    {
        [CrystalGenerateType.Random] = new CrystalGeneratorRandom(5),
        [CrystalGenerateType.InOrder] = new CrystalGeneratorInOrder(5),
    };

    readonly Dictionary<GameDifficultyType, ILevelGenerator> GroundGenerators = new Dictionary<GameDifficultyType, ILevelGenerator>
    {
        [GameDifficultyType.Easy] = new MazeGenerator(5, 3),
        [GameDifficultyType.Medium] = new MazeGenerator(5, 2),
        [GameDifficultyType.Hard] = new MazeGenerator(5, 1),
    };

    TextMesh scoreText;
    Camera mainCamera;
    Vector3 cameraPos;

    Vector3 moveDirection;
    LevelData level;

    public void Start()
    {
        scoreText = GameObject.Find("ScoreText").GetComponent<TextMesh>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        cameraPos = mainCamera.transform.position;

        level = new LevelData(cube, capsule);
        CreateLevel();
    }

    private void CreateLevel()
    {
        //Создание уровня по выбранной сложности и расстановке кристолов
        var groundGenerator = GroundGenerators[GameDifficulty];
        var crystalGenerator = CrystalGenerators[CrystalGenerate];
        groundGenerator.Generate(level, crystalGenerator);
    }

    public void Update()
    {
        if (UnityEngine.Input.GetMouseButtonDown(0))
        {
            if (sphere.transform.position.y <= -10)
            {
                //Если упали, то начинаем уровень заново
                moveDirection = new Vector3(0, 0, 0);
                sphere.transform.position = new Vector3(0, 0.25f, 0);
                CreateLevel();
            }
            else if(moveDirection.y >= 0)
            {
                //Смена направления
                if (moveDirection.x > 0)
                    moveDirection = new Vector3(0, 0, 1f) * Speed;
                else
                    moveDirection = new Vector3(1f, 0, 0) * Speed;
            }
        }

        //Шарик движется
        sphere.transform.position += moveDirection * Time.deltaTime;

        //Проверка, что шарик на дороге
        if (level.Check(sphere.transform.position))
        {
            scoreText.text = "Score: " + level.Score;
        }
        else
        {
            //Шарик упал с дороги
            if (sphere.transform.position.y > -15)
                moveDirection += new Vector3(0, -1f, 0);
            else
            {
                //Остановим шарик, чтоб не улетел в бесконечность
                moveDirection = new Vector3(0, 0, 0);
                scoreText.text = "Score: " + level.Score + "\nTap to continue" ;
            }
        }
    }

    public void LateUpdate()
    {
        //камера двигается за шариком
        mainCamera.transform.position = sphere.transform.position + cameraPos;
    }
}
