using System.Collections.Generic;
using UnityEngine;

namespace FishingRodSystem
{
    public class VeinControllerBase : MonoBehaviour
    {
        [Header("Line renderer which is used to visualize the vein")]
        [SerializeField] protected LineRenderer lineRenderer;

        [Header("Two ends of the vein")]
        [SerializeField] protected Transform startPoint;
        [SerializeField] protected Transform endPoint;

        [Range(.1f, 10f)]
        [SerializeField] protected float veinSegmentLength = 1f;

        [Range(.001f, 10f)]
        [SerializeField] protected float veinThickness = .1f;

        [SerializeField] protected int maximumStretchIterations = 2;

        [Header("List including all of the vein segments")]
        [SerializeField] protected List<VeinSegment> veinSegmentsList = new List<VeinSegment>();
    }
}
