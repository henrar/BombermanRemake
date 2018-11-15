using Godot;
using System;

public class TileMap : Godot.TileMap { 
    public Dictionary<Enemy, Vector2> enemyOnCell;

    public override void _Ready() {
        this.enemyOnCell = new Dictionary<Enemy, Vector2>();
    }

    public override void _Process(float delta) {
    }

    public Enemy findEnemyOnCell(Vector2 pos) {
        foreach(var entry in this.enemyOnCell) {
            if (entry.Value == pos) {
                return entry.Key;
            }
        }
        return null;
    }

    public void removeEnemyEntry(Enemy enemy) {
        this.enemyOnCell.Remove(enemy);
    }
}
