# Harvest Grove

A small 2D farming prototype built with Unity and C#. Plant crops, water plots,
wait for growth and harvest to earn money for more crops and land.

## Features

- Five crop types configured with ScriptableObjects: sprites, growth intervals and prices.
- Watering, staged growth and fertilizer that increases growth speed up to 2×.
- Crop selection, a store and a shared balance for planting and plot purchases.
- Isometric plots, sprite interaction and TextMesh Pro UI.

## Run

1. Clone this repository.
2. Install **Unity 2021.3.45f2** through Unity Hub, matching
   `ProjectSettings/ProjectVersion.txt`.
3. Add the repository folder as a project and let Unity restore its packages.
4. Open `Assets/Scenes/SampleScene.unity` and enter Play mode.

Use the store's **Buy** button to select a crop, then click an owned empty plot.
Select the water, fertilizer or plot tool and click a plot to use it. Selecting
an active crop or tool again cancels it. With no selection, click a mature crop
to harvest.

Fertilizer costs $10 per application, with at most five applications per crop.
It only applies to growing crops. Additional plots cost $100. Game state is
kept in memory and resets when Play mode stops; there is no save system.

## Tests

The growth and balance rules are tested independently of Unity. In the editor,
open **Window → General → Test Runner** and run the EditMode tests.

The same tests can run without the editor using the **.NET 10 SDK**:

```sh
dotnet restore Tests/Gameplay/Gameplay.Tests.csproj --locked-mode
dotnet test Tests/Gameplay/Gameplay.Tests.csproj --no-restore
```

GitHub Actions runs these rules tests. They cover watering, growth timing,
fertilizer limits, affordability and balance bounds; they do not validate
Unity rendering, scene import or input. See [validation notes](docs/VALIDATION.md).

## Structure

`Assets/Scripts` contains scene components. `Gameplay` contains engine-independent
growth and economy rules. Crop definitions are in `Assets/Resources/Plants`;
scene and UI assets are under `Assets/Scenes` and `Assets/Prefabs`.

Lilita One and VT323 font licenses are included in [Assets/Fonts](Assets/Fonts).
