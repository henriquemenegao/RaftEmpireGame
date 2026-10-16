using Crew;
using Equipment.Core;
using Resources;
using UnityEngine;

namespace Equipment.Fishing
{
    public class FishingStation : Workstation
    {
        public override CrewActivity Activity => CrewActivity.Fishing;

        private ResourceInventory inventory;
        private float elapsedTime;
        private bool isProducing;

        public override float Progress =>
            Mathf.Clamp01(
                elapsedTime / Mathf.Max(0.01f, ProductionDuration)
            );

        public override bool IsProducing => isProducing;

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
            if (inventory == null || Operator == null)
            {
                Debug.LogWarning(
                    "FishingStation: inventário ou operador indisponível."
                );
                return;
            }

            if (ProductionDuration <= 0f || BaseProductionAmount <= 0)
            {
                Debug.LogWarning(
                    "FishingStation: configuração de produção inválida."
                );
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
            if (!isProducing || Operator == null)
                return;

            elapsedTime += Time.deltaTime;
            if (elapsedTime >= ProductionDuration)
            {
                elapsedTime -= ProductionDuration;
                int producedAmount = BaseProductionAmount * Operator.Efficiency;
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