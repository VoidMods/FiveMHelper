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

            RegisterCommand("car", new Action<int, List<object>, string>(async (source, args, raw) => {
                // account for the argument not being passed
                var model = "sentinel3";
                if (args.Count > 0) {
                    model = args[0].ToString();
                }

                // check if the model actually exists, if not, use Sentinel3
                var hash = (uint)GetHashKey(model);
                if (!IsModelInCdimage(hash) || !IsModelAVehicle(hash)) {
                    hash = (uint)GetHashKey("sentinel3");
                }

                // create the vehicle
                var vehicle = await World.CreateVehicle(model, Game.PlayerPed.Position, Game.PlayerPed.Heading);
                // enable car mods
                SetVehicleModKit(vehicle.GetHashCode(), 0);
                // set vehicle colors
                SetVehicleModColor_1(vehicle.GetHashCode(), 3, 0, 0);
                SetVehicleModColor_2(vehicle.GetHashCode(), 3, 38);
                // set mods
                SetVehicleMod(vehicle.GetHashCode(), (int)VehicleModType.Spoilers, 3, false);
                SetVehicleMod(vehicle.GetHashCode(), (int)VehicleModType.Suspension, 1, false);
                SetVehicleMod(vehicle.GetHashCode(), (int)VehicleModType.Exhaust, 3, false);
                // set vanity plate
                SetVehicleNumberPlateText(vehicle.GetHashCode(), "0xVoid");                
                // set the player ped into the vehicle and driver seat
                Game.PlayerPed.SetIntoVehicle(vehicle, VehicleSeat.Driver);
            }), false);

        }
    }
}
