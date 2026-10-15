using UnityEngine;

namespace Crew
{
    public class CrewController : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        public CrewActivity CurrentActivity { get; private set; } = CrewActivity.Idle;

        public void StartActivity(
            CrewActivity activity,
            Transform interactionPoint)
        {
            if (animator == null || interactionPoint == null)
            {
                Debug.LogWarning(
                    "CrewController: referências não configuradas."
                );
                return;
            }

            transform.SetPositionAndRotation(
                interactionPoint.position,
                interactionPoint.rotation
            );

            CurrentActivity = activity;

            switch (activity)
            {
                case CrewActivity.Fishing:
                    animator.SetTrigger("StartFishing");
                    break;

                case CrewActivity.CollectingWater:
                    animator.SetTrigger("StartCollectingWater");
                    break;

                case CrewActivity.Idle:
                    StopActivity();
                    break;
            }
        }

        public void StopActivity()
        {
            if (animator == null)
                return;

            animator.ResetTrigger("StartFishing");
            animator.ResetTrigger("StartCollectingWater");
            animator.CrossFade("Idle", 0.05f, 0);

            CurrentActivity = CrewActivity.Idle;
        }
    }
}