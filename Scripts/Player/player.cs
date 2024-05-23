using Godot;
using System;


public partial class player : CharacterBody3D
{
	[Export] public float acceleration = 1.0f / 0.25f;
	[Export] public float stopping_acceleration = 20.0f;
	[Export] public float mouseSensitivity = 0.003f;
	
	Vector3 velocity = Vector3.Zero;
	Camera3D camera = null;
	Node3D cameraRotX = null;
	RayCast3D cameraColisionDetector = null;
	CanvasLayer ui = null;
	ScoreBoard scoreBoard = null;
	CanvasLayer login = null;
	YesNo yesNo = null;

	
	public override void _Ready() {
		// parāda peles kursoru
		Input.MouseMode = Input.MouseModeEnum.Visible;

		// Iegūst mezglus
		camera = GetNode<Camera3D>("CameraRig/RotY/Camera");
		cameraRotX = GetNode<Node3D>("CameraRig/RotY");
		cameraColisionDetector = GetNode<RayCast3D>("CameraRig/RotY/CameraColisionDetector");
		ui = GetNode<CanvasLayer>("../UI");
		scoreBoard = GetNode<ScoreBoard>("../Scores");
		MaxCameraDistance = (float)ui.GetNode<Slider>("Control/Panel/Control/HBoxContainer/Camera settings/CameraSettings/Distance/Input").Value;
		CameraOfset = (float)ui.GetNode<Slider>("Control/Panel/Control/HBoxContainer/Camera settings/CameraSettings/Ofset/Input").Value;
		login = GetNode<CanvasLayer>("../Login");
		yesNo = GetNode<YesNo>("../YesNo");

		// Staram pievieno virzienu
		cameraColisionDetector.TargetPosition = new Vector3(CameraOfset, CameraHight, MaxCameraDistance);
		cameraColisionDetector.HitBackFaces = false;

		
	}
	
	public const float Speed = 5.0f;
	public const float JumpVelocity = 5.2f;
	float LastJump = 1.0f;
	public bool Started = false;

	[Export] public float MaxCameraDistance; 
	[Export] public float CameraHight = 0.0f; 
	[Export] public float CameraOfset;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle(); // Iegūst gravitāciju no projekta iestatījumiem

	public override void _Process(double delta)
	{
		// Izmanto bāzes klases fiziku
		base._Process(delta);
		
	}

	public override void _PhysicsProcess(double delta)
	{
		// Kad nav UI apstādina laiku
		if (login.Visible || ui.Visible || yesNo.Visible) {
			HandleCameraCollision();
			scoreBoard.Stop();
			// HandleCameraCollision(new Vector2(0.0f, 0.0f));
			return;
		}
		Vector3 velocity = Velocity;
		float VelocityY = velocity.Y;
		
		// No lietotāja nospiestajām pogām iegūst kustības virzienu
		Vector2 inputXZ = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
		if (!inputXZ.IsZeroApprox()) StartClock();
		Vector3 target_velocity = new Vector3();
		Vector3 forward = GlobalTransform.Basis.Z;
		Vector3 right = GlobalTransform.Basis.X;
		
		target_velocity += right * inputXZ.X;
		target_velocity += forward * inputXZ.Y;
		
		target_velocity = target_velocity.Normalized() * Speed;

		// Izveido vienmērīgu kustību
		float target_acceleration = acceleration;
		if (target_velocity.LengthSquared() <= 0.1f) target_acceleration = stopping_acceleration;
		if (!IsOnFloor()) target_acceleration /= 10;
		velocity = velocity.Lerp(target_velocity, target_acceleration * (float)delta);
		velocity.Y = VelocityY;
		
		// Pievieno gravitāciju, ja nestāv ne uz kā
		if (!IsOnFloor()) velocity.Y -= gravity * (float)delta;

		// Izveido palekšanos
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor()) {
			velocity.Y = JumpVelocity;
			StartClock();
		}

		// Nomaina objekta ātrumu uz jaunizveidoto ātrumu
		Velocity = velocity;

		// Izveido līganu kustību
		MoveAndSlide();

		// Tiek galā ar kameras atudri pret citiem objektiem
		HandleCameraCollision();
	}
	
	public override void _Input(InputEvent e) {
		// Pagriežas attiecīgi no tā kā kustina peli, ja peles kursors nav redzams
		if (e is InputEventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured) {
			var m_event = e as InputEventMouseMotion;
			Vector2 motion = -m_event.Relative * mouseSensitivity;

			// Tiek galā ar kameras atudri pret citiem objektiem
			HandleCameraCollision();

			// Pagriež spēlētāju attiecīgajā virzienā
			RotateY(motion.X);

			// Pagriež kameru
			cameraRotX.RotateX(motion.Y);

			// Pārbauda ar noteiktu precizitāti, lai neietu uz otru pusi, kad skatās tieši uz augšu vai apakšu
			if (cameraRotX.RotationDegrees.X <= -89.997f) {
				cameraRotX.RotationDegrees = new Vector3(-90.0f, 0.0f, 0.0f);
			
			} else if (cameraRotX.RotationDegrees.X >= 89.997f) {
				cameraRotX.RotationDegrees = new Vector3(90.0f, 0.0f, 0.0f);

			} else {

				camera.LookAt(ToGlobal(new Vector3(camera.Position.X, camera.Position.Y, 0.0f)), new Vector3(0.0f, 1.0f, 0.0f));
			}
		}
		// Pārslēdz peles redzamību un iestatījumus, kad "esc" nospiests
		if (e.IsActionPressed("ui_cancel") && !login.Visible && !yesNo.Visible) {
			if (Started) scoreBoard.Continue();
			Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
			ui.Visible = Input.MouseMode == Input.MouseModeEnum.Visible;
		}
	}

	void HandleCameraCollision(){
		

		// Pārmaina stara virzienu
		cameraColisionDetector.TargetPosition = new Vector3(CameraOfset, CameraHight, MaxCameraDistance);

		// Pārbauda vai stars atduras pret kādu objektu
		cameraColisionDetector.ForceRaycastUpdate();
		Vector3 posOfCameraColision;

		// Ja atduras pret objektu noliek kameru atdures punktā citādi iestatījumos norādītajā punktā
		if (cameraColisionDetector.IsColliding()) {
			posOfCameraColision = ToLocal(cameraColisionDetector.GetCollisionPoint());
			float len = new Vector3(CameraOfset, CameraHight, MaxCameraDistance).Length() / posOfCameraColision.Length() + 0.01f;
			camera.Position = new Vector3(CameraOfset, CameraHight, MaxCameraDistance) / len;
		} else {
			camera.Position = new Vector3(CameraOfset, CameraHight, MaxCameraDistance);
		}
	}

	void StartClock() {
		if (!Started) scoreBoard.Continue();
		Started = true;
	}

	public void resetSpeedAndDirection() {
		// Pagriež spēlētāju attiecīgajā virzienā
		// RotateY(motion.X);
		// RotationDegrees
		// Pagriež kameru
		// cameraRotX.RotateX(motion.Y);
		Rotation = new Vector3(0, 0, 0);
		velocity = new Vector3(0, 0, 0);
	}
}
