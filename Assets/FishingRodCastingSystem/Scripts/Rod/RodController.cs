using System.Collections;
using System.Collections.Generic;
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

        [Header("Reel")]
        [SerializeField] private ReelController reelController;

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
            // left mouse button
            if (Input.GetButtonDown("Fire1")) 
            {
                CastRod();

            }

            // middle button of the mouse
            if (Input.GetButtonDown("Fire3")) 
            {
                reelController.StopSpinning();
            }

            if (Input.mouseScrollDelta.y != 0)
            {
                Debug.Log(name + " | scroll delta y" + Input.mouseScrollDelta.y);

                if (Input.mouseScrollDelta.y > 0)
                {
                    reelController.SpinForward();
                }
                else
                {
                    reelController.SpinBackward();
                }

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
