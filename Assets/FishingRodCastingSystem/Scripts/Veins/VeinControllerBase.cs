using System.Collections.Generic;
using UnityEngine;

namespace FishingRodSystem
{
    public abstract class VeinControllerBase : MonoBehaviour
    {
        [Header("Line renderer which is used to visualize the vein")]
        [SerializeField] protected LineRenderer lineRenderer;

        [Header("Two ends of the vein")]
        [SerializeField] protected Transform startPoint;
        [SerializeField] protected Transform endPoint;

        [Header("Parameters to tweak")]
        [SerializeField] protected bool areBothSidesStatic = false;

        [Range(.001f, 10f)]
        [SerializeField] protected float veinSegmentLength = .05f; 

        [Range(.001f, 10f)]
        [SerializeField] protected float veinThickness = .002f;


        [Range(1, 100)]
        [SerializeField] protected int maximumStretchIterations = 10;

        [Range(1, 100)]
        [SerializeField] protected int simulationIterationNumber = 10;

        [Header("List including all of the vein segments")]
        [SerializeField] protected List<VeinSegment> veinSegmentsList = new List<VeinSegment>();

        private void Awake()
        {
            if (lineRenderer == null)
            {
                lineRenderer = GetComponentInChildren<LineRenderer>();
            }
        }

        protected abstract void CreateVein();
        protected abstract void UpdateVein();
        protected abstract void DrawVein();

    }
}
