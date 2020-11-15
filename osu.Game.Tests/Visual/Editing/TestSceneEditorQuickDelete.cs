// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using NUnit.Framework;
using osu.Framework.Testing;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Osu.Edit.Blueprints.HitCircles;
using osu.Game.Rulesets.Osu.Edit.Blueprints.HitCircles.Components;
using osu.Game.Rulesets.Osu.Edit.Blueprints.Sliders;
using osu.Game.Tests.Beatmaps;
using osu.Game.Rulesets.Edit;
using osu.Game.Screens.Edit.Compose;
using osu.Game.Screens.Edit.Compose.Components;
using osuTK;
using osuTK.Input;

namespace osu.Game.Tests.Visual.Editing
{
    public class TestSceneEditorQuickDelete : EditorTestScene
    {
        protected override Ruleset CreateEditorRuleset() => new OsuRuleset();

        protected override IBeatmap CreateBeatmap(RulesetInfo ruleset) => new TestBeatmap(ruleset, false);

        [Test]
        public void TestQuickDeleteRemovesObject()
        {
            var addedObject = new HitCircle { StartTime = 1000 };

            AddStep("add hitobject", () => EditorBeatmap.Add(addedObject));

            AddStep("select added object", () => EditorBeatmap.SelectedHitObjects.Add(addedObject));

            AddStep("move mouse to object", () =>
            {
                var pos = getBlueprintContainer.SelectionBlueprints
                            .ChildrenOfType<HitCircleSelectionBlueprint>().First()
                            .ChildrenOfType<HitCirclePiece>().First().ScreenSpaceDrawQuad.Centre;
                InputManager.MoveMouseTo(pos);
            });
            AddStep("hold shift", () => InputManager.PressKey(Key.ShiftLeft));
            AddStep("rightclick", () => InputManager.Click(MouseButton.Right));
            AddStep("release shift", () => InputManager.ReleaseKey(Key.ShiftLeft));

            AddAssert("no hitobjects in beatmap", () => EditorBeatmap.HitObjects.Count == 0);
        }

        [Test]
        public void TestQuickDeleteRemovesSliderControlPoint()
        {
            Slider slider = new Slider { StartTime = 1000 };

            PathControlPoint[] points = new PathControlPoint[]
            {
                new PathControlPoint(),
                new PathControlPoint(new Vector2(50, 0)),
                new PathControlPoint(new Vector2(100, 0))
            };

            AddStep("add slider", () =>
            {
                slider.Path = new SliderPath(points);
                EditorBeatmap.Add(slider);
            });

            AddStep("select added slider", () => EditorBeatmap.SelectedHitObjects.Add(slider));

            AddStep("move mouse to controlpoint", () =>
            {
                // This doesn't get the HitCirclePiece corresponding to the last control point on consecutive runs,
                // causing the slider to translate by 50 every time and go off the screen after about 10 runs.
                var pos = getBlueprintContainer.SelectionBlueprints
                            .ChildrenOfType<SliderSelectionBlueprint>().First()
                            .ChildrenOfType<HitCirclePiece>().Last().ScreenSpaceDrawQuad.Centre;
                InputManager.MoveMouseTo(pos);
            });
            AddStep("hold shift", () => InputManager.PressKey(Key.ShiftLeft));
            AddStep("rightclick", () => InputManager.Click(MouseButton.Right));
            AddStep("release shift", () => InputManager.ReleaseKey(Key.ShiftLeft));

            AddAssert("slider has 2 points", () => slider.Path.ControlPoints.Count == 2);
        }

        private BlueprintContainer getBlueprintContainer => Editor.ChildrenOfType<ComposeScreen>().First()
                                                                  .ChildrenOfType<HitObjectComposer>().First()
                                                                  .ChildrenOfType<BlueprintContainer>().First();
    }
}
