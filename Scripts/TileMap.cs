using Godot;
using System;
using System.Linq;

public enum TileType {
    TileType_Bricks = 0,
    TileType_Wall = 1,
    TileType_Floor = 2,
    TileType_Pipe = 3,
    TileType_Railway = 4,
    TileType_Corner = 5,
    TileType_RDCorner = 6,
    TileType_LUCorner = 7,
    TileType_Floor2 = 8,
}

public class TileMap : Godot.TileMap {
    public Dictionary<Enemy, Vector2> enemyOnCell;
    public Dictionary<Vector2, Powerup> powerups;
    public Exit exitTile;
    public bool exitTileUncovered;

    private Random random = new Random();
    private bool generatedEnemies;
    public Godot.Array<Enemy> enemies;
    public Dictionary<Bomb, Vector2> droppedBombPositions;
    public static Vector2 invalidTile = new Vector2(-1, -1);

    private SceneVariables sceneVariables;
    private SoundPlayer soundPlayer;

    private bool lastEnemyPlayed;

    public override void _Ready() {
        this.soundPlayer = GetTree().GetRoot().GetNode("SoundPlayer") as SoundPlayer;
        this.sceneVariables = GetTree().GetRoot().GetNode("SceneVariables") as SceneVariables;

        this.enemyOnCell = new Dictionary<Enemy, Vector2>();

        GenerateBricks();
        this.generatedEnemies = false;
        this.enemies = new Godot.Array<Enemy>();
        this.droppedBombPositions = new Dictionary<Bomb, Vector2>();

        this.exitTile = null;
        this.exitTileUncovered = false;

        this.powerups = new Dictionary<Vector2, Powerup>();
        GeneratePowerups();
        GenerateExit();

        this.lastEnemyPlayed = false;
    }

    public override void _PhysicsProcess(float delta) {
        if (!this.generatedEnemies) {
            GenerateEnemies();
            this.generatedEnemies = true;
        }

        CheckForExitActivation();

        if (GetEnemyCountByType(EnemyType.Ghost) <= 0 && this.exitTileUncovered) {
            this.exitTile.exploded = false;
            this.exitTile.LoadTexture();
        }

        if (GetRemainingEnemiesCount() == 0 && this.lastEnemyPlayed) {
            this.soundPlayer.PlaySoundEffect(SoundEffect.LastEnemy);
            this.lastEnemyPlayed = true;
        }

        if (this.exitTile != null && this.exitTile.positionOnTileMap != invalidTile && GetCellv(this.exitTile.positionOnTileMap) == (int)TileType.TileType_Floor && !this.exitTileUncovered) {
            Console.WriteLine("Uncovered exit!");
            this.exitTile.LoadTexture();

            this.exitTileUncovered = true;
        }
    }

    private void CheckForExitActivation() {
        if (this.exitTileUncovered) {
            if ((this.enemies == null || this.enemies.Count == 0) && !this.exitTile.active) {
                this.exitTile.active = true;
                this.exitTile.LoadTexture();
            } else if (this.enemies != null && this.enemies.Count > 0 && this.exitTile.active) {
                this.exitTile.active = false;
                this.exitTile.LoadTexture();
            }
        }
    }

    public Vector2 FindPowerUpPosition(Powerup powerup) {
        foreach (var entry in this.powerups) {
            if (entry.Value == powerup) {
                return entry.Key;
            }
        }
        return invalidTile;
    }

    public Bomb FindBombOnTile(Vector2 tile) {
        foreach (var entry in this.droppedBombPositions) {
            if (entry.Value == tile) {
                return entry.Key;
            }
        }
        return null;
    }

    public Enemy FindEnemyOnTile(Vector2 pos) {
        foreach (var entry in this.enemyOnCell) {
            if (entry.Value == pos) {
                return entry.Key;
            }
        }
        return null;
    }

    public void RemoveEnemyEntry(Enemy enemy) {
        this.enemyOnCell.Remove(enemy);
    }

