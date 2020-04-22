using System;
using System.Collections.Generic;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace FiveMHelperClient {
    public class Class1 : BaseScript {
        public Class1() {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
        }

        private void OnClientResourceStart(string resourceName) {
            if (GetCurrentResourceName() != resourceName) return;

            int[,] SentinelClassicMods = {
                { (int)VehicleModType.Spoilers, 3 },
                { (int)VehicleModType.Suspension, 1 },
                { (int)VehicleModType.Exhaust, 3 }
            };

            RegisterCommand("car", new Action<int, List<object>, string>(async (source, args, raw) => {
                // account for the argument not being passed
                string model = "sentinel3";
                if (args.Count > 0) {
                    model = args[0].ToString();
                }

                // check if the model actually exists, if not, use Sentinel3
                uint hash = (uint)GetHashKey(model);
                if (!IsModelInCdimage(hash) || !IsModelAVehicle(hash)) {
                    hash = (uint)GetHashKey("sentinel3");
                }

                // create the vehicle
                Vehicle vehicle = await World.CreateVehicle(model, Game.PlayerPed.Position, Game.PlayerPed.Heading);
                int vehicleHash = vehicle.GetHashCode();
                // enable car mods
                SetVehicleModKit(vehicleHash, 0);
                // set vehicle colors
                SetVehicleModColor_1(vehicleHash, 3, 0, 0);
                SetVehicleModColor_2(vehicleHash, 3, 38);
                // if sentinel classic apply mods
                if (model == "sentinel3") {
                    // set mods
                    for (int i = 0; i < SentinelClassicMods.GetLength(0); i++) {
                        SetVehicleMod(vehicleHash, SentinelClassicMods[i, 0], SentinelClassicMods[i, 1], false);
                    }
                }
                // set vanity plate
                SetVehicleNumberPlateText(vehicleHash, "0xVoid");
                // set the player ped into the vehicle and driver seat
                Game.PlayerPed.SetIntoVehicle(vehicle, VehicleSeat.Driver);
            }), false);
        }
    }
}
