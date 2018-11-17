using Godot;
using System;

public class LevelTimer : Timer {
    public override void _Ready() {
    }

    public override void _PhysicsProcess(float delta) {
        if (this.GetTimeLeft() <= 0.0f) {
            Console.WriteLine("LOST");
            Player player = GetTree().GetRoot().GetNode("World/Player") as Player;
            if (player == null) {
                Console.WriteLine("Something went terribly wrong");
            }
            player.Die();
        }
    }
}