    public Vector2 GetTileMapDimensions() {
        Vector2 dim = new Vector2(0, 0);

        Godot.Array usedCells = GetUsedCells();

        if (usedCells.Count == 0) {
            return dim;
        }

        Vector2 minCell = (Vector2)usedCells[0];
        Vector2 maxCell = minCell;

        float minX = minCell.x;
        float minY = minCell.y;
        float maxX = minX;
        float maxY = minY;

        for (int i = 1; i < usedCells.Count; ++i) {
            Vector2 position = (Vector2)usedCells[i];

            if (position.x < minX) {
                minX = position.x;
            } else if (position.x > maxX) {
                maxX = position.x;
            }

            if (position.y < minY) {
                minY = position.y;
            } else if (position.y > maxY) {
                maxY = position.y;
            }

        }
        dim.x = maxX - minX + 1;
        dim.y = maxY - minY + 1;
        return dim;
    }

    private Godot.Array GetCells(TileType type) {
        return GetUsedCellsById((int)type);
    }

    private int GetTileCount(TileType type) {
        Godot.Array cells = GetCells(type);
        return cells.Count;
    }

    public Vector2 GetRandomCell(TileType type) {
        Godot.Array cells = GetCells(type);
        int index = this.random.Next(0, cells.Count);
        return (Vector2)cells[index];
    }

    private bool IsNotAllowedCell(Vector2 cell) {
        return (cell.x == 1 && cell.y >= 1 && cell.y <= 5) || (cell.x >= 1 && cell.x <= 5 && cell.y == 1);
    }

    private void GenerateBricks() {
        int grassTileCount = GetTileCount(TileType.TileType_Floor);
        int maxGeneratedBricks = Convert.ToInt32(grassTileCount * (GetTree().GetRoot().GetNode("SceneVariables") as SceneVariables).maxRandomCellsPercentage);
        int generatedTilesCount = 0;
        while (generatedTilesCount < maxGeneratedBricks) {
            Vector2 tile = GetRandomCell(TileType.TileType_Floor);
            if (IsNotAllowedCell(tile)) {
                continue;
            }

            SetCellv(tile, (int)TileType.TileType_Bricks);
            generatedTilesCount++;
        }
    }

    private void SpawnEnemy(Vector2 tile, EnemyType type, int count) {
        var world = GetTree().GetRoot().GetNode("World");

        Enemy enemy = new Enemy();
        enemy.currentPositionOnTileMap = tile;
        enemy.SetEnemyType(type);
        this.enemies.Add(enemy);
        this.enemyOnCell[enemy] = tile;
        enemy.currentPositionOnTileMap = this.enemyOnCell[enemy];
        enemy.SetName("Enemy" + enemy.enemyType.ToString() + " " + count);

        world.AddChild(enemy);
    }

    private bool isAnyCellNearbyFree(Vector2 tile) {
        return GetCellv(tile + Directions.directionLeft) == (int)TileType.TileType_Floor
            || GetCellv(tile + Directions.directionRight) == (int)TileType.TileType_Floor
            || GetCellv(tile + Directions.directionUp) == (int)TileType.TileType_Floor
            || GetCellv(tile + Directions.directionDown) == (int)TileType.TileType_Floor;
    }

    private void SpawnEnemies(EnemyType type, int maxCount) {
        int enemyCount = 0;

        while (enemyCount < maxCount) {
            Vector2 tile = GetRandomCell(TileType.TileType_Floor);

            if (IsNotAllowedCell(tile) 
                || GetCellv(tile) != (int)TileType.TileType_Floor 
                || FindEnemyOnTile(tile) != null
                || !isAnyCellNearbyFree(tile)) {
                continue;
            }

            SpawnEnemy(tile, type, enemyCount);

            enemyCount++;
        }
    }

    public void GenerateEnemies() {
        SpawnEnemies(EnemyType.Balloon, this.sceneVariables.GetEnemyConfig().numberOfBalloons);
        SpawnEnemies(EnemyType.Mushroom, this.sceneVariables.GetEnemyConfig().numberOfShrooms);
        SpawnEnemies(EnemyType.Barrel, this.sceneVariables.GetEnemyConfig().numberOfBarrels);
    }

