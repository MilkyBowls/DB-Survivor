using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;

public class TilemapSpawner : MonoBehaviour
{
    [System.Serializable]
    public class WeightedTile
    {
        public TileBase tile;
        public int weight = 1;
    }

    [System.Serializable]
    public class WeightedProp
    {
        public GameObject prefab;
        public int weight = 1;
    }

    [System.Serializable]
    public class PropGroup
    {
        public string name;
        [Range(0f, 1f)] public float spawnChance = 0.1f;
        public List<WeightedProp> props;
        public float minimumSpacingInTiles = 10f;  // NEW: group-specific spacing
    }

    [Header("Tilemap Settings")]
    public Transform player;
    public Tilemap tilemap;
    public List<WeightedTile> tileOptions;

    [Header("Tile Placement Settings")]
    public float tileSize = 0.3f;
    public int tileRadius = 32;
    public int despawnBuffer = 4;

    [Header("Prop Groups")]
    public List<PropGroup> propGroups;

    private HashSet<Vector2Int> activeTiles = new HashSet<Vector2Int>();
    private Dictionary<Vector3, GameObject> activeProps = new Dictionary<Vector3, GameObject>();
    private Vector2Int lastPlayerTileCoord = Vector2Int.zero;

    [Header("Player Safe Zone")]
    public float propSpawnAvoidRadius = 1f; // Radius (in Unity units) around player to avoid spawning props

    void Start()
    {
        Vector2 playerPos = player.position;
        Vector2Int initialCoord = new Vector2Int(
            Mathf.FloorToInt(playerPos.x / tileSize),
            Mathf.FloorToInt(playerPos.y / tileSize)
        );

        SpawnNewTiles(initialCoord);
        lastPlayerTileCoord = initialCoord;
    }

    void LateUpdate()
    {
        Vector2 playerPos = player.position;
        Vector2Int currentCoord = new Vector2Int(
            Mathf.FloorToInt(playerPos.x / tileSize),
            Mathf.FloorToInt(playerPos.y / tileSize)
        );

        if (currentCoord != lastPlayerTileCoord)
        {
            SpawnNewTiles(currentCoord);
            DespawnFarTiles(currentCoord);
            lastPlayerTileCoord = currentCoord;
        }
    }

    void SpawnNewTiles(Vector2Int center)
    {
        ResizeGridIfNeeded();
        for (int x = -tileRadius; x <= tileRadius; x++)
        {
            for (int y = -tileRadius; y <= tileRadius; y++)
            {
                Vector2Int tileCoord = new Vector2Int(center.x + x, center.y + y);
                if (!activeTiles.Contains(tileCoord))
                {
                    Vector3Int cell = new Vector3Int(tileCoord.x, tileCoord.y, 0);
                    TileBase tile = GetWeightedTile();
                    tilemap.SetTile(cell, tile);
                    activeTiles.Add(tileCoord);

                    Vector3 worldPos = tilemap.CellToWorld(cell) + new Vector3(tileSize / 2f, tileSize / 2f, 0);
                    Bounds tileBounds = new Bounds(worldPos, Vector3.one * tileSize);

                    // Update A* pathfinding graph for this tile using GraphUpdateObject
                    GraphUpdateObject guoTile = new GraphUpdateObject(tileBounds);
                    AstarPath.active.UpdateGraphs(guoTile);

                    // Skip prop spawn near player
                    if (Vector3.Distance(worldPos, player.position) < propSpawnAvoidRadius)
                        continue;

                    foreach (var group in propGroups)
                    {
                        if (group.props.Count == 0) continue;
                        if (Random.value < group.spawnChance)
                        {
                            if (!IsTooCloseToExistingProps(worldPos, group.minimumSpacingInTiles))
                            {
                                GameObject prefab = GetWeightedPropFromGroup(group);
                                if (prefab != null)
                                {
                                    GameObject propInstance = Instantiate(prefab, worldPos, Quaternion.identity, this.transform);
                                    activeProps[worldPos] = propInstance;

                                    // Update A* pathfinding graph for the prop collider
                                    Collider2D col = propInstance.GetComponent<Collider2D>();
                                    if (col != null)
                                    {
                                        Bounds propBounds = col.bounds;

                                        GraphUpdateObject guoProp = new GraphUpdateObject(propBounds);
                                        // Optional: make the prop block movement
                                        guoProp.modifyWalkability = true;
                                        guoProp.setWalkability = false;

                                        AstarPath.active.UpdateGraphs(guoProp);
                                    }

                                    break; // Prevent multiple props per tile
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void ResizeGridIfNeeded()
    {
        var grid = AstarPath.active.data.gridGraph;

        Vector3 playerPos = player.position;
        Bounds currentBounds = grid.bounds;

        if (!currentBounds.Contains(playerPos))
        {
            grid.center = playerPos;
            grid.UpdateTransform();
            AstarPath.active.Scan();
            Debug.Log("A* Grid recentered and rescanned around player.");
        }
    }

    void DespawnFarTiles(Vector2Int center)
    {
        int despawnRadius = tileRadius + despawnBuffer;
        int despawnDistSq = despawnRadius * despawnRadius;

        List<Vector2Int> tilesToRemove = new List<Vector2Int>();

        foreach (Vector2Int tileCoord in activeTiles)
        {
            int dx = tileCoord.x - center.x;
            int dy = tileCoord.y - center.y;
            int distSq = dx * dx + dy * dy;

            if (distSq > despawnDistSq)
            {
                Vector3Int cell = new Vector3Int(tileCoord.x, tileCoord.y, 0);
                tilemap.SetTile(cell, null);
                tilemap.SetTransformMatrix(cell, Matrix4x4.identity);
                tilesToRemove.Add(tileCoord);

                Vector3 worldPos = tilemap.CellToWorld(cell) + new Vector3(tileSize / 2f, tileSize / 2f, 0);

                if (activeProps.TryGetValue(worldPos, out GameObject propGO))
                {
                    Destroy(propGO);
                    activeProps.Remove(worldPos);
                }
            }
        }

        foreach (Vector2Int tileCoord in tilesToRemove)
        {
            activeTiles.Remove(tileCoord);
        }
    }

    bool IsTooCloseToExistingProps(Vector3 position, float minTiles)
    {
        float minDistance = minTiles * tileSize;

        foreach (Vector3 existingPos in activeProps.Keys)
        {
            if (Vector3.Distance(position, existingPos) < minDistance)
            {
                return true;
            }
        }

        return false;
    }

    TileBase GetWeightedTile()
    {
        int total = 0;
        foreach (var wt in tileOptions) total += wt.weight;

        int rand = Random.Range(0, total);
        int curr = 0;

        foreach (var wt in tileOptions)
        {
            curr += wt.weight;
            if (rand < curr)
                return wt.tile;
        }

        return tileOptions[0].tile;
    }

    GameObject GetWeightedPropFromGroup(PropGroup group)
    {
        int total = 0;
        foreach (var prop in group.props) total += prop.weight;

        int rand = Random.Range(0, total);
        int curr = 0;

        foreach (var prop in group.props)
        {
            curr += prop.weight;
            if (rand < curr)
                return prop.prefab;
        }

        return group.props[0].prefab;
    }
}
