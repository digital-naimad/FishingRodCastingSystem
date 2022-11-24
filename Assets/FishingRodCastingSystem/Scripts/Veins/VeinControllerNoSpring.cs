using UnityEngine;

namespace FishingRodSystem
{
    /// <summary>
    /// Simulates  the vein with Verlet integration and without using any springs
    /// </summary>
    public class VeinControllerNoSpring : VeinControllerBase
    {
        #region MonoBehaviour's callbacks
        private void Start()
        {
            CreateVein();

            //maximumStretchIterations = 20; // TODO: remove hardcoded value
        }

        private void Update()
        {
            UpdateVein();
            DrawVein();

            if (!areBothSidesStatic)
            {
                // Moves the attachement to the end of the vein
                endPoint.position = veinSegmentsList[veinSegmentsList.Count - 1].Position;
                endPoint.LookAt(veinSegmentsList[veinSegmentsList.Count - 2].Position);
            }

            
        }

        private void FixedUpdate()
        {
            
        }

        #endregion

        #region Override methods

        protected override void CreateVein()
        {
            Vector3 segmentPosition = startPoint.position;

            for (int i = 0; i < 15; i++)
            {
                veinSegmentsList.Add(new VeinSegment(segmentPosition));

                segmentPosition.y -= veinSegmentLength;
            }
        }

        protected override void UpdateVein()
        {
            float deltaTime = Time.fixedDeltaTime;

            // Moves the first segment to the start position
            VeinSegment firstSegment = veinSegmentsList[0];
            firstSegment.Position = startPoint.position;

            veinSegmentsList[0] = firstSegment;

            // Moves the other segment with Verlet integration
            for (int i = 1; i < veinSegmentsList.Count; i++)
            {
                VeinSegment currentSegment = veinSegmentsList[i];

                //Calculate velocity this update
                Vector3 vel = currentSegment.Position - currentSegment.OldPosition;

                //Update the old position with the current position
                currentSegment.OldPosition = currentSegment.Position;

                //Find the new position
                currentSegment.Position += vel;

                //Adds gravity
                currentSegment.Position += Physics.gravity * deltaTime;

                //Add it back to the array
                veinSegmentsList[i] = currentSegment;
            }


            // Makes sure the vein segments have the correct lengths
            for (int i = 0; i < maximumStretchIterations; i++)
            {
                ImplementMaximumStretch();
            }
        }

       
        /// <summary>
        /// Display the vein with a line renderer
        /// </summary>
        protected override void DrawVein()
        {
            lineRenderer.startWidth = veinThickness;
            lineRenderer.endWidth = veinThickness;

            // Vein segments positions
            Vector3[] positions = new Vector3[veinSegmentsList.Count];

            for (int i = 0; i < veinSegmentsList.Count; i++)
            {
                positions[i] = veinSegmentsList[i].Position;
            }

            lineRenderer.positionCount = positions.Length;
            lineRenderer.SetPositions(positions);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Provides the vein segments have the correct lengths
        /// </summary>
        private void ImplementMaximumStretch()
        {
            for (int i = 0; i < veinSegmentsList.Count - 1; i++)
            {
                VeinSegment topSection = veinSegmentsList[i];

                VeinSegment bottomSection = veinSegmentsList[i + 1];

                // The distance between the sections
                float dist = (topSection.Position - bottomSection.Position).magnitude;

                //What's the stretch/compression
                float distError = Mathf.Abs(dist - veinSegmentLength);

                Vector3 changeDir = Vector3.zero;

                // Compress this segments
                if (dist > veinSegmentLength)
                {
                    changeDir = (topSection.Position - bottomSection.Position).normalized;
                }
                else if (dist < veinSegmentLength) // Extends this section
                {
                    changeDir = (bottomSection.Position - topSection.Position).normalized;
                }
                else // Idling
                {
                    continue;
                }


                Vector3 change = changeDir * distError;

                if (i != 0)
                {
                    bottomSection.Position += change * 0.5f;

                    veinSegmentsList[i + 1] = bottomSection;

                    topSection.Position -= change * 0.5f;

                    veinSegmentsList[i] = topSection;
                }
                else  // The vein is connected
                {
                    bottomSection.Position += change;

                    veinSegmentsList[i + 1] = bottomSection;
                }
            }
        }
        #endregion


    }
}