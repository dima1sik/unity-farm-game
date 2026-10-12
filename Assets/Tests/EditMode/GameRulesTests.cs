using System;
using HarvestGrove.Gameplay;
using NUnit.Framework;

public class GameRulesTests
{
    [Test]
    public void DryCropDoesNotAccumulateGrowthTime()
    {
        var crop = new CropGrowth(4, 2f);
        Assert.That(crop.Advance(100f), Is.False);
        crop.Water();
        Assert.That(crop.Advance(1f), Is.False);
        Assert.That(crop.Stage, Is.EqualTo(0));
        Assert.That(crop.Advance(1f), Is.True);
        Assert.That(crop.Stage, Is.EqualTo(1));
    }

    [Test]
    public void GrowthPreservesTimeAcrossStageBoundaries()
    {
        var crop = new CropGrowth(4, 2f);
        crop.Water();
        crop.Advance(2.5f);
        Assert.That(crop.Stage, Is.EqualTo(1));
        crop.Advance(1.5f);
        Assert.That(crop.Stage, Is.EqualTo(2));
    }

    [Test]
    public void LongFrameCanAdvanceMultipleStagesAndStopsAtMaturity()
    {
        var crop = new CropGrowth(4, 2f);
        crop.Water();
        Assert.That(crop.Advance(100f), Is.True);
        Assert.That(crop.Stage, Is.EqualTo(3));
        Assert.That(crop.IsMature, Is.True);
        Assert.That(crop.Advance(100f), Is.False);
    }

    [Test]
    public void FramePartitionsProduceTheSameGrowth()
    {
        var singleFrame = new CropGrowth(5, 2f);
        var manyFrames = new CropGrowth(5, 2f);
        singleFrame.Water();
        manyFrames.Water();
        singleFrame.Advance(6f);
        for (int frame = 0; frame < 24; frame++)
            manyFrames.Advance(0.25f);
        Assert.That(manyFrames.Stage, Is.EqualTo(singleFrame.Stage));
    }

    [Test]
    public void FertilizerCapsAtDoubleSpeedWithoutAcceptingAnotherApplication()
    {
        var crop = new CropGrowth(4, 2f);
        for (int application = 0; application < CropGrowth.MaximumFertilizerApplications; application++)
            Assert.That(crop.ApplyFertilizer(), Is.True);
        Assert.That(crop.Speed, Is.EqualTo(2f));
        Assert.That(crop.CanFertilize, Is.False);
        Assert.That(crop.ApplyFertilizer(), Is.False);
        crop.Water();
        crop.Advance(1f);
        Assert.That(crop.Stage, Is.EqualTo(1));
    }

    [Test]
    public void MatureCropDoesNotAcceptFertilizer()
    {
        var crop = new CropGrowth(2, 2f);
        crop.Water();
        crop.Advance(2f);
        Assert.That(crop.CanFertilize, Is.False);
        Assert.That(crop.ApplyFertilizer(), Is.False);
    }

    [Test]
    public void NewCropStartsDryAndUnfertilized()
    {
        var crop = new CropGrowth(4, 2f);
        crop.Water();
        crop.ApplyFertilizer();
        crop.Advance(100f);
        var nextCrop = new CropGrowth(4, 2f);
        Assert.That(nextCrop.IsWatered, Is.False);
        Assert.That(nextCrop.Stage, Is.EqualTo(0));
        Assert.That(nextCrop.Speed, Is.EqualTo(1f));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void CropRejectsMissingStages(int stages)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CropGrowth(stages, 2f));
    }

    [TestCase(0f)]
    [TestCase(-1f)]
    [TestCase(float.NaN)]
    [TestCase(float.PositiveInfinity)]
    public void CropRejectsInvalidGrowthDuration(float seconds)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CropGrowth(4, seconds));
    }

    [TestCase(-1f)]
    [TestCase(float.NaN)]
    [TestCase(float.PositiveInfinity)]
    public void CropRejectsInvalidFrameTime(float deltaTime)
    {
        var crop = new CropGrowth(4, 2f);
        Assert.Throws<ArgumentOutOfRangeException>(() => crop.Advance(deltaTime));
    }

    [TestCase(100, -100, 0)]
    [TestCase(0, 10, 10)]
    [TestCase(10, -5, 5)]
    public void TransactionAcceptsValidBalanceChanges(int balance, int amount, int expected)
    {
        Assert.That(FarmEconomy.TryTransaction(balance, amount, out int updated), Is.True);
        Assert.That(updated, Is.EqualTo(expected));
    }

    [TestCase(5, -10)]
    [TestCase(int.MaxValue, 1)]
    [TestCase(0, int.MinValue)]
    [TestCase(-1, 10)]
    public void RejectedTransactionPreservesBalance(int balance, int amount)
    {
        Assert.That(FarmEconomy.TryTransaction(balance, amount, out int updated), Is.False);
        Assert.That(updated, Is.EqualTo(balance));
    }

    [TestCase(100, 100, true)]
    [TestCase(99, 100, false)]
    [TestCase(100, -1, false)]
    [TestCase(0, 0, true)]
    public void AffordabilityRejectsNegativePrices(int balance, int price, bool expected)
    {
        Assert.That(FarmEconomy.CanAfford(balance, price), Is.EqualTo(expected));
    }
}
