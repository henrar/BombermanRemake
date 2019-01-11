using Godot;

class Credits: Node {
    public override void _PhysicsProcess(float delta) {
        if (Input.IsActionPressed("ui_accept") || Input.IsActionPressed("ui_cancel") || Input.IsActionPressed("ui_select")) {
            GetTree().ChangeScene("res://Scenes/MainMenu.tscn");
        }
    }
}
