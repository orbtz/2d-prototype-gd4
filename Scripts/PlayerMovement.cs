using Godot;
using System;
using System.Threading.Tasks;

public partial class PlayerMovement : CharacterBody2D {
	[Export]
	public Area2D area2d;

	[Export]
	public int StepValue = 32;

	[Export]
	public double MovementInterval;
	public double CurrentMovementInterval = 0;

	[Export]
	public TileMap tilemap;

	public Vector4 blockedPath;
	public Vector4 pushablePath;

	[Signal]
	public delegate void OnMovePushableEventHandler(Vector4 directionPushable, TileMap stage);

	[Signal]
	public delegate void OnFutureWallCheckEventHandler(Vector4 directionFutureWall, TileMap stage);

	public override void _Process(double delta) {
		Vector2 direction = new Vector2();

		if (Input.IsActionJustPressed("ui_down") && blockedPath.X == 0) {
			direction.Y += 1;
			if (pushablePath.X == 1)
				EmitSignal(SignalName.OnMovePushable, new Vector4(1, 0, 0, 0), tilemap);

		}
		else if (Input.IsActionJustPressed("ui_left") && blockedPath.Y == 0) {
			direction.X -= 1;
			if (pushablePath.Y == 1)
				EmitSignal(SignalName.OnMovePushable, new Vector4(0, 1, 0, 0), tilemap);
		}
		else if (Input.IsActionJustPressed("ui_up") && blockedPath.Z == 0) {
			direction.Y -= 1;
			if (pushablePath.Z == 1)
				EmitSignal(SignalName.OnMovePushable, new Vector4(0, 0, 1, 0), tilemap);
		}
		else if (Input.IsActionJustPressed("ui_right") && blockedPath.W == 0) {
			direction.X += 1;
			if (pushablePath.W == 1)
				EmitSignal(SignalName.OnMovePushable, new Vector4(0, 0, 0, 1), tilemap);
		}

		if (CurrentMovementInterval <= 0 && direction != Vector2.Zero) {
			#region tween movement
			//area2d.Monitoring = false;

			//Tween tween = GetTree().CreateTween();
			//tween.TweenCallback(
			//	Callable.From(
			//		() => EnableCollisionBox(0.1)
			//	)
			//);

			//PropertyTweener tweener = tween.TweenProperty(this, "position", this.Position + (direction * StepValue), 0.1);
			//tweener.SetTrans(Tween.TransitionType.Linear);
			#endregion

			Vector2I aux =  new Vector2I((int)this.Position.X, (int)this.Position.Y);

			if (direction.Y == 1)
				aux += new Vector2I(0, 32);
			else if (direction.Y == -1)
				aux += new Vector2I(0, -32);
			else if (direction.X == 1)
				aux += new Vector2I(32, 0);
			else if (direction.X == 1)
				aux += new Vector2I(-32, 0);

			aux /= 32;

			if (tilemap.GetCellSourceId(1, aux) == -1 && tilemap.GetCellSourceId(4, aux) == -1) {
				Translate(direction * StepValue);
				CurrentMovementInterval = MovementInterval;
			}
		}

		if (direction != Vector2.Zero)
			CurrentMovementInterval -= delta;
		else
			CurrentMovementInterval = 0;
	}

	private void OnPlayerCollisionCheck_CollisionCheck(Vector4 directionBlocked) {
		blockedPath = directionBlocked;
	}

	private void OnPlayerCollisionCheck_PushableCheck(Vector4 directionPushable) {
		pushablePath = directionPushable;

		blockedPath = directionPushable - directionPushable;

		if (blockedPath.X == -1)
			blockedPath.X = 0;
		else if (blockedPath.Y == -1)
			blockedPath.Y = 0;
		else if (blockedPath.Z == -1)
			blockedPath.Z = 0;
		else if (blockedPath.W == -1)
			blockedPath.W = 0;

		EmitSignal(SignalName.OnFutureWallCheck, directionPushable, tilemap);

	}

	//private async void EnableCollisionBox(double seconds) {
	//	await Task.Delay(TimeSpan.FromSeconds(seconds));

	//	area2d.Monitoring = true;
	//}

	private void OnTileMap_OnTeleport(Vector2 teleportPosition) {
		Position = teleportPosition * StepValue;
	}

	private void OnTileMap_OnFutureWallDetect(Vector4 futureWallPosition)
	{
		blockedPath = futureWallPosition;
	}

	private void OnMap_OnStageChange(TileMap newStage) {
		tilemap = newStage;
	}
}
