using UnityEngine;

using ManicMiner.Global;
using ManicMiner.Utility;

namespace ManicMiner.Enemies
{
    public class Eugene : MonoBehaviour
    {
        // Flag is the elevator activated.
        private bool _IsElevatorActivated = false;

        // Moving bounds of the gardian.
        [SerializeField] private float _TopBound = -1;
        [SerializeField] private float _BottomBound = -1;

        // Direction in which in guarding currently moving to.
        [SerializeField] private bool _IsMovingDown = true;

        // Ticks to animate.
        [SerializeField] private int _AnimateOnTicksForEugene = 4;

        // Move increment on animate.
        [SerializeField] private float _MoveUnitsOnAnimate = 0.25f;

        // Move increment on animate.
        [SerializeField] private float _MoveUnitsOnAnimateAfterElevatorActivated = 0.25f;


        // Increment of sprite position.
        private Vector2 _SpritePositionIncrement;

        // For visual animation.
        private SpriteRenderer _SpriteR;

        // We use 50 fixed frames per second, as in project settings.
        private TickCounter _TickerCounter;

        // Flags if true, we should render.
        private bool _IsTimeToDraw = false;

        // Start is called before the first frame update
        void Start()
        {
            // Get reference to sprite renderer.
            _SpriteR = gameObject.GetComponent<SpriteRenderer>();

            // Set animation counter rate.
            _TickerCounter = new TickCounter(_AnimateOnTicksForEugene);
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
                if (GameManager.ElevatorIsActive == true && _IsElevatorActivated == false)
                {
                    // Yes,  mark elevator activated.
                    _IsElevatorActivated = true;
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
            // Set sprite and position.
            transform.position = new Vector3(transform.position.x + _SpritePositionIncrement.x, transform.position.y + _SpritePositionIncrement.y);

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
            }

            // Is Elevator activated?
            if (_IsElevatorActivated == true)
            {
                // Yes.
                // Did guardian not hit the bound on the bottom side?
                if (transform.position.y > _BottomBound)
                {
                    // Yes.
                    _SpritePositionIncrement.y = -_MoveUnitsOnAnimateAfterElevatorActivated;
                }
            }
            else
            {
                // No.
                // Is guardian moving down?
                _SpritePositionIncrement.y += (_IsMovingDown == true) ? -_MoveUnitsOnAnimate : _MoveUnitsOnAnimate;
            }

            _IsTimeToDraw = true;
        }

    } // Class end.
}