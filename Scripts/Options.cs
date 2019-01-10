using Godot;

public class Options : Node {
    public override void _Ready() {

    }

    public override void _PhysicsProcess(float delta) {
        if (Input.IsActionPressed("ui_cancel")) {
            GetTree().ChangeScene("res://Scenes/MainMenu.tscn");
        }
    }
}