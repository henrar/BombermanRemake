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

    public override void _Ready() {
        this.enemyOnCell = new Dictionary<Enemy, Vector2>();
        this.maxRandomCellsPercentage = 0.4f;
        GenerateBricks();
    }

    public override void _PhysicsProcess(float delta) {
    }

    public Enemy FindEnemyOnCell(Vector2 pos) {
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

    private bool isNotAllowedCell(Vector2 cell) {
        return (cell.x == 1 && cell.y == 1) || (cell.x == 1 && cell.y == 2) || (cell.x == 2 && cell.y == 1);
    }

    private void GenerateBricks() {
        int grassTileCount = GetTileCount(TileType.TileType_Grass);
        int maxGeneratedBricks = Convert.ToInt32(grassTileCount * this.maxRandomCellsPercentage);
        int generatedTiles = 0;
        while(generatedTiles < maxGeneratedBricks) {
            Vector2 cell = GetRandomCell(TileType.TileType_Grass);
            if(isNotAllowedCell(cell)) {
                continue;
            }

            SetCellv(cell, (int)TileType.TileType_Bricks);
            generatedTiles++;
        }
    }

    public Vector2 GetPositionOfTileCenter(Vector2 tile) {
        Vector2 pos = MapToWorld(tile);
        pos = pos + GetCellSize() / 2.0f;
        return pos;
    }
}

