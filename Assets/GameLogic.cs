using System;
using System.Collections;
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
    public GameObject scoreText;

    readonly Dictionary<CrystalGenerateType, ICrystalGenerator> CrystalGenerateFunc = new Dictionary<CrystalGenerateType, ICrystalGenerator>
    {
        [CrystalGenerateType.Random] = new CrystalGeneratorRandom(5),
        [CrystalGenerateType.InOrder] = new CrystalGeneratorInOrder(5),
    };

    readonly Dictionary<GameDifficultyType, IGroundGenerator> GroundGenerators = new Dictionary<GameDifficultyType, IGroundGenerator>
    {
        [GameDifficultyType.Easy] = new MazeGenerator(5, 3),
        [GameDifficultyType.Medium] = new MazeGenerator(5, 2),
        [GameDifficultyType.Hard] = new MazeGenerator(5, 1),
    };

    Camera mainCamera;
    Vector3 cameraPos;
    Vector3 moveDirection;
    IGroundGenerator ground;

    public void Start()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        cameraPos = mainCamera.transform.position;

        CreateLevel();
    }

    private void CreateLevel()
    {
        ground = GroundGenerators[GameDifficulty];
        ground.Generate(CrystalGenerateFunc[CrystalGenerate], cube, capsule);
    }

    public void Update()
    {
        if (UnityEngine.Input.GetMouseButtonDown(0))
        {
            if (moveDirection.y >= 0)
            {
                if (moveDirection.x > 0)
                    moveDirection = new Vector3(0, 0, 1f) * Speed;
                else
                    moveDirection = new Vector3(1f, 0, 0) * Speed;
            }
        }

        sphere.transform.position += moveDirection * Time.deltaTime;

        if (ground.Check(sphere.transform.position))
        {
            scoreText.GetComponent<TextMesh>().text = "Score: " + ground.Points;

            if (moveDirection.y < 0 && sphere.transform.position.y < 0.25)
            {
                moveDirection = new Vector3(0, 0, 0);
                sphere.transform.position = new Vector3(0, 0.25f, 0);
            }
        }
        else
        {
            if (sphere.transform.position.y > -10)
                moveDirection += new Vector3(0, -1f, 0);
            else
            {
                moveDirection = new Vector3(0, moveDirection.y, 0);
                sphere.transform.position = new Vector3(0, 20, 0);
                CreateLevel();
            }
        }
    }

    public void LateUpdate()
    {
        mainCamera.transform.position = sphere.transform.position + cameraPos;
    }
}
