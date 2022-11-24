using System.Collections.Generic;
using UnityEngine;


namespace FishingRodSystem
{
    public class VeinControllerRealistic : VeinControllerBase
    {
        [Header("Physical properties of the vein")]
        [SerializeField] private float springConstant = 50f;
        [SerializeField] private float veinFriction = 1f;
        [SerializeField] private float airResistance = .025f;
        [SerializeField] private float veinSectionMass = .5f;

        [Header("Dynamic values")]
        [Range(1, 1000)]
        [SerializeField] private int numberOfSegments = 10;

        
        [Range(1, 100)]
        [SerializeField] private int simulationIterationNumber = 10;



        #region MonoBehaviour's callbacks

        private void OnEnable()
        {
           // Application.onBeforeRender += DrawVein;
        }

        private void OnDisable()
        {
           // Application.onBeforeRender -= DrawVein;
        }


        void Start()
        {
            CreateVein();
        }

        void Update()
        {
            if (veinSegmentsList.Count > 0)
            {

                   // UpdateVein(veinSegmentsList, Time.deltaTime);

                
                
                float timeStep = Time.deltaTime / (float)simulationIterationNumber;

                for (int i = 0; i < simulationIterationNumber; i++)
                {
                   UpdateVein(veinSegmentsList, timeStep);

                }
                
            }

            DrawVein();
            //VerifyVeinLength();

            if (!areBothSidesStatic)
            {
                // Moves what is hanging from the vein to the end
                endPoint.position = veinSegmentsList[0].Position;
            }

            endPoint.LookAt(veinSegmentsList[1].Position);
        }

        void FixedUpdate()
        {
            /*
            if (veinSegmentsList.Count > 0)
            {
                float timeStep = Time.fixedDeltaTime / (float)simulationIterationNumber;

                for (int i = 0; i < simulationIterationNumber; i++)
                {
                    UpdateVein(veinSegmentsList, timeStep);
                    
                }
            }
            */
        }

        /// <summary>
        /// Works only added to gameObject with added Camera component 
        /// </summary>
        private void OnPreRender()
        {
           // DrawVein();
        }

        #endregion

        #region Override methods

        /// <summary>
        /// Builds the vein top to bottom
        /// </summary>
        protected override void CreateVein()
        {
            Vector3 startPosition = startPoint.position;

            List<Vector3> veinSegmentsPositions = new List<Vector3>();

            for (int i = 0; i < numberOfSegments; i++)
            {
                veinSegmentsPositions.Add(startPosition);

                startPosition.y -= veinSegmentLength;
            }

            // Adds a segments from bottom to provide a dynamic length of the vein
            for (int i = veinSegmentsPositions.Count - 1; i >= 0; i--)
            {
                veinSegmentsList.Add(new VeinSegment(veinSegmentsPositions[i]));
            }
        }

        /// <summary>
        /// Visualizes vein using line renderer
        /// </summary>
        protected override void DrawVein()
        {
            lineRenderer.startWidth = veinThickness;
            lineRenderer.endWidth = veinThickness;

            Vector3[] segmentsPositions = new Vector3[veinSegmentsList.Count];

            for (int i = 0; i < veinSegmentsList.Count; i++)
            {
                segmentsPositions[i] = veinSegmentsList[i].Position;
            }

            lineRenderer.positionCount = segmentsPositions.Length;
            lineRenderer.SetPositions(segmentsPositions);
        }

        /// <summary>
        /// Dummy method
        /// </summary>
        protected override void UpdateVein()
        {
            
        }

        #endregion

        #region Private methods

        private void UpdateVein(List<VeinSegment> veinSegments, float timeStep)
        {
            // Move the last-top position to what the vein is attached to
            VeinSegment lastSegment = veinSegments[veinSegments.Count - 1];

            lastSegment.Position = startPoint.position;

            veinSegments[veinSegments.Count - 1] = lastSegment;


            // Calculates Forward Euler formula
            List<Vector3> accelerations = CalculateAccelerations(veinSegments);
            List<VeinSegment> nextEulers = new List<VeinSegment>();

            // Loops  through all line segments, but last
            for (int i = 0; i < veinSegments.Count - 1; i++)
            {
                VeinSegment currentSegment = VeinSegment.Zero;
                currentSegment.Velocity = veinSegments[i].Velocity + accelerations[i] * timeStep;
                currentSegment.Position = veinSegments[i].Position + veinSegments[i].Velocity * timeStep;

                nextEulers.Add(currentSegment);
            }

            // Add the last which is always the same because it's attached to something
            nextEulers.Add(veinSegments[veinSegments.Count - 1]);

            // Calculates the next pos with Heuns formulas
            List<Vector3> accelerationFromEuler = CalculateAccelerations(nextEulers);
            List<VeinSegment> nextHeuns = new List<VeinSegment>();

            // Loops through all line segments (except the last because it's always connected to something)
            for (int i = 0; i < veinSegments.Count - 1; i++)
            {
                VeinSegment currentSegment = VeinSegment.Zero;
                currentSegment.Velocity = veinSegments[i].Velocity + (accelerations[i] + accelerationFromEuler[i]) * 0.5f * timeStep;
                currentSegment.Position = veinSegments[i].Position + (veinSegments[i].Velocity + nextEulers[i].Velocity) * 0.5f * timeStep;

                nextHeuns.Add(currentSegment);
            }

            // Adds the last one
            nextHeuns.Add(veinSegments[veinSegments.Count - 1]);

            // From the temp list to the main list
            for (int i = 0; i < veinSegments.Count; i++)
            {
                veinSegments[i] = nextHeuns[i];
            }

            //Implement maximum stretch to avoid numerical instabilities
            //May need to run the algorithm several times


            for (int i = 0; i < maximumStretchIterations; i++)
            {
                ImplementMaximumStretch(veinSegments);
            }
        }

        /// <summary>
        /// Calculate accelerations in each segment which is what is needed to get the next position and velocity
        /// </summary>
        /// <param name="veinSegments"></param>
        /// <returns></returns>
        private List<Vector3> CalculateAccelerations(List<VeinSegment> veinSegments)
        {
            List<Vector3> accelerations = new List<Vector3>();
            List<Vector3> forces = new List<Vector3>();

            for (int i = 0; i < veinSegments.Count - 1; i++)
            {
                Vector3 vectorBetween = veinSegments[i + 1].Position - veinSegments[i].Position;

                float distanceBetween = vectorBetween.magnitude;

                Vector3 dir = vectorBetween.normalized;

                float springForce = springConstant * (distanceBetween - veinSegmentLength);

                // Damping 
                float frictionForce = veinFriction * ((Vector3.Dot(veinSegments[i + 1].Velocity - veinSegments[i].Velocity, vectorBetween)) / distanceBetween);

                // Force on a spring
                Vector3 springForceVec = -(springForce + frictionForce) * dir;

                // It is negative because is looping from the bottom
                springForceVec = -springForceVec;

                forces.Add(springForceVec);
            }

            // Loop through all segments and calculate the acceleration
            for (int i = 0; i < veinSegments.Count - 1; i++)
            {
                Vector3 springForce = Vector3.zero;

                //Spring 1 - above
                springForce += forces[i];

                //Spring 2 - below
                //The first spring is at the bottom so it doesnt have a section below it
                if (i != 0)
                {
                    springForce -= forces[i - 1];
                }

                //Damping from air resistance, which depends on the square of the velocity
                float velocity = veinSegments[i].Velocity.magnitude;

                Vector3 dampingForce = airResistance * velocity * velocity * veinSegments[i].Velocity.normalized;

                // The mass attached to this spring
                float springMass = veinSectionMass;

                //end of the vein is attached to a box with a mass
                if (!areBothSidesStatic && i == 0)
                {

                    springMass += endPoint.GetComponent<Rigidbody>().mass;
                }

                // Force from gravity
                Vector3 gravityForce = springMass * Physics.gravity;

                // The total force on this spring
                Vector3 totalForce = springForce + gravityForce - dampingForce;

                // Calculate the acceleration a = F/m
                Vector3 acceleration = totalForce / springMass;

                accelerations.Add(acceleration);
            }

            // The acceleration of the last segment is always 0 because it is fixed
            accelerations.Add(Vector3.zero);

            return accelerations;
        }

        /// <summary>
        /// Implements maximum stretch to avoid numerical instabilities
        /// </summary>
        /// <param name="veinSegments"></param>
        private void ImplementMaximumStretch(List<VeinSegment> veinSegments)
        {
            // Makes sure each spring are not less compressed than 90% nor more stretched than 110%
            float maxStretch = 1.1f;
            float minStretch = 0.9f;

            // Loop from the end because it's better to adjust the top section of the vein before the bottom
            // And the top of the vein is at the end of the list
            for (int i = veinSegments.Count - 1; i > 0; i--)
            {
                VeinSegment topSegment = veinSegments[i];
                VeinSegment bottomSegment = veinSegments[i - 1];

                //The distance between the sections
                float dist = (topSegment.Position - bottomSegment.Position).magnitude;

                //What's the stretch/compression
                float stretch = dist / veinSegmentLength;

                if (stretch > maxStretch)
                {
                    // How far do compress the spring
                    float compressLength = dist - (veinSegmentLength * maxStretch);

                    // In what direction should we compress the spring?
                    Vector3 compressDir = (topSegment.Position - bottomSegment.Position).normalized;

                    Vector3 change = compressDir * compressLength;

                    MoveSegment(change, i - 1);
                }
                else if (stretch < minStretch)
                {
                    // How far do stretch the spring?
                    float stretchLength = (veinSegmentLength * minStretch) - dist;

                    // Direction to compress the spring
                    Vector3 stretchDir = (bottomSegment.Position - topSegment.Position).normalized;

                    Vector3 change = stretchDir * stretchLength;

                    MoveSegment(change, i - 1);
                }
            }
        }

        /// <summary>
        /// Moves a segment based on stretch/compression
        /// </summary>
        /// <param name="finalChange"></param>
        /// <param name="index"></param>
        private void MoveSegment(Vector3 finalChange, int index)
        {
            VeinSegment lastSegment = veinSegmentsList[index];

            // Moves the bottom section
            Vector3 position = lastSegment.Position;

            position += finalChange;

            lastSegment.Position = position;

            veinSegmentsList[index] = lastSegment;
        }

        /// <summary>
        /// Debug method: Compares the current length of the vein with the expected one
        /// </summary>
        private void VerifyVeinLength()
        {
            float currentLength = 0f;

            for (int iSegment = 1; iSegment < veinSegmentsList.Count; iSegment++)
            {
                currentLength += (veinSegmentsList[iSegment].Position - veinSegmentsList[iSegment - 1].Position).magnitude;
            }

            float expectedLength = veinSegmentLength * (float)(veinSegmentsList.Count - 1);

            //Debug.Log(name + " | Expected length: " + expectedLength + " Current: " + currentLength);
        }
        #endregion

    }
}