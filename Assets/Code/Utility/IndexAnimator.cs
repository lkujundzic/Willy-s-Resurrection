using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManicMiner.Utility
{
    public enum IndexDirection
    {
        Forward,
        Backward,
    }

    // Class can be used for calculating indexes for animation of colors, sprites in sprite sheets...Specific for this game.
    // Bidirectional means half of indexes is for moving one side, half for moving other side.
    // If it is not bidirectional, it can be just circular and also rotational (fros side to side, forard-backward).
    public class IndexAnimator
    {
        // Settings.
        private int _IndexBase;
        private int _IndexLenght; 
        private int _CurrentIndex;
        private IndexDirection _IndexMovingTo;
        private bool _IsBiderctional;

        public int CurrentIndex
        {
            get
            {
                return _CurrentIndex;
            }
        }

        public IndexAnimator(int indexLenght = 1, int currentIndex = 0, IndexDirection indexMovingTo = IndexDirection.Forward, bool isBiderctional = false)
        {
            _IndexLenght = indexLenght;
            _CurrentIndex = currentIndex;
            _IndexMovingTo = indexMovingTo;
            _IsBiderctional = isBiderctional;

            _IndexBase = (_CurrentIndex / _IndexLenght) * _IndexLenght;
        }

        public void MakeStep()
        {
            // Is animation toward end of index?
            if(_IndexMovingTo == IndexDirection.Forward)
            {
                // Yes.
                if(IsStepOnBound() == true)
                {
                    int shiftDivider = (_IsBiderctional == true)? 2 : 1;
                    
                    _CurrentIndex -= _IndexLenght / shiftDivider - 1;
                }
                else
                {
                    _CurrentIndex++;
                }
            }
            else
            {
                // No.
                if (IsStepOnBound() == true)
                {
                    int shiftDivider = (_IsBiderctional == true) ? 2 : 1;

                    _CurrentIndex += _IndexLenght / shiftDivider - 1;
                }
                else
                {
                    _CurrentIndex--;
                }
            }
        }

        public bool IsStepOnBound()
        {
            // Index on outer boundaries.
            return ((_CurrentIndex == _IndexBase && _IndexMovingTo == IndexDirection.Backward) || (_CurrentIndex == _IndexBase + _IndexLenght - 1 && _IndexMovingTo == IndexDirection.Forward)) ? true : false;
        }

        public void ChangeDirection()
        {
            // Is it bidirectional index?
            if (_IsBiderctional == true)
            {
                // Yes.
                _CurrentIndex += (_IndexMovingTo == IndexDirection.Forward) ? -_IndexLenght / 2 : _IndexLenght / 2;
            }

            _IndexMovingTo = (_IndexMovingTo == IndexDirection.Forward) ? IndexDirection.Backward : IndexDirection.Forward;
        }
    } // Class end.
}
