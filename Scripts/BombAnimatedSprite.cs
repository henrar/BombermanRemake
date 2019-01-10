using System;
using Godot;

class BombAnimatedSprite : Node2D {
    private enum BombSpriteNum {
        JustDropped = 0,
        FuseLit = 1,
        BeforeExplosion = 2,
    }

    private AnimatedSprite bombAnimatedSprite;

    private readonly string bombTickingAnimnName = "bomb_ticking";

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

    public void SwapBombTexture(float timeLeft) {
        if (timeLeft < 1.5f && timeLeft >= 0.7f) {
            this.bombAnimatedSprite.SetFrame((int)BombSpriteNum.FuseLit);
        } else if (timeLeft < 0.7f) {
            this.bombAnimatedSprite.SetFrame((int)BombSpriteNum.BeforeExplosion);
        }
    }

    public void SetBombAnimation() {
        LoadBombAnimatedSprite();

        this.bombAnimatedSprite.SetZIndex(this.map.GetZIndex() + 5);

        this.bombAnimatedSprite.SetAnimation(this.bombTickingAnimnName);
        this.bombAnimatedSprite.SetFrame((int)BombSpriteNum.JustDropped);

        this.bombAnimatedSprite.SetPosition(GetPosition());

        AddChild(this.bombAnimatedSprite);
    }
}
