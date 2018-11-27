using Godot;
using System;

public enum TileType {
    TileType_Grass = 0,
    TileType_Wall = 1,
    TileType_Bricks = 2
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
    }

    public override void _PhysicsProcess(float delta) {
        if(!this.generatedEnemies) {
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
        int grassTileCount = GetTileCount(TileType.TileType_Grass);
        int maxGeneratedBricks = Convert.ToInt32(grassTileCount * this.maxRandomCellsPercentage);
        int generatedTilesCount = 0;
        while (generatedTilesCount < maxGeneratedBricks) {
            Vector2 tile = GetRandomCell(TileType.TileType_Grass);
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
            Vector2 tile = GetRandomCell(TileType.TileType_Grass);

            if (IsNotAllowedCell(tile) && GetCellv(tile) != (int)TileType.TileType_Grass && FindEnemyOnTile(tile) != null) {
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

    public Vector2 GetPositionOfTileCenter(Vector2 tile) {
        Vector2 pos = MapToWorld(tile);
        pos = pos + GetCellSize() / 2.0f;
        return pos;
    }
}

