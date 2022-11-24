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
#if UNITY_EDITOR
            if (reelAnimator != null)
            {

                if (Input.GetKeyDown(KeyCode.O))
                {
                    SpinForward();
                }

                if (Input.GetKeyDown(KeyCode.L))
                {
                    SpinBackward();
                }

                if (Input.GetKeyUp(KeyCode.O) || Input.GetKeyUp(KeyCode.L))
                {
                    StopSpinning();
                }
            }
#endif
        }

        #region Public methods
        public void SpinForward()
        {
            if (reelAnimator != null)
            {
                //reelAnimator.SetTrigger(nameof(ReelParameter.SpinForwardTrigger));

                reelAnimator.SetBool(nameof(ReelParameter.IsSpinning), true);
                reelAnimator.SetBool(nameof(ReelParameter.IsSpinningForward), true);
            }
        }

        public void SpinBackward()
        {
            //reelAnimator.SetTrigger(nameof(ReelParameter.SpinBackwardTrigger));
            if (reelAnimator != null)
            {
                reelAnimator.SetBool(nameof(ReelParameter.IsSpinning), true);
                reelAnimator.SetBool(nameof(ReelParameter.IsSpinningForward), false);
            }
        }

        public void StopSpinning()
        {
            if (reelAnimator != null)
                reelAnimator.SetBool(nameof(ReelParameter.IsSpinning), false);
        }
        #endregion
    }
}
