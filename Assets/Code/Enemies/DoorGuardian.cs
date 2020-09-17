using System.Collections.Generic;
using UnityEngine;

using ManicMiner.Global;
using ManicMiner.Utility;

namespace ManicMiner.Enemies
{
    public class DoorGuardian : MonoBehaviour
    {
        // Flag is the hidden door activated.
        private bool _IsHiddenDoorOpened = false;

        // Does Guardian has separate sprite sets for moving in different directions.
        [SerializeField] private bool _IsBidirectional = true;

        // Moving bounds of the gardian.
        [SerializeField] private float _LeftBound = -1;
        [SerializeField] private float _RightBound = -1;

        // Moving bounds of the gardian after door opens.
        [SerializeField] private float _LeftWiderBound = -1;
        [SerializeField] private float _RightWiderBound = -1;

        // Direction in which in guarding currently moving to.
        [SerializeField] private bool _IsMovingRight = true;

        // Ticks to animate.
        [SerializeField] private int _AnimateOnTicksForGuardian = 4;

        // Increment of sprite position.
        private Vector2 _SpritePositionIncrement;

        // For visual animation.
        private SpriteRenderer _SpriteR;
        private IndexAnimator _SpriteAnimator;

        // For contacts with other colliders.
        private Collider2D _GuardianCollider;

        // We use 50 fixed frames per second, as in project settings.
        private TickCounter _TickerCounter;

        // Flags if true, we should render.
        private bool _IsTimeToDraw = false;

        // Start is called before the first frame update
        void Start()
        {
            int indexLenght;
            int currentIndex;
            IndexDirection indexMovingTo;

            // Get reference to sprite renderer.
            _SpriteR = gameObject.GetComponent<SpriteRenderer>();

            // Reference to the sprite collider.
            _GuardianCollider = gameObject.GetComponent<Collider2D>();

            // Set animation counter rate.
            _TickerCounter = new TickCounter(_AnimateOnTicksForGuardian);

            // Calculate indexes of guardian sprites in sprite sheet.
            indexLenght = (_IsBidirectional == true) ? 8 : 4;
            currentIndex = ResourceManager.FindSpriteIndex(ResourceManager.GuardiansSprites, _SpriteR.sprite);
            indexMovingTo = (_IsMovingRight == true) ? IndexDirection.Forward : IndexDirection.Backward;

            _SpriteAnimator = new IndexAnimator(indexLenght, currentIndex, indexMovingTo, _IsBidirectional);
        }

        // Update is called once per frame.
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
                // Is the first lever activated (there are two of them)?
                if (GameManager.CurrentLeversToBeActivated == 1 && _IsHiddenDoorOpened == false)
                {
                    // Yes,  means hidden door is opened, use wider bounds for movement.
                    _IsHiddenDoorOpened = true;

                    _LeftBound = _LeftWiderBound;
                    _RightBound = _RightWiderBound;
                }

                // Is time to resetCountdown?
                if (_TickerCounter.IsItTimeToCalculate() == true)
                {
                    // Yes.
                    CalculateMoveSprite();
                }
            }
        }

        // Drawing sprite method.
        private void Draw()
        {
            List<Vector2> physicsShape = new List<Vector2>();

            // Set sprite and position.
            _SpriteR.sprite = ResourceManager.GuardiansSprites[_SpriteAnimator.CurrentIndex];
            transform.position = new Vector3(transform.position.x + _SpritePositionIncrement.x, transform.position.y + _SpritePositionIncrement.y);

            // Set collider's physics shape.
            _SpriteR.sprite.GetPhysicsShape(0, physicsShape);
            ((PolygonCollider2D)_GuardianCollider).SetPath(0, physicsShape);

            _IsTimeToDraw = false;
        }

        private void CalculateMoveSprite()
        {
            _SpritePositionIncrement = Vector2.zero;

            // Did guardian reach last animation sprite?
            if (_SpriteAnimator.IsStepOnBound() == true)
            {
                // Yês.
                // Did guardian hit the bound on the right side?
                if ((transform.position.x == _RightBound && _IsMovingRight == true) || (transform.position.x == _LeftBound && _IsMovingRight == false))
                {
                    // Yes, change direction of the movement.
                    _IsMovingRight = !_IsMovingRight;
                    _SpriteAnimator.ChangeDirection();
                }
                else
                {
                    // No, move position.
                    _SpriteAnimator.MakeStep();
                    _SpritePositionIncrement.x = (_IsMovingRight == true) ? 1 : -1;
                }
            }
            else
            {
                _SpriteAnimator.MakeStep();
            }

            _IsTimeToDraw = true;
        }
    } // Class end.
}
