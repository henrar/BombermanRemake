using Godot;
using System;

public class World : Node {
    private SceneVariables sceneVariables;

    public override void _Ready() {
        this.sceneVariables = GetTree().GetRoot().GetNode("SceneVariables") as SceneVariables;
    }

    public void Reload() {
        GetTree().ReloadCurrentScene();
    }

    public void SwitchLevel() {
        if (this.sceneVariables.currentLevel < 3) {
            this.sceneVariables.currentLevel += 1;
            GetTree().ChangeScene("res://Scenes/Level" + this.sceneVariables.currentLevel + ".tscn");
        }
    }

    public void GoToMainMenu() {
        GetTree().ReloadCurrentScene(); //TODO: change to go to menu
    }
}
