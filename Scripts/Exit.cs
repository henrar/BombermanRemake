using Godot;
using System;

public class Exit : Node2D {
    private Sprite sprite;
    private TileMap map;
    public Vector2 positionOnTileMap;
    public bool active;
    public bool exploded;

    private readonly string pathToOpenDoorTexture = "res://Assets/assetyver2/opendoor.png";
    private readonly string pathToClosedDoorTexture = "res://Assets/assetyver2/door.png";
    private readonly string pathToOpenDoorWithGhostsTexture = "res://Assets/assetyver2/opendoorwithghost.png";

    public override void _Ready() {
        this.map = GetTree().GetRoot().GetNode("World/TileMap") as TileMap;
        this.active = false;
        this.exploded = false;
        this.SetZIndex(this.map.GetZIndex() + 2);
    }

    public void LoadTexture() {
        if (this.sprite != null) {
            RemoveChild(this.sprite);
        }

        this.sprite = new Sprite();
        ImageTexture exitTexture = new ImageTexture();

        if (this.exploded) {
            exitTexture.Load(this.pathToOpenDoorWithGhostsTexture);
        } else if (this.active) {
            exitTexture.Load(this.pathToOpenDoorTexture);
        } else {
            exitTexture.Load(this.pathToClosedDoorTexture);
        }

        sprite.SetTexture(exitTexture);

        sprite.SetGlobalPosition(this.map.GetPositionOfTileCenter(this.positionOnTileMap));

        AddChild(sprite);
    }
}
