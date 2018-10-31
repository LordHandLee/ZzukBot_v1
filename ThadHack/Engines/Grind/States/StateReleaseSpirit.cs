using ZzukBot.FSM;
using ZzukBot.Helpers;
using ZzukBot.Mem;
using System;

namespace ZzukBot.Engines.Grind.States
{
    internal class StateReleaseSpirit : State
    {
        private readonly Random ran = new Random();

        internal override int Priority => 55;

        internal override bool NeedToRun => ObjectManager.Player.IsDead;

        internal override string Name => "Releasing Spirit";

        internal override void Run()
        {
            // Delay for Spirit Release
            if (Wait.For("ReleasingSpiritDelay", ran.Next(3000, 8000), true))
            {
                if (!Wait.For("ReleasingSpirit", 250)) return;
                ObjectManager.Player.CtmStopMovement();
                Wait.Remove("StartGhostWalk");
                Functions.DoString("RepopMe()");
                Grinder.Access.Info.SpiritWalk.GeneratePath = true;
            }
        }
    }
}