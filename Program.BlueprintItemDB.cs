using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRage.Game;
using VRage.Game.ModAPI.Ingame;

namespace IngameScript
{
    public partial class Program : MyGridProgram
    {
        const string AM = "AmmoMagazine";
        const string PG = "PhysicalGunObject";
        const string C = "Component";
        const string CI = "ConsumableItem";
        const string IN = "Ingot";
        const string SD = "SeedItem";
        const string DP = "Datapad";
        const string GC = "GasContainerObject";
        const string OC = "OxygenContainerObject";
        const string O = "Ore";

        struct Meta
        {
            public MyDefinitionId BlueprintId;
            public MyItemType TypeId;
            public Meta(string blueprintId, string itemType) {
                BlueprintId = MyDefinitionId.Parse($"MyObjectBuilder_BlueprintDefinition/{blueprintId}");
                TypeId = MyItemType.Parse($"MyObjectBuilder_{itemType}");
            }
        }
        Dictionary<string, Meta> Items = new Dictionary<string, Meta> {
            {"Stone"                                  , new Meta(""                                                , $"{O}/Stone")},
            {"Ice"                                    , new Meta(""                                                , $"{O}/Ice")},
            {"Cobalt Ore"                             , new Meta(""                                                , $"{O}/Cobalt")},
            {"Iron Ore"                               , new Meta(""                                                , $"{O}/Iron")},
            {"Gold Ore"                               , new Meta(""                                                , $"{O}/Gold")},
            {"Magnesium Ore"                          , new Meta(""                                                , $"{O}/Magnesium")},
            {"Nickel Ore"                             , new Meta(""                                                , $"{O}/Nickel")},
            {"Platinum Ore"                           , new Meta(""                                                , $"{O}/Platinum")},
            {"Silicon Ore"                            , new Meta(""                                                , $"{O}/Silicon")},
            {"Silver Ore"                             , new Meta(""                                                , $"{O}/Silver")},
            {"Uranium Ore"                            , new Meta(""                                                , $"{O}/Uranium")},

            {"Cobalt Ingot"                           , new Meta("CobaltOreToIngot"                                , $"{IN}/Cobalt")},
            {"Iron Ingot"                             , new Meta("IronOreToIngot"                                  , $"{IN}/Iron")},
            {"Gold Ingot"                             , new Meta("GoldOreToIngot"                                  , $"{IN}/Gold")},
            {"Magnesium Powder"                       , new Meta("MagnesiumOreToIngot"                             , $"{IN}/Magnesium")},
            {"Nickel Ingot"                           , new Meta("NickelOreToIngot"                                , $"{IN}/Nickel")},
            {"Platinum Ingot"                         , new Meta("PlatinumOreToIngot"                              , $"{IN}/Platinum")},
            {"Silicon Wafer"                          , new Meta("SiliconOreToIngot"                               , $"{IN}/Silicon")},
            {"Silver Ingot"                           , new Meta("SilverOreToIngot"                                , $"{IN}/Silver")},
            {"Uranium Ingot"                          , new Meta("UraniumOreToIngot"                               , $"{IN}/Uranium")},

            {"Bulletproof Glass"                      , new Meta("BulletproofGlass"                                , $"{C}/BulletproofGlass")},
            {"Canvas Cartridge"                       , new Meta("Position0030_Canvas"                             , $"{C}/Canvas")},
            {"Computer"                               , new Meta("ComputerComponent"                               , $"{C}/Computer")},
            {"Construction Component"                 , new Meta("ConstructionComponent"                           , $"{C}/Construction")},
            {"Detector Components"                    , new Meta("DetectorComponent"                               , $"{C}/Detector")},
            {"Display"                                , new Meta("Display"                                         , $"{C}/Display")},
            {"Engineer Plushie"                       , new Meta("EngineerPlushie"                                 , $"{C}/EngineerPlushie")},
            {"Explosives"                             , new Meta("ExplosivesComponent"                             , $"{C}/Explosives")},
            {"Girder"                                 , new Meta("GirderComponent"                                 , $"{C}/Girder")},
            {"Gravity Generator Components"           , new Meta("GravityGeneratorComponent"                       , $"{C}/GravityGenerator")},
            {"Interior Plate"                         , new Meta("InteriorPlate"                                   , $"{C}/InteriorPlate")},
            {"Large Steel Tube"                       , new Meta("LargeTube"                                       , $"{C}/LargeTube")},
            {"Medical Components"                     , new Meta("MedicalComponent"                                , $"{C}/Medical")},
            {"Metal Grid"                             , new Meta("MetalGrid"                                       , $"{C}/MetalGrid")},
            {"Motor"                                  , new Meta("MotorComponent"                                  , $"{C}/Motor")},
            {"Power Cell"                             , new Meta("PowerCell"                                       , $"{C}/PowerCell")},
            {"Prototech Capacitor"                    , new Meta("PrototechCapacitor"                              , $"{C}/PrototechCapacitor")},
            {"Prototech Circuitry"                    , new Meta("PrototechCircuitry"                              , $"{C}/PrototechCircuitry")},
            {"Prototech Cooling Unit"                 , new Meta("PrototechCoolingUnit"                            , $"{C}/PrototechCoolingUnit")},
            {"Prototech Frame"                        , new Meta("PrototechFrame"                                  , $"{C}/PrototechFrame")},
            {"Prototech Machinery"                    , new Meta("PrototechMachinery"                              , $"{C}/PrototechMachinery")},
            {"Prototech Panel"                        , new Meta("PrototechPanel"                                  , $"{C}/PrototechPanel")},
            {"Prototech Propulsion Unit"              , new Meta("PrototechPropulsionUnit"                         , $"{C}/PrototechPropulsionUnit")},
            {"Radio Communication Components"         , new Meta("RadioCommunicationComponent"                     , $"{C}/RadioCommunication")},
            {"Reactor Components"                     , new Meta("ReactorComponent"                                , $"{C}/Reactor")},
            {"Sabiroid Plushie"                       , new Meta("SabiroidPlushie"                                 , $"{C}/SabiroidPlushie")},
            {"Small Steel Tube"                       , new Meta("SmallTube"                                       , $"{C}/SmallTube")},
            {"Solar Cell"                             , new Meta("SolarCell"                                       , $"{C}/SolarCell")},
            {"Steel Plate"                            , new Meta("SteelPlate"                                      , $"{C}/SteelPlate")},
            {"Superconductor"                         , new Meta("Superconductor"                                  , $"{C}/Superconductor")},
            {"Thrust Components"                      , new Meta("ThrustComponent"                                 , $"{C}/Thrust")},
            {"Zone Chip"                              , new Meta("ZoneChip"                                        , $"{C}/ZoneChip")},

            {"Datapad"                                , new Meta("Position0040_Datapad"                            , $"{DP}/Datapad")},
            {"Hydrogen Bottle"                        , new Meta("Position0020_HydrogenBottle"                     , $"{GC}/HydrogenBottle")},
            {"Oxygen Bottle"                          , new Meta("Position0010_OxygenBottle"                       , $"{OC}/OxygenBottle")},

            {"Advanced Hand Held Launcher"            , new Meta("Position0090_AdvancedHandHeldLauncher"           , $"{PG}/AdvancedHandHeldLauncherItem")},
            {"Angle Grinder 2"                        , new Meta("Position0020_AngleGrinder2"                      , $"{PG}/AngleGrinder2Item")},
            {"Angle Grinder 3"                        , new Meta("Position0030_AngleGrinder3"                      , $"{PG}/AngleGrinder3Item")},
            {"Angle Grinder 4"                        , new Meta("Position0040_AngleGrinder4"                      , $"{PG}/AngleGrinder4Item")},
            {"Angle Grinder"                          , new Meta("Position0010_AngleGrinder"                       , $"{PG}/AngleGrinderItem")},
            {"Automatic Rifle"                        , new Meta("Position0040_AutomaticRifle"                     , $"{PG}/AutomaticRifleItem")},
            {"Basic Hand Held Launcher"               , new Meta("Position0080_BasicHandHeldLauncher"              , $"{PG}/BasicHandHeldLauncherItem")},
            {"Elite Automatic Rifle"                  , new Meta("Position0070_UltimateAutomaticRifle"             , $"{PG}/UltimateAutomaticRifleItem")},
            {"Elite Pistol"                           , new Meta("Position0030_EliteAutoPistol"                    , $"{PG}/ElitePistolItem")},
            {"Flare Gun"                              , new Meta("Position0050_FlareGun"                           , $"{PG}/FlareGunItem")},
            {"Full-Auto Pistol"                       , new Meta("Position0020_FullAutoPistol"                     , $"{PG}/FullAutoPistolItem")},
            {"Hand Drill 2"                           , new Meta("Position0060_HandDrill2"                         , $"{PG}/HandDrill2Item")},
            {"Hand Drill 3"                           , new Meta("Position0070_HandDrill3"                         , $"{PG}/HandDrill3Item")},
            {"Hand Drill 4"                           , new Meta("Position0080_HandDrill4"                         , $"{PG}/HandDrill4Item")},
            {"Hand Drill"                             , new Meta("Position0050_HandDrill"                          , $"{PG}/HandDrillItem")},
            {"Precise Automatic Rifle"                , new Meta("Position0060_PreciseAutomaticRifle"              , $"{PG}/PreciseAutomaticRifleItem")},
            {"Rapid-Fire Automatic Rifle"             , new Meta("Position0050_RapidFireAutomaticRifle"            , $"{PG}/RapidFireAutomaticRifleItem")},
            {"Semi-Auto Pistol"                       , new Meta("Position0010_SemiAutoPistol"                     , $"{PG}/SemiAutoPistolItem")},
            {"Welder 2"                               , new Meta("Position0100_Welder2"                            , $"{PG}/Welder2Item")},
            {"Welder 3"                               , new Meta("Position0110_Welder3"                            , $"{PG}/Welder3Item")},
            {"Welder 4"                               , new Meta("Position0120_Welder4"                            , $"{PG}/Welder4Item")},
            {"Welder"                                 , new Meta("Position0090_Welder"                             , $"{PG}/WelderItem")},

            {"AutoCannon Clip"                        , new Meta("Position0090_AutocannonClip"                     , $"{AM}/AutocannonClip")},
            {"Automatic Rifle Gun Mag 20rd"           , new Meta("Position0040_AutomaticRifleGun_Mag_20rd"         , $"{AM}/AutomaticRifleGun_Mag_20rd")},
            {"Elite Pistol Magazine"                  , new Meta("Position0030_ElitePistolMagazine"                , $"{AM}/ElitePistolMagazine")},
            {"Fireworks Box Blue"                     , new Meta("Position0060_FireworksBoxBlue"                   , $"{AM}/FireworksBoxBlue")},
            {"Fireworks Box Green"                    , new Meta("Position0061_FireworksBoxGreen"                  , $"{AM}/FireworksBoxGreen")},
            {"Fireworks Box Pink"                     , new Meta("Position0064_FireworksBoxPink"                   , $"{AM}/FireworksBoxPink")},
            {"Fireworks Box Rainbow"                  , new Meta("Position0065_FireworksBoxRainbow"                , $"{AM}/FireworksBoxRainbow")},
            {"Fireworks Box Red"                      , new Meta("Position0062_FireworksBoxRed"                    , $"{AM}/FireworksBoxRed")},
            {"Fireworks Box Yellow"                   , new Meta("Position0063_FireworksBoxYellow"                 , $"{AM}/FireworksBoxYellow")},
            {"Flare Clip"                             , new Meta("Position0051_FlareGunMagazine"                   , $"{AM}/FlareClip")},
            {"Full-Auto Pistol Magazine"              , new Meta("Position0020_FullAutoPistolMagazine"             , $"{AM}/FullAutoPistolMagazine")},
            {"Small Railgun Ammo"                     , new Meta("Position0130_SmallRailgunAmmo"                   , $"{AM}/SmallRailgunAmmo")},
            {"Large Calibre Ammo"                     , new Meta("Position0120_LargeCalibreAmmo"                   , $"{AM}/LargeCalibreAmmo")},
            {"Large Railgun Ammo"                     , new Meta("Position0140_LargeRailgunAmmo"                   , $"{AM}/LargeRailgunAmmo")},
            {"Medium Calibre Ammo"                    , new Meta("Position0110_MediumCalibreAmmo"                  , $"{AM}/MediumCalibreAmmo")},
            {"Missile 200mm"                          , new Meta("Position0100_Missile200mm"                       , $"{AM}/Missile200mm")},
            {"NATO 25x184mm"                          , new Meta("Position0080_NATO_25x184mmMagazine"              , $"{AM}/NATO_25x184mm")},
            {"Precise Automatic Rifle Gun Mag 5rd"    , new Meta("Position0060_PreciseAutomaticRifleGun_Mag_5rd"   , $"{AM}/PreciseAutomaticRifleGun_Mag_5rd")},
            {"Rapid-Fire Automatic Rifle Gun Mag 50rd", new Meta("Position0050_RapidFireAutomaticRifleGun_Mag_50rd", $"{AM}/RapidFireAutomaticRifleGun_Mag_50rd")},
            {"Semi-Auto Pistol Magazine"              , new Meta("Position0010_SemiAutoPistolMagazine"             , $"{AM}/SemiAutoPistolMagazine")},
            {"Ultimate Automatic Rifle Gun Mag 30rd"  , new Meta("Position0070_UltimateAutomaticRifleGun_Mag_30rd" , $"{AM}/UltimateAutomaticRifleGun_Mag_30rd")},

            {"Medkit"                                 , new Meta("Position0021_Medkit"                             , $"{CI}/Medkit")},
            {"Powerkit"                               , new Meta("Position0022_Powerkit"                           , $"{CI}/Powerkit")},
            {"Cooked Mammal Meat"                     , new Meta("Position0010_CookMammalMeat"                     , $"{CI}/MammalMeatCooked")},
            {"Cooked Spider Meat"                     , new Meta("Position0020_CookSpiderMeat"                     , $"{CI}/InsectMeatCooked")},
            {"Burrito"                                , new Meta("Position0170_MealPack_Burrito"                   , $"{CI}/MealPack_Burrito")},
            {"Chili"                                  , new Meta("Position0070_MealPack_Chili"                     , $"{CI}/MealPack_Chili")},
            {"Curry"                                  , new Meta("Position0120_MealPack_Curry"                     , $"{CI}/MealPack_Curry")},
            {"Dumplings"                              , new Meta("Position0140_MealPack_Dumplings"                 , $"{CI}/MealPack_Dumplings")},
            {"Flatbread"                              , new Meta("Position0080_MealPack_Flatbread"                 , $"{CI}/MealPack_Flatbread")},
            {"Frontier Stew"                          , new Meta("Position0180_MealPack_FrontierStew"              , $"{CI}/MealPack_FrontierStew")},
            {"Fruit Bar"                              , new Meta("Position0040_MealPack_FruitBar"                  , $"{CI}/MealPack_FruitBar")},
            {"Fruit Pastry"                           , new Meta("Position0100_MealPack_FruitPastry"               , $"{CI}/MealPack_FruitPastry")},
            {"Garden Slaw"                            , new Meta("Position0050_MealPack_GardenSlaw"                , $"{CI}/MealPack_GardenSlaw")},
            {"Green Pellets"                          , new Meta("Position0130_MealPack_GreenPellets"              , $"{CI}/MealPack_GreenPellets")},
            {"Kelp Crisp"                             , new Meta("Position0030_MealPack_KelpCrisp"                 , $"{CI}/MealPack_KelpCrisp")},
            {"Lasagna"                                , new Meta("Position0160_MealPack_Lasagna"                   , $"{CI}/MealPack_Lasagna")},
            {"Ramen"                                  , new Meta("Position0090_MealPack_Ramen"                     , $"{CI}/MealPack_Ramen")},
            {"Red Pellets"                            , new Meta("Position0060_MealPack_RedPellets"                , $"{CI}/MealPack_RedPellets")},
            {"Seared Sabiroid"                        , new Meta("Position0190_MealPack_SearedSabiroid"            , $"{CI}/MealPack_SearedSabiroid")},
            {"Spaghetti"                              , new Meta("Position0150_MealPack_Spaghetti"                 , $"{CI}/MealPack_Spaghetti")},
            {"Steak Dinner"                           , new Meta("Position0200_MealPack_SteakDinner"               , $"{CI}/MealPack_SteakDinner")},
            {"Veggie Burger"                          , new Meta("Position0110_MealPack_VeggieBurger"              , $"{CI}/MealPack_VeggieBurger")},

            {"Grain Seeds"                            , new Meta("Position0020_Seeds_Grain"                        , $"{SD}/Grain")},
            {"Mushroom Spores"                        , new Meta("Position0040_Spores_Mushrooms"                   , $"{SD}/Mushrooms")},
            {"Vegetable Seeds"                        , new Meta("Position0030_Seeds_Vegetables"                   , $"{SD}/Vegetables")},
            {"Fruit Seeds"                            , new Meta("Position0010_Seeds_Fruit"                        , $"{SD}/Fruit")},
        };
    }
}