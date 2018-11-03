using System;
using System.Collections.Generic;
using ZzukBot.GUI_Forms;
using ZzukBot.Mem;
using ZzukBot.Objects;
using ZzukBot.Settings;

namespace ZzukBot.Engines.Grind.Info
{
    internal class NearbyPlayersHelper
    {
        private static Dictionary<string, DateTime> nearbyPlayers = new Dictionary<string, DateTime>();

        internal bool HasPlayersNearby
        {
            get
            {
                if (ObjectManager.EnumObjects() && ObjectManager.Players != null && ObjectManager.Players.Count > 1)
                {
                    Dictionary<string, DateTime> newNearbyPlayers = new Dictionary<string, DateTime>();
                    ObjectManager.Players.ForEach(delegate (WoWUnit x)
                    {
                        if (x.Name != ObjectManager.Player.Name && x.DistanceToPlayer < 70f)
                        {
                            if (nearbyPlayers.ContainsKey(x.Name))
                            {
                                newNearbyPlayers.Add(x.Name, nearbyPlayers[x.Name]);
                                return;
                            }
                            newNearbyPlayers.Add(x.Name, DateTime.Now);
                        }
                    });
                    nearbyPlayers = newNearbyPlayers;
                    DateTime now = DateTime.Now;
                    using (Dictionary<string, DateTime>.Enumerator enumerator = nearbyPlayers.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            KeyValuePair<string, DateTime> keyValuePair = enumerator.Current;
                            WoWUnit woWUnit = ObjectManager.Players.Find((WoWUnit xs) => keyValuePair.Key.Equals(xs.Name));
                            if ((now - keyValuePair.Value).TotalSeconds > 12.0 || (woWUnit != null && woWUnit.DistanceToPlayer < 38f))
                            {
                                string parMessage = string.Concat(new object[]
                                {
                                    "Player nearby (",
                                    keyValuePair.Key,
                                    ")!"
                                });
                                if (woWUnit != null)
                                {
                                    parMessage = string.Concat(new object[]
                                    {
                                        "Player nearby (",
                                        woWUnit.Name,
                                        " - Level: ",
                                        woWUnit.Level,
                                        " - Distance: ",
                                        (int)woWUnit.DistanceToPlayer,
                                        ")!"
                                    });
                                }
                                Main.MainForm.updateNotification(parMessage);
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
        }
    }
}
