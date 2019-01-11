using Godot;
using System;

public class LevelTimer : Timer {
    private TileMap map;
    private bool coinSpawned;

    public override void _Ready() {
        this.map = GetTree().GetRoot().GetNode("World/TileMap") as TileMap;
        this.coinSpawned = false;
    }

    public override void _PhysicsProcess(float delta) {
        if (this.GetTimeLeft() <= 0.0f && !this.coinSpawned) {
            Console.WriteLine("LOST");
            this.map.SpawnCoins();
            this.coinSpawned = true;
        }
    }
}
