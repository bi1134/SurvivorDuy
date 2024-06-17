using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Sources;

public partial class player : CharacterBody2D
{
	public float movementSpeed = 60.0f;
	[Export] public double hp = 80;
	private int maxhp = 80;
	Vector2 lastMovement = Vector2.Up;
	private Vector2 playerPosition;
	Vector2 zero = Vector2.Zero;
	int time = 0;
	private int playerScore = 0;
	public string mapName;
	public int mapNum;
	private int collectedreroll = 0;
	public CollisionShape2D grabArea;
	public CollisionShape2D hurtCollision;
	public CollisionShape2D collectArea;
	AnimationPlayer animationPlayer;

	int exp = 0;
	int expLevel = 1;
	int collectedExp = 0;

	AudioStreamPlayer2D snd_hurt;



	//attack
	PackedScene iceSpear = ResourceLoader.Load<PackedScene>("res://Player/Attack/ice_spear.tscn");
	PackedScene tornado = ResourceLoader.Load<PackedScene>("res://Player/Attack/tornado.tscn");
	PackedScene javelin = ResourceLoader.Load<PackedScene>("res://Player/Attack/javelin.tscn");
	PackedScene Slash = ResourceLoader.Load<PackedScene>("res://Player/Attack/slash.tscn");
	PackedScene toxicCloud = ResourceLoader.Load<PackedScene>("res://Player/Attack/hurtZone.tscn");



	//attack node
	Timer iceSpearTimer;
	Timer iceSpearAttackTimer;
	Timer tornadoTimer;
	Timer tornadoAttackTimer;
	Timer SlashTimer;
	Timer SlashAttackTimer;
	Node2D javelinBase;
	Node2D toxicBase;


	//Upgrades
	List<string> collectedUpgrades = new List<string>();
	List<string> UpgradeOptions = new List<string>();

	int armor = 0;
	float speed = 0;
	public float spellCooldown = 0;
	public float spellSize = 0;
	int additionalAttack = 0;
	int additionalHp = 0;
	public float additionalDamage = 0;


	//ice spear
	int iceSpearAmmo = 0;
	int iceSpearBaseAmmo = 0;
	float iceSpearAttackSpeed = 1f;
	int iceSpearLevel = 0;

	//tornado
	int tornadoAmmo = 0;
	int tornadoBaseAmmo = 0;
	float tornadoAttackSpeed = 3f;
	int tornadoLevel = 0;

	//slash
	int slashAmmo = 0;
	int slashBaseAmmo = 1;
	float slashAttackSpeed = 1f;

	//Toxic Cloud
	public int toxicLevel = 0 ;
	public int totalCloud = 0;

	//javelin
	int javelinAmmo = 0;
	public int javelinLevel = 0;

	//Enemy related
	List<Node2D> enemyClose = new List<Node2D>();
	List<enemy> enemyCloseList = new List<enemy>();
	Node2D randomNode;

	//GUI
	TextureProgressBar expBar;
	Label lblLevel;
	Panel levelPanel;
	HBoxContainer upgradeOptions;
	AudioStreamPlayer2D sndLevelUp;
	PackedScene itemOption;
	TextureProgressBar healthBar;
	Label lblTimer;
	GridContainer collectedWeapon;
	GridContainer collectedUpgrade;
	PackedScene itemContainer;

	Panel deathPanel;
	Label lblResult;
	Label lblScore;
	AudioStreamPlayer2D sndVictory;
	AudioStreamPlayer2D sndLose;
	Panel pausePanel;
	Button btnNext;
	Label lblRollNum;



	//signal
	[Signal] public delegate void PlayerDeathEventHandler();
	[Signal] public delegate void PausedEventHandler();

	//mouse
	Vector2 mousePosition ;
	private bool isTouchingScreen = false;
	bool isShooting = false;

	private Sprite2D sprite;
	private Timer walkTimer;


