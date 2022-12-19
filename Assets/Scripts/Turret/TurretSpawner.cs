using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[Serializable]
public class TowerData
{
    public List<Vector2> Normals = new List<Vector2>();
    public List<Vector2> Rocketeers = new List<Vector2>();
    public List<Vector2> EmptySpots = new List<Vector2>();
}

public class TurretSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] TurretPrefabs;

    [Space(20)]

    [SerializeField] private Tilemap mapTile;
    [SerializeField] private TileBase emptyTile;
    [SerializeField] Grid GameGrid;
    [SerializeField] private List<Vector2> EmptySpots = new List<Vector2>();

    private List<Vector2> rocketeers = new List<Vector2>();
    private List<Vector2> normals = new List<Vector2>();




    private void OnEnable()
    {
        Events.BoughtTurret += BuyTurret;
        Events.SaveTowers += SaveTowers;
    }
    private void OnDisable()
    {
        Events.BoughtTurret -= BuyTurret;
        Events.SaveTowers -= SaveTowers;
    }

    private void SaveTowers()
    {
        TowerData twData = new TowerData();

        twData.Normals = normals;
        twData.Rocketeers = rocketeers;
        twData.EmptySpots = EmptySpots;


        string json = JsonUtility.ToJson(twData);
        PlayerPrefs.SetString("TowerData", json);
    }


    private void Start()
    {
        string enemySp = PlayerPrefs.GetString("TowerData");
        TowerData ths = JsonUtility.FromJson<TowerData>(enemySp);
        if (ths != null)
        {
            foreach (Vector2 sPoint in ths.Normals)
            {
                Instantiate(TurretPrefabs[0], sPoint, Quaternion.identity);
            }
            foreach (Vector2 sPoint in ths.Rocketeers)
            {
                Instantiate(TurretPrefabs[1], sPoint, Quaternion.identity);
            }
            EmptySpots = ths.EmptySpots;

        }
        else
        {
            for (int i = -Mathf.FloorToInt(mapTile.size.x / 2); i < mapTile.size.x - Mathf.FloorToInt(mapTile.size.x / 2); i++)
            {
                for (int j = -Mathf.FloorToInt(mapTile.size.y / 2); j < mapTile.size.y - Mathf.FloorToInt(mapTile.size.y / 2); j++)
                {
                    TileBase thisTile = mapTile.GetTile(new Vector3Int(i, j));

                    if (thisTile == emptyTile)
                    {
                        EmptySpots.Add(new Vector2(i, j));
                    }
                }
            }
        }

    }

    public void BuyTurret()
    {
        if (EmptySpots.Count > 0)
        {
            int randichoice = UnityEngine.Random.Range(0, EmptySpots.Count);
            Vector2 randPosChoice = EmptySpots[randichoice];
            EmptySpots.RemoveAt(randichoice);

            // This tilemap supported Instantiate script is just to ensure turrets get created at empty spots, also in center of the cell
            // no matter the size of grids or tilemaps :)
            GameObject TurretPrefab = TurretPrefabs[UnityEngine.Random.Range(0, TurretPrefabs.Length)];

            if (TurretPrefab.TryGetComponent<Rocketeer>(out Rocketeer roketeer))
            {
                rocketeers.Add((randPosChoice + new Vector2(GameGrid.cellSize.x / 2, GameGrid.cellSize.y / 2)) * GameGrid.cellSize.x);
            }
            else
            {
                normals.Add((randPosChoice + new Vector2(GameGrid.cellSize.x / 2, GameGrid.cellSize.y / 2)) * GameGrid.cellSize.x);
            }
            GameObject Turret = Instantiate(TurretPrefab, (randPosChoice + new Vector2(GameGrid.cellSize.x/2, GameGrid.cellSize.y/2))*GameGrid.cellSize.x, Quaternion.identity);
            Turret.transform.parent = transform;

        }
    }
}
