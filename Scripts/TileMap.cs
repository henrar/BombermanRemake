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
    public float maxRandomCellsPercentage;
    public Dictionary<Enemy, Vector2> enemyOnCell;
    private Random random = new Random();
    private bool generatedEnemies;
    private Godot.Array<Enemy> enemies;
    public Vector2 droppedBombPosition;
    public static Vector2 invalidTile = new Vector2(-1, -1);

    public override void _Ready() {
        this.enemyOnCell = new Dictionary<Enemy, Vector2>();
        this.maxRandomCellsPercentage = 0.4f;
        GenerateBricks();
        this.generatedEnemies = false;
        this.enemies = new Godot.Array<Enemy>();
        this.droppedBombPosition = invalidTile;

        GeneratePowerups();
    }

    public override void _PhysicsProcess(float delta) {
        if (!this.generatedEnemies) {
            GenerateEnemies();
            this.generatedEnemies = true;
        }
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
        int maxGeneratedBricks = Convert.ToInt32(grassTileCount * this.maxRandomCellsPercentage);
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

        while (generatedEnemiesCount < (GetTree().GetRoot().GetNode("SceneVariables") as SceneVariables).numberOfEnemies) {
            Vector2 tile = GetRandomCell(TileType.TileType_Floor);

            if (IsNotAllowedCell(tile) && GetCellv(tile) != (int)TileType.TileType_Floor && FindEnemyOnTile(tile) != null) {
                continue;
            }

            Enemy enemy = new Enemy();
            enemy.currentPositionOnTileMap = tile;
            this.enemies.Add(enemy);
            this.enemyOnCell[enemy] = tile;
            enemy.currentPositionOnTileMap = this.enemyOnCell[enemy];

            enemy.SetName("Enemy" + generatedEnemiesCount);
            world.AddChild(enemy);

            generatedEnemiesCount++;
        }
    }

    public void GeneratePowerups() {
        int brickTileCount = GetTileCount(TileType.TileType_Bricks);

        int generatedPowerUps = 0;

        while (generatedPowerUps < (GetTree().GetRoot().GetNode("SceneVariables") as SceneVariables).numberOfPowerUps) {

            generatedPowerUps++;
        }
    }

    public bool isWall(Vector2 tile) {
        return GetCellv(tile) == (int)TileType.TileType_Floor && GetCellv(tile) == (int)TileType.TileType_Floor2 && GetCellv(tile) == (int)TileType.TileType_Bricks;
    }

    public Vector2 GetPositionOfTileCenter(Vector2 tile) {
        Vector2 pos = MapToWorld(tile);
        pos = pos + GetCellSize() / 2.0f;
        return pos;
    }
}

