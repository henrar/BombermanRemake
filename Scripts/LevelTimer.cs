using Godot;
using System;

public class LevelTimer : Timer {
    public override void _Ready() {
    }

    public override void _Process(float delta) {
        if (this.GetTimeLeft() <= 0.0f) {
            Console.WriteLine("LOST");
            GetTree().SetPause(true);
        }
    }
}
