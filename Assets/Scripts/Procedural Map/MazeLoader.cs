using UnityEngine;
using System.Collections;
using System;

public class MazeLoader : MonoBehaviour
{
    [SerializeField] private int mazeRows, mazeColumns;
    [SerializeField] private float size = 10f;
    [SerializeField] private GameObject wall;
    [SerializeField] private GameObject mapParent;

    private MazeCell[,] mazeCells;

    private void Start()
    {
        InitializeMaze();
        MazeAlgorithm ma = new HuntAndKillMazeAlgorithm(mazeCells);
        ma.CreateMaze();
    }

    internal void startGame()
    {
    }

    private void InitializeMaze()
    {
        int column;
        int row;

        mazeCells = new MazeCell[mazeRows, mazeColumns];

        for (row = 0; row < mazeRows; row++)
        {
            for (column = 0; column < mazeColumns; column++)
            {
                mazeCells[row, column] = new MazeCell();

                if (column != 6)
                {
                    mazeCells[row, column].eastWall = Instantiate(wall, new Vector2((row * size), (column * size) + (size / 2f)), Quaternion.identity, mapParent.transform) as GameObject;
                    mazeCells[row, column].eastWall.name = "xAxisWall";
                }

                if (row != 4)
                {
                    mazeCells[row, column].southWall = Instantiate(wall, new Vector2((row * size) + (size / 2f), column * size), Quaternion.identity, mapParent.transform) as GameObject;
                    mazeCells[row, column].southWall.name = "yAxisWall";
                    mazeCells[row, column].southWall.transform.Rotate(new Vector3(0, 0, 1) * 90f);
                }
            }
        }
        mapParent.transform.position = new Vector2(-24f, -38.6f);
    }
}