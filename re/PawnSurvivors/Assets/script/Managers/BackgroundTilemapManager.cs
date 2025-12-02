using UnityEngine;
using UnityEngine.Tilemaps;
using PawnSurvivors.Managers;

public class BackgroundTilemapManager : MonoBehaviour
{
    [Header("Grid 참조")]
    [SerializeField] private GameObject gridObject;
    [SerializeField] private string gridObjectName = "BackgroundGrid";
    
    private Grid _grid;
    private Tilemap _backgroundTilemap;
    
    [Header("배경 설정")]
    [SerializeField] private string tileSpriteFolderPath = "Sprites/tiles";
    [SerializeField] private Vector2Int mapSize = new Vector2Int(50, 50);
    [SerializeField] private Vector3 cellSize = Vector3.zero;
    [SerializeField] private int sortingOrder = -10;
    [SerializeField] private bool useRandomTiles = true;

    public void CreateBackgroundTilemap()
    {
        if (gridObject == null)
        {
            gridObject = GameObject.Find(gridObjectName);
        }

        if (gridObject == null)
        {
            LogManager.LogError(LogCategory.System, $"'{gridObjectName}' 오브젝트를 찾을 수 없습니다.");
            return;
        }

        _grid = gridObject.GetComponent<Grid>();
        if (_grid == null)
        {
            _grid = gridObject.AddComponent<Grid>();
        }

        Sprite[] tileSprites = Resources.LoadAll<Sprite>(tileSpriteFolderPath);
        
        Vector3 calculatedCellSize = cellSize;
        if (calculatedCellSize == Vector3.zero && tileSprites != null && tileSprites.Length > 0)
        {
            Sprite firstSprite = tileSprites[0];
            if (firstSprite != null)
            {
                float spriteWidth = firstSprite.rect.width / firstSprite.pixelsPerUnit;
                float spriteHeight = firstSprite.rect.height / firstSprite.pixelsPerUnit;
                calculatedCellSize = new Vector3(spriteWidth, spriteHeight, 0f);
            }
        }
        
        if (calculatedCellSize == Vector3.zero)
        {
            calculatedCellSize = new Vector3(1f, 1f, 0f);
        }

        _grid.cellSize = calculatedCellSize;
        _grid.cellLayout = GridLayout.CellLayout.Rectangle;

        Transform tilemapTransform = gridObject.transform.Find("BackgroundTilemap");
        GameObject tilemapObject;
        
        if (tilemapTransform != null)
        {
            tilemapObject = tilemapTransform.gameObject;
        }
        else
        {
            tilemapObject = new GameObject("BackgroundTilemap");
            tilemapObject.transform.SetParent(gridObject.transform);
        }
        
        _backgroundTilemap = tilemapObject.GetComponent<Tilemap>();
        if (_backgroundTilemap == null)
        {
            _backgroundTilemap = tilemapObject.AddComponent<Tilemap>();
        }
        
        TilemapRenderer renderer = tilemapObject.GetComponent<TilemapRenderer>();
        if (renderer == null)
        {
            renderer = tilemapObject.AddComponent<TilemapRenderer>();
        }
        renderer.sortingOrder = sortingOrder;

        _backgroundTilemap.ClearAllTiles();
        LoadAndPlaceTiles(tileSprites);
    }

    private void LoadAndPlaceTiles(Sprite[] tileSprites = null)
    {
        if (tileSprites == null)
        {
            tileSprites = Resources.LoadAll<Sprite>(tileSpriteFolderPath);
        }
        
        if (tileSprites == null || tileSprites.Length == 0)
        {
            CreateDefaultTiles();
            return;
        }

        Tile[] tiles = new Tile[tileSprites.Length];
        for (int i = 0; i < tileSprites.Length; i++)
        {
            tiles[i] = ScriptableObject.CreateInstance<Tile>();
            tiles[i].sprite = tileSprites[i];
        }

        System.Random random = useRandomTiles ? new System.Random() : null;
        
        for (int x = -mapSize.x / 2; x < mapSize.x / 2; x++)
        {
            for (int y = -mapSize.y / 2; y < mapSize.y / 2; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                Tile selectedTile = (useRandomTiles && tiles.Length > 1) ? tiles[random.Next(0, tiles.Length)] : tiles[0];
                _backgroundTilemap.SetTile(position, selectedTile);
            }
        }
    }

    private void CreateDefaultTiles()
    {
        Texture2D texture = new Texture2D(32, 32);
        Color[] pixels = new Color[32 * 32];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = new Color(0.3f, 0.3f, 0.4f, 1f);
        }
        texture.SetPixels(pixels);
        texture.Apply();
        
        Sprite defaultSprite = Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
        Tile tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = defaultSprite;

        for (int x = -mapSize.x / 2; x < mapSize.x / 2; x++)
        {
            for (int y = -mapSize.y / 2; y < mapSize.y / 2; y++)
            {
                _backgroundTilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }

    public void DestroyBackgroundTilemap()
    {
        if (_backgroundTilemap != null)
        {
            _backgroundTilemap.ClearAllTiles();
        }
        _backgroundTilemap = null;
        _grid = null;
    }

    public void SetTileSpriteFolderPath(string path)
    {
        tileSpriteFolderPath = path;
    }

    public void SetMapSize(Vector2Int size)
    {
        mapSize = size;
    }
}
