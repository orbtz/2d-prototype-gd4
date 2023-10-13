using Godot;
using System;
using System.Collections.Generic;

public partial class StageHandler : Node
{
	int currentStage = 0;
	List<TileMap> stages;
	
	[Export]
	public Label congratulations;

	[Signal]
	public delegate void OnChangeStageReferenceEventHandler(TileMap newStage);

	public override void _Ready() {
		stages = new List<TileMap>();

		foreach (Node node in this.GetChildren()) {
			if (node.GetType().FullName.Contains("TileMap")) {
				stages.Add((TileMap)node);
			}
		}

		foreach (TileMap stage in stages) {
			if (stages.IndexOf(stage) == currentStage)
				stage.Position = Vector2.Zero;
			else
				stage.Position = new Vector2(5000, 5000);

			stage.Visible = stages.IndexOf(stage) == currentStage;
			//stage.GetParent().GetChild<TileMapManipulation>(stages.IndexOf(stage)).enabled = stages.IndexOf(stage) == currentStage;

		}
	}

	private void OnStageChange() {
		currentStage++;

		if (currentStage + 1 > stages.Count) {
			GD.Print("PARABÉNS!");
			congratulations.Visible = true;
			return;
		}

		foreach (TileMap stage in stages) {
			if (stages.IndexOf(stage) == currentStage)
				stage.Position = Vector2.Zero;
			else
				stage.Position = new Vector2(5000, 5000);

			stage.Visible = stages.IndexOf(stage) == currentStage;

			if (stage.Visible)
				EmitSignal(SignalName.OnChangeStageReference, stage);
		}
	}
}
