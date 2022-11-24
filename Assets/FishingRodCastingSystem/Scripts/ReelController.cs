using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace FishingRodSystem
{
    public class ReelController : MonoBehaviour
    {
        [SerializeField] private Animator reelAnimator;

        private void Awake()
        {
            reelAnimator = GetComponent<Animator>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (reelAnimator != null)
            {
                if (Input.GetKeyDown(KeyCode.O))
                {
                    //reelAnimator.SetTrigger(nameof(ReelParameter.SpinForwardTrigger));

                    reelAnimator.SetBool(nameof(ReelParameter.IsSpinning), true);
                    reelAnimator.SetBool(nameof(ReelParameter.IsSpinningForward), true);
                }

                if (Input.GetKeyDown(KeyCode.L))
                {
                    //reelAnimator.SetTrigger(nameof(ReelParameter.SpinBackwardTrigger));

                    reelAnimator.SetBool(nameof(ReelParameter.IsSpinning), true);
                    reelAnimator.SetBool(nameof(ReelParameter.IsSpinningForward), false);
                }

                if (Input.GetKeyUp(KeyCode.O) || Input.GetKeyUp(KeyCode.L))
                {
                    reelAnimator.SetBool(nameof(ReelParameter.IsSpinning), false);
                }
            }
        }
    }
}
