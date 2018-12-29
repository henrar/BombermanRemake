﻿using System;
using Godot;

class BombAnimatedSprite: Node2D {
    private AnimatedSprite bombAnimatedSprite;
    private AnimatedSprite explosionAnimatedSprite;

    private readonly string bombTickingAnimnName = "bomb_ticking";
    private readonly string explosionAnimName = "explosion";

    public readonly string pathToSplash = "res://Assets/explosion_anim/splash.png";
    public readonly string pathToDestroyBox = "res://Assets/explosion_anim/explosion_anim_2/boxdestroy.png";

    private TileMap map;

    public override void _Ready() {
        this.map = GetTree().GetRoot().GetNode("World/TileMap") as TileMap;
    }

    private void LoadBombAnimatedSprite() {
        this.bombAnimatedSprite = new AnimatedSprite();

        SpriteFrames spriteFrames = new SpriteFrames();
        spriteFrames.AddAnimation(this.bombTickingAnimnName);

        for (int i = 1; i <= 3; ++i) {
            ImageTexture tex = new ImageTexture();
            tex.Load("res://Assets/explosion_anim/bomb-" + i + "s.png");
            spriteFrames.AddFrame(this.bombTickingAnimnName, tex);
        }

        this.bombAnimatedSprite.SetSpriteFrames(spriteFrames);
        this.bombAnimatedSprite.SetAnimation(this.bombTickingAnimnName);
    }

    private void LoadExplosionAnimatedSprite() {
        this.explosionAnimatedSprite = new AnimatedSprite();

        SpriteFrames spriteFrames = new SpriteFrames();
        spriteFrames.AddAnimation(this.explosionAnimName);

        for(int i = 1; i <= 5; ++i) {

        }
    }

    public void SwapBombTexture() {

    }

    public void SetBombAnimation() {
        LoadBombAnimatedSprite();
        LoadExplosionAnimatedSprite();

        this.bombAnimatedSprite.SetZIndex(this.map.GetZIndex() + 5);
        this.explosionAnimatedSprite.SetZIndex(this.map.GetZIndex() + 5);

        this.bombAnimatedSprite.SetAnimation(this.bombTickingAnimnName);
        this.bombAnimatedSprite.SetPosition(GetPosition());

        AddChild(this.bombAnimatedSprite);

        this.bombAnimatedSprite.SetPosition(new Vector2(0, 0));
    }
}