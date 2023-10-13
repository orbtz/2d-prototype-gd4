using Godot;
using System;

public partial class PlayerCollisionCheck : Node
{
	[Signal]
	public delegate void OnCollisionCheckEventHandler(Vector4 directionBlocked);

	[Signal]
	public delegate void OnPushableCheckEventHandler(Vector4 directionPushable);

	//WALL
	RayCast2D downCast;	//X
	RayCast2D leftCast;	//Y
	RayCast2D topCast;	//Z
	RayCast2D rightCast;//W

	bool wasTopColliding = false;
	bool wasDownColliding = false;
	bool wasLeftColliding = false;
	bool wasRightColliding = false;

	bool isTopColliding = false;
	bool isDownColliding = false;
	bool isLeftColliding = false;
	bool isRightColliding = false;

	//PUSHABLE
	RayCast2D downCastP; //X
	RayCast2D leftCastP; //Y
	RayCast2D topCastP;  //Z
	RayCast2D rightCastP;//W

	bool hadTopColliding = false;
	bool hadDownColliding = false;
	bool hadLeftColliding = false;
	bool hadRightColliding = false;

	bool hasTopColliding = false;
	bool hasDownColliding = false;
	bool hasLeftColliding = false;
	bool hasRightColliding = false;

	Vector4 blockedPath = Vector4.Zero;
	Vector4 pushablePath = Vector4.Zero;

	[Export]
	public TileMap map;
	public override void _Ready()
	{
		downCast = GetChild<RayCast2D>(0);
		leftCast = GetChild<RayCast2D>(1);
		topCast = GetChild<RayCast2D>(2);
		rightCast = GetChild<RayCast2D>(3);

		downCastP = GetChild<RayCast2D>(4);
		leftCastP = GetChild<RayCast2D>(5);
		topCastP = GetChild<RayCast2D>(6);
		rightCastP = GetChild<RayCast2D>(7);
	}

	public override void _PhysicsProcess(double delta) {
		isTopColliding = topCast.IsColliding();
		isDownColliding = downCast.IsColliding();
		isLeftColliding = leftCast.IsColliding();
		isRightColliding = rightCast.IsColliding();

		hasTopColliding = topCastP.IsColliding();
		hasDownColliding = downCastP.IsColliding();
		hasLeftColliding = leftCastP.IsColliding();
		hasRightColliding = rightCastP.IsColliding();

		CheckBlockedPath();
		CheckPushablePath();

		if (HasPushableCheckChanged())
			EmitSignal(SignalName.OnPushableCheck, pushablePath);

		if (HasCollisionCheckChanged())
			EmitSignal(SignalName.OnCollisionCheck, blockedPath);

		wasTopColliding =	isTopColliding;
		wasDownColliding =	isDownColliding;
		wasLeftColliding =	isLeftColliding;
		wasRightColliding = isRightColliding;

		hadTopColliding =	hasTopColliding;
		hadDownColliding =	hasDownColliding;
		hadLeftColliding =	hasLeftColliding;
		hadRightColliding = hasRightColliding;
	}

	public void CheckBlockedPath() {
		if (isDownColliding && !wasDownColliding)
			blockedPath.X = 1;
		else if (!isDownColliding && wasDownColliding)
			blockedPath.X = 0;

		if (isLeftColliding && !wasLeftColliding)
			blockedPath.Y = 1;
		else if (!isLeftColliding && wasLeftColliding)
			blockedPath.Y = 0;

		if (isTopColliding && !wasTopColliding)
			blockedPath.Z = 1;
		else if (!isTopColliding && wasTopColliding)
			blockedPath.Z = 0;

		if (isRightColliding && !wasRightColliding)
			blockedPath.W = 1;
		else if (!isRightColliding && wasRightColliding)
			blockedPath.W = 0;
	}

	public void CheckPushablePath() {
		if (hasDownColliding && !hadDownColliding)
			pushablePath.X = 1;
		else if (!hasDownColliding && hadDownColliding)
			pushablePath.X = 0;

		if (hasLeftColliding && !hadLeftColliding)
			pushablePath.Y = 1;
		else if (!hasLeftColliding && hadLeftColliding)
			pushablePath.Y = 0;

		if (hasTopColliding && !hadTopColliding)
			pushablePath.Z = 1;
		else if (!hasTopColliding && hadTopColliding)
			pushablePath.Z = 0;

		if (hasRightColliding && !hadRightColliding)
			pushablePath.W = 1;
		else if (!hasRightColliding && hadRightColliding)
			pushablePath.W = 0;
	}

	public bool HasCollisionCheckChanged() {
		return 
			(
				(wasTopColliding !=		isTopColliding)		|| 
				(wasDownColliding !=	isDownColliding)	|| 
				(wasLeftColliding !=	isLeftColliding)	||
				(wasRightColliding !=	isRightColliding)
			);
	}

	public bool HasPushableCheckChanged() {
		return
			(
				( hadTopColliding !=	hasTopColliding ) ||
				( hadDownColliding !=	hasDownColliding ) ||
				( hadLeftColliding !=	hasLeftColliding ) ||
				( hadRightColliding !=	hasRightColliding )
			);
	}
}
