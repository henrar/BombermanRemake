using Godot;
using System;

enum TileTypes {
    TileType_Grass = 0,
    TileType_Wall = 1,
    TileType_Bricks = 2
}

public class TileMap : Godot.TileMap {
    public Dictionary<Enemy, Vector2> enemyOnCell;
    private Random random = new Random();

    public override void _Ready() {
        this.enemyOnCell = new Dictionary<Enemy, Vector2>();
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

        Godot.Array usedCells = this.GetUsedCells();

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

    private Godot.Array GetGrassTilesPosition() {
        return this.GetUsedCellsById((int)TileTypes.TileType_Grass);
    }

    private Godot.Array GetBrickCellPosition() {
        return this.GetUsedCellsById((int)TileTypes.TileType_Bricks);
    }

    public Vector2 GetRandomGrassCell() {
        Godot.Array grassCells = GetGrassTilesPosition();
        int index = this.random.Next(0, grassCells.Count);
        return (Vector2)grassCells[index];
    }

    public Vector2 GetRandomBrickCell() {
        Godot.Array brickCells = GetBrickCellPosition();
        int index = this.random.Next(0, brickCells.Count);
        return (Vector2)brickCells[index];
    }
}

