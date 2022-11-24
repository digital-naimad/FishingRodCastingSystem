using System.Collections.Generic;
using UnityEngine;

namespace FishingRodSystem
{
    public class VeinControllerDummy : VeinControllerBase
    {
        [SerializeField] private List<Transform> grommets = new List<Transform>();

        // Start is called before the first frame update
        void Start()
        {
            CreateVein();
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < simulationIterationNumber; i++)
            {
                UpdateVein();
                DrawVein();
            }
        }

        /// <summary>
        /// Inits the vein segments list
        /// </summary>
        protected override void CreateVein()
        {
            veinSegmentsList.Clear();

            veinSegmentsList.Add(new VeinSegment(startPoint.position));

            for (int i = 0; i < grommets.Count; i++)
            {
                veinSegmentsList.Add(new VeinSegment(grommets[i].position));
            }

            veinSegmentsList.Add(new VeinSegment(endPoint.position));
        }

        /// <summary>
        /// TODO: replace duplicated method with base one
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

        protected override void UpdateVein()
        {
            CreateVein();
        }
    }
}
