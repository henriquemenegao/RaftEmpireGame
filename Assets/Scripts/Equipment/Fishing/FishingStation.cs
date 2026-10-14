using Crew;
using Equipment.Core;
using Resources;
using UnityEngine;

namespace Equipment.Fishing
{
    public class FishingStation : Workstation
    {
        
        public override CrewActivity Activity => CrewActivity.Fishing;

        [Header("Produção")]
        [SerializeField] private float productionDuration = 20f;
        [SerializeField] private int fishPerCycle = 1;

        private ResourceInventory inventory;
        private float elapsedTime;
        private bool isProducing;

        public float Progress => Mathf.Clamp01(elapsedTime / Mathf.Max(0.01f, productionDuration));

        public bool IsProducing => isProducing;
        
        
        private void Awake()
        {
            inventory = FindAnyObjectByType<ResourceInventory>();

            if (inventory == null)
            {
                Debug.LogError(
                    "FishingStation: ResourceInventory não encontrado na cena."
                );
            }
        }

        

        public void Initialize(ResourceInventory resourceInventory)
        {
            inventory = resourceInventory;
        }

        public void StartProduction()
        {
            if (inventory == null)
            {
                Debug.LogWarning("FishingStation: inventário não configurado.");
                return;
            }

            isProducing = true;
        }

        public void StopProduction()
        {
            isProducing = false;
            elapsedTime = 0f;
        }

        private void Update()
        {
            if (!isProducing)
                return;

            elapsedTime += Time.deltaTime;

            if (elapsedTime >= productionDuration)
            {
                elapsedTime -= productionDuration;

                int producedAmount = fishPerCycle * Operator.Efficiency;
                inventory.Add(ResourceType.Fish, producedAmount);

                Debug.Log(
                    $"[FishingStation] {Operator.CrewName} produziu " +
                    $"{producedAmount} peixe(s)."
                );
            }
        }
        
        public override void AssignOperator(CrewMember member)
        {
            base.AssignOperator(member);
            StartProduction();
        }
        
        public override void RemoveOperator()
        {
            StopProduction();
            base.RemoveOperator();
        }


    }
}