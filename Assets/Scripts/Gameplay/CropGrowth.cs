using System;

namespace HarvestGrove.Gameplay
{
    // Pure crop timing rules, independent of rendering and frame rate.
    public sealed class CropGrowth
    {
        public const int MaximumFertilizerApplications = 5;

        private readonly int stageCount;
        private readonly double secondsPerStage;
        private double elapsed;
        private int fertilizerApplications;

        public int Stage { get; private set; }
        public bool IsWatered { get; private set; }
        public bool IsMature => Stage == stageCount - 1;
        public bool CanFertilize => !IsMature && fertilizerApplications < MaximumFertilizerApplications;
        public float Speed => 1f + fertilizerApplications * 0.2f;

        public CropGrowth(int stageCount, float secondsPerStage)
        {
            if (stageCount < 1)
                throw new ArgumentOutOfRangeException(nameof(stageCount));
            if (secondsPerStage <= 0f || float.IsNaN(secondsPerStage) || float.IsInfinity(secondsPerStage))
                throw new ArgumentOutOfRangeException(nameof(secondsPerStage));

            this.stageCount = stageCount;
            this.secondsPerStage = secondsPerStage;
        }

        public void Water()
        {
            IsWatered = true;
        }

        public bool ApplyFertilizer()
        {
            if (!CanFertilize)
                return false;

            fertilizerApplications++;
            return true;
        }

        public bool Advance(float deltaTime)
        {
            if (deltaTime < 0f || float.IsNaN(deltaTime) || float.IsInfinity(deltaTime))
                throw new ArgumentOutOfRangeException(nameof(deltaTime));
            if (!IsWatered || IsMature || deltaTime == 0f)
                return false;

            int previousStage = Stage;
            elapsed += (double)deltaTime * Speed;
            while (elapsed >= secondsPerStage && !IsMature)
            {
                elapsed -= secondsPerStage;
                Stage++;
            }

            return Stage != previousStage;
        }
    }
}
