using Godot;

public partial class YesNo : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	UI ui = null;
	string Question = "";
	Label labelQuestion = null;
	public override void _Ready()
	{
		ui = GetNode<UI>("../UI/Control");
		labelQuestion = GetNode<Label>("Control/Panel/Label");
	}
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	private void Yes() {
		// Replace with function body.
		ui.YesNoFunc[Question].Func();
		Visible = false;
	}


	private void No() {
		// Replace with function body.
		Visible = false;
		ui.GetParent<CanvasLayer>().Visible = true;
	}
	public void AskYesNo(string what) {
		Question = what;
		labelQuestion.Text = ui.YesNoFunc[Question].Msg;
		Visible = true;
	}
}
