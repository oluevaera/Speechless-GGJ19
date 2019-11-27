using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
	/* PlayerControl CLASS
	 * Handles player inputs, and
	 * movement/attacks based on
	 * those inputs.
	 *
	 * -Kieran
	 * */

	//Used for mouse position raycasts.
	private Camera cam;

	//Set to FALSE when menus etc. are open,
	//preventing the user from supplying
	//any inputs.
	static public bool allowMovement = false;

	//Movement speed.
	[SerializeField]
	private float speed = 3.0f;

	//Stamina used for attacks.
	[SerializeField]
	private float maxStam = 100;

	[SerializeField] 
	private float stamRegen = 25f;

	[SerializeField] 
	private float stamRegenWhenDepletedMult = 2f;

	private float stamina = 100;
	public float Stamina {
		get => stamina;
		set {
			stamina = value;

			if(value <= 0) {
				stamina = 0;
				lostAllStamina = true;

				//Disable all shouts.
				activeShout = -1;
				foreach(GameObject go in shouts) go.SetActive(false);
			} else if(value >= MaxStamina) {
				lostAllStamina = false;
				stamina = MaxStamina;
			}
		}
	}

	private bool lostAllStamina = false;

	[SerializeField]
	private float[] attackRanges;
	private float[] sqrAttackRanges;

	[SerializeField]
	private float[] attackArcs;

	[SerializeField]
	private float[] attackDamages;

	//Stamina consumed by an attack.
	[SerializeField]
	private float[] attackStamina;

	//How long each shout is displayed for
	//before it is disabled.
	//Set to -1 for "until the player does
	//a different shout".
	[SerializeField]
	private float[] shoutDurations;

	//The last time the player shouted.
	private float timeBeganShout = 0;

	//Which shout the player is currently using.
	private int activeShout = -1;

	[SerializeField]
	private GameObject[] shouts;

	//When the player last pressed a button.
	private float buttonTime = 0;

	//The gimbal used by the camera.
	//Used to ensure that the player's
	//movement is always in sync with
	//their appearance on screen.
	[SerializeField]
	private Transform gimbal;

	//The player's CharacterController.
	private CharacterController ctrl;

	//Internal temp store for the player's
	//movement vector.
	private Vector3 movement;

	//The last time the player was hit
	//by an enemy.
	private float lastHit = 0;

	//The immunity time after being hit.
	[SerializeField]
	private float immuneTime = 0.5f;

	//How long the player is pushed
	//back for after being hit.
	[SerializeField]
	private float knockbackTime = 0.2f;

	//How far the player is knocked back.
	[SerializeField]
	private float knockbackDist = 3.0f;

	//Where the player is knocked back to
	//when they are hit.
	private Vector2 knockbackTarget = Vector2.positiveInfinity;



	//All words currently in-game.
	private GameObject[] words;

	//The word currently being held
	//by the player. null if there
	//is no word carried.
	private GameObject _carriedWord;

	private GameObject carriedWord {
		get { return _carriedWord; }
		set {

			if(_carriedWord != null) {
			 	//The word was dropped.
			 	_carriedWord.transform.parent = null;
				_carriedWord.transform.position =
					new Vector3(_carriedWord.transform.position.x, 0.65f, _carriedWord.transform.position.z);
			}

			if(value != null) {
				//A new word was picked up.
				//Parent that word to the player
				//and display it above their head.
				value.transform.parent = transform;
				value.transform.localPosition = new Vector3(0, 2, 0);
			}

			_carriedWord = value;
		}
	}

	public float MaxStamina => maxStam;

	public bool LostAllStamina => lostAllStamina;


	//Cached variables for aiming,
	//used to determine if the mouse
	//or right stick were moved.
	private Vector2 lastMouse = Vector2.zero;
	private Vector2 lastJoystick = Vector2.zero;

	private Vector2 currentMouse = Vector2.zero;
	private Vector2 currentJoystick = Vector2.zero;


	//Performs all necessary operations
	//for player movement.
	private void Move() {
		movement = Vector3.zero;

		if(ctrl.isGrounded) {
			//Character is on the ground
			//(this should be the case
			//most of the time...)

			//Move based on user inputs.
			movement += Vector3.right * Input.GetAxis("Horizontal");
			movement += Vector3.forward * Input.GetAxis("Vertical");

			//Transform to the gimbal's local space,
			//to ensure that "forward" matches how
			//the player appears on-screen.
			movement = gimbal.TransformDirection(movement);

			movement.Normalize();

			movement *= speed;
		}

		//Apply gravity.
		movement += Vector3.down * 9.81f;

		//Move the player.
		ctrl.Move(movement * Time.deltaTime);

		//The player should face the direction
		//they're moving in.
		//transform.LookAt(transform.position + new Vector3(movement.x, 0, movement.z));


		//The player should face the direction
		//that the mouse/right stick is pointing in.
		//To determine which input method is active,
		//cached variables are used and compared.
		currentMouse = Input.mousePosition;
		currentJoystick = new Vector2(Input.GetAxis("LookHorizontal"),
																	Input.GetAxis("LookVertical"));


		if(currentJoystick != lastJoystick
			&& currentJoystick != Vector2.zero) {
			//Joystick has moved. Look in
			//this new direction.
			//(Note: the current flow means that
			//the joystick always has preference
			//over the mouse.)
			Vector3 dir = gimbal.TransformDirection(new Vector3(currentJoystick.x, 0, currentJoystick.y));
			transform.LookAt(transform.position + new Vector3(dir.x, 0, dir.z));
		} else if(currentMouse != lastMouse) {
			//Mouse has moved. Look towards
			//its new position.
			Ray r = cam.ScreenPointToRay(currentMouse);
			//Follow the ray until (y = 0) is reached.
			Vector3 pos = r.origin;
			pos = r.origin - (r.direction * (r.origin.y / r.direction.y));

			pos.y = transform.position.y;

			//Look towards this point.
			transform.LookAt(pos);
		} else if(currentJoystick == Vector2.zero) {
			//The joystick is not being used to aim,
			//so assume that the player wants to
			//aim at wherever the mouse is,
			//but doesn't need the mouse to move right now.
			//("Circling round an enemy")
			Ray r = cam.ScreenPointToRay(currentMouse);
			//Follow the ray until (y = 0) is reached.
			Vector3 pos = r.origin;
			pos = r.origin - (r.direction * (r.origin.y / r.direction.y));

			pos.y = transform.position.y;

			//Look towards this point.
			transform.LookAt(pos);
		} else {
			//If the player is holding the joystick in a
			//specific direction without moving the mouse,
			//the player will keep looking in that direction.
			//(This needs actual code only for future-proofing
			//against any camera rotations. If the camera doesn't
			//rotate, it's safe to comment this all out.)
			Vector3 dir = gimbal.TransformDirection(new Vector3(currentJoystick.x, 0, currentJoystick.y));
			transform.LookAt(transform.position + new Vector3(dir.x, 0, dir.z));
		}


		//Update cached variables.
		lastMouse = currentMouse;
		lastJoystick = currentJoystick;
	}



	//Returns a list of all the enemies which are
	//in range of the player's attack.
	private List<Enemy> GetTargetedEnemies(float range, float arc) {
		List<Enemy> targets = new List<Enemy>();
		foreach(Enemy e in Enemy.enemies)
		{
			if(!e.gameObject.activeSelf) continue;

			//Check if the enemy's in range.
			if((e.transform.position - transform.position).sqrMagnitude < range)
			{
				//Check if the enemy's in the fire arc.
				if(Vector2.Angle(new Vector2(e.transform.position.x - transform.position.x, e.transform.position.z - transform.position.z),
					new Vector2(transform.forward.x, transform.forward.z)) < arc)
				{
					targets.Add(e);
				}
			}
		}
		return targets;
	}

	private void Shout(int index) {
		if(lostAllStamina) return;

		//Activate this shout.
		for(int i = 0; i < shouts.Length; i++) {
			shouts[i].SetActive((i == index ? true : false));
		}

		activeShout = index;
		timeBeganShout = Time.time;

		//If the shout does not loop,
		//activate all of its particle effects.
		if(shoutDurations[index] != -1) {
			Stamina -= attackStamina[index];
			foreach(ParticleSystem par in shouts[index].GetComponentsInChildren<ParticleSystem>())
				par.Play();
		} else Stamina -= attackStamina[index] * Time.deltaTime;

		if(Stamina == 0) return;

		List<Enemy> targets = GetTargetedEnemies(sqrAttackRanges[index], attackArcs[index]);

		//Damage all targets.
		foreach(Enemy e in targets) e.Damage(attackDamages[index] * (shoutDurations[index] == -1 ? Time.deltaTime : 1));
	}

	private void StopShout(int index) {
		//End this shout.
		shouts[index].SetActive(false);
		if(activeShout == index) activeShout = -1;
	}

	private void Attack() {
		//Regenerate stamina.
		Stamina += Time.deltaTime * stamRegen * (LostAllStamina ? stamRegenWhenDepletedMult : 1f);

		if(activeShout != -1 && shoutDurations[activeShout] != -1) {

			if(Time.time - timeBeganShout < shoutDurations[activeShout]) return;
			else {
				activeShout = -1;
				//Disable all shouts.
				foreach(GameObject go in shouts) go.SetActive(false);
			}
		}

		//Test all the different attack inputs.
		if(Input.GetAxis("Fire1") >= 0.5f) {
			if(Time.time - buttonTime > 0.15f) Shout(2);
		} else if(Input.GetAxis("Fire2") >= 0.5f) {
			if(Time.time - buttonTime > 0.15f) Shout(3);
		} else if(Input.GetAxis("Fire1") < 0.5f) {
			StopShout(2);
			StopShout(3);
		} else if(Input.GetAxis("Fire2") < 0.5f) {
			StopShout(2);
			StopShout(3);
		}
	}

	private void EnableControl()
	{
		allowMovement = true;
	}

	private void DisableControl()
	{
		allowMovement = false;
	}

    // Start is called before the first frame update
    void Start()
    {
		ctrl = GetComponent<CharacterController>();
		cam = Camera.main;

		sqrAttackRanges = new float[attackRanges.Length];
		for(int i = 0; i < attackRanges.Length; i++) {
			sqrAttackRanges[i] = attackRanges[i] * attackRanges[i];
		}

		EventManager.AddListener("START_GAME", EnableControl);
		EventManager.AddListener("RESUME_GAME", EnableControl);
		EventManager.AddListener("END_GAME", DisableControl);
		EventManager.AddListener("PAUSE_GAME", DisableControl);
    }

    // Update is called once per frame
    void Update()
    {
		if(Time.time - immuneTime >= lastHit) {
			//Check all enemies for proximity.
			foreach(Enemy e in Enemy.enemies) {
				if(!e.gameObject.activeSelf) continue;

				//Proximity collision is dependent
				//on the scale of the enemy.
				if(Vector3.Distance(e.transform.position, transform.position) < (0.75f * e.transform.localScale.x)) {
					//The player was hit. They are
					//pushed away from the enemy
					//and temporarily become immune.
					lastHit = Time.time;
					allowMovement = false;
					Vector3 target = (transform.position - e.transform.position).normalized * knockbackDist;
					knockbackTarget = new Vector2(target.x, target.z);
					EventManager.PostEvent("PLAYER_KNOCKED_BACK");

					//The player then drops the word
					//they were carrying.
					carriedWord = null;
					break;
				}
			}
		}

		//If the player was pushed recently,
		//continue pushing them.
		if(knockbackTarget.x < 1000) {
			if(Time.time - knockbackTime >= lastHit) {
				ctrl.Move(knockbackTarget * (Time.deltaTime - (Time.time - knockbackTime - lastHit)));
				knockbackTarget = Vector2.positiveInfinity;
				allowMovement = true;
			} else {
				ctrl.Move(new Vector3(knockbackTarget.x, 0, knockbackTarget.y) * Time.deltaTime);
			}
		}

		if(!allowMovement) return;

		Move();
		Attack();

		//Test to see if a word
		//is close enough to be obtained.
		if(carriedWord == null && Time.time - knockbackTime >= lastHit) {
			words = GameObject.FindGameObjectsWithTag("Word");
			foreach(GameObject go in words) {
				Vector2 flatPos = new Vector2(transform.position.x, transform.position.z);
				Vector2 wordPos = new Vector2(go.transform.position.x, go.transform.position.z);
				if((flatPos - wordPos).sqrMagnitude < 0.1f) {
					//The word is very close by.
					//Pick it up.
					carriedWord = go;
					break;
				}
			}
		}
	}

	void OnControllerColliderHit(ControllerColliderHit col) {
		//This is only for checking words.
		if(carriedWord == null) return;

		//Check for collision with the house.
		if(col.gameObject.tag == "House") {
			//Drop off the object.
			carriedWord.transform.position = new Vector3(-0.5f, 0.65f, 0.2f);
			carriedWord = null;
			EventManager.PostEvent("WORD_RETRIEVED");
		}
	}
}
