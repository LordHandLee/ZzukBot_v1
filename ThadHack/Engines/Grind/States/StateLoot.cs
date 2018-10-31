using System;
using ZzukBot.FSM;
using ZzukBot.Helpers;
using ZzukBot.Mem;
using System.Collections.Generic;
using ZzukBot.Objects;

namespace ZzukBot.Engines.Grind.States
{
    internal class StateLoot : State
    {
        private int lastCheck;

        private ulong oldMobGuid;
        private readonly Random ran = new Random();
        private int randomOpenLootDelay;
        private int randomTakeLootDelay;

        internal override int Priority => 46;

        internal override bool NeedToRun => Grinder.Access.Info.Loot.NeedToLoot && !Grinder.Access.Info.Vendor.GoBackToGrindAfterVendor
                                            && !Grinder.Access.Info.Vendor.TravelingToVendor;

        internal override string Name => "Looting";

        internal override void Run()
        {
            //Open Clams
            using (List<WoWItem>.Enumerator enumerator = ObjectManager.Items.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Name.ToLower().Contains("clam"))
                    {
                        ObjectManager.Player.Inventory.UseItem(enumerator.Current);
                        if (Wait.For("OpenClamDelay", 1500, true))
                        {
                            ObjectManager.Player.LootAll();
                        }
                    }
                }
            }


            var mob = Grinder.Access.Info.Loot.LootableMob;
            if (mob == null) return;
            if (mob.Guid != oldMobGuid)
            {
                oldMobGuid = mob.Guid;
                lastCheck = Environment.TickCount;
                Wait.Remove("RunToLoot");
                Wait.Remove("Looting");
            }
            if (Calc.Distance3D(mob.Position, ObjectManager.Player.Position) > 2)
            {
                var tu = Grinder.Access.Info.PathToUnit.ToUnit(mob);
                if (tu.Item1)
                    ObjectManager.Player.CtmTo(tu.Item2);

                if (Environment.TickCount - lastCheck >= 5000)
                {
                    Wait.Remove("RunToLoot");
                }
                lastCheck = Environment.TickCount;

                if (Wait.For("RunToLoot", 10000))
                {
                    Grinder.Access.Info.Loot.AddToLootBlacklist(mob.Guid);
                }
                Wait.Remove("Looting");
                randomOpenLootDelay = ran.Next(250, 750) + Grinder.Access.Info.Latency;
                randomTakeLootDelay = ran.Next(50, 250) + Grinder.Access.Info.Latency;
            }
            else
            {
                if (!ObjectManager.Player.IsLooting)
                {
                    var auto = mob.IsSkinable;
                    if (Wait.For("LootClick", randomOpenLootDelay))
                        ObjectManager.Player.RightClick(mob, auto);
                }
                else
                {
                    if (Wait.For("LootTake12", randomTakeLootDelay))
                    {
                        ObjectManager.Player.LootAll();
                        if (ObjectManager.Player.LootSlots == 0)
                            Grinder.Access.Info.Loot.AddToLootBlacklist(mob.Guid);
                        Wait.Remove("Looting");
                    }
                }
                if (Wait.For("Looting", 5000))
                {
                    Grinder.Access.Info.Loot.AddToLootBlacklist(mob.Guid);
                }
            }
        }
    }
}