namespace Eco.Mods.TechTree
{
    using System;
    using System.Threading;
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
    using Eco.Core.Utils;
    using Eco.Gameplay.Audio;
    using Eco.Gameplay.Auth;
    using Eco.Gameplay.GameActions;
    using Eco.Shared.IoC;

    [Serialized]
    [RequireComponent(typeof(PropertyAuthComponent))]
    [RequireComponent(typeof(SolidAttachedSurfaceRequirementComponent))]
    public partial class FenceGateCenterObject : DoorObject, IRepresentsItem
    {
        [Serialized] public bool Door = true;
        public override LocString DisplayName { get { return Localizer.DoStr("Chainlink Gate Center Left"); } }
        public override TableTextureMode TableTexture => TableTextureMode.Stone;
        public virtual Type RepresentedItemType { get { return typeof(FenceGateCenterItem); } }
        public override bool HasTier { get { return true; } }
        public override int Tier { get { return 4; } }

        protected override void Initialize()
        {
            this.ModsPreInitialize();
            base.Initialize();

            this.ModsPostInitialize();
        }

        public override InteractResult OnActRight(InteractionContext context)
        {
            if (ServiceHolder<IAuthManager>.Obj.IsAuthorized(context, AccessType.ConsumerAccess, (GameAction)null))
            {
                Door = !Door;
                return InteractResult.Success;
            }
            context.Player.ErrorLocStr("You Are Not Authorized To Do That");
            return InteractResult.Fail;
        }

        public override void Tick()
        {
            base.Tick();
            SetAnimatedState("gateswing", this.Operating && Door);
        }

        /// <summary>Hook for mods to customize WorldObject before initialization. You can change housing values here.</summary>
        partial void ModsPreInitialize();
        /// <summary>Hook for mods to customize WorldObject after initialization.</summary>
        partial void ModsPostInitialize();
    }

    [Serialized]
    [LocDisplayName("Chainlink Gate Center Left")]
    [Tier(4)]
    [Ecopedia("Housing Objects", "Doors", createAsSubPage: true)]
    public partial class FenceGateCenterItem : WorldObjectItem<FenceGateCenterObject>
    {
        public override LocString DisplayDescription => Localizer.DoStr("Right click icon to change left/right variations");
        public override DirectionAxisFlags RequiresSurfaceOnSides { get; } = 0 | DirectionAxisFlags.Down;
        public override string OnUsed(Player player, ItemStack itemStack)
        {
            var user = player.User;

            if (user.Inventory.TryRemoveItem<FenceGateCenterItem>(user))
            {
                user.Inventory.TryAddItem<FenceGateCenterAltItem>(user);
                user.Player.InfoBox(new LocString("Chainlink Gate switched to Right."));
            }
            return base.OnUsed(player, itemStack);
        }
    }

    [RequiresSkill(typeof(SmeltingSkill), 6)]
    public partial class FenceGateCenterCenterLeftRecipe : RecipeFamily
    {
        public FenceGateCenterCenterLeftRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                "ChainlinkGateCenter",  //noloc
                Localizer.DoStr("Chainlink Gate Center"),
                new List<IngredientElement>
                {
                    new IngredientElement(typeof(IronBarItem), 12, typeof(SmeltingSkill), typeof(SmeltingLavishResourcesTalent)),
                },
                new List<CraftingElement>
                {
                    new CraftingElement<FenceGateCenterItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 2;
            this.LaborInCalories = CreateLaborInCaloriesValue(100, typeof(SmeltingSkill));
            this.CraftMinutes = CreateCraftTimeValue(typeof(FenceGateCenterCenterLeftRecipe), 1.5f, typeof(SmeltingSkill), typeof(SmeltingFocusedSpeedTalent), typeof(SmeltingParallelSpeedTalent));
            this.ModsPreInitialize();
            this.Initialize(Localizer.DoStr("Chainlink Gate Center"), typeof(FenceGateCenterCenterLeftRecipe));
            this.ModsPostInitialize();
            CraftingComponent.AddRecipe(typeof(AnvilObject), this);
        }

        /// <summary>Hook for mods to customize RecipeFamily before initialization. You can change recipes, xp, labor, time here.</summary>
        partial void ModsPreInitialize();
        /// <summary>Hook for mods to customize RecipeFamily after initialization, but before registration. You can change skill requirements here.</summary>
        partial void ModsPostInitialize();
    }
}
