using UnityEngine;
using UnityEngine.Tilemaps;
using PawnSurvivors.Managers;

/// <summary>
/// 타일맵으로 배경을 생성하고 관리하는 매니저입니다.
/// </summary>
public class BackgroundTilemapManager : MonoBehaviour
{
    private GameObject _gridObject;
    private Grid _grid;
    private Tilemap _backgroundTilemap;
    
    [Header("배경 설정")]
    [SerializeField] private string tileSpriteFolderPath = "Sprites/tiles"; // Resources 폴더 기준 경로 (폴더)
    [SerializeField] private Vector2Int mapSize = new Vector2Int(50, 50); // 타일맵 크기
    [SerializeField] private Vector3 cellSize = Vector3.zero; // 타일 크기 (0이면 스프라이트에서 자동 계산)
    [SerializeField] private int sortingOrder = -10; // 배경 레이어 순서 (낮을수록 뒤에)
    [SerializeField] private bool useRandomTiles = true; // 여러 타일을 랜덤하게 배치할지 여부

    /// <summary>
    /// 배경 타일맵을 생성합니다.
    /// </summary>
    public void CreateBackgroundTilemap()
    {
        // 이미 생성되어 있으면 스킵
        if (_gridObject != null)
        {
            LogManager.LogWarning(LogCategory.System, "배경 타일맵이 이미 생성되어 있습니다.");
            return;
        }

        // 타일 스프라이트 먼저 로드해서 크기 확인
        Sprite[] tileSprites = Resources.LoadAll<Sprite>(tileSpriteFolderPath);
        
        // cellSize 자동 계산 (스프라이트 PPU 기반)
        Vector3 calculatedCellSize = cellSize;
        if (calculatedCellSize == Vector3.zero && tileSprites != null && tileSprites.Length > 0)
        {
            Sprite firstSprite = tileSprites[0];
            if (firstSprite != null)
            {
                // 스프라이트의 실제 Unity 단위 크기 계산
                // 스프라이트 픽셀 크기 / PPU = Unity 단위 크기
                float spriteWidth = firstSprite.rect.width / firstSprite.pixelsPerUnit;
                float spriteHeight = firstSprite.rect.height / firstSprite.pixelsPerUnit;
                calculatedCellSize = new Vector3(spriteWidth, spriteHeight, 0f);
                LogManager.LogInfo(LogCategory.System, 
                    $"타일 크기 자동 계산: {spriteWidth}x{spriteHeight} (스프라이트: {firstSprite.rect.width}x{firstSprite.rect.height}px, PPU: {firstSprite.pixelsPerUnit})");
            }
        }
        
        // cellSize가 여전히 0이면 기본값 사용
        if (calculatedCellSize == Vector3.zero)
        {
            calculatedCellSize = new Vector3(1f, 1f, 0f);
            LogManager.LogWarning(LogCategory.System, "타일 크기를 자동 계산할 수 없어 기본값(1x1)을 사용합니다.");
        }

        // Grid 생성
        _gridObject = new GameObject("BackgroundGrid");
        _grid = _gridObject.AddComponent<Grid>();
        _grid.cellSize = calculatedCellSize;
        _grid.cellLayout = GridLayout.CellLayout.Rectangle;
        _grid.cellSwizzle = GridLayout.CellSwizzle.XYZ;

        // Tilemap 생성
        GameObject tilemapObject = new GameObject("BackgroundTilemap");
        tilemapObject.transform.SetParent(_gridObject.transform);
        
        _backgroundTilemap = tilemapObject.AddComponent<Tilemap>();
        TilemapRenderer renderer = tilemapObject.AddComponent<TilemapRenderer>();
        renderer.sortingOrder = sortingOrder;
        renderer.sortingLayerName = "Default";

        // 타일 스프라이트 로드 및 배치
        LoadAndPlaceTiles(tileSprites);

        LogManager.LogInfo(LogCategory.System, $"배경 타일맵 생성 완료: {mapSize.x}x{mapSize.y}");
    }

    /// <summary>
    /// 타일 스프라이트를 로드하고 배치합니다.
    /// </summary>
    private void LoadAndPlaceTiles(Sprite[] tileSprites = null)
    {
        // Resources에서 타일 스프라이트들 로드 (폴더에서 모든 스프라이트 로드)
        if (tileSprites == null)
        {
            tileSprites = Resources.LoadAll<Sprite>(tileSpriteFolderPath);
        }
        
        if (tileSprites == null || tileSprites.Length == 0)
        {
            LogManager.LogWarning(LogCategory.System, 
                $"타일 스프라이트를 찾을 수 없습니다: {tileSpriteFolderPath}. " +
                $"Resources 폴더에 해당 경로의 스프라이트를 추가해주세요.");
            
            // 기본 타일 생성 (스프라이트가 없을 때)
            CreateDefaultTiles();
            return;
        }

        LogManager.LogInfo(LogCategory.System, $"{tileSprites.Length}개의 타일 스프라이트를 로드했습니다.");

        // Tile 배열 생성
        Tile[] tiles = new Tile[tileSprites.Length];
        for (int i = 0; i < tileSprites.Length; i++)
        {
            tiles[i] = ScriptableObject.CreateInstance<Tile>();
            tiles[i].sprite = tileSprites[i];
        }

        // 타일맵에 타일 배치
        System.Random random = useRandomTiles ? new System.Random() : null;
        
        for (int x = -mapSize.x / 2; x < mapSize.x / 2; x++)
        {
            for (int y = -mapSize.y / 2; y < mapSize.y / 2; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                
                // 랜덤 타일 선택 또는 첫 번째 타일 사용
                Tile selectedTile;
                if (useRandomTiles && tiles.Length > 1)
                {
                    int randomIndex = random.Next(0, tiles.Length);
                    selectedTile = tiles[randomIndex];
                }
                else
                {
                    selectedTile = tiles[0];
                }
                
                _backgroundTilemap.SetTile(position, selectedTile);
            }
        }
    }

    /// <summary>
    /// 기본 타일을 생성합니다 (스프라이트가 없을 때 사용).
    /// </summary>
    private void CreateDefaultTiles()
    {
        // 간단한 색상 타일 생성
        Texture2D texture = new Texture2D(32, 32);
        Color[] pixels = new Color[32 * 32];
        Color tileColor = new Color(0.3f, 0.3f, 0.4f, 1f); // 어두운 회색
        
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = tileColor;
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        Sprite defaultSprite = Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
        
        Tile tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = defaultSprite;

        // 타일맵에 타일 배치
        for (int x = -mapSize.x / 2; x < mapSize.x / 2; x++)
        {
            for (int y = -mapSize.y / 2; y < mapSize.y / 2; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                _backgroundTilemap.SetTile(position, tile);
            }
        }
        
        LogManager.LogInfo(LogCategory.System, "기본 타일로 배경을 생성했습니다.");
    }

    /// <summary>
    /// 배경 타일맵을 제거합니다.
    /// </summary>
    public void DestroyBackgroundTilemap()
    {
        if (_gridObject != null)
        {
            Destroy(_gridObject);
            _gridObject = null;
            _grid = null;
            _backgroundTilemap = null;
            LogManager.LogInfo(LogCategory.System, "배경 타일맵이 제거되었습니다.");
        }
    }

    /// <summary>
    /// 타일 스프라이트 폴더 경로를 설정합니다.
    /// </summary>
    public void SetTileSpriteFolderPath(string path)
    {
        tileSpriteFolderPath = path;
    }

    /// <summary>
    /// 타일맵 크기를 설정합니다.
    /// </summary>
    public void SetMapSize(Vector2Int size)
    {
        mapSize = size;
    }
}

