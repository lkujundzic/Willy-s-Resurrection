using System.Collections.Generic;
using UnityEngine;

using ManicMiner.Global;
using ManicMiner.Utility;
using ManicMiner.Platforms;

namespace ManicMiner.Friends
{
    public class Player : MonoBehaviour
    {
        private bool _IsPlayerFacingRight;

        private float _PressedKeyDirection = 0f;
        private float _PressedKeyForJump = 0f;

        // Increment of sprite position.
        private Vector2 _SpritePositionIncrement;

        // Jump Array In Pixels
        private float[] _UnitsToJump = { 0.5f, 0.5f, 0.375f, 0.375f, 0.25f, 0.25f, 0.125f, 0.125f, 0f };
        private float[] _UnitsToDrop = { 0.5f, 0.5f };
        private int _UnitsToJumpOrDropIndex = 0;

        private int[] _CheckpointsToCeillingCollisionCheck = { 0, 0, 1, 0, 0, 1, 0, 0, 0 };
        private int[] _CheckpointsToFloorCollisionCheck = { 0, 1, 0, 0, 1, 0, 0, 0, 0 };

        // Jump sounds.
        private AudioClip[] _JumpSounds = new AudioClip[35];
        private int _JumpSoundsIndex = 0;

        // States in the air
        private bool _IsPlayerInJumpPhase = false;
        private bool _IsPlayerInFallPhase = false;
        private bool _IsPlayerInDropPhase = false;
        private bool _JustWasPlayerInJumpOrDropPhase = false;
        private float _UnitsToDropSafe = 4.5f;

        private float _SpriteJumpDirection;
        private Vector2 _SpriteAltitudeBeforeFallOrDrop;

        // For visual animation.
        private SpriteRenderer _SpriteR;
        private IndexAnimator _SpriteAnimator;

        // For contacts with other colliders.
        private Collider2D _PlayerCollider;

        private LayerMask _MaskWithWalls;
        private LayerMask _MaskWithGround;
        private LayerMask _MaskWithMud;
        private LayerMask _MaskWithPickups;
        private LayerMask _MaskWithLevers;
        private LayerMask _MaskWithEnemies;
        private LayerMask _MaskWithElevator;
        private int _ThreadmillsLayerIndex;
        private int _MudLayerIndex;

        private enum _CollisionSide { Top, Bottom, Left, Right };

        // For threadmill and mugs interaction.
        private bool _ForceMovingOnThreadmill = false;
        private RaycastHit2D[] _lastGroundContacts = null;

        // For sounds.
        private AudioSource _SoundsSource;

        // Resources for sounds.
        [SerializeField] private AudioClip _PickupSound = null;
        [SerializeField] private AudioClip _DeathSound = null;

        // We use 50 fixed frames per second, as in project settings.
        private TickCounter _TickerCounter = new TickCounter(ResourceManager.AnimateOnTicksForPlayer);

        // Flags if true, we should render.
        private bool _IsTimeToDraw = false;

        // Start is called before the first frame update.
        void Start()
        {
            int currentIndex;
            IndexDirection indexMovingTo;

            // Get reference to sprite renderer.
            _SpriteR = gameObject.GetComponent<SpriteRenderer>();

            // Reference to the sprite collider.
            _PlayerCollider = gameObject.GetComponent<Collider2D>();

            // Masks needed for different kind of collisions.
            _MaskWithWalls = LayerMask.GetMask("Walls");
            _MaskWithGround = LayerMask.GetMask("Walls", "Platforms", "Threadmills", "Mud");
            _MaskWithMud = LayerMask.GetMask("Mud");
            _MaskWithPickups = LayerMask.GetMask("Pickups");
            _MaskWithLevers = LayerMask.GetMask("Levers");
            _MaskWithEnemies = LayerMask.GetMask("Guardians", "Obstacles");
            _MaskWithElevator = LayerMask.GetMask("Elevators");

            // Layer index needed to indentify different colliders.
            _ThreadmillsLayerIndex = LayerMask.NameToLayer("Threadmills");
            _MudLayerIndex = LayerMask.NameToLayer("Mud");

            // Initial setup of the player. This is different for every level.
            currentIndex = ResourceManager.FindSpriteIndex(ResourceManager.PlayerSprites, _SpriteR.sprite);

            _IsPlayerFacingRight = (currentIndex > 3) ? true : false;
            indexMovingTo = (_IsPlayerFacingRight == true) ? IndexDirection.Forward : IndexDirection.Backward;

            _SpriteAnimator = new IndexAnimator(8, currentIndex, indexMovingTo, true);

            // For sounds effects.
            _SoundsSource = gameObject.GetComponent<AudioSource>();

            // Load jump sounds.
            for (int c = 0; c < _JumpSounds.Length; c++)
            {
                _JumpSounds[c] = Resources.Load<AudioClip>("Sounds/Jump" + (c + 1).ToString());
            }
        }

