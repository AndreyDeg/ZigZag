using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public float Speed = 2f;
    public CrystalGenerateType CrystalGenerate = CrystalGenerateType.Random;
    public GameDifficultyType GameDifficulty = GameDifficultyType.Medium;

    
    readonly Dictionary<CrystalGenerateType, ICrystalGenerator> CrystalGenerateFunc = new Dictionary<CrystalGenerateType, ICrystalGenerator>
    {
        [CrystalGenerateType.Random] = new CrystalGeneratorRandom(5),
        [CrystalGenerateType.InOrder] = new CrystalGeneratorInOrder(5),
    };

    readonly Dictionary<GameDifficultyType, int> WidthByDifficulty = new Dictionary<GameDifficultyType, int>
    {
        [GameDifficultyType.Easy] = 3,
        [GameDifficultyType.Medium] = 2,
        [GameDifficultyType.Hard] = 1,
    };

    Camera mainCamera;
    Vector3 cameraPos;
    Vector3 moveDirection;
    Maze maze;

    public void Start()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        cameraPos = mainCamera.transform.position;
        var cube = GameObject.Find("Cube");
        var capsule = GameObject.Find("Capsule");
        maze = new Maze(cube, capsule);
        maze.Generate(WidthByDifficulty[GameDifficulty], CrystalGenerateFunc[CrystalGenerate]);
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
            else if (gameObject.transform.position.y < 0)
            {
                moveDirection = new Vector3(0, -9f, 0);
                gameObject.transform.position = new Vector3(0, 5, 0);
                maze.Generate(WidthByDifficulty[GameDifficulty], CrystalGenerateFunc[CrystalGenerate]);
            }
        }

        gameObject.transform.position += moveDirection * Time.deltaTime;

        if (maze.Check(gameObject.transform.position))
        {
            if(moveDirection.y < 0 && gameObject.transform.position.y < 0.25)
            {
                moveDirection = new Vector3(0, 0, 0);
                gameObject.transform.position = new Vector3(0, 0.25f, 0);
            }
        }
        else
        {
            moveDirection += new Vector3(0, -1f, 0);
        }
    }

    public void LateUpdate()
    {
        mainCamera.transform.position = gameObject.transform.position + cameraPos;
    }
}
