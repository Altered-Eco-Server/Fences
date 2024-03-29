﻿// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.
// <auto-generated from WorldObjectTemplate.tt />

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
    public partial class FenceGateEdgeAltObject : DoorObject, IRepresentsItem
    {
        [Serialized] public bool Door = true;
        public override LocString DisplayName { get { return Localizer.DoStr("Chainlink Gate Edge Right"); } }
        public override TableTextureMode TableTexture => TableTextureMode.Stone;
        public virtual Type RepresentedItemType { get { return typeof(FenceGateEdgeAltItem); } }
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
    [LocDisplayName("Chainlink Gate Edge Right")]
    [Tier(4)]
    [Ecopedia("Housing Objects", "Doors", createAsSubPage: true)]
    public partial class FenceGateEdgeAltItem : WorldObjectItem<FenceGateEdgeAltObject>
    {
        public override LocString DisplayDescription => Localizer.DoStr("Right click icon to change left/right variations");
        public override DirectionAxisFlags RequiresSurfaceOnSides { get; } = 0 | DirectionAxisFlags.Down;
        public override string OnUsed(Player player, ItemStack itemStack)
        {
            var user = player.User;

            if (user.Inventory.TryRemoveItem<FenceGateEdgeAltItem>(user))
            {
                user.Inventory.TryAddItem<FenceGateEdgeItem>(user);
                user.Player.InfoBox(new LocString("Chainlink Gate switched to Left."));
            }
            return base.OnUsed(player, itemStack);
        }
    }
}
