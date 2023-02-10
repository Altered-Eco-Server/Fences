namespace Eco.Mods.TechTree
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Eco.Core.Items;
    using Eco.Gameplay.Blocks;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Components.Auth;
    using Eco.Gameplay.DynamicValues;
    using Eco.Gameplay.Economy;
    using Eco.Gameplay.Housing;
    using Eco.Gameplay.Interactions;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Modules;
    using Eco.Gameplay.Minimap;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Players;
    using Eco.Gameplay.Property;
    using Eco.Gameplay.Skills;
    using Eco.Gameplay.Systems.TextLinks;
    using Eco.Gameplay.Pipes.LiquidComponents;
    using Eco.Gameplay.Pipes.Gases;
    using Eco.Gameplay.Systems.Tooltip;
    using Eco.Shared;
    using Eco.Shared.Math;
    using Eco.Shared.Localization;
    using Eco.Shared.Serialization;
    using Eco.Shared.Utils;
    using Eco.Shared.View;
    using Eco.Shared.Items;
    using Eco.Gameplay.Pipes;
    using Eco.World.Blocks;
    using Eco.Gameplay.Housing.PropertyValues;
    using static Eco.Gameplay.Housing.PropertyValues.HomeFurnishingValue;

    [Serialized]
    [RequireComponent(typeof(SolidAttachedSurfaceRequirementComponent))]
    public partial class FenceEdgeObject : WorldObject, IRepresentsItem
    {
        public override LocString DisplayName { get { return Localizer.DoStr("Chainlink Fence Edge"); } }
        public virtual Type RepresentedItemType { get { return typeof(FenceEdgeItem); } }

        protected override void Initialize()
        {
            this.ModsPreInitialize();
            this.ModsPostInitialize();
        }

        /// <summary>Hook for mods to customize WorldObject before initialization. You can change housing values here.</summary>
        partial void ModsPreInitialize();
        /// <summary>Hook for mods to customize WorldObject after initialization.</summary>
        partial void ModsPostInitialize();
    }

    [Serialized]
    [LocDisplayName("Chainlink Fence Edge")]
    public partial class FenceEdgeItem : WorldObjectItem<FenceEdgeObject>
    {
        public override LocString DisplayDescription => Localizer.DoStr("Right click icon to change edge/center variations");
        public override DirectionAxisFlags RequiresSurfaceOnSides { get; } = 0 | DirectionAxisFlags.Down;

        public override string OnUsed(Player player, ItemStack itemStack)
        {
            var user = player.User;
            user.Inventory.TryRemoveItem<FenceEdgeItem>(user);
            user.Inventory.TryAddItem<FenceCenterItem>(user);
            user.Player.InfoBox(new LocString("Chainlink Fence switched to Center."));
            return base.OnUsed(player, itemStack);
        }
    }

    [RequiresSkill(typeof(SmeltingSkill), 6)]
    public partial class FenceEdgeRecipe : RecipeFamily
    {
        public FenceEdgeRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                "ChainlinkFenceEdge",  //noloc
                Localizer.DoStr("Chainlink Fence Edge"),
                new List<IngredientElement>
                {
                    new IngredientElement(typeof(IronBarItem), 8, typeof(SmeltingSkill), typeof(SmeltingLavishResourcesTalent)),
                },
                new List<CraftingElement>
                {
                    new CraftingElement<FenceEdgeItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 2;
            this.LaborInCalories = CreateLaborInCaloriesValue(100, typeof(SmeltingSkill));
            this.CraftMinutes = CreateCraftTimeValue(typeof(FenceEdgeRecipe), 1.5f, typeof(SmeltingSkill), typeof(SmeltingFocusedSpeedTalent), typeof(SmeltingParallelSpeedTalent));
            this.ModsPreInitialize();
            this.Initialize(Localizer.DoStr("Chainlink Fence Edge"), typeof(FenceEdgeRecipe));
            this.ModsPostInitialize();
            CraftingComponent.AddRecipe(typeof(AnvilObject), this);
        }

        /// <summary>Hook for mods to customize RecipeFamily before initialization. You can change recipes, xp, labor, time here.</summary>
        partial void ModsPreInitialize();
        /// <summary>Hook for mods to customize RecipeFamily after initialization, but before registration. You can change skill requirements here.</summary>
        partial void ModsPostInitialize();
    }
}
