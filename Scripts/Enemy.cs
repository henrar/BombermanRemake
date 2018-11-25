using Godot;
using System;
using System.Collections.Generic;

public class Enemy : KinematicBody2D {
    public Vector2 currentPositionOnTileMap;
    private readonly int moveModifier = 180;
    private Vector2 currentDestination;
    private bool reachedDestination;
    private TileMap tileMap;
    private List<Vector2> path;
    private Navigation2D navigation;
    private Vector2 previousDirection;

    public override void _Ready() {
        this.currentDestination = new Vector2();
        this.reachedDestination = true;
        this.tileMap = GetTree().GetRoot().GetNode("World/Nav/TileMap") as TileMap;
        this.navigation = GetTree().GetRoot().GetNode("World/Nav") as Navigation2D;
        this.previousDirection = new Vector2(0, 0);
        UpdatePositionOnTileMap();
    }

    public override void _PhysicsProcess(float delta) {
       /* if (this.reachedDestination) {
            this.currentDestination = this.tileMap.GetRandomCell(TileType.TileType_Grass);
            Console.WriteLine("Setting destination at " + this.currentDestination);
            UpdatePath();
            this.reachedDestination = false;
        }*/

        UpdatePositionOnTileMap();

        Vector2 direction = GetRandomDirection();
        Console.WriteLine("Direction: ", direction);
        KinematicCollision2D collision = MoveAndCollide(direction * this.moveModifier * delta);
        if (collision != null && collision.GetCollider().GetType() == typeof(Player)) {
            (collision.GetCollider() as Player).Die();
            return;
        }
        /*  if (this.path != null && this.path.Count > 1) {
              float distance = GetPosition().DistanceTo(this.path[0]);
              if (distance > 2) {
                  Vector2 motion = this.path[0] - GetPosition();
                  motion = motion / distance;

                  KinematicCollision2D collision = MoveAndCollide(motion * this.moveModifier * delta);
                  if (collision != null && collision.GetCollider().GetType() == typeof(Player)) {
                      (collision.GetCollider() as Player).Die();
                      return;
                  }
              } else {
                  this.path.RemoveAt(0);
              }
          } else {
              this.reachedDestination = true;
          }

          if (this.path == null || this.path.Count == 0 || this.currentDestination == this.tileMap.WorldToMap(this.GetGlobalPosition())) {
              this.reachedDestination = true;
          }*/
    }

    private void UpdatePath() {
        Vector2[] path = this.navigation.GetSimplePath(this.GetPosition(), this.tileMap.MapToWorld(this.currentDestination), false);
        this.path = new List<Vector2>(path);
    }

    private bool isAllowedToStep(Vector2 direction) {
        if (direction == Directions.directionUp) {
            Vector2 tile = this.currentPositionOnTileMap;
            tile.y -= 1;
            if (this.tileMap.GetCellv(tile) == (int)TileType.TileType_Grass) {
                return true;
            }
        } else if (direction == Directions.directionDown) {
            Vector2 tile = this.currentPositionOnTileMap;
            tile.y += 1;
            if (this.tileMap.GetCellv(tile) == (int)TileType.TileType_Grass) {
                return true;
            }
        } else if (direction == Directions.directionLeft) {
            Vector2 tile = this.currentPositionOnTileMap;
            tile.x -= 1;
            if (this.tileMap.GetCellv(tile) == (int)TileType.TileType_Grass) {
                return true;
            }
        } else if (direction == Directions.directionRight) {
            Vector2 tile = this.currentPositionOnTileMap;
            tile.x += 1;
            if (this.tileMap.GetCellv(tile) == (int)TileType.TileType_Grass) {
                return true;
            }
        }
        return false;
    }

    private Vector2 GetRandomDirection() {
        bool directionFound = false;
        Vector2 direction = this.previousDirection;
        while (!directionFound) {
            if (direction != Directions.noDirection) {
                if (isAllowedToStep(direction)) {
                    directionFound = true;
                } else if(direction == Directions.noDirection) {
                    direction = Directions.GetRandomDirection();
                    directionFound = false;
                } else {
                    direction = Directions.GetRandomDirection();
                }
            } else {
                direction = Directions.GetRandomDirection();
                directionFound = true;
            }
        }

        this.previousDirection = direction;

        return direction;
    }

    public void Die() {
        this.QueueFree();
    }

    private void UpdatePositionOnTileMap() {
        this.currentPositionOnTileMap = this.tileMap.WorldToMap(this.GetGlobalPosition());
        this.tileMap.enemyOnCell[this] = this.currentPositionOnTileMap;
    }
}
