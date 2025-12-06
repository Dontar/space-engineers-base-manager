using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRage.Game;
using VRage.Game.ModAPI.Ingame;

namespace IngameScript
{
    public partial class Program : MyGridProgram
    {
        const string AM = "AmmoMagazine";
        const string PGO = "PhysicalGunObject";
        const string C = "Component";
        const string CI = "ConsumableItem";

        struct Meta
        {
            public MyDefinitionId BlueprintId;
            public MyItemType ItemType;
            public Meta(string blueprintId, string itemType) {
                BlueprintId = MyDefinitionId.Parse($"MyObjectBuilder_BlueprintDefinition/{blueprintId}");
                ItemType = MyItemType.Parse($"MyObjectBuilder_{itemType}");
            }
        }
        Dictionary<string, Meta> Items = new Dictionary<string, Meta> {
            {"Gravel"                                 , new Meta("StoneOreToIngot_Deconstruction"                  , "Ingot/Stone")},
            {"Iron Ingot"                             , new Meta("IronOreToIngot"                                  , "Ingot/Iron")},
            {"Nickel Ingot"                           , new Meta("NickelOreToIngot"                                , "Ingot/Nickel")},
            {"Cobalt Ingot"                           , new Meta("CobaltOreToIngot"                                , "Ingot/Cobalt")},
            {"Magnesium Powder"                       , new Meta("MagnesiumOreToIngot"                             , "Ingot/Magnesium")},
            {"Silicon Wafer"                          , new Meta("SiliconOreToIngot"                               , "Ingot/Silicon")},
            {"Silver Ingot"                           , new Meta("SilverOreToIngot"                                , "Ingot/Silver")},
            {"Gold Ingot"                             , new Meta("GoldOreToIngot"                                  , "Ingot/Gold")},
            {"Platinum Ingot"                         , new Meta("PlatinumOreToIngot"                              , "Ingot/Platinum")},
            {"Uranium Ingot"                          , new Meta("UraniumOreToIngot"                               , "Ingot/Uranium")},
            {"Iron Ingot"                             , new Meta("ScrapIngotToIronIngot"                           , "Ingot/Iron")},
            {"Iron Ingot"                             , new Meta("ScrapToIronIngot"                                , "Ingot/Iron")},

            {"Construction Component"                 , new Meta("ConstructionComponent"                           , $"{C}/Construction")},
            {"Girder"                                 , new Meta("GirderComponent"                                 , $"{C}/Girder")},
            {"Metal Grid"                             , new Meta("MetalGrid"                                       , $"{C}/MetalGrid")},
            {"Interior Plate"                         , new Meta("InteriorPlate"                                   , $"{C}/InteriorPlate")},
            {"Steel Plate"                            , new Meta("SteelPlate"                                      , $"{C}/SteelPlate")},
            {"Small Steel Tube"                       , new Meta("SmallTube"                                       , $"{C}/SmallTube")},
            {"Large Steel Tube"                       , new Meta("LargeTube"                                       , $"{C}/LargeTube")},
            {"Motor"                                  , new Meta("MotorComponent"                                  , $"{C}/Motor")},
            {"Display"                                , new Meta("Display"                                         , $"{C}/Display")},
            {"Bulletproof Glass"                      , new Meta("BulletproofGlass"                                , $"{C}/BulletproofGlass")},
            {"Computer"                               , new Meta("ComputerComponent"                               , $"{C}/Computer")},
            {"Reactor Components"                     , new Meta("ReactorComponent"                                , $"{C}/Reactor")},
            {"Thrust Components"                      , new Meta("ThrustComponent"                                 , $"{C}/Thrust")},
            {"Gravity Generator Components"           , new Meta("GravityGeneratorComponent"                       , $"{C}/GravityGenerator")},
            {"Medical Components"                     , new Meta("MedicalComponent"                                , $"{C}/Medical")},
            {"Radio Communication Components"         , new Meta("RadioCommunicationComponent"                     , $"{C}/RadioCommunication")},
            {"Detector Components"                    , new Meta("DetectorComponent"                               , $"{C}/Detector")},
            {"Explosives"                             , new Meta("ExplosivesComponent"                             , $"{C}/Explosives")},
            {"Solar Cell"                             , new Meta("SolarCell"                                       , $"{C}/SolarCell")},
            {"Power Cell"                             , new Meta("PowerCell"                                       , $"{C}/PowerCell")},
            {"Superconductor"                         , new Meta("Superconductor"                                  , $"{C}/Superconductor")},
            {"Oxygen Bottle"                          , new Meta("Position0010_OxygenBottle"                       , "OxygenContainerObject/OxygenBottle")},
            {"Hydrogen Bottle"                        , new Meta("Position0020_HydrogenBottle"                     , "GasContainerObject/HydrogenBottle")},
            {"Medkit"                                 , new Meta("Position0021_Medkit"                             , $"{CI}/Medkit")},
            {"Powerkit"                               , new Meta("Position0022_Powerkit"                           , $"{CI}/Powerkit")},
            {"Canvas Cartridge"                       , new Meta("Position0030_Canvas"                             , $"{C}/Canvas")},
            {"Datapad"                                , new Meta("Position0040_Datapad"                            , "Datapad/Datapad")},
            {"Flare Gun"                              , new Meta("Position0050_FlareGun"                           , $"{PGO}/FlareGunItem")},
            {"Flare Clip"                             , new Meta("Position0051_FlareGunMagazine"                   , $"{AM}/FlareClip")},
            {"Fireworks Box Blue"                     , new Meta("Position0060_FireworksBoxBlue"                   , $"{AM}/FireworksBoxBlue")},
            {"Fireworks Box Green"                    , new Meta("Position0061_FireworksBoxGreen"                  , $"{AM}/FireworksBoxGreen")},
            {"Fireworks Box Red"                      , new Meta("Position0062_FireworksBoxRed"                    , $"{AM}/FireworksBoxRed")},
            {"Fireworks Box Yellow"                   , new Meta("Position0063_FireworksBoxYellow"                 , $"{AM}/FireworksBoxYellow")},
            {"Fireworks Box Pink"                     , new Meta("Position0064_FireworksBoxPink"                   , $"{AM}/FireworksBoxPink")},
            {"Fireworks Box Rainbow"                  , new Meta("Position0065_FireworksBoxRainbow"                , $"{AM}/FireworksBoxRainbow")},
            {"Semi-Auto Pistol"                       , new Meta("Position0010_SemiAutoPistol"                     , $"{PGO}/SemiAutoPistolItem")},
            {"Full-Auto Pistol"                       , new Meta("Position0020_FullAutoPistol"                     , $"{PGO}/FullAutoPistolItem")},
            {"Elite Pistol"                           , new Meta("Position0030_EliteAutoPistol"                    , $"{PGO}/ElitePistolItem")},
            {"Automatic Rifle"                        , new Meta("Position0040_AutomaticRifle"                     , $"{PGO}/AutomaticRifleItem")},
            {"Rapid-Fire Automatic Rifle"             , new Meta("Position0050_RapidFireAutomaticRifle"            , $"{PGO}/RapidFireAutomaticRifleItem")},
            {"Precise Automatic Rifle"                , new Meta("Position0060_PreciseAutomaticRifle"              , $"{PGO}/PreciseAutomaticRifleItem")},
            {"Elite Automatic Rifle"                  , new Meta("Position0070_UltimateAutomaticRifle"             , $"{PGO}/UltimateAutomaticRifleItem")},
            {"Basic Hand Held Launcher"               , new Meta("Position0080_BasicHandHeldLauncher"              , $"{PGO}/BasicHandHeldLauncherItem")},
            {"Advanced Hand Held Launcher"            , new Meta("Position0090_AdvancedHandHeldLauncher"           , $"{PGO}/AdvancedHandHeldLauncherItem")},
            {"Angle Grinder"                          , new Meta("Position0010_AngleGrinder"                       , $"{PGO}/AngleGrinderItem")},
            {"Angle Grinder 2"                        , new Meta("Position0020_AngleGrinder2"                      , $"{PGO}/AngleGrinder2Item")},
            {"Angle Grinder 3"                        , new Meta("Position0030_AngleGrinder3"                      , $"{PGO}/AngleGrinder3Item")},
            {"Angle Grinder 4"                        , new Meta("Position0040_AngleGrinder4"                      , $"{PGO}/AngleGrinder4Item")},
            {"Hand Drill"                             , new Meta("Position0050_HandDrill"                          , $"{PGO}/HandDrillItem")},
            {"Hand Drill 2"                           , new Meta("Position0060_HandDrill2"                         , $"{PGO}/HandDrill2Item")},
            {"Hand Drill 3"                           , new Meta("Position0070_HandDrill3"                         , $"{PGO}/HandDrill3Item")},
            {"Hand Drill 4"                           , new Meta("Position0080_HandDrill4"                         , $"{PGO}/HandDrill4Item")},
            {"Welder"                                 , new Meta("Position0090_Welder"                             , $"{PGO}/WelderItem")},
            {"Welder 2"                               , new Meta("Position0100_Welder2"                            , $"{PGO}/Welder2Item")},
            {"Welder 3"                               , new Meta("Position0110_Welder3"                            , $"{PGO}/Welder3Item")},
            {"Welder 4"                               , new Meta("Position0120_Welder4"                            , $"{PGO}/Welder4Item")},
            {"Semi-Auto Pistol Magazine"              , new Meta("Position0010_SemiAutoPistolMagazine"             , $"{AM}/SemiAutoPistolMagazine")},
            {"Full-Auto Pistol Magazine"              , new Meta("Position0020_FullAutoPistolMagazine"             , $"{AM}/FullAutoPistolMagazine")},
            {"Elite Pistol Magazine"                  , new Meta("Position0030_ElitePistolMagazine"                , $"{AM}/ElitePistolMagazine")},
            {"Automatic Rifle Gun Mag 20rd"           , new Meta("Position0040_AutomaticRifleGun_Mag_20rd"         , $"{AM}/AutomaticRifleGun_Mag_20rd")},
            {"Rapid-Fire Automatic Rifle Gun Mag 50rd", new Meta("Position0050_RapidFireAutomaticRifleGun_Mag_50rd", $"{AM}/RapidFireAutomaticRifleGun_Mag_50rd")},
            {"Precise Automatic Rifle Gun Mag 5rd"    , new Meta("Position0060_PreciseAutomaticRifleGun_Mag_5rd"   , $"{AM}/PreciseAutomaticRifleGun_Mag_5rd")},
            {"Ultimate Automatic Rifle Gun Mag 30rd"  , new Meta("Position0070_UltimateAutomaticRifleGun_Mag_30rd" , $"{AM}/UltimateAutomaticRifleGun_Mag_30rd")},
            {"NATO 25x184mm"                          , new Meta("Position0080_NATO_25x184mmMagazine"              , $"{AM}/NATO_25x184mm")},
            {"AutoCannon Clip"                        , new Meta("Position0090_AutocannonClip"                     , $"{AM}/AutocannonClip")},
            {"Missile 200mm"                          , new Meta("Position0100_Missile200mm"                       , $"{AM}/Missile200mm")},
            {"Medium Calibre Ammo"                    , new Meta("Position0110_MediumCalibreAmmo"                  , $"{AM}/MediumCalibreAmmo")},
            {"Large Calibre Ammo"                     , new Meta("Position0120_LargeCalibreAmmo"                   , $"{AM}/LargeCalibreAmmo")},
            {"Small Railgun Ammo"                     , new Meta("Position0130_SmallRailgunAmmo"                   , $"{AM}/SmallRailgunAmmo")},
            {"Large Railgun Ammo"                     , new Meta("Position0140_LargeRailgunAmmo"                   , $"{AM}/LargeRailgunAmmo")},
            {"Zone Chip"                              , new Meta("ZoneChip"                                        , $"{C}/ZoneChip")},
            {"Engineer Plushie"                       , new Meta("EngineerPlushie"                                 , $"{C}/EngineerPlushie")},
            {"Sabiroid Plushie"                       , new Meta("SabiroidPlushie"                                 , $"{C}/SabiroidPlushie")},
            {"Prototech Panel"                        , new Meta("PrototechScrap"                                  , "Ingot/PrototechScrap")},
            {"Prototech Panel"                        , new Meta("PrototechFrame"                                  , $"{C}/PrototechFrame")},
            {"Prototech Panel"                        , new Meta("PrototechPanel"                                  , $"{C}/PrototechPanel")},
            {"Prototech Capacitor"                    , new Meta("PrototechCapacitor"                              , $"{C}/PrototechCapacitor")},
            {"Prototech Propulsion Unit"              , new Meta("PrototechPropulsionUnit"                         , $"{C}/PrototechPropulsionUnit")},
            {"Prototech Machinery"                    , new Meta("PrototechMachinery"                              , $"{C}/PrototechMachinery")},
            {"Prototech Circuitry"                    , new Meta("PrototechCircuitry"                              , $"{C}/PrototechCircuitry")},
            {"Prototech Cooling Unit"                 , new Meta("PrototechCoolingUnit"                            , $"{C}/PrototechCoolingUnit")},
            {"Cooked Mammal Meat"                     , new Meta("Position0010_CookMammalMeat"                     , $"{CI}/MammalMeatCooked")},
            {"Cooked Spider Meat"                     , new Meta("Position0020_CookSpiderMeat"                     , $"{CI}/InsectMeatCooked")},
            {"Kelp Crisp"                             , new Meta("Position0030_MealPack_KelpCrisp"                 , $"{CI}/MealPack_KelpCrisp")},
            {"Fruit Bar"                              , new Meta("Position0040_MealPack_FruitBar"                  , $"{CI}/MealPack_FruitBar")},
            {"Garden Slaw"                            , new Meta("Position0050_MealPack_GardenSlaw"                , $"{CI}/MealPack_GardenSlaw")},
            {"Red Pellets"                            , new Meta("Position0060_MealPack_RedPellets"                , $"{CI}/MealPack_RedPellets")},
            {"Chili"                                  , new Meta("Position0070_MealPack_Chili"                     , $"{CI}/MealPack_Chili")},
            {"Flatbread"                              , new Meta("Position0080_MealPack_Flatbread"                 , $"{CI}/MealPack_Flatbread")},
            {"Ramen"                                  , new Meta("Position0090_MealPack_Ramen"                     , $"{CI}/MealPack_Ramen")},
            {"Fruit Pastry"                           , new Meta("Position0100_MealPack_FruitPastry"               , $"{CI}/MealPack_FruitPastry")},
            {"Veggie Burger"                          , new Meta("Position0110_MealPack_VeggieBurger"              , $"{CI}/MealPack_VeggieBurger")},
            {"Curry"                                  , new Meta("Position0120_MealPack_Curry"                     , $"{CI}/MealPack_Curry")},
            {"Green Pellets"                          , new Meta("Position0130_MealPack_GreenPellets"              , $"{CI}/MealPack_GreenPellets")},
            {"Dumplings"                              , new Meta("Position0140_MealPack_Dumplings"                 , $"{CI}/MealPack_Dumplings")},
            {"Spaghetti"                              , new Meta("Position0150_MealPack_Spaghetti"                 , $"{CI}/MealPack_Spaghetti")},
            {"Lasagna"                                , new Meta("Position0160_MealPack_Lasagna"                   , $"{CI}/MealPack_Lasagna")},
            {"Burrito"                                , new Meta("Position0170_MealPack_Burrito"                   , $"{CI}/MealPack_Burrito")},
            {"Frontier Stew"                          , new Meta("Position0180_MealPack_FrontierStew"              , $"{CI}/MealPack_FrontierStew")},
            {"Seared Sabiroid"                        , new Meta("Position0190_MealPack_SearedSabiroid"            , $"{CI}/MealPack_SearedSabiroid")},
            {"Steak Dinner"                           , new Meta("Position0200_MealPack_SteakDinner"               , $"{CI}/MealPack_SteakDinner")},
            {"Fruit Seeds"                            , new Meta("Position0010_Seeds_Fruit"                        , "SeedItem/Fruit")},
            {"Grain Seeds"                            , new Meta("Position0020_Seeds_Grain"                        , "SeedItem/Grain")},
            {"Vegetable Seeds"                        , new Meta("Position0030_Seeds_Vegetables"                   , "SeedItem/Vegetables")},
            {"Mushroom Spores"                        , new Meta("Position0040_Spores_Mushrooms"                   , "SeedItem/Mushrooms")},

        };
    }
}