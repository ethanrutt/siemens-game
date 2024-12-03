// Rishi Santhanam
// CSCE 482 Siemens Gamification

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * @brief This class handles character movement. This script will be attached
 * to the player object
 *
 * There are lots of moving parts, since we have separate entities for the
 * player's head, chest, arms, legs, as well as handling movement for all of
 * their cosmetics. Emotes are also handled in this class.
 *
 * @see CosmeticHandler
 */
public class Character_Movement : MonoBehaviour
{
	[SerializeField] private float charSpeed = 4f;
	[SerializeField] private CosmeticHandler cosmeticHandler;

	private string currentState;

	private Rigidbody2D rb;
	private Animator animator;
	private Animator child_ChestAnimator;
	private Animator child_LegAnimator;
	private Animator child_ShoeAnimator;
	private Animator child_HatAnimator;

	// Grab the equipped items dictionary from PlayerData
	// The PlayerData is a singleton
	// PlayerData.Instance.equipped_items, PlayerData.Instance.original_load_items
	private PlayerData playerData => PlayerData.Instance;
	private List<int> equipped_items => playerData.equipped_items;
	private List<int> original_load_items = new List<int>();

	private Vector2 movementInputDirection;
	private Vector2 lastMovementInputDirection;

	private Dictionary<Vector2, string> playerMovingAnimations;
	private Dictionary<Vector2, string> playerIdleAnimations;

	private Dictionary<Vector2, string> chestMovingAnimations;
	private Dictionary<Vector2, string> chestIdleAnimations;

	private Dictionary<Vector2, string> legMovingAnimations;
	private Dictionary<Vector2, string> legIdleAnimations;

	private Dictionary<Vector2, string> hatMovingAnimations;
	private Dictionary<Vector2, string> hatIdleAnimations;

	private Dictionary<Vector2, string> shoeMovingAnimations;
	private Dictionary<Vector2, string> shoeIdleAnimations;

	private bool syncFlag = false;
    private bool canMove = true; // New variable to control movement

    private List<GameObject> interactiveButtons = new List<GameObject>();
	private bool isResettingAnimations;
	private bool isEmoting;

	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		child_ChestAnimator = transform.GetChild(0).GetComponent<Animator>();
		child_LegAnimator = transform.GetChild(1).GetComponent<Animator>();
		child_ShoeAnimator = transform.GetChild(2).GetComponent<Animator>();
		child_HatAnimator = transform.GetChild(3).GetComponent<Animator>();

		CreateAnimationDictionary();

		lastMovementInputDirection = Vector2.down;

		if (playerData != null)
		{
			original_load_items = new List<int>(playerData.equipped_items);
		}

		// Sort the equipped_items and iterate through 100-499
		// If the equipped_items contains a number in the 100s, 200s, 300s, 400s
		// Set the hat, chest, leg, shoe sprites accordingly
		if (equipped_items.Count != 0)
		{
			equipped_items.Sort();

			for (int i = 0; i < equipped_items.Count; i++)
			{
				if (equipped_items[i] >= 100 && equipped_items[i] < 200)
				{
					SetHatSprite(cosmeticHandler.GetHatController(equipped_items[i] - 100));
				}
				if (equipped_items[i] >= 200 && equipped_items[i] < 300)
				{
					SetChestSprite(cosmeticHandler.GetChestController(equipped_items[i] - 200));
				}
				if (equipped_items[i] >= 300 && equipped_items[i] < 400)
				{
					SetLegSprite(cosmeticHandler.GetLegController(equipped_items[i] - 300));
				}
				if (equipped_items[i] >= 400 && equipped_items[i] < 500)
				{
					SetShoeSprite(cosmeticHandler.GetShoeController(equipped_items[i] - 400));
				}
			}
		}
	}

	// Use the Start() method
	private void Start()
	{
		if (GameManager.Instance != null)
		{
			transform.position = GameManager.Instance.playerSpawnPosition;
		}
	}

	// Can't move function
	public void StopPlayer()
	{
		// Stop the player from moving
		canMove = false;

		// Change movement input direction to zero
		movementInputDirection = Vector2.zero;

		// Update player animations
		UpdateAnimator();
	}

	// Can move function
	public void UnstopPlayer()
	{
		// Allow the player to move
		canMove = true;
	}

