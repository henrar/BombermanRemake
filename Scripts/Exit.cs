using Godot;

public class Exit : Node2D {
    private Sprite sprite;
    private TileMap map;
    public Vector2 positionOnTileMap;
    public bool active;

    public override void _Ready() {
        this.map = GetTree().GetRoot().GetNode("World/TileMap") as TileMap;
        this.active = false;
        this.SetZIndex(this.map.GetZIndex() + 2);
    }

    public void LoadTexture() {
        if (this.sprite != null) {
            RemoveChild(this.sprite);
        }

        this.sprite = new Sprite();
        ImageTexture exitTexture = new ImageTexture();

        if (this.active) {
            exitTexture.Load("res://Assets/assetyver2/opendoor.png");
        } else {
            exitTexture.Load("res://Assets/assetyver2/door.png");
        }

        sprite.SetTexture(exitTexture);

        sprite.SetGlobalPosition(this.map.GetPositionOfTileCenter(this.positionOnTileMap));

        AddChild(sprite);
    }
}
