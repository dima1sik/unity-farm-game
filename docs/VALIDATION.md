# Validation

## Automated coverage

`GameRulesTests` runs as Unity EditMode tests and as a standalone .NET test project
using the same gameplay source files. It verifies:

- Dry crops do not grow or accumulate elapsed growth time.
- Stage transitions include their exact time boundary and retain excess time.
- Long frames can advance multiple stages without exceeding maturity.
- Fertilizer caps at 2× speed and mature crops reject it.
- New crops start dry and without fertilizer.
- Invalid stage counts, timing values and negative prices are rejected.
- Transactions cannot make balances negative or overflow, and rejected
  transactions preserve the balance.

The standalone test run is not a Unity build. Unity-dependent scene components
must also be checked in the pinned editor.

## Unity acceptance checks

Open `Assets/Scenes/SampleScene.unity` in Unity 2021.3.45f2 and verify:

1. The scene and all five crop items load without Console errors.
2. Crop and tool selection toggle correctly, and changing selection clears the
   previous button state.
3. Planting charges once, rejects unaffordable crops and cannot replace a crop
   already planted on a plot.
4. A dry crop stays at its first stage; watering allows it to mature.
5. Fertilizer costs $10 for each of five applications. Empty plots, mature crops
   and crops at the cap do not consume money.
6. Clicking a mature crop's sprite or its plot harvests it once, pays its sale
   price and resets the plot to dry, unfertilized soil.
7. Purchasing an unowned plot charges $100 once. An insufficient balance leaves
   it unowned.
8. Store buttons and scrolling do not also interact with plots behind the UI;
   hover highlighting matches whether the action is available.
9. Build and run a desktop player through **File → Build Settings** with the
   included scene enabled. Runtime scripts must not reference `UnityEditor`.

The cloud audit ran all 27 standalone cases successfully and checked asset metadata,
crop definitions and package configuration. Unity Editor and an activated Unity
license were unavailable in that environment, so editor tests, scene execution
and player builds have not been validated there. Package versions were preserved;
resolve and test any Unity/editor package upgrade separately.