        // Update is called once per frame. Actual drawing goes here.
        void Update()
        {
            if (_IsTimeToDraw == true)
            {
                Draw();
            }
        }

        // Update is called on fixed time intervals. 
        // We'll use it for user controls, calculation for movement and animation, since we want to make it consistent over time (same speed on different time frames).
        private void FixedUpdate()
        {
            if (GameManager.IsGamePaused == false)
            {
                // Is time to resetCountdown?
                if (_TickerCounter.IsItTimeToCalculate() == true)
                {
                    // Yes.
                    // Is elevator active, and not counting?
                    if (GameManager.ElevatorIsActive == true && GameManager.ElevatorCounting == false)
                    {
                        // Yes.
                        CheckElevatorHits();
                    }

                    // Is player not dead, and elevator is not counting?
                    if (GameManager.PlayerIsDead == false && GameManager.ElevatorCounting == false)
                    {
                        _SpritePositionIncrement = Vector2.zero;

                        // Do key transformations.
                        TransformKeyInput();

                        // Is player in the jump phase?
                        if (_IsPlayerInJumpPhase == true)
                        {
                            // Yes.
                            CalculateJump();
                        }
                        else
                        {
                            // No.
                            // Is player in the fall phase?
                            if (_IsPlayerInFallPhase == true)
                            {
                                // Yes.
                                CalculateFall();
                            }
                            else
                            {
                                // No.
                                // Is player in the drop phase?
                                if (_IsPlayerInDropPhase == true)
                                {
                                    // Yes.
                                    CalculateDrop();
                                }
                                else
                                {
                                    // No.
                                    RaycastHit2D[] groundContacts = GetCollidersOnSide(_CollisionSide.Bottom, _MaskWithGround);

                                    //Is player grounded?
                                    if (groundContacts.Length > 0)
                                    {
                                        // Yes.

                                        // Do mud and threadmill calculations.
                                        CalculateThreadmillAndMudInteraction(groundContacts);

                                        // Are we forced to move on threadmill?
                                        if (_ForceMovingOnThreadmill == true)
                                        {
                                            // Yes.
                                            _PressedKeyDirection = (GameManager.ThreadmillIsRotatingRight == true) ? 1 : -1;
                                        }


                                        // Was player just finished jump or drop phase?
                                        if (_JustWasPlayerInJumpOrDropPhase == true)
                                        {
                                            // Yes.
                                            _JustWasPlayerInJumpOrDropPhase = false;

                                            // Is he stepping over mud?
                                            if (GetCollidersOnSide(_CollisionSide.Bottom, _MaskWithMud).Length > 0)
                                            {
                                                // Yes, he cannot jump right away.
                                                _PressedKeyForJump = 0f;
                                            }
                                        }

                                        // Is the jump button pressed?
                                        if (_PressedKeyForJump > 0)
                                        {
                                            // Yes.

                                            // Is player not hitting the ceiling?
                                            if (GetCollidersOnSide(_CollisionSide.Top, _MaskWithWalls).Length == 0)
                                            {
                                                // Yes, initialize jump.
                                                _SpriteJumpDirection = _PressedKeyDirection;

                                                _IsPlayerInJumpPhase = true;
                                                _UnitsToJumpOrDropIndex = 0;
                                                _JumpSoundsIndex = 0;

                                                CalculateJump();

                                                // Is there a jump direction?
                                                if (_SpriteJumpDirection != 0)
                                                {
                                                    // Yes.
                                                    CalculateMoveSprite();
                                                }

                                                // Reset mud and threadmill calculations.
                                                CalculateThreadmillAndMudInteraction(null);
                                            }
                                        }
                                        else
                                        {
                                            // No.
                                            // Is direction button pressed?
                                            if (_PressedKeyDirection != 0)
                                            {
                                                //Yes, is player changing direction?
                                                if ((_PressedKeyDirection < 0 && _IsPlayerFacingRight == true) || (_PressedKeyDirection > 0 && _IsPlayerFacingRight == false))
                                                {
                                                    // Yes.
                                                    _IsPlayerFacingRight = !_IsPlayerFacingRight;
                                                    _SpriteAnimator.ChangeDirection();
                                                }
                                                else
                                                {
                                                    // No.
                                                    CalculateMoveSprite();
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // No, initialize drop.
                                        _SpriteAltitudeBeforeFallOrDrop = new Vector2(transform.position.x, transform.position.y);

                                        _IsPlayerInDropPhase = true;
                                        _UnitsToJumpOrDropIndex = 0;
                                        _JumpSoundsIndex = 18;

                                        CalculateDrop();

                                        // Reset mud and threadmill calculations.
                                        CalculateThreadmillAndMudInteraction(null);
                                    }
                                }
                            }
                        }

                        // Is player in the air?
                        if (_IsPlayerInJumpPhase == true || _IsPlayerInFallPhase == true || _IsPlayerInDropPhase == true)
                        {
                            // Yes.
                            PlayMovementInAirSound();
                        }

                        // Checking out on the end of movement phases, so will include projected position into calculation.
                        CheckPickupHits();
                        CheckLeverHits();
                        CheckEnemyHits();
                    }
                }
            }
        }

        // Drawing sprite method.
        private void Draw()
        {
            List<Vector2> physicsShape = new List<Vector2>();

            // Set sprite and position.
            _SpriteR.sprite = ResourceManager.PlayerSprites[_SpriteAnimator.CurrentIndex];
            transform.position = new Vector3(transform.position.x + _SpritePositionIncrement.x, transform.position.y + _SpritePositionIncrement.y);

            // Set collider's physics shape.
            _SpriteR.sprite.GetPhysicsShape(0, physicsShape);
            ((PolygonCollider2D)_PlayerCollider).SetPath(0, physicsShape);

            _IsTimeToDraw = false;
        }

        // Key transorms.
        private void TransformKeyInput()
        {
            if (ResourceManager.KeyForLeft.Pressed)
            {
                _PressedKeyDirection = -1f;
            }
            else
            {
                if (ResourceManager.KeyForRight.Pressed)
                {
                    _PressedKeyDirection = 1f;
                }
                else
                {
                    _PressedKeyDirection = 0f;
                }
            }

            _PressedKeyForJump = (ResourceManager.KeyForJump.Pressed) ? 1f : 0f;
        }

        // Sprite calculation methods.
        private void CalculateMoveSprite()
        {
            // Is it bound step?
            if (_SpriteAnimator.IsStepOnBound() == true)
            {
                // Yes.
                // Is player facing right?
                if (_IsPlayerFacingRight == true)
                {
                    // Yes.
                    // Is there a wall on the right side?
                    if (GetCollidersOnSide(_CollisionSide.Right, _MaskWithWalls).Length == 0)
                    {
                        // No, move position to the right.
                        _SpriteAnimator.MakeStep();
                        _SpritePositionIncrement.x = 1;
                    }
                }
                else
                {
                    // No.
                    // Is there a wall on the left side?
                    if (GetCollidersOnSide(_CollisionSide.Left, _MaskWithWalls).Length == 0)
                    {
                        // No, move position to the left.
                        _SpriteAnimator.MakeStep();
                        _SpritePositionIncrement.x = -1;
                    }
                }
            }
            else
            {
                // No.
                _SpriteAnimator.MakeStep();
            }

            _IsTimeToDraw = true;
        }
        private void CalculateJump()
        {
            _SpritePositionIncrement.y = _UnitsToJump[_UnitsToJumpOrDropIndex];
            _IsTimeToDraw = true;

            // Next cycle.
            _UnitsToJumpOrDropIndex++;

            // Is it time to check for ceiling collision?
            if (_UnitsToJumpOrDropIndex < _UnitsToJump.Length && _CheckpointsToCeillingCollisionCheck[_UnitsToJumpOrDropIndex] == 1)
            {
                // Yes.
                // Is player hitting the ceilling?
                if (GetCollidersOnSide(_CollisionSide.Top, _MaskWithWalls).Length > 0)
                {
                    // Yes, initialize drop.
                    _IsPlayerInJumpPhase = false;
                    _IsPlayerInDropPhase = true;

                    _SpriteAltitudeBeforeFallOrDrop = new Vector2(transform.position.x, transform.position.y);
                    _JumpSoundsIndex = (8 - _UnitsToJumpOrDropIndex) + 9;
                    _UnitsToJumpOrDropIndex = 0;

                    // Is player hitting also the ground?
                    if (GetCollidersOnSide(_CollisionSide.Bottom, _MaskWithGround).Length > 0)
                    {
                        // Yes.
                        _IsPlayerInDropPhase = false;

                        // We need this because if stepd on mud from fall or drop we cannot jump immediately.
                        _JustWasPlayerInJumpOrDropPhase = true;
                    }
                }
            }

            // Is jump phase ended?
            if (_UnitsToJumpOrDropIndex == _UnitsToJump.Length)
            {
                // Yes, initialize fall.
                _IsPlayerInJumpPhase = false;
                _IsPlayerInFallPhase = true;

                _SpriteAltitudeBeforeFallOrDrop = new Vector2(transform.position.x, transform.position.y);
                _UnitsToJumpOrDropIndex = _UnitsToJump.Length - 1;
            }

            // Is there a jump direction?
            if (_SpriteJumpDirection != 0)
            {
                // Yes.
                CalculateMoveSprite();
            }
        }
        private void CalculateFall()
        {
            _SpritePositionIncrement.y = -_UnitsToJump[_UnitsToJumpOrDropIndex];
            _IsTimeToDraw = true;

            // Is there a jump direction?
            if (_SpriteJumpDirection != 0)
            {
                // Yes.
                CalculateMoveSprite();
            }

            // Next cycle.
            _UnitsToJumpOrDropIndex--;

            // Is it time to check for ground collision?
            if (_UnitsToJumpOrDropIndex >= 0 && _CheckpointsToFloorCollisionCheck[_UnitsToJumpOrDropIndex] == 1)
            {
                // Yes.
                // Is player hitting the floor?
                if (GetCollidersOnSide(_CollisionSide.Bottom, _MaskWithGround).Length > 0)
                {
                    // Yes.
                    _IsPlayerInFallPhase = false;

                    // Was fall over the maximum safe height?
                    if ((_SpriteAltitudeBeforeFallOrDrop.y - transform.position.y - _SpritePositionIncrement.y) > _UnitsToDropSafe)
                    {
                        // Yes.
                        _SoundsSource.PlayOneShot(_DeathSound);
                        GameManager.PlayerIsDead = true;
                    }

                    // We need this because if stepd on mud from fall or drop we cannot jump immediately.
                    _JustWasPlayerInJumpOrDropPhase = true;
                }
            }

            // Is fall phase ended?
            if (_UnitsToJumpOrDropIndex < 0)
            {
                // Yes.
                _IsPlayerInFallPhase = false;

                // Is player not hitting the floor?
                if (GetCollidersOnSide(_CollisionSide.Bottom, _MaskWithGround).Length == 0)
                {
                    // Yes, initialize drop.
                    _IsPlayerInDropPhase = true;
                    _UnitsToJumpOrDropIndex = 0;
                }
                else
                {
                    // We need this because if stepd on mud from fall or drop we cannot jump immediately.
                    _JustWasPlayerInJumpOrDropPhase = true;
                }
            }
        }
        private void CalculateDrop()
        {
            _SpritePositionIncrement.y = -_UnitsToDrop[_UnitsToJumpOrDropIndex];
            _IsTimeToDraw = true;

            // Next cycle.
            _UnitsToJumpOrDropIndex++;

            // Is it time to check for floor collision?
            if (_UnitsToJumpOrDropIndex == _UnitsToDrop.Length)
            {
                // Yes.
                _UnitsToJumpOrDropIndex = 0;

                // Is player hitting the ground?
                if (GetCollidersOnSide(_CollisionSide.Bottom, _MaskWithGround).Length > 0)
                {
                    // Yes.
                    _IsPlayerInDropPhase = false;

                    // Was fall over the maximum safe height?
                    if ((_SpriteAltitudeBeforeFallOrDrop.y - transform.position.y - _SpritePositionIncrement.y) > _UnitsToDropSafe)
                    {
                        // Yes.
                        _SoundsSource.PlayOneShot(_DeathSound);
                        GameManager.PlayerIsDead = true;
                    }

                    // We need this because if stepd on mud from fall or drop we cannot jump immediately.
                    _JustWasPlayerInJumpOrDropPhase = true;
                }
            }
        }

        void PlayMovementInAirSound()
        {
            if (_JumpSoundsIndex < _JumpSounds.Length)
            {
                _SoundsSource.PlayOneShot(_JumpSounds[_JumpSoundsIndex]);
                _JumpSoundsIndex++;
            }
        }

        // Threadmill and mug calculations.
        void CalculateThreadmillAndMudInteraction(RaycastHit2D[] freshContacts)
        {
            // Was last movement ground contact?
            if (_lastGroundContacts != null)
            {
                // Yes.

                // Go through contact cubes.
                foreach (RaycastHit2D rh in _lastGroundContacts)
                {
                    // Is Mud cube under already destroyed?
                    if (rh.collider != null)
                    {
                        // No.
                        GameObject go = rh.collider.gameObject;

                        // Is it mud?
                        if (go.layer == _MudLayerIndex)
                        {
                            //Yes.
                            Mud mc = go.GetComponent<Mud>();
                            // Stop sliding of cube.
                            mc.Slide(false);
                        }
                    }
                }
            }

            // Was new movement ground contact?
            if (freshContacts != null)
            {
                // Yes.

                // Threadmill flag.
                bool isThreadmill = false;

                // Go through contact cubes.
                foreach (RaycastHit2D rh in freshContacts)
                {
                    GameObject go = rh.collider.gameObject;

                    // Is it mud?
                    if (go.layer == _MudLayerIndex)
                    {
                        //Yes.
                        // Start sliding of cube.
                        go.GetComponent<Mud>().Slide(true);
                    }

                    // Is it threadmill?
                    if (go.layer == _ThreadmillsLayerIndex)
                    {
                        //Yes.
                        isThreadmill = true;

                        // Is direction button not pressed, or movement is opposite of face direction?
                        if (_PressedKeyDirection == 0 || (_PressedKeyDirection < 0 && _IsPlayerFacingRight == true) || (_PressedKeyDirection > 0 && _IsPlayerFacingRight == false))
                        {
                            // Yes, force the movement on the threadmill. 
                            _ForceMovingOnThreadmill = true;
                        }
                    }
                }

                // Was there none fresh threadmill contacts?
                if (isThreadmill == false)
                {
                    // Yes, reset moving on the threadmill.
                    _ForceMovingOnThreadmill = false;
                }
            }
            else
            {
                // No, reset moving on the threadmill.
                _ForceMovingOnThreadmill = false;
            }

            // Fresh ones are now last ground contacts.
            _lastGroundContacts = freshContacts;
        }

        // Pickup collision test.
        void CheckPickupHits()
        {
            // Get overlaped pickup collider.
            Collider2D pickupsHit = GetColliderCollided(_MaskWithPickups);

            // Did player hit pickup?
            if (pickupsHit != null)
            {
                // Yes.

                // Pickup collected.
                GameManager.PickupCollected();
                _SoundsSource.PlayOneShot(_PickupSound);
                Destroy(pickupsHit.gameObject);
            }
        }

        // Lever collision test.
        void CheckLeverHits()
        {
            // Get overlaped lever collider.
            Collider2D leversHit = GetColliderCollided(_MaskWithLevers);

            // Did player hit lever?
            if (leversHit != null)
            {
                // Yes.

                // Lever activated.
                leversHit.gameObject.GetComponent<Lever>().ActivateLever();
            }
        }

        // Enemy collision test.
        void CheckEnemyHits()
        {
            // Get overlaped enemy collider.
            Collider2D enemyHit = GetColliderCollided(_MaskWithEnemies);

            // Did player hit any enemy (guardian obtacle)?
            if (enemyHit != null)
            {
                // Yes.
                _SoundsSource.PlayOneShot(_DeathSound);
                GameManager.PlayerIsDead = true;

                // Freeze the movement in game upon death.
                GameManager.IsGamePaused = true;
            }
        }

        // Elevator collision test.
        void CheckElevatorHits()
        {
            // Get overlaped elevator collider.
            Collider2D elevatorHit = GetColliderCollided(_MaskWithElevator);

            // Did player hit elevator?
            if (elevatorHit != null)
            {
                // Yes.

                // Is it enough inside it?
                if (Mathf.Abs(transform.position.x - elevatorHit.transform.position.x) < 1 && Mathf.Abs(transform.position.y - elevatorHit.transform.position.y) < 1.5)
                {
                    // Yes.
                    GameManager.ElevatorCounting = true;
                }
            }
        }

        // Sprite collision methods.
        private RaycastHit2D[] GetCollidersOnSide(_CollisionSide side, LayerMask lmask)
        {
            RaycastHit2D[] objectsHit;
            Vector2 origin, size, direction;

            // Whitch Side?
            switch (side)
            {
                case _CollisionSide.Left:

                    // Origin on the left.
                    origin = (Vector2)_SpriteR.bounds.center + _SpritePositionIncrement + new Vector2(-1.5f, 0f);
                    size = new Vector2(0.5f, 1f);
                    direction = Vector2.left;

                    break;
                case _CollisionSide.Right:

                    // Origin on the right.
                    origin = (Vector2)_SpriteR.bounds.center + _SpritePositionIncrement + new Vector2(1.5f, 0f);
                    size = new Vector2(0.5f, 1f);
                    direction = Vector2.right;

                    break;
                case _CollisionSide.Top:

                    // Origin on top.
                    origin = (Vector2)_SpriteR.bounds.center + _SpritePositionIncrement + new Vector2(0f, 1.5f);
                    size = new Vector2(1f, 0.5f);
                    direction = Vector2.up;

                    break;
                case _CollisionSide.Bottom:
                default:

                    // Origin on the bottom.
                    origin = (Vector2)_SpriteR.bounds.center + _SpritePositionIncrement + new Vector2(0f, -1.5f);
                    size = new Vector2(1f, 0.5f);
                    direction = Vector2.down;
                    break;
            }

            objectsHit = Physics2D.BoxCastAll(origin, size, 0f, direction, 0f, lmask);
            return objectsHit;
        }

        private Collider2D GetColliderCollided(LayerMask lmask)
        {
            Collider2D[] objectsHit = new Collider2D[1];
            ContactFilter2D filter = new ContactFilter2D();

            filter.SetLayerMask(lmask);

            // Is there any collider of the type overlaped?
            if (_PlayerCollider.OverlapCollider(filter, objectsHit) > 0)
            {
                // Yes.
                return objectsHit[0];
            }
            else
            {
                // No.
                return null;
            }
        }
    } // Class end.
}
