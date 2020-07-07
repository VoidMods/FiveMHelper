using System;
using System.Collections.Generic;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace FiveMHelperClient {
    public class Main : BaseScript {
        // Constants
        readonly string plateNumber = "OxVoid";
        readonly int[,] SentinelClassicMods = {
            { (int)VehicleModType.Spoilers, 3 },
            { (int)VehicleModType.Suspension, 1 },
            { (int)VehicleModType.Exhaust, 3 }
        };
        readonly int[,] ElegyClassicMods = {
            { 0, 7 },
            { 1, 5 },
            { 3, 3 },
            { 4, 1 },
            { 6, 1 },
            { 7, 5 },
            { 8, 2 },
            { 15, 3 },
            { 25, 0 },
            { 26, 2 },
            { 32, 5 },
            { 33, 7 },
        };

        public Main() {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
        }

        private void OnClientResourceStart(string resourceName) {
            // Quit if a different resource is started
            if (GetCurrentResourceName() != resourceName) return;

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
                } else if (model == "elegy") {
                    // set mods
                    for (int i = 0; i < ElegyClassicMods.GetLength(0); i++) {
                        SetVehicleMod(vehicleHash, ElegyClassicMods[i, 0], ElegyClassicMods[i, 1], false);
                    }
                }
                // set vanity plate
                SetVehicleNumberPlateText(vehicleHash, plateNumber);
                // set the player ped into the vehicle and driver seat
                Game.PlayerPed.SetIntoVehicle(vehicle, VehicleSeat.Driver);
            }), false);

            RegisterCommand("mod", new Action<int, List<object>, string>(async (source, args, raw) => {
                int playerPed = PlayerPedId();

                if (IsPedInAnyVehicle(playerPed, false)
                    && !IsPedInAnyBoat(playerPed)
                    && !IsPedInAnyHeli(playerPed)
                    && !IsPedInAnyPlane(playerPed)
                    && !IsPedInAnySub(playerPed)
                    && !IsPedInAnyTrain(playerPed)
                    ) {
                    string mod;
                    string val;
                    if (args.Count > 1) {
                        mod = args[0].ToString();
                        val = args[1].ToString();
                        // Make sure vehicle is moddable
                        SetVehicleModKit(GetVehiclePedIsIn(playerPed, false), 0);
                        SetVehicleMod(GetVehiclePedIsIn(playerPed, false), Int32.Parse(mod), Int32.Parse(val), false);
                    }
                }
            }), false);

            RegisterCommand("setModel", new Action<int, List<object>, string>(async (source, args, raw) => {
                // account for the argument not being passed
                string modelName = "base";
                if (args.Count > 0) {
                    modelName = args[0].ToString();
                }
                switch (modelName) {
                    case "normal":
                        modelName = "s_m_m_ciasec_01";
                        break;
                    default:
                        modelName = "a_c_crow";
                        break;

                }
                Model model = new Model(modelName);
                await Game.Player.ChangeModel(model);
                model.MarkAsNoLongerNeeded();
                SetPedDefaultComponentVariation(PlayerPedId());
            }), false);
        }
    }
}
