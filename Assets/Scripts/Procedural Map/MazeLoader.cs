using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MazeLoader : MonoBehaviourPunCallbacks
{
    [SerializeField] private int mazeRows, mazeColumns;
    [SerializeField] private float size = 10f;
    [SerializeField] private GameObject wall;
    [SerializeField] private GameObject mapParent;

    private MazeCell[,] mazeCells;


    private int randomSeedNum;
    private List<Vector2> yAxisWallPositions;


    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            randomSeedNum = Random.Range(1, 2000);
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable
            {
                ["randomSeed"] = randomSeedNum
            });
            PhotonNetwork.CurrentRoom.CustomProperties["IWantRematch"] = randomSeedNum;
            StartCoroutine(WaitForOpponent());
        }
        else
        {
            // for rematch since customproperties changes after some frames, avoid using the same number by starting coroutine and waiting till it changes from zero
            StartCoroutine(GenerateMap());
        }
    }


    private IEnumerator WaitForOpponent()
    {
        while (true)
        {
            if (PhotonNetwork.CurrentRoom.Players.Count == 2)
            {
                InitializeMaze(randomSeedNum);
                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator GenerateMap()
    {
        while (true)
        {
            if ((int) PhotonNetwork.CurrentRoom.CustomProperties["randomSeed"] != 0)
            {
                randomSeedNum = (int) PhotonNetwork.CurrentRoom.CustomProperties["randomSeed"];
                InitializeMaze(randomSeedNum);
                PhotonNetwork.CurrentRoom.CustomProperties["randomSeed"] = 0;
                yield break;
            }

            yield return null;
        }
    }


    public void InitializeMaze(int randomSeed)
    {
        yAxisWallPositions = new List<Vector2>();
        int column;
        int row;

        mazeCells = new MazeCell[mazeRows, mazeColumns];

        for (row = 0; row < mazeRows; row++)
        for (column = 0; column < mazeColumns; column++)
        {
            mazeCells[row, column] = new MazeCell();

            if (column != mazeColumns - 1)
            {
                mazeCells[row, column].eastWall = Instantiate(wall, new Vector2(row * size, column * size + size / 2f),
                    Quaternion.identity, mapParent.transform);
                mazeCells[row, column].eastWall.name = "xAxisWall";
            }

            if (row != mazeRows - 1)
            {
                mazeCells[row, column].southWall = Instantiate(wall, new Vector2(row * size + size / 2f, column * size),
                    Quaternion.identity, mapParent.transform);
                mazeCells[row, column].southWall.name = "yAxisWall";
                mazeCells[row, column].southWall.transform.Rotate(new Vector3(0, 0, 1) * 90f);
                //yAxisWallPositions.Add(mazeCells[row, column].southWall.transform.position + new Vector3(6.5f, -6.5f));
                yAxisWallPositions.Add(mazeCells[row, column].southWall.transform.position + new Vector3(6.5f, -6.5f));
            }
        }

        MazeAlgorithm ma = new HuntAndKillMazeAlgorithm(mazeCells);
        ma.CreateMaze(randomSeed);

        //map borders
        for (row = 0; row < mazeRows; row++)
        for (column = 0; column < mazeColumns; column++)
        {
            if (column == mazeColumns - 1)
            {
                mazeCells[row, column].eastWall = Instantiate(wall, new Vector2(row * size, column * size + size / 2f),
                    Quaternion.identity, mapParent.transform);
                mazeCells[row, column].eastWall.name = "xAxisWall";
            }

            if (row == mazeRows - 1)
            {
                mazeCells[row, column].southWall = Instantiate(wall, new Vector2(row * size + size / 2f, column * size),
                    Quaternion.identity, mapParent.transform);
                mazeCells[row, column].southWall.name = "yAxisWall";
                mazeCells[row, column].southWall.transform.Rotate(new Vector3(0, 0, 1) * 90f);
            }

            if (column == 0)
            {
                mazeCells[row, column].eastWall = Instantiate(wall, new Vector2(row * size, column * size - size / 2f),
                    Quaternion.identity, mapParent.transform);
                mazeCells[row, column].eastWall.name = "xAxisWall";
            }

            if (row == 0)
            {
                mazeCells[row, column].southWall = Instantiate(wall, new Vector2(row * size - size / 2f, column * size),
                    Quaternion.identity, mapParent.transform);
                mazeCells[row, column].southWall.name = "yAxisWall";
                mazeCells[row, column].southWall.transform.Rotate(new Vector3(0, 0, 1) * 90f);
                yAxisWallPositions.Add(mazeCells[row, column].southWall.transform.position + new Vector3(6.5f, -6.5f));
            }
        }

        for (var i = 0; i < yAxisWallPositions.Count; i++) yAxisWallPositions[i] += new Vector2(-17.4f, -38.6f);

        mapParent.transform.position = new Vector2(-17.4f, -38.6f);
    }

    public List<Vector2> CalculateCenterPoints()
    {
        var cellCenterPoints = new List<Vector2>();
        for (var i = 0; i < yAxisWallPositions.Count; i += 2)
        {
            var centerPoint = (yAxisWallPositions[i] + yAxisWallPositions[i + 1]) / 2;
            cellCenterPoints.Add(centerPoint);
        }

        return cellCenterPoints;
    }
}