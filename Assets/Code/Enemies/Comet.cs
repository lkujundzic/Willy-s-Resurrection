using System.Collections.Generic;
using UnityEngine;

using ManicMiner.Global;
using ManicMiner.Utility;

namespace ManicMiner.Enemies
{
    public class Comet : MonoBehaviour
    {
        // Moving bounds of the gardian.
        [SerializeField] private float _TopBound = -1;
        [SerializeField] private float _BottomBound = -1;

        // Ticks to animate.
        [SerializeField] private int _AnimateOnTicksForComet = 4;

        // Move increment on animate.
        [SerializeField] private float _MoveUnitsOnAnimate = 0.25f;

        // Increment of sprite position.
        private Vector2 _SpritePositionIncrement;

        // For visual animation.
        private SpriteRenderer _SpriteR;
        private IndexAnimator _SpriteAnimator;

        // Comet color.
        Color32 _OriginalCometColor;
        Color32 _CurrentCometColor;

        // For contacts with other colliders.
        private Collider2D _CometCollider;

        // We use 50 fixed frames per second, as in project settings.
        private TickCounter _TickerCounter;

        // Flags if true, we should render.
        private bool _IsTimeToDraw = false;

        // Start is called before the first frame update
        void Start()
        {
            int currentIndex;

            // Get reference to sprite renderer.
            _SpriteR = gameObject.GetComponent<SpriteRenderer>();

            // Get original comet color.
            _OriginalCometColor = _SpriteR.color;

            // Reference to the sprite collider.
            _CometCollider = gameObject.GetComponent<Collider2D>();

            // Set animation counter rate.
            _TickerCounter = new TickCounter(_AnimateOnTicksForComet);

            // Calculate indexes of guardian sprites in sprite sheet.
            currentIndex = ResourceManager.FindSpriteIndex(ResourceManager.GuardiansSprites, _SpriteR.sprite);
            _SpriteAnimator = new IndexAnimator(8, currentIndex, IndexDirection.Forward, false);
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
            _SpriteR.color = _CurrentCometColor;

            transform.position = new Vector3(transform.position.x + _SpritePositionIncrement.x, transform.position.y + _SpritePositionIncrement.y);

            // Set collider's physics shape.
            _SpriteR.sprite.GetPhysicsShape(0, physicsShape);
            ((PolygonCollider2D)_CometCollider).SetPath(0, physicsShape);

            _IsTimeToDraw = false;
        }

        private void CalculateMoveSprite()
        {
            _SpritePositionIncrement = Vector2.zero;

            // Did comet show on the top bound?
            if (transform.position.y >= _TopBound)
            {
                // Yes, change direction of the movement.
                _CurrentCometColor = Color.black;
            }
            else
            {
                _CurrentCometColor = _OriginalCometColor;
            }

            // Did comet hit the bound on the bottom side?
            if (transform.position.y <= _BottomBound)
            {
                // Yes.
                // Did guardian reach last animation sprite?
                if (_SpriteAnimator.IsStepOnBound() == true)
                {
                    // Yês.
                    _CurrentCometColor = Color.black;

                    _SpritePositionIncrement.x = +8f;
                    _SpritePositionIncrement.y += _TopBound - _BottomBound;

                    // Is the x coordiante out of bounds?
                    if (transform.position.x + _SpritePositionIncrement.x > 30f)
                    {
                        // Yes, move it to the first position.
                        _SpritePositionIncrement.x -= 32f;
                    }
                }

                _SpriteAnimator.MakeStep();
            }
            else
            {
                // No, move commet down.
                _SpritePositionIncrement.y = -_MoveUnitsOnAnimate;
            }

            _IsTimeToDraw = true;
        }
    } // Class end.
}