<<<<<<< HEAD
=======
	// A coroutine to stop the player from moving for a certain amount of time
	public IEnumerator MoveAndStopForSeconds(int direction, float seconds)
	{
		// Set the movement input direction to the correct direction
		switch (direction)
		{
			case 0:
				movementInputDirection = Vector2.up;
				break;
			case 1:
				movementInputDirection = Vector2.down;
				break;
			case 2:
				movementInputDirection = Vector2.left;
				break;
			case 3:
				movementInputDirection = Vector2.right;
				break;
			default:
				Debug.LogError("Invalid movement direction.");
				break;
		}

		// Wait for the seconds
		yield return new WaitForSeconds(seconds);

		// Stop the player from moving
		movementInputDirection = Vector2.zero;
		UpdateAnimator();
	}
	
>>>>>>> 878ff7f2413801b48682745b6faf2f5a490799a2
	// Moving Up (Joystick)
	public void MoveUp()
	{
		// Essentially, Move Up and Stop
		// Set the movement input direction to up
		StartCoroutine(MoveAndStopForSeconds(0, 3.0f));
	}

	// Moving Down (Joystick)
	public void MoveDown()
	{
		StartCoroutine(MoveAndStopForSeconds(1, 3.0f));
	}

	// Moving Left (Joystick)
	public void MoveLeft()
	{
		StartCoroutine(MoveAndStopForSeconds(2, 3.0f));
	}

	// Moving Right (Joystick)
	public void MoveRight()
	{
		StartCoroutine(MoveAndStopForSeconds(3, 3.0f));
	}

	// Stopping the player
	public void StopMoving()
	{
		movementInputDirection = Vector2.zero;
		HandleMovement(movementInputDirection);
	}

	// Check if lists are same content
	private bool AreListsEqual(List<int> list1, List<int> list2)
	{
		if (list1.Count != list2.Count)
		{
			return false;
		}

		for (int i = 0; i < list1.Count; i++)
		{
			if (list1[i] != list2[i])
			{
				return false;
			}
		}

		return true;
	}

	// Use update for animations
	private void Update()
	{
		// First, check if the equipped items have changed
		// If they have, update the player's equipped items
		if (!AreListsEqual(original_load_items, equipped_items))
		{
			// Occurs
			// Organize equippedItems by lowest to highest id
			equipped_items.Sort();

			// Check the count of equipped items
			// and set accordingly for each of the 0, 1, 2, 3 elements
			// For loop of equipped items
			// If the equipped item is in the 100s, set the chest sprite, etc
			for (int i = 0; i < equipped_items.Count; i++)
			{
				if (equipped_items[i] >= 100 && equipped_items[i] < 200)
				{
					if (equipped_items[i] == 199)
					{
						// Disable the hat sprite
						child_HatAnimator.runtimeAnimatorController = null;
					} else
					{
					child_HatAnimator.runtimeAnimatorController = transform.GetChild(3).GetComponent<Animator>().runtimeAnimatorController;
					SetHatSprite(cosmeticHandler.GetHatController(equipped_items[i] - 100));
					}
				}
				if (equipped_items[i] >= 200 && equipped_items[i] < 300)
				{
					if (equipped_items[i] == 299)
					{
						// Disable the chest sprite
						child_ChestAnimator.runtimeAnimatorController = null;
					} else
					{
					child_ChestAnimator.runtimeAnimatorController = transform.GetChild(0).GetComponent<Animator>().runtimeAnimatorController;
					SetChestSprite(cosmeticHandler.GetChestController(equipped_items[i] - 200));
					}
				}
				if (equipped_items[i] >= 300 && equipped_items[i] < 400)
				{
					if (equipped_items[i] == 399)
					{
						// Disable the leg sprite
						child_LegAnimator.runtimeAnimatorController = null;
					} else
					{
					child_LegAnimator.runtimeAnimatorController = transform.GetChild(1).GetComponent<Animator>().runtimeAnimatorController;
					SetLegSprite(cosmeticHandler.GetLegController(equipped_items[i] - 300));
					}
				}
				if (equipped_items[i] >= 400 && equipped_items[i] < 500)
				{
					if (equipped_items[i] == 499)
					{
						// Disable the shoe sprite
						child_ShoeAnimator.runtimeAnimatorController = null;
					} else
					{
					child_ShoeAnimator.runtimeAnimatorController = transform.GetChild(2).GetComponent<Animator>().runtimeAnimatorController;
					SetShoeSprite(cosmeticHandler.GetShoeController(equipped_items[i] - 400));
					}
				}
			}

			// Change the original_load_items to the equipped items
			original_load_items = new List<int>(equipped_items);
		}

		// Update the player's animations
		if (canMove)
		{
			UpdateAnimator();
		}
	}

	// Dance emote function
	// Check the player's equipped items and set the dance emote
	// If 500, headripper (3), 501, robotdance (2), 502, zenflip (1)
	public void DanceEmote()
	{
		// Check the equipped items
		// If the player has the headripper, robotdance, or zenflip
		// Set the dance emote accordingly
		if (equipped_items.Contains(500))
		{
			PerformEmote(3);
		}
		else if (equipped_items.Contains(501))
		{
			PerformEmote(2);
		}
		else if (equipped_items.Contains(502))
		{
			PerformEmote(1);
		}
	}

	private void FixedUpdate()
	{
		if(canMove)
            HandleMovement(movementInputDirection);
        else{
            Vector2 movementInput = Vector2.zero;
            HandleMovement(movementInput);
            isEmoting = false;
        }
	}

    // New method to toggle player movement
    public void ToggleMovement()
    {
        canMove = !canMove; // Toggle the movement state
    }

	private void ChangeAnimationState(Animator animator, string newState, ref string currentState)
	{
		if (currentState == newState) return;

		animator.Play(newState);
		currentState = newState;
	}

	private void ChangePlayerAnimationState(string newState)
	{
		ChangeAnimationState(animator, newState, ref currentState);
	}

	private void ChangeChestAnimationState(string newState)
	{
		ChangeAnimationState(child_ChestAnimator, newState, ref currentState);
	}

	private void ChangeLegAnimationState(string newState)
	{
		ChangeAnimationState(child_LegAnimator, newState, ref currentState);
	}

	private void ChangeShoeAnimationState(string newState)
	{
		ChangeAnimationState(child_ShoeAnimator, newState, ref currentState);
	}
	private void ChangeHatAnimationState(string newState)
	{
		ChangeAnimationState(child_HatAnimator, newState, ref currentState);
	}

	private Vector2 GetInput()
	{
		float moveHorizontal = Input.GetAxisRaw("Horizontal");
		float moveVertical = Input.GetAxisRaw("Vertical");
		return new Vector2(moveHorizontal, moveVertical).normalized;
	}

	private void HandleMovement(Vector2 movement)
	{
		// Movement restriction
		if (movement.x != 0)
		{
			movement.y = 0;
		}
		else if (movement.y != 0)
		{
			movement.x = 0;
		}

		if (movement != Vector2.zero)
		{
			// Stops emoting

			// Stops CoRoutines iff emoting
			if (isEmoting)
			{
				StopEmoting();
			}
			isEmoting = false;
		}

		rb.velocity = movement * charSpeed;
	}

	private void UpdateAnimator()
	{
		if (isResettingAnimations || isEmoting) return;

		if (movementInputDirection != Vector2.zero)
		{
			if (playerMovingAnimations.ContainsKey(movementInputDirection))
			{
				ChangePlayerAnimationState(playerMovingAnimations[movementInputDirection]);
				ChangeChestAnimationState(chestMovingAnimations[movementInputDirection]);
				ChangeLegAnimationState(legMovingAnimations[movementInputDirection]);
				ChangeShoeAnimationState(shoeMovingAnimations[movementInputDirection]);
				ChangeHatAnimationState(hatMovingAnimations[movementInputDirection]);

				lastMovementInputDirection = movementInputDirection;
			}
		}
		else
		{
			if (playerIdleAnimations.ContainsKey(lastMovementInputDirection))
			{
				ChangePlayerAnimationState(playerIdleAnimations[lastMovementInputDirection]);
				ChangeChestAnimationState(chestIdleAnimations[lastMovementInputDirection]);
				ChangeLegAnimationState(legIdleAnimations[lastMovementInputDirection]);
				ChangeShoeAnimationState(shoeIdleAnimations[lastMovementInputDirection]);
				ChangeHatAnimationState(hatIdleAnimations[lastMovementInputDirection]);
			}
		}
	}

	// Set the chest sprite based on the prefab from the CosmeticHandler
	public void SetChestSprite(RuntimeAnimatorController newController)
	{
		if (newController != null)
		{
			child_ChestAnimator.runtimeAnimatorController = newController; // Reassign the animator

			SyncAnimations(movementInputDirection);
		}
	}
	public void SetLegSprite(RuntimeAnimatorController newController)
	{
		if (newController != null)
		{
			child_LegAnimator.runtimeAnimatorController = newController; // Reassign the animator

			SyncAnimations(movementInputDirection);
		}
	}
	public void SetShoeSprite(RuntimeAnimatorController newController)
	{
		if (newController != null)
		{
			child_ShoeAnimator.runtimeAnimatorController = newController; // Reassign the animator

			SyncAnimations(movementInputDirection);
		}
	}
	public void SetHatSprite(RuntimeAnimatorController newController)
	{
		if (newController != null)
		{
			child_HatAnimator.runtimeAnimatorController = newController; // Reassign the animator

			SyncAnimations(movementInputDirection);
		}
	}

	private void PerformEmote(int emoteIndex)
	{
		isEmoting = true;

		// Play the emote animation
		ChangePlayerAnimationState("Char_Emote_" + emoteIndex);
		ChangeChestAnimationState("Chest_Emote_" + emoteIndex);
		ChangeLegAnimationState("Leg_Emote_" + emoteIndex);
		ChangeShoeAnimationState("Shoe_Emote_" + emoteIndex);
		ChangeHatAnimationState("Hat_Emote_" + emoteIndex);

		// Start a coroutine to wait for the emote animation to finish
		StartCoroutine(WaitForEmoteToFinish(emoteIndex));
	}

	private IEnumerator WaitForEmoteToFinish(int emoteIndex)
	{
		// Wait until the emote animation is fully playing
		while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Char_Emote_" + emoteIndex))
		{
			yield return null; // Wait for the next frame
		}

		// Get the length of the emote animation
		float emoteLength = animator.GetCurrentAnimatorStateInfo(0).length;

		// Wait for the emote animation to finish
		yield return new WaitForSeconds(emoteLength);

		// After emote finishes, stop emoting and return to idle
		StopEmoting();
	}

	private void StopEmoting()
	{
		isEmoting = false;

		ChangePlayerAnimationState("Char_Idle_Down");
		ChangeChestAnimationState("Chest_Idle_Down");
		ChangeLegAnimationState("Leg_Emote_Idle");
		ChangeShoeAnimationState("Shoe_Emote_Idle");
		ChangeHatAnimationState("Hat_Emote_Idle");
	}

	private void SyncAnimations(Vector2 movement)
	{
		isResettingAnimations = true;

		// Movement restriction
		if (movement.x != 0)
		{
			movement.y = 0;

			if (movement.x > 0)
			{
				movement.x = 1;
			}
			else if (movement.x < 0)
			{
				movement.x = -1;
			}
		}
		else if (movement.y != 0)
		{
			movement.x = 0;

			if (movement.y > 0)
			{
				movement.y = 1;
			}
			else if (movement.y < 0)
			{
				movement.y = -1;
			}
		}

		if (movement != Vector2.zero)
		{
			if (playerMovingAnimations.ContainsKey(movement))
			{
				animator.Play(playerMovingAnimations[movement], 0, 0f);
				child_ChestAnimator.Play(chestMovingAnimations[movement], 0, 0f);
				child_LegAnimator.Play(legMovingAnimations[movement], 0, 0f);
				child_ShoeAnimator.Play(shoeMovingAnimations[movement], 0, 0f);
				child_HatAnimator.Play(hatMovingAnimations[movement], 0, 0f);

				lastMovementInputDirection = movement;
			}

		}
		else
		{
			if (playerIdleAnimations.ContainsKey(lastMovementInputDirection))
			{
				animator.Play(playerIdleAnimations[lastMovementInputDirection], 0, 0f);
				child_ChestAnimator.Play(chestIdleAnimations[lastMovementInputDirection], 0, 0f);
				child_LegAnimator.Play(legIdleAnimations[lastMovementInputDirection], 0, 0f);
				child_ShoeAnimator.Play(shoeIdleAnimations[lastMovementInputDirection], 0, 0f);
				child_HatAnimator.Play(hatIdleAnimations[lastMovementInputDirection], 0, 0f);
			}
		}


		isResettingAnimations = false;
	}

	private void CreateAnimationDictionary()
	{
		playerMovingAnimations = new Dictionary<Vector2, string>
		{
			{ Vector2.down, "Char_Walk_Down" },
			{ Vector2.up, "Char_Walk_Up" },
			{ Vector2.left, "Char_Walk_Left" },
			{ Vector2.right, "Char_Walk_Right" }
		};

		playerIdleAnimations = new Dictionary<Vector2, string>
		{
			{ Vector2.down, "Char_Idle_Down" },
			{ Vector2.up, "Char_Idle_Up" },
			{ Vector2.left, "Char_Idle_Left" },
			{ Vector2.right, "Char_Idle_Right" }
		};

		chestMovingAnimations = new Dictionary<Vector2, string>
		{
			{ Vector2.down, "Chest_Walk_Down" },
			{ Vector2.up, "Chest_Walk_Up" },
			{ Vector2.left, "Chest_Walk_Left" },
			{ Vector2.right, "Chest_Walk_Right" }
		};

		chestIdleAnimations = new Dictionary<Vector2, string>
		{
			{ Vector2.down, "Chest_Idle_Down" },
			{ Vector2.up, "Chest_Idle_Up" },
			{ Vector2.left, "Chest_Idle_Left" },
			{ Vector2.right, "Chest_Idle_Right" }
		};

		legMovingAnimations = new Dictionary<Vector2, string>
		{
			{ Vector2.down, "Leg_Walk_Down" },
			{ Vector2.up, "Leg_Walk_Up" },
			{ Vector2.left, "Leg_Walk_Left" },
			{ Vector2.right, "Leg_Walk_Right" }
		};

		legIdleAnimations = new Dictionary<Vector2, string>
		{
			{ Vector2.down, "Leg_Idle_Down" },
			{ Vector2.up, "Leg_Idle_Up" },
			{ Vector2.left, "Leg_Idle_Left" },
			{ Vector2.right, "Leg_Idle_Right" }
		};

		shoeMovingAnimations = new Dictionary<Vector2, string>
		{
			{ Vector2.down, "Shoe_Walk_Down" },
			{ Vector2.up, "Shoe_Walk_Up" },
			{ Vector2.left, "Shoe_Walk_Left" },
			{ Vector2.right, "Shoe_Walk_Right" }
		};

		shoeIdleAnimations = new Dictionary<Vector2, string>
		{
			{ Vector2.down, "Shoe_Idle_Down" },
			{ Vector2.up, "Shoe_Idle_Up" },
			{ Vector2.left, "Shoe_Idle_Left" },
			{ Vector2.right, "Shoe_Idle_Right" }
		};

		hatMovingAnimations = new Dictionary<Vector2, string>
		{
			{ Vector2.down, "Hat_Walk_Down" },
			{ Vector2.up, "Hat_Walk_Up" },
			{ Vector2.left, "Hat_Walk_Left" },
			{ Vector2.right, "Hat_Walk_Right" }
		};

		hatIdleAnimations = new Dictionary<Vector2, string>
		{
			{ Vector2.down, "Hat_Idle_Down" },
			{ Vector2.up, "Hat_Idle_Up" },
			{ Vector2.left, "Hat_Idle_Left" },
			{ Vector2.right, "Hat_Idle_Right" }
		};
	}
}
