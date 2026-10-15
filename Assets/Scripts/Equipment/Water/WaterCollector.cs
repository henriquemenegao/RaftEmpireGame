
using Crew;
using Equipment.Core;
using Resources;
using UnityEngine;

namespace Equipment.Water
{
    public class WaterCollector : Workstation
    {
        public override CrewActivity Activity =>
            CrewActivity.CollectingWater;

        [Header("Produção")]
        [SerializeField, Min(0.1f)]
        private float productionDuration = 20f;

        [SerializeField, Min(1)]
        private int waterPerCycle = 1;

        private ResourceInventory inventory;
        private float elapsedTime;
        private bool isProducing;

        public override int BaseProductionAmount => waterPerCycle;
        public override float ProductionDuration => productionDuration;

        public override  float Progress =>
            Mathf.Clamp01(
                elapsedTime / Mathf.Max(0.01f, productionDuration)
            );

        public override  bool IsProducing => isProducing;

        private void Awake()
        {
            inventory = FindAnyObjectByType<ResourceInventory>();

            if (inventory == null)
            {
                Debug.LogError(
                    "WaterCollector: ResourceInventory não encontrado."
                );
            }
        }

        public void StartProduction()
        {
            if (inventory == null || Operator == null)
            {
                Debug.LogWarning(
                    "WaterCollector: inventário ou operador indisponível."
                );
                return;
            }

            elapsedTime = 0f;
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

            if (elapsedTime >= productionDuration)
            {
                elapsedTime -= productionDuration;

                int producedAmount =
                    waterPerCycle * Operator.Efficiency;

                inventory.Add(ResourceType.Water, producedAmount);

                Debug.Log(
                    $"[WaterCollector] {Operator.CrewName} produziu " +
                    $"{producedAmount} água(s). " +
                    $"Total: {inventory.GetAmount(ResourceType.Water)}"
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
