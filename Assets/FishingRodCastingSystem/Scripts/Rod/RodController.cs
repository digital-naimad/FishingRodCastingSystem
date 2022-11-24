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

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                CastRode();
            }
        }

        private void CastRode()
        {
            Debug.Log(name + " | Cast rode");

            RaycastHit hit;

            bool isHitAnything = Physics.Raycast(fppCamera.transform.position, fppCamera.transform.forward, out hit, raycastRange);

            if (isHitAnything)
            {
                Debug.Log(name + " | Raycast hit: " + hit.transform.name);

                GameObject hitMarker = Instantiate(raycastHitMarker, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(hitMarker, hitMarkerLifetime);
            }

        }
    }
}
