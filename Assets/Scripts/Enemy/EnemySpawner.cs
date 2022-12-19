using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Tilemaps;
using System;

[Serializable]
public class SpawnerData // Do not inherit from MonoBehaviour here
{
    [SerializeField] public float timePassed = 0f;
    [SerializeField] public float minSpawnTime = 5f;
    [SerializeField] public float maxSpawnTime = 12f;
    [SerializeField] public float spawnIn = 0f;
    [SerializeField] public float timeBetweenDifficultyUpgrades = 10f;
    [SerializeField]
    public int startDifficulty = 1;
    [SerializeField]
    public float DifficultySpawnTimeReducer = 0.5f;
    [SerializeField]
    public float DifficultySpeedIncreaseAmount = 0.4f;
    [SerializeField]
    public float enemyStartSpeed = 1f;
    [SerializeField] public List<Vector2> Waypoints;

}
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] public float timePassed = 0f;
    [SerializeField] public float minSpawnTime = 5f;
    [SerializeField] public float maxSpawnTime = 12f;
    [SerializeField] public float spawnIn = 0f;
    [SerializeField] public float timeBetweenDifficultyUpgrades = 10f;
    [SerializeField] public int startDifficulty = 1;
    [SerializeField] public float DifficultySpawnTimeReducer = 0.5f;
    [SerializeField] public float DifficultySpeedIncreaseAmount = 0.4f;
    [SerializeField] public float enemyStartSpeed = 1f;

    [SerializeField] public List<Vector2> Waypoints;

    [SerializeField] private GameObject enemyPrefab;


    public SpawnerData spData;

    private List<Enemy> enemies = new List<Enemy>(); 



    // Serialized vals to use for road navigator waypoints generation
    [Space(50)]
    [Header("Waypoint Generation!")]


    [SerializeField] private Grid gameGrid;
    [SerializeField] private Tilemap mapTile;

    [Space(20)]
    // (add start and end tiles to this list too!)
    [SerializeField] private List<TileBase> CornerTiles;
    [Space(10)]
    [SerializeField] private List<TileBase> RoadTiles;
    
    [Space(10)]
    [SerializeField] private TileBase startTile;
    [SerializeField] private TileBase endTile;


    private void Awake()
    {
        string enemySp = PlayerPrefs.GetString("SpawnerData");
        SpawnerData ths = JsonUtility.FromJson<SpawnerData>(enemySp);
        if (ths == null) return;
        Debug.Log(enemySp);
        Debug.Log(ths);
        timePassed = ths.timePassed;
        minSpawnTime = ths.minSpawnTime;
        maxSpawnTime = ths.maxSpawnTime;
        spawnIn = ths.spawnIn;

        timeBetweenDifficultyUpgrades = ths.timeBetweenDifficultyUpgrades;
        startDifficulty = ths.startDifficulty;
        DifficultySpawnTimeReducer = ths.DifficultySpawnTimeReducer;
        DifficultySpeedIncreaseAmount = ths.DifficultySpeedIncreaseAmount;
        enemyStartSpeed = ths.enemyStartSpeed;
        Waypoints = ths.Waypoints;
    }

    void Start()
    {
        // did not bother pre-referancing waypoints
        // ill probably regret this when I get to the save system.. but nvm we'll see.. 16.12.2022 01:04

        /*
             Waypoints = FindObjectsOfType<Waypoint>();
        */


        // yeah I did regret but not because of save system. Apparently findobjectsoftype just randomly picks objects Lol.
        // I'll cache them myself by hand than becuase I'm lazy to order them by the number at the end of their name.. 16.12.2022 01:11


        // I had a great idea! Could implement a solution where instead of hand placing waypoints, it will find its way through
        // the tilemap by checking for corner pieces!! this should allow for level to be modified however you want.
        // however, ordering these waypoints will be much harder.. let me think of a solution.. 16.12.2022 02:02

        // think I found a way! I will foreach loop every corner tilemap and pair them with other tilemaps.
        // during this pair, I will only compute for those that are in the same x or same y coordinate since they should be linear
        // (diagonal movement is not a case in this project so this is ok to use to save some compute power)
        // after this check, I will see if they have road pieces between them by interpolating x or y coordinates!
        // don't know if this an overkill for an demo project but nvm. I guess it'll be fun 16.12.2022 02:08


        // WAYPOINT GENERATOR !
        if (Waypoints.Count != 0) return;
        // Get Corner Pieces
        List<Vector2> cornerTiles = new List<Vector2>();

        Vector2 startPos = new Vector2();
        Vector2 endPos = new Vector2();

        
        // Assume tilemap is at the origin and cache cornertiles/starttile/endtile
        for (int i = -Mathf.FloorToInt(mapTile.size.x / 2); i < mapTile.size.x - Mathf.FloorToInt(mapTile.size.x / 2); i++)
        {
            for (int j = -Mathf.FloorToInt(mapTile.size.y / 2); j < mapTile.size.y - Mathf.FloorToInt(mapTile.size.y / 2); j++)
            {
                TileBase thisTile = mapTile.GetTile(new Vector3Int(i, j));
                if (CornerTiles.Contains(thisTile)) cornerTiles.Add(new Vector2(i,j));
                if (thisTile == startTile) startPos = new Vector2(i, j);
                else if (thisTile == endTile) endPos = new Vector2(i, j);
            }
        }


        Debug.Log("Corners Found: " + cornerTiles.Count);
        Debug.Log("Start Pos: " + startPos * 1.25f);

        // now is the hard part.. since we have every corner piece lets math them by pairs and do necessary checks!
        // !! Start tile is the one that defined at the top!

        Vector2 TileLeftOff = startPos;

        // Remove start tile manually since it is not possible in any way to return to the start tile.
        cornerTiles.Remove(TileLeftOff);

        

            
        List<Vector2> tilesProcessed = new List<Vector2>();


        // For every cornertile, choose any other corner tile where either x or y position of tile is the same
        // and tiles between these two corners should be inside the list roadtiles
        for (int count = 0; count < cornerTiles.Count; count++)
        {
            // 2nd tile to check
            foreach (Vector2 compareThisVector in cornerTiles)
            {
                // if already "visited" corner, skip
                if (tilesProcessed.Contains(compareThisVector)) continue;

                bool validRoadFound = true;

                if (TileLeftOff.x == compareThisVector.x)
                {

                    if (TileLeftOff.y < compareThisVector.y)
                    {
                        for (int i = (int)TileLeftOff.y+1; i < compareThisVector.y; i++)
                        {
                            if (!RoadTiles.Contains(mapTile.GetTile(new Vector3Int((int)TileLeftOff.x, i))))
                            {
                                // if x coor. of tiles are same but tiles in between are not all road
                                validRoadFound = false;
                                break;
                            }
                        }
                    }

                    else
                    {
                        for (int i = (int)compareThisVector.y + 1; i < TileLeftOff.y; i++)
                        {
                            if (!RoadTiles.Contains(mapTile.GetTile(new Vector3Int((int)TileLeftOff.x, i))))
                            {
                                validRoadFound = false;
                                break;
                            }
                        }
                    }
                }

                if (TileLeftOff.y == compareThisVector.y)
                {

                    if (TileLeftOff.x < compareThisVector.x)
                    {
                        for (int i = (int)TileLeftOff.x+1; i < compareThisVector.x; i++)
                        {
                            if (!RoadTiles.Contains(mapTile.GetTile(new Vector3Int(i, (int)TileLeftOff.y))))
                            {
                                // if y coor. of tiles are same but tiles in between are not all road
                                validRoadFound = false;
                                break;
                            }
                        }
                    }

                    else
                    {
                        for (int i = (int)compareThisVector.x+1; i < TileLeftOff.x; i++)
                        {
                            if (!RoadTiles.Contains(mapTile.GetTile(new Vector3Int(i, (int)TileLeftOff.y))))
                            {
                                validRoadFound = false;
                                break;
                            }
                        }
                    }

                }

                if (compareThisVector.x != TileLeftOff.x && compareThisVector.y != TileLeftOff.y) validRoadFound = false;


                // if none of conditions broke the pair Vector2 pair ( compareThisVector, TileLeftOff ), than they should be connected
                if (validRoadFound)
                {
                    Waypoints.Add(compareThisVector * gameGrid.cellSize.x + new Vector2(mapTile.cellSize.x / 2, mapTile.cellSize.y / 2));
                    TileLeftOff = compareThisVector;
                    tilesProcessed.Add(TileLeftOff);
                    break;

                }

            }
        }


        // Pheww.. did it :D

        // since this sc. relies on tilemaps only, therotically this could be implemented to any game that level created with tilemap!
        // - Emir Yaman Sivrikaya 16.12.2022 03:28 
        // took 1 hr 20 mins 

    }

    void Update()
    {
        timePassed += Time.deltaTime;
        spawnIn -= Time.deltaTime;

        // Do spawn new enemy if random time passed btween min-max "spawntime"
        if (spawnIn <= 0f)
        {
            SpawnEnemy();
            spawnIn = UnityEngine.Random.Range(minSpawnTime, maxSpawnTime);
        }
        

        // Difficulty upgrade -- Upgrades both enemy speed and speeds up new enemies >:] 
        if (timePassed >= timeBetweenDifficultyUpgrades)
        {
            timePassed = timePassed % timeBetweenDifficultyUpgrades;
            if (minSpawnTime > DifficultySpawnTimeReducer) minSpawnTime -= DifficultySpawnTimeReducer;
            if (maxSpawnTime > DifficultySpawnTimeReducer) maxSpawnTime -= DifficultySpawnTimeReducer;

            enemyStartSpeed += DifficultySpeedIncreaseAmount;
            startDifficulty++;
            if (minSpawnTime - maxSpawnTime < 0.2 || minSpawnTime - maxSpawnTime > -0.2)
            {
                maxSpawnTime += 5;
                DifficultySpawnTimeReducer = 0.2f;
            }

            Events.DifficultyIncrease?.Invoke();
        }
    }

    private void SpawnEnemy()
    {
        Enemy Enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity).GetComponent<Enemy>();
        enemies.Add(Enemy);
        //set enemy vals
        Enemy.gameObject.transform.parent = transform;
        Enemy.waypoints = Waypoints;
        Enemy.speed = enemyStartSpeed;
    }
}
