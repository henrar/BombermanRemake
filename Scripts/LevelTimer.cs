using Godot;
using System;

public class LevelTimer : Timer {
    public override void _Ready() {
		base._Ready();
    }

    public override void _Process(float delta) {
		base._Process(delta);

        if (this.TimeLeft <= 0.0f) {
            //TODO: stop game
		}
    }
}