	public override void _PhysicsProcess(double delta)
	{
		if (hp > maxhp)
		{
			hp = maxhp;
		}
		Movement();
		healthBar.MaxValue = maxhp ;
		healthBar.Value = hp;
		lblRollNum.Text = collectedreroll.ToString();
		
		playerPosition = GlobalPosition;
	

	}

	public override void _Ready()
	{
		sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
		walkTimer = GetNodeOrNull<Timer>("%walkTimer");
		iceSpearTimer = GetNodeOrNull<Timer>("%IceSpearTimer");
		iceSpearAttackTimer = GetNodeOrNull<Timer>("%IceSpearAttackTimer");
		tornadoTimer = GetNodeOrNull<Timer>("%TornadoTimer");
		tornadoAttackTimer = GetNodeOrNull<Timer>("%TornadoAttackTimer");
		SlashTimer = GetNodeOrNull<Timer>("%SlashTimer");
		SlashAttackTimer = GetNodeOrNull<Timer>("%SlashAttackTimer");
		javelinBase = GetNodeOrNull<Node2D>("%JavelinBase");
		toxicBase = GetNodeOrNull<Node2D>("%ToxicBase");
		expBar = GetNodeOrNull<TextureProgressBar>("%ExperienceBar");
		lblLevel = GetNodeOrNull<Label>("%lbl_level");
		levelPanel = GetNodeOrNull<Panel>("%LevelUp");
		upgradeOptions = GetNodeOrNull<HBoxContainer>("%UpgradeOptions");
		sndLevelUp = GetNodeOrNull<AudioStreamPlayer2D>("%snd_levelup");
		itemOption = ResourceLoader.Load<PackedScene>("res://Utility/item_option.tscn");
		healthBar = GetNodeOrNull<TextureProgressBar>("%HealthBar");
		lblTimer = GetNodeOrNull<Label>("%lblTimer");
		collectedWeapon = GetNodeOrNull<GridContainer>("%CollectedWeapons");
		collectedUpgrade = GetNodeOrNull<GridContainer>("%CollectedUpgrades");
		itemContainer = ResourceLoader.Load<PackedScene>("res://Player/GUI/item_container.tscn");
		deathPanel = GetNodeOrNull<Panel>("%DeathPanel");
		lblResult = GetNodeOrNull<Label>("%lbl_result");
		lblScore = GetNodeOrNull<Label>("%lbl_score");
		sndVictory = GetNodeOrNull<AudioStreamPlayer2D>("%snd_victory");		
		sndLose = GetNodeOrNull<AudioStreamPlayer2D>("%snd_lose");
		pausePanel = GetNodeOrNull<Panel>("%PausePanel");
		lblRollNum = GetNodeOrNull<Label>("%lbl_RollNum");
		btnNext = GetNodeOrNull<Button>("%btn_next");
		snd_hurt = GetNodeOrNull<AudioStreamPlayer2D>("snd_hurt");
		grabArea = GetNodeOrNull<CollisionShape2D>("%grabAreaCollision");
		hurtCollision = GetNodeOrNull<CollisionShape2D>("HurtBox/CollisionShape2D");
		collectArea = GetNodeOrNull<CollisionShape2D>("%collectAreaCollision");
		animationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		collectArea.Disabled = false;
		SlashTimer.Start();

		mapName = GetTree().CurrentScene.Name;
		mapNum = mapName.Substring(3).ToInt();
		attack();
		SetExpBar(exp, calculateExperienceCap());
		collectArea.Disabled = false;
		hurtCollision.Disabled = false;
	}


