using Godot;

public class EndScreen: Node {
    private SoundPlayer soundPlayer;

    public override void _Ready() {
        this.soundPlayer = GetTree().GetRoot().GetNode("SoundPlayer") as SoundPlayer;
        this.soundPlayer.PlayMusic(Music.Menu);
    }

    public override void _PhysicsProcess(float delta) {
        if (Input.IsActionPressed("ui_accept") || Input.IsActionPressed("ui_cancel") || Input.IsActionPressed("ui_select")) {
            this.soundPlayer.PlaySoundEffect(SoundEffect.PushMenu);
            GetTree().ChangeScene("res://Scenes/MainMenu.tscn");
        }
    }
}