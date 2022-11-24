using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace FishingRodSystem
{

    public class RodController : MonoBehaviour
    {
        [Header("Raycasting")]
        [SerializeField] private Camera fppCamera;
        [SerializeField] private float raycastRange = 100f;
        [SerializeField] private GameObject raycastHitMarker;
        [SerializeField] private float hitMarkerLifetime = 2.0f;
        [SerializeField] private LayerMask raycastingLayer;

        [Header("Reel & veins")]
        [SerializeField] private ReelController reelController;

        [SerializeField] private VeinControllerRealistic veinController;

        private void Awake()
        {
            if (reelController == null)
            {
                reelController = GetComponentInChildren<ReelController>();
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            ProcessInputs();
        }

        private void ProcessInputs()
        {
            // left mouse button
            if (Input.GetButtonDown("Fire1"))
            {
                CastRod();

            }

            /*
            // middle button of the mouse
            if (Input.GetButtonDown("Fire3"))
            {
                reelController.StopSpinning();
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
            {
                reelController.SpinForward();
                veinController.UnwindVein();
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
            {
                reelController.SpinBackward();
                veinController.WindVein();
            }
            else // wheel == 0
            {
                reelController.StopSpinning();
            }
            */

            
            //if (Input.mouseScrollDelta.y != 0)
            {
                //Debug.Log(name + " | scroll delta y" + Input.mouseScrollDelta.y);

                if (Input.mouseScrollDelta.y < 0)
                {
                   
                    reelController.SpinBackward();
                    veinController.WindVein();

                }
                else
                {
                    reelController.StopSpinning();
                }

            }
            //else //if (Input.mouseScrollDelta.y == 0)
            {
               
            }
            
        }

        private void CastRod()
        {
            // TODO: animate casting rod 

            Debug.Log(name + " | Cast rode");

            DoRaycast();
            

        }

        private void DoRaycast()
        {
            RaycastHit hit;

            bool isHitAnything = Physics.Raycast(fppCamera.transform.position, fppCamera.transform.forward, out hit, raycastRange, raycastingLayer);

            if (isHitAnything)
            {
                Debug.Log(name + " | Raycast hit: " + hit.transform.name);

                //if (hit.collider.gameObject.layer == raycastingLayer)
                {
                    //Debug.Log(name + " | Raycast hit layer: " + raycastingLayer);
                }
                //raycastingLayer

                GameObject hitMarker = Instantiate(raycastHitMarker, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(hitMarker, hitMarkerLifetime);


            }
        }


    }
}
