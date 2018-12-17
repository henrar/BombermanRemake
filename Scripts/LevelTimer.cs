using Godot;
using System;

public class LevelTimer : Timer {
    private TileMap map;

    public override void _Ready() {
        this.map = GetTree().GetRoot().GetNode("World/TileMap") as TileMap;
    }

    public override void _PhysicsProcess(float delta) {
        if (this.GetTimeLeft() <= 0.0f) {
            Console.WriteLine("LOST");
            this.map.SpawnCoins();
        }
    }
}
