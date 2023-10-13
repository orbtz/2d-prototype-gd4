using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

public partial class TileMapManipulation : TileMap {
	[ExportGroup("Tile Actions")]
	[Export]
	Vector2[] interactableTilePosition;

	[Export]
	Vector2[] manipulatedTilePosition;

	[Export]
	string[] action;

	[Signal]
	public delegate void OnTeleportEventHandler(Vector2 teleportPosition);

	[Signal]
	public delegate void OnFutureWallDetectEventHandler(Vector4 futureWallPosition);

	[Signal]
	public delegate void OnStageChangeDetectEventHandler();

	Node playerNode;
	CharacterBody2D playerBody;
	TileMap tm;

	public override void _Ready() {
		playerNode = GetTree().GetNodesInGroup("playerBody") [0];
		playerBody = (CharacterBody2D)playerNode;

		tm = this;
	}

	private void OnArea2DCheck_BodyEntered(Node2D body) {
		Vector2I pos = this.LocalToMap(playerBody.Position);

		if (!interactableTilePosition.Contains(pos))
			return;

		Vector2 result = interactableTilePosition.First(x => x == pos);
		int index = interactableTilePosition.ToList().IndexOf(result);

		if (action [index] == "DELETE") {
			this.SetCell(2, (Vector2I)manipulatedTilePosition.ElementAt(index), -1);
		}
		else if (action [index] == "TELEPORT") {
			int sourceIdWall = this.GetCellSourceId(4, (Vector2I)manipulatedTilePosition.ElementAt(index));

			if (sourceIdWall != -1)
				return;

			EmitSignal(SignalName.OnTeleport, (Vector2I)manipulatedTilePosition.ElementAt(index));
		}
		else if (action [index] == "GOAL") {
			EmitSignal(SignalName.OnTeleport, (Vector2I)manipulatedTilePosition.ElementAt(index));
			EmitSignal(SignalName.OnStageChangeDetect);
		}
	}

	private void OnPlayerMovement_PushableMove(Vector4 directionPushable, TileMap stage) {
		Vector2I pos = Vector2I.Zero;
		Vector2I posNew = Vector2I.Zero;
		Vector2I posFuture = Vector2I.Zero;
		Vector4 futureWallDirection = Vector4.Zero;

		if (directionPushable.X == 1) {
			pos = stage.LocalToMap(new Vector2(playerBody.Position.X, playerBody.Position.Y + 32));
			posNew = new Vector2I(pos.X, pos.Y + 1);
			posFuture = new Vector2I(pos.X, pos.Y + 2);
			futureWallDirection.X = 1;

		}
		else if (directionPushable.Y == 1) {
			pos = stage.LocalToMap(new Vector2(playerBody.Position.X - 32, playerBody.Position.Y));
			posNew = new Vector2I(pos.X - 1, pos.Y);
			posFuture = new Vector2I(pos.X - 2, pos.Y);
			futureWallDirection.Y = 1;
		}
		else if (directionPushable.Z == 1) {
			pos = stage.LocalToMap(new Vector2(playerBody.Position.X, playerBody.Position.Y - 32));
			posNew = new Vector2I(pos.X, pos.Y - 1);
			posFuture = new Vector2I(pos.X, pos.Y - 2);
			futureWallDirection.Z = 1;
		}
		else if (directionPushable.W == 1) {
			pos = stage.LocalToMap(new Vector2(playerBody.Position.X + 32, playerBody.Position.Y));
			posNew = new Vector2I(pos.X + 1, pos.Y);
			posFuture = new Vector2I(pos.X + 2, pos.Y);
			futureWallDirection.W = 1;
		}

		int sourceId = stage.GetCellSourceId(4, pos);
		Vector2I coords = stage.GetCellAtlasCoords(4, pos);

		int sourceIdOtherPushable = stage.GetCellSourceId(4, posNew);

		int sourceIdWall = stage.GetCellSourceId(1, posNew);
		int sourceIdWallFuture = stage.GetCellSourceId(1, posFuture);

		if (sourceIdWallFuture != -1)
			EmitSignal(SignalName.OnFutureWallDetect, futureWallDirection);

		if (sourceIdWall != -1)
			return;

		stage.SetCell(4, pos, -1);

		if (interactableTilePosition.Contains(posNew)) {
			Vector2 result = interactableTilePosition.First(x => x == posNew);
			int index = interactableTilePosition.ToList().IndexOf(result);

			if (action [index] == "TELEPORT")
				stage.SetCell(4, (Vector2I)manipulatedTilePosition.ElementAt(index), sourceId, coords);
			else
				stage.SetCell(4, posNew, sourceId, coords);
			
		} else
			stage.SetCell(4, posNew, sourceId, coords);

	}

	//private void OnPlayerMovement_FutureWallCheck(Vector4 directionFutureWall) {
	//	Vector2I pos = Vector2I.Zero;

	//	if (directionFutureWall.X == 1)
	//		pos = tm.LocalToMap(new Vector2(playerBody.Position.X, playerBody.Position.Y + 64));
	//	else if (directionFutureWall.Y == 1)
	//		pos = tm.LocalToMap(new Vector2(playerBody.Position.X - 64, playerBody.Position.Y));
	//	else if (directionFutureWall.Z == 1)
	//		pos = tm.LocalToMap(new Vector2(playerBody.Position.X, playerBody.Position.Y - 64));
	//	else if (directionFutureWall.W == 1)
	//		pos = tm.LocalToMap(new Vector2(playerBody.Position.X + 64, playerBody.Position.Y));

	//	//int sourceId = tm.GetCellSourceId(1, pos);

	//	if (tm.GetCellSourceId(1, pos) != -1)
	//		EmitSignal(SignalName.OnFutureWallDetect, directionFutureWall);
	//	else if (tm.GetCellSourceId(4, pos) != -1)
	//		EmitSignal(SignalName.OnFutureWallDetect, directionFutureWall);
	//}

	private void _on_character_body_2d_on_future_wall_check(Vector4 directionFutureWall, TileMap stage) {
		Vector2I pos = Vector2I.Zero;

		if (directionFutureWall.X == 1)
			pos = stage.LocalToMap(new Vector2(playerBody.Position.X, playerBody.Position.Y + 64));
		else if (directionFutureWall.Y == 1)
			pos = stage.LocalToMap(new Vector2(playerBody.Position.X - 64, playerBody.Position.Y));
		else if (directionFutureWall.Z == 1)
			pos = stage.LocalToMap(new Vector2(playerBody.Position.X, playerBody.Position.Y - 64));
		else if (directionFutureWall.W == 1)
			pos = stage.LocalToMap(new Vector2(playerBody.Position.X + 64, playerBody.Position.Y));

		if (stage.GetCellSourceId(1, pos) != -1)
			EmitSignal(SignalName.OnFutureWallDetect, directionFutureWall);
		else if (stage.GetCellSourceId(4, pos) != -1)
			EmitSignal(SignalName.OnFutureWallDetect, directionFutureWall);
	}
}



