using Godot;
using System;

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
    private Godot.Array<Enemy> enemies;
    public Dictionary<Bomb, Vector2> droppedBombPositions;
    public static Vector2 invalidTile = new Vector2(-1, -1);

    private SceneVariables sceneVariables;

    public override void _Ready() {
        this.enemyOnCell = new Dictionary<Enemy, Vector2>();

        this.sceneVariables = GetTree().GetRoot().GetNode("SceneVariables") as SceneVariables;

        GenerateBricks();
        this.generatedEnemies = false;
        this.enemies = new Godot.Array<Enemy>();
        this.droppedBombPositions = new Dictionary<Bomb, Vector2>();

        this.exitTile = null;

        this.powerups = new Dictionary<Vector2, Powerup>();
        GeneratePowerups();
        GenerateExit();
    }

    public override void _PhysicsProcess(float delta) {
        if (!this.generatedEnemies) {
            GenerateEnemies();
            this.generatedEnemies = true;
        }

        if (this.exitTile != null && this.exitTile.positionOnTileMap != invalidTile && GetCellv(this.exitTile.positionOnTileMap) == (int)TileType.TileType_Floor && !this.exitTileUncovered) {
            this.exitTile.LoadTexture();

            this.exitTileUncovered = true;
        }
    }

    public void FreeBombs() {
        foreach (var entry in this.droppedBombPositions) {
            entry.Key.QueueFree();
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
        return (cell.x == 1 && cell.y == 1) || (cell.x == 1 && cell.y == 2) || (cell.x == 2 && cell.y == 1);
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

    public void GenerateEnemies() {
        var world = GetTree().GetRoot().GetNode("World");

        int generatedEnemiesCount = 0;

        while (generatedEnemiesCount < this.sceneVariables.numberOfEnemies) {
            Vector2 tile = GetRandomCell(TileType.TileType_Floor);

            if (IsNotAllowedCell(tile) && GetCellv(tile) != (int)TileType.TileType_Floor && FindEnemyOnTile(tile) != null) {
                continue;
            }

            Enemy enemy = new Enemy();
            enemy.currentPositionOnTileMap = tile;
            enemy.SetEnemyType(EnemyType.EnemyType_Balloon);
            this.enemies.Add(enemy);
            this.enemyOnCell[enemy] = tile;
            enemy.currentPositionOnTileMap = this.enemyOnCell[enemy];

            enemy.SetName("Enemy" + generatedEnemiesCount);
            world.AddChild(enemy);

            generatedEnemiesCount++;
        }
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
    }

    public void GeneratePowerups() {
        int brickTileCount = GetTileCount(TileType.TileType_Bricks);

        int generatedPowerUps = 0;

        bool powerUp1Generated = false;
        bool powerUp2Generated = false;
        bool powerUp3Generated = false;

        while (generatedPowerUps < this.sceneVariables.numberOfPowerUps) {
            Vector2 tile = GetRandomCell(TileType.TileType_Bricks);

            if (this.powerups.ContainsKey(tile)) {
                continue;
            }

            double result = Convert.ToDouble(this.random.Next(0, 100)) / 100.0f;

            if (result < this.sceneVariables.powerup1DropChance && !powerUp1Generated && generatedPowerUps == 0) {
                AddPowerUp(tile);
                powerUp1Generated = true;
            }

            if (result < this.sceneVariables.powerup2DropChance && !powerUp2Generated && generatedPowerUps == 1) {
                AddPowerUp(tile);
                powerUp2Generated = true;
            }

            if (result < this.sceneVariables.powerup3DropChance && !powerUp3Generated && generatedPowerUps == 2) {
                AddPowerUp(tile);
                powerUp3Generated = true;
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
            this.exitTile.positionOnTileMap = tile;
            exitGenerated = true;
            AddChild(this.exitTile);
        }
    }

    public bool IsWall(Vector2 tile) {
        return GetCellv(tile) != (int)TileType.TileType_Floor && GetCellv(tile) != (int)TileType.TileType_Floor2 && GetCellv(tile) != (int)TileType.TileType_Bricks;
    }

    public bool IsTileValidForMovement(Vector2 tile) {
        return GetCellv(tile) == (int)TileType.TileType_Floor || GetCellv(tile) == (int)TileType.TileType_Floor2;
    }

    public bool IsTileNotValidForMovement(Vector2 tile) {
        return GetCellv(tile) != (int)TileType.TileType_Floor && GetCellv(tile) != (int)TileType.TileType_Floor2;
    }

    public Vector2 GetPositionOfTileCenter(Vector2 tile) {
        Vector2 pos = MapToWorld(tile);
        pos = pos + GetCellSize() / 2.0f;
        return pos;
    }

    public void SpawnCoins() {

    }
}

