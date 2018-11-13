using Godot;
using System;

public class SceneVariables : Node {
    public Node CurrentScene { get; set; }
    public int savedNumberOfLives;

    public override void _Ready() {        
        Viewport root = GetTree().GetRoot();
        CurrentScene = root.GetChild(root.GetChildCount() - 1);
        this.savedNumberOfLives = 3;
        Console.WriteLine("Scene variables: " + this.savedNumberOfLives);
    }

    //    public override void _Process(float delta) {       
    //    }
}