	public void Movement()
	{
		//right = 1, left = -1 (when the player pressed both key then our character doesn't move)
		var x_move = Input.GetActionStrength("right") - Input.GetActionStrength("left");
		//some how up = -1 and down = 1
		var y_move = Input.GetActionStrength("down") - Input.GetActionStrength("up");

		Vector2 move = new Vector2(x_move, y_move);
		mousePosition = GetGlobalMousePosition();
		if (mousePosition.X > GlobalPosition.X)
		{
			sprite.FlipH = true;
		}
		else if(mousePosition.X < GlobalPosition.X)
		{
			sprite.FlipH = false;
		}
		if (move != Vector2.Zero)
		{
			lastMovement = move;
			if (walkTimer.IsStopped())
			{
				if (sprite.Frame >= sprite.Hframes - 1)
				{
					sprite.Frame = (sprite.Frame + 1) % sprite.Hframes;
				}
				else
				{
					sprite.Frame += 1;
				}
				walkTimer.Start();

			}
		}
		else
		{
			sprite.Frame = 0;
		}
		//Normalize make the character go diagonaly the same speed as going up and down
		Velocity = move.Normalized()*movementSpeed;

		MoveAndSlide();
	}

	public override void _Input(InputEvent @event)
	{
		if(@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed )
			{
			
				if(slashAmmo > 0 && !SlashTimer.IsStopped())
				{
					Slashing();
				}
			}
		}
	}

	private void _on_hurt_box_hurt(double damage, Vector2 angle, int knockback)
	{
		hp -= Mathf.Clamp(damage - armor, 1.0, 999.0);
		animationPlayer.Play("blinkHit");
		if (hp <= 0)
		{
			death();
		}
		else
		{
			snd_hurt.Play();
		}
	}

	

	private void attack()
	{
		
		if (iceSpearLevel > 0)
		{
			iceSpearTimer.WaitTime = iceSpearAttackSpeed * (1 - spellCooldown);
			if (iceSpearTimer.IsStopped())
			{
				iceSpearTimer.Start();
			}
		}
		if (tornadoLevel > 0)
		{
			tornadoTimer.WaitTime = tornadoAttackSpeed * (1 - spellCooldown);
			if (tornadoTimer.IsStopped())
			{
				tornadoTimer.Start();
			}
		}
		if (javelinLevel > 0)
		{
			spawnJavelin();
		}

		if (toxicLevel > 0)
		{
			spawnCloud();
		}
	}

	
	private void _on_ice_spear_timer_timeout()
	{
		iceSpearAmmo += iceSpearBaseAmmo + additionalAttack;
		iceSpearAttackTimer.Start();
	}

	private void _on_ice_spear_attack_timer_timeout()
	{
		mousePosition = GetGlobalMousePosition();
		if(iceSpearAmmo > 0)
		{
			var iceSpearAttack = (ice_spear)iceSpear.Instantiate();
			iceSpearAttack.Position = Position; 
			iceSpearAttack.target = mousePosition;
			iceSpearAttack.level = iceSpearLevel;
			AddChild(iceSpearAttack);
			iceSpearAmmo -= 1;
			if (iceSpearAmmo > 0)
			{
				iceSpearAttackTimer.Start();
			}
			else
			{
				iceSpearAttackTimer.Stop();
			}
		}
	}
	
	private void _on_tornado_timer_timeout()
	{
		tornadoAmmo += tornadoBaseAmmo + additionalAttack;
		tornadoAttackTimer.Start();
	}
	private void spawnJavelin()
	{
		//get total javelin
		var getJavelinTotal = javelinBase.GetChildCount();
		var calcSpawn = javelinAmmo + additionalAttack - getJavelinTotal;
		while (calcSpawn > 0) 
		{
			var javelinSpawn = (javelin)javelin.Instantiate();
			javelinSpawn.GlobalPosition = GlobalPosition;
			//spawn javelin
			javelinBase.AddChild(javelinSpawn);
			//increase counter to exit loop
			calcSpawn -= 1;
		}
		//Update javelin
		var getJavelin = javelinBase.GetChildren();
		foreach (var i in getJavelin)
		{
			if(i.HasMethod("updateJavelin"))
			{
				i.Call("updateJavelin");
			}
		}
	}
	private void spawnCloud()
	{
		var totalCloudspawn = toxicBase.GetChildCount();
		var calcCloud = totalCloud - totalCloudspawn;
		while (calcCloud > 0)
		{
			var cloudSpawn = (hurtZone)toxicCloud.Instantiate();
			cloudSpawn.GlobalPosition = GlobalPosition;
			toxicBase.AddChild(cloudSpawn);
			calcCloud -= 1;
		}
		var getToxic = toxicBase.GetChildren();
		foreach (var i in getToxic)
		{
			if (i.HasMethod("updateToxic"))
			{
				i.Call("updateToxic");
			}
		}
	}
	private void _on_tornado_attack_timer_timeout()
	{
		if (tornadoAmmo > 0)
		{
			var tornadoAttack = (tornado)tornado.Instantiate();
			tornadoAttack.GlobalPosition = Position;
			tornadoAttack.last_movement = lastMovement;
			tornadoAttack.level = tornadoLevel;
			AddChild(tornadoAttack);
			tornadoAmmo -= 1;
			if (tornadoAmmo > 0)
			{
				tornadoAttackTimer.Start();
			}
			else
			{
				tornadoAttackTimer.Stop();
			}
		}
	}
	public void StartSlashTimer()
	{
		SlashTimer.Start();
	}

	private void SetTimer(float duration)
	{
		GetTree().CreateTimer(duration).Connect("timeout", new Callable(this, MethodName.ResetShootingFlag));
	}

	private void ResetShootingFlag()
	{
		isShooting = false;
	}
	
	private void Slashing()
	{
		var slashAttack = (slash)Slash.Instantiate();
		slashAttack.Position = Position;
		slashAttack.target = mousePosition;
		GetParent().AddChild(slashAttack);
		slashAmmo -= 1;
		SlashAttackTimer.Start();
	}

	public Vector2 getRandomTarget()
	{
		//check if there any enemy nearby
		if (enemyClose.Count > 0)
		{
			//generate a random index within the range enemyClose list
			int randomIndex = GD.RandRange(0, enemyClose.Count);
			if (randomIndex >= 0 && randomIndex < enemyClose.Count)
			{
				//retrieve the random index and return the position
				randomNode = enemyClose[randomIndex];
				return randomNode.GlobalPosition;
			}
			else
			{
				//return a random position near the player position
				return playerPosition + new Vector2((float)GD.RandRange(-5, 5), (float)GD.RandRange(-5, 5));
			}
		}
		else
		{
			//if not then go up
			return Vector2.Up;
		}
	}
	

	private void _on_enemy_detection_area_body_entered(Node2D body)
	{
		if (!enemyClose.Contains(body))
		{
			enemyClose.Add(body);
		}
		if (body is enemy enemy)
		{
			if (!enemyCloseList.Contains(enemy))
			{
				enemyCloseList.Add(enemy);
			}
		}
	}

	private void _on_enemy_detection_area_body_exited(Node2D body)
	{
		enemyClose.RemoveAll(enemy => enemy == body);
		enemyCloseList.RemoveAll(enemy => enemy == body);
	}
	private void _on_grab_area_area_entered(Area2D area)
	{
		if (area.IsInGroup("loot"))
		{
			((experience_gem)area).target = this;
		}
		if (area.IsInGroup("reroll"))
		{
			((reroll)area).target = this;
		}
		if (area.IsInGroup("expMagnet"))
		{
			((magnet)area).target = this;
		}
	}

	private void _on_collect_area_area_entered(Area2D area)
	{
		if (area.IsInGroup("loot"))
		{
			var gem_exp = ((experience_gem)area).Collect();
			
			//start the calculations
			calculateExperience(gem_exp);
		}
		if (area.IsInGroup("reroll"))
		{
			var rerolls = ((reroll)area).Collect();
			collectedreroll += rerolls;
		}
		if (area.IsInGroup("expMagnet"))
		{
		   ((magnet)area).Collect();
		}
	}

	

	private void calculateExperience(int gem_exp)
	{
		//calculate the exp needed to level up
		var expRequired = calculateExperienceCap();
		//transfer gem_exp to our handler variable
		collectedExp += gem_exp;
		if (exp + collectedExp >= expRequired) // check if level up
		{
			//only take exp that is needed
			collectedExp -= expRequired - exp;
			//increase level by 1
			expLevel += 1;
			//increase player speed by 1 for each level
			movementSpeed += 1;
			exp = 0;
			expRequired = calculateExperienceCap();
			LevelUp();
		}
		else
		{
			//if not then add collected exp to our pool and move on
			exp += collectedExp;
			collectedExp = 0;
		}
		playerScore += gem_exp;
		//alter exp bar
		SetExpBar(exp, expRequired);
	}
	private int calculateExperienceCap()
	{
		//level 5 = 5*5
		//level 25 = 95 + 5*5
		//level 57 = 255 + 17*12
		var expCap = expLevel;
		if (expLevel < 20)
		{
			expCap = expLevel * 5;
		}
		else if (expLevel < 40)
		{
			expCap += 95 + (expLevel - 19) * 8;
		}
		else
		{
			expCap = 225 + (expLevel - 39) * 12;	
		}
		return expCap;
	}
	public void SetExpBar(int setValue = 1, int setMaxValue = 100)
	{
		//adjust GUI
		expBar.Value = setValue;
		expBar.MaxValue = setMaxValue;
	}
	public void LevelUp() 
	{
		sndLevelUp.Play();
		hurtCollision.CallDeferred("set", "disabled", true);
		lblLevel.Text = "Level " + expLevel;
		var tween = levelPanel.CreateTween();
		tween.TweenProperty(levelPanel, "position", new Vector2(155, 80), 0.2).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.In);
		tween.Play();
		levelPanel.Visible = true;
		var options = 0;
		var optionsmax = 3;
		while (options < optionsmax)
		{
			var optionChoice = (item_option)itemOption.Instantiate();
			optionChoice.item = GetRandomItem();
			upgradeOptions.AddChild(optionChoice);
			options +=1;
		}
		GetTree().Paused = true;
	}
	public void UpgradeCharacter(string upgrade)
	{
		switch (upgrade)
		{
			case "magnet1":
				grabArea.Scale += new Vector2(1.5f, 1.5f);
				break;
			
			case "iceSpear1":
				iceSpearLevel = 1;
				iceSpearBaseAmmo += 1;
				break;
			case "iceSpear2":
				iceSpearLevel = 2;
				iceSpearBaseAmmo += 1;
				break;
			case "iceSpear3":
				iceSpearLevel = 3;
				break;
			case "iceSpear4":
				iceSpearLevel = 4;
				iceSpearBaseAmmo += 2;
				break;
			case "tornado1":
				tornadoLevel = 1;
				tornadoBaseAmmo += 1;
				break;
			case "tornado2":
				tornadoLevel = 2;
				tornadoBaseAmmo += 1;
				break;
			case "tornado3":
				tornadoLevel = 3;
				tornadoAttackSpeed -= 0.5f;
				break;
			case "tornado4":
				tornadoLevel = 4;
				tornadoBaseAmmo += 1;
				break;
			case "javelin1":
				javelinLevel = 1;
				javelinAmmo = 1;
				break;
			case "javelin2":
				javelinLevel = 2;
				break;
			case "javelin3":
				javelinLevel = 3;
				break;
			case "javelin4":
				javelinLevel = 4;
				break;
			case "toxic1":
				toxicLevel = 1;
				totalCloud = 1;
				break;
			case "toxic2":
				toxicLevel = 2;
				break;
			case "toxic3":
				toxicLevel = 3;
				break;
			case "toxic4":
				toxicLevel = 4;
				break;
			case "armor1":
			case "armor2":
			case "armor3":
			case "armor4":
				armor += 1;
				break;
			case "emptyHeart1":
			case "emptyHeart2":
			case "emptyHeart3":
			case "emptyHeart4":
				additionalHp += 5;
				maxhp += additionalHp;
				hp += 5;
				hp = Mathf.Clamp(hp, 0, maxhp);
				
				break;
			case "lossStreak1":
			case "lossStreak2":
			case "lossStreak3":
			case "lossStreak4":
			case "lossStreak5":
				additionalDamage += 0.1f;
				GD.Print(additionalDamage);
				break;
			case "speed1":
			case "speed2":
			case "speed3":
			case "speed4":
				movementSpeed += 20.0f;
				break;
			case "tome1":
			case "tome2":
			case "tome3":
			case "tome4":
				spellSize += 0.10f;
				break;
			case "scroll1":
			case "scroll2":
			case "scroll3":
			case "scroll4":
				spellCooldown += 0.05f;
				break;
			case "ring1":
			case "ring2":
				additionalAttack += 1;
				break;
			case "food":
				hp += 20;
				hp = Mathf.Clamp(hp, 0, maxhp);
				break;
		}
		AdjustGUICollection(upgrade);
		attack();
		foreach (var i in upgradeOptions.GetChildren())
		{
			i.QueueFree();
		}
		UpgradeOptions.Clear();
		collectedUpgrades.Add(upgrade);
		levelPanel.Visible = false;
		levelPanel.Position = new Vector2(155, 500);
		GetTree().Paused = false;
		hurtCollision.CallDeferred("set", "disabled", false);
		//call the func again to loop (this will let us use up the remaining collected exp)
		calculateExperience(0);
	}
	public string GetRandomItem()
	{
		List<string> dbList = new List<string>();

		foreach (var upgrade in upgrade_db.UPGRADES)
		{
			var upgradeKey = upgrade.Key;
			var upgradeData = upgrade.Value;

			if (collectedUpgrades.Contains(upgradeKey) ||
				UpgradeOptions.Contains(upgradeKey) ||
				upgradeData.Type == "item")
			{
				continue;
			}
			if (upgradeData.Prerequisite.Count > 0)
			{
				bool toAdd = true;
				foreach (var prerequisite in upgradeData.Prerequisite)
				{
					if (!collectedUpgrades.Contains(prerequisite))
					{
						toAdd = false;
						break;
					}
				}
				if (!toAdd)
				{
					continue;
				}
			}
			dbList.Add(upgradeKey);
		}

		if (dbList.Count > 0)
		{
			Random rnd = new Random();
			//food drop rate 10%
			double probability = 0.1;
			if (rnd.NextDouble() < probability)
			{
				return null;
			}
			else
			{
				string randomItem = dbList[rnd.Next(0, dbList.Count)];
				UpgradeOptions.Add(randomItem);
				return randomItem;
			}
		}
		else
		{
			return null;
		}
	}
	public void ChangeTime(int argTime = 0)
	{
		time = argTime;
		var getM = (int)(time / 60.0);
		var getMstr = getM.ToString(); //turn mins from int to string
		var getS = time % 60;
		var getSstr = getS.ToString(); //turn second from int to string
		if(getM < 10)
		{
			getMstr = "0" + getM.ToString(); //if the minute is below 10 then add a zero in front
		}
		if (getS < 10) 
		{
			getSstr = "0" + getS.ToString(); //same with the second
		}
		lblTimer.Text = getMstr + ":" + getSstr; //change the text
	}
	public void AdjustGUICollection(string upgrade)
	{
		var getUpgradedDisplayNames = upgrade_db.UPGRADES[upgrade].DisplayName;
		var getType = upgrade_db.UPGRADES[upgrade].Type;
		if (getType != "item")
		{
			var CollectedDisplayNames = new List<string>();
			foreach (var i in collectedUpgrades)
			{
				CollectedDisplayNames.Add(upgrade_db.UPGRADES[i].DisplayName);
			}
			if(!CollectedDisplayNames.Contains(getUpgradedDisplayNames))
			{
				var newItem = (item_container)itemContainer.Instantiate();
				newItem.upgrade = upgrade;
				switch (getType)
				{
					case "weapon":
						collectedWeapon.AddChild(newItem);
						break;
					case "upgrade":
						collectedUpgrade.AddChild(newItem);
						break;
				}
			}
		}
	}
	private void death()
	{
		hurtCollision.CallDeferred("set", "disabled", true);
		collectArea.CallDeferred("set", "disabled", true);
		deathPanel.Visible = true;
		EmitSignal(SignalName.PlayerDeath);
		GetTree().Paused = true;
		var tween = deathPanel.CreateTween();
		tween.TweenProperty(deathPanel, "position", new Vector2(220, 50), 3.0).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.Out);
		tween.Play();
		lblScore.Text = "SCORE: " + playerScore;
		lblResult.Text = "You lose";
		btnNext.Visible = false;
		sndLose.Play();

		LevelData.LevelDictionary[mapName].beaten = false;
		LevelData.UpdateMapStatus();
	}
	public void showWinPanel()
	{
		hurtCollision.CallDeferred("set", "disabled", true);
		collectArea.CallDeferred("set", "disabled", true);
		deathPanel.Visible = true;
		EmitSignal(SignalName.PlayerDeath);
		GetTree().Paused = true;
		var tween = deathPanel.CreateTween();
		tween.TweenProperty(deathPanel, "position", new Vector2(220, 50), 3.0).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.Out);
		tween.Play();
		lblScore.Text = "SCORE: " + playerScore;
		lblResult.Text = "You Win";
		btnNext.Visible = true;
		sndVictory.Play();
	}
	private void _on_btn_menu_click_end()
	{
		hurtCollision.CallDeferred("set", "disabled", true);
		collectArea.CallDeferred("set", "disabled", true);
		EmitSignal(SignalName.PlayerDeath);
		transition_screen.Instance.ChangeScene("res://TitleScreen/menu.tscn");
		GetTree().Paused = false;
	}
	private void _on_btn_pause_pressed()
	{
		hurtCollision.CallDeferred("set", "disabled", true);
		pausePanel.Visible = true;
		var tween = pausePanel.CreateTween();
		tween.TweenProperty(pausePanel, "position", new Vector2(220, 50), 0.0).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.In);
		GetTree().Paused = true;
	}
	private void _on_btn_back_click_end()
	{
		hurtCollision.CallDeferred("set", "disabled", false);
		pausePanel.Visible = false;
		pausePanel.Position = new Vector2(800, 55);
		GetTree().Paused = false;
	}
	private void _on_btn_retry_click_end()
	{
		hurtCollision.CallDeferred("set", "disabled", false);
		pausePanel.Visible = false;
		pausePanel.Position = new Vector2(800, 55);
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://World/" + mapName + ".tscn");
	}
	private void _on_btn_select_click_end()
	{
		hurtCollision.CallDeferred("set", "disabled", true);
		collectArea.CallDeferred("set", "disabled", true);
		GetTree().Paused = false;
		EmitSignal(SignalName.PlayerDeath);
		transition_screen.Instance.ChangeScene("res://World/world_map.tscn");
	}
	private void _on_btn_next_click_end()
	{
		mapNum++;
		hurtCollision.CallDeferred("set", "disabled", false);
		pausePanel.Visible = false;
		pausePanel.Position = new Vector2(800, 55);
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://World/" + "Map" + mapNum + ".tscn");
	}
	private void _on_reroll_button_click_end()
	{
		if (collectedreroll >= 1)
		{
			foreach (var i in upgradeOptions.GetChildren())
			{
				i.QueueFree();
			}
			UpgradeOptions.Clear();
			LevelUp();
			collectedreroll--;
		}
		lblRollNum.Text = collectedreroll.ToString();
	}

	private void _on_slash_timer_timeout()
	{
		if (slashAmmo <= slashBaseAmmo)
		{
			slashAmmo += slashBaseAmmo;
		}
		SlashAttackTimer.Start();
	}
	
}