    public void UncoverPowerUp(Vector2 tile) {
        if (this.powerups.ContainsKey(tile)) {
            this.powerups[tile].uncovered = true;
            this.powerups[tile].SetSpriteBasedOnType();
            Console.WriteLine("Powerup found!");
        }
    }

    private void AddPowerUp(Vector2 tile) {
        Powerup powerup = new Powerup();
        powerup.SetType((PowerUpType)this.random.Next(0, Enum.GetNames(typeof(PowerUpType)).Length));
        this.powerups[tile] = powerup;
        AddChild(powerup);
        Console.WriteLine("Powerup at: " + tile);
    }

    public void GeneratePowerups() {
        int brickTileCount = GetTileCount(TileType.TileType_Bricks);

        int generatedPowerUps = 0;

        while (generatedPowerUps < this.sceneVariables.numberOfPowerUps) {
            Vector2 tile = GetRandomCell(TileType.TileType_Bricks);

            if (this.powerups.ContainsKey(tile)) {
                continue;
            }

            double result = Convert.ToDouble(this.random.Next(0, 100)) / 100.0f;

            if (result < this.sceneVariables.powerup1DropChance && generatedPowerUps == 0) {
                AddPowerUp(tile);
            }

            if (result < this.sceneVariables.powerup2DropChance && generatedPowerUps == 1) {
                AddPowerUp(tile);
            }

            if (result < this.sceneVariables.powerup3DropChance && generatedPowerUps == 2) {
                AddPowerUp(tile);
            }

            generatedPowerUps++;
        }
    }

    public void GenerateExit() {
        bool exitGenerated = false;

        while (!exitGenerated) {
            Vector2 tile = GetRandomCell(TileType.TileType_Bricks);

            if (this.powerups.ContainsKey(tile)) {
                continue;
            }
            this.exitTile = new Exit();
            exitGenerated = true;
            AddChild(this.exitTile);
            this.exitTile.positionOnTileMap = tile;
            Console.WriteLine("Exit tile: " + this.exitTile.positionOnTileMap);
        }
    }

    public bool IsWall(Vector2 tile) {
        return GetCellv(tile) != (int)TileType.TileType_Floor && GetCellv(tile) != (int)TileType.TileType_Floor2 && GetCellv(tile) != (int)TileType.TileType_Bricks;
    }

    private bool IsFloor(Vector2 tile) {
        return GetCellv(tile) == (int)TileType.TileType_Floor || GetCellv(tile) == (int)TileType.TileType_Floor2;
    }

    private bool IsBricks(Vector2 tile) {
        return GetCellv(tile) == (int)TileType.TileType_Bricks;
    }

    public bool IsTileValidForMovement(Vector2 tile, bool isCollisionDisabled) {
        if (isCollisionDisabled) {
            return IsFloor(tile) || IsBricks(tile);
        }
        return IsFloor(tile);
    }

    public bool IsTileNotValidForMovement(Vector2 tile) {
        return GetCellv(tile) != (int)TileType.TileType_Floor && GetCellv(tile) != (int)TileType.TileType_Floor2;
    }

    public Vector2 GetPositionOfTileCenter(Vector2 tile) {
        Vector2 pos = MapToWorld(tile);
        pos = pos + GetCellSize() / 2.0f;
        return pos;
    }

    public int GetRemainingEnemiesCount() {
        return this.enemies.Count;
    }

    public void SpawnCoins() {
        SpawnEnemyAtLocation(EnemyType.Coin, this.exitTile.positionOnTileMap, this.sceneVariables.coinCountToSpawn);
        this.lastEnemyPlayed = false;
    }

    public void SpawnEnemyAtLocation(EnemyType type, Vector2 tile, int count) {
        for (int i = 0; i < count; ++i) {
            SpawnEnemy(tile, type, i);
        }
        this.lastEnemyPlayed = false;
    }

    private int GetEnemyCountByType(EnemyType type) {
        int count = this.enemies.Where(enemy => enemy.enemyType == type).Count();
        return count;
    }
}

