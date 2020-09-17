using System.Collections.Generic;

using UnityEngine;

using ManicMiner.Global;
using ManicMiner.Utility;

namespace ManicMiner.Enemies
{
    public class VerticalGuardian : MonoBehaviour
    {
        // Moving bounds of the gardian.
        [SerializeField] private float _TopBound = -1;
        [SerializeField] private float _BottomBound = -1;

        // Direction in which in guarding currently moving to.
        [SerializeField] private bool _IsMovingDown = true;

        // Ticks to animate.
        [SerializeField] private int _AnimateOnTicksForGuardian = 4;

        // Move increment on animate.
        [SerializeField] private float _MoveUnitsOnAnimate = 0.25f;

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
            int currentIndex;
            IndexDirection indexMovingTo;

            // Get reference to sprite renderer.
            _SpriteR = gameObject.GetComponent<SpriteRenderer>();

            // Reference to the sprite collider.
            _GuardianCollider = gameObject.GetComponent<Collider2D>();

            // Set animation counter rate.
            _TickerCounter = new TickCounter(_AnimateOnTicksForGuardian);

            // Calculate indexes of guardian sprites in sprite sheet.
            currentIndex = ResourceManager.FindSpriteIndex(ResourceManager.GuardiansSprites, _SpriteR.sprite);
            indexMovingTo = (_IsMovingDown == true) ? IndexDirection.Forward : IndexDirection.Backward;

            _SpriteAnimator = new IndexAnimator(4, currentIndex, indexMovingTo, false);
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

            // Did guardian hit the bound on the bottom or top side?
            if ((transform.position.y <= _BottomBound && _IsMovingDown == true) || (transform.position.y >= _TopBound && _IsMovingDown == false))
            {
                // Yes, change direction of the movement.
                _IsMovingDown = !_IsMovingDown;
                _SpriteAnimator.ChangeDirection();
            }

            // Is guardian moving down?
            _SpritePositionIncrement.y += (_IsMovingDown == true) ? -_MoveUnitsOnAnimate : _MoveUnitsOnAnimate;

            _SpriteAnimator.MakeStep();

            _IsTimeToDraw = true;
        }
    } // Class end.
}
