using UnityEngine;

namespace FishingRodSystem
{
    /// <summary>
    /// Helper struct for holding data about a vein segments. Used by VeinControllerRealistic class.
    /// </summary>
    public struct VeinSegment
    {
        /// <summary>
        /// Shortcut for Segment with the Velocity and Positions values set to Vector3.zero.
        /// </summary>
        public static readonly VeinSegment Zero = new VeinSegment(Vector3.zero);

        public Vector3 Position 
        { 
            get { return _position; } 
            set { _position = value; }
        }

        public Vector3 OldPosition
        {
            get { return _oldPosition; }
            set { _oldPosition = value; }
        }

        public Vector3 Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        private Vector3 _position;
        private Vector3 _oldPosition;
        private Vector3 _velocity;

        public VeinSegment(Vector3 position)
        {
            _position = position;
            _oldPosition = position;
            _velocity = Vector3.zero;
        }
    }
}
