using System.Collections.Generic;
using UnityEngine;

namespace FishingRodSystem
{
    /// <summary>
    /// The accurate solution for a stiff veins, such as a metal wire, because a metal wire is not swinging as much as a vein made of a lighter materials
    /// </summary>
    public class VeinControllerSimple : VeinControllerBase
    {
        [Header("Physical properties of the vein")]
        [SerializeField] private float veinDensity = 7750f;
        [SerializeField] private float veinRadius = 0.02f;

        [Header("Dynamic values")]
        [SerializeField] private float veinLength = 1f;
        [SerializeField] private float minVeinLength = 1f;
        [SerializeField] private float maxVeinLength = 10f;

        [SerializeField] private float attachedWeight = 100f;

        [SerializeField] private float winchSpeed = 2f;

        // The joint used to approximate the vein
        private SpringJoint springJoint;

        #region MonoBehaviour's callbacks
        void Start()
        {
            springJoint = startPoint.GetComponent<SpringJoint>();

            // The first update inits the spring we use to approximate the vein between point A and B
            UpdateSpring();

            // Adds the weight of an attachement
            endPoint.GetComponent<Rigidbody>().mass = attachedWeight;
        }

        void Update()
        {
            UpdateWinch();
            DrawVein();
        }

        #endregion

        #region Private methods
        /// <summary>
        /// Updates the spring constant and the length of the spring
        /// </summary>
        private void UpdateSpring()
        {
            float veinVolume = Mathf.PI * veinRadius * veinRadius * veinLength;
            float veinMass = veinVolume * veinDensity;

            // Adds the weight of the attachement
            veinMass += attachedWeight;

            // The spring constant
            float veinForce = veinMass * 9.81f;

            // Use the spring equation to calculate F = k * x should balance this force, where x is how much the top segment should stretch
            float springConstant = veinForce / 0.01f;

            // Adds the value to the spring
            springJoint.spring = springConstant * 1.0f;
            springJoint.damper = springConstant * 0.8f;

            // Updates the length of the vein
            springJoint.maxDistance = veinLength;
        }

        /// <summary>
        /// Displays the vein using a line renderer
        /// </summary>
        private void DrawVein()
        {
            lineRenderer.startWidth = veinThickness; 
            lineRenderer.endWidth = veinThickness;

            // Updates the list with a vein segments by approximating the vein with just a Bezier curve
            Vector3 bezierA = startPoint.position;
            Vector3 bezierD = endPoint.position;
            Vector3 bezierB = bezierA + startPoint.up * (-(bezierA - bezierD).magnitude * 0.1f);
            Vector3 bezierC = bezierD + endPoint.up * ((bezierA - bezierD).magnitude * 0.5f);

            List<Vector3> tempPositions = new List<Vector3>();

            for (int i = 0; i < veinSegmentsList.Count; i++)
            {
                tempPositions.Add(veinSegmentsList[i].Position);
            }

            BezierCurve.GetBezierCurve(bezierA, bezierB, bezierC, bezierD, tempPositions);

            // Vein segments positions
            Vector3[] positions = new Vector3[veinSegmentsList.Count];

            for (int i = 0; i < veinSegmentsList.Count; i++)
            {
                positions[i] = veinSegmentsList[i].Position;
            }

            //Add the positions to the line renderer
            lineRenderer.positionCount = positions.Length;

            lineRenderer.SetPositions(positions);
        }

        /// <summary>
        /// Updates the length of the vein
        /// </summary>
        private void UpdateWinch()
        {
            bool hasChangedLength = false;

            if (Input.GetKey(KeyCode.O) && veinLength < maxVeinLength) // Extends the vein
            {
                veinLength += winchSpeed * Time.deltaTime;

                hasChangedLength = true;
            }
            else if (Input.GetKey(KeyCode.I) && veinLength > minVeinLength) // Shortens the vein
            {
                veinLength -= winchSpeed * Time.deltaTime;

                hasChangedLength = true;
            }

            if (hasChangedLength)
            {
                veinLength = Mathf.Clamp(veinLength, minVeinLength, maxVeinLength);

                // Needs to recalculate the k-value due the fact it depends on the length of the vein
                UpdateSpring();
            }
        }
        #endregion
    }
}