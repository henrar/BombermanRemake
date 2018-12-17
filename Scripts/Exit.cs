﻿using Godot;

public class Exit : Node {
    private Sprite sprite;
    private TileMap map;
    public Vector2 positionOnTileMap;

    public override void _Ready() {
        this.map = GetTree().GetRoot().GetNode("World/TileMap") as TileMap;
    }

    public void LoadTexture() {
        this.sprite = new Sprite();
        ImageTexture exitTexture = new ImageTexture();
        exitTexture.Load("res://Assets/assetyver2/door.png");
        sprite.SetTexture(exitTexture);
        sprite.SetPosition(this.map.GetPositionOfTileCenter(this.positionOnTileMap));

        AddChild(sprite);
    }

    public void DestroyExit() {
        this.positionOnTileMap = TileMap.invalidTile;
        this.map.exitTileUncovered = false;
        this.QueueFree();
        this.map.exitTile = null;
    }
}
