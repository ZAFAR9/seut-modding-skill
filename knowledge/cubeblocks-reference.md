# CubeBlocks Definition Reference

Source: https://spaceengineers.wiki.gg/wiki/Modding/Reference/SBC/CubeBlocks
+ sub-agent SBC schema research. Archived: 2026-07-17

## Core definition elements

- **`<Definitions>`** — root wrapper for all definitions in the file.
- **`<Definition xsi:type="...">`** — one definition; `xsi:type` names the schema
  (e.g. `MyObjectBuilder_CubeBlockDefinition`).
- **`<Id>`** — unique index. Contains:
  - **`<TypeId>`** — engine class (`CubeBlock`, `OxygenGenerator`, `Thrust`, …).
  - **`<SubtypeId>`** — unique variant name (e.g. `MyHeavyArmorSlope`).
- **`<DisplayName>` / `<Description>`** — UI text (can be string IDs for i18n).
- **`<Icon>`** — path to `.dds` thumbnail, usually `Textures\Icons\...`.
- **`<CubeSize>`** — `Large` (2.5m grid) or `Small` (0.5m grid).
- **`<Size x="" y="" z="" />`** — footprint in grid cells.
- **`<Model>`** — relative path to the completed `.mwm`, e.g.
  `Models\Cues\MyBlock.mwm`.
- **`<BlockPairName>`** — groups the large- and small-grid versions under one
  toolbar icon; the engine picks the right one based on the grid.
- **`<Components>`** — bill of materials + functional threshold. Each
  `<Component Subtype="SteelPlate" Count="20" />`. The final/critical component
  represents structural integrity.
- **`<CriticalComponent Subtype="" Index="" />`** — which component, when
  destroyed, destroys the block.
- **`<MountPoints>`** — faces where the block can weld/snap to neighbors.
  `<MountPoint Side="Back" StartX="0" StartY="0" EndX="1" EndY="1" />`.
  No mount points = block can't attach to anything. Also affects airtightness.
- **`<BuildProgressModels>`** — construction-stage models that swap as the block
  is welded/ground:
  `<Model BuildPercentUpperBound="0.33" File="Models\Cues\MyBlock_BS1.mwm" />`
  (SEUT export note: the wiki uses `BuildPercentUpperBound`; some older docs show
  `BuildPercentFraction` — verify against your game version).
- **`<EdgeType>`** — `Light` or `Heavy` (armor edge visuals).
- **`<BuildTimeSeconds>`**, **`<DeformationRatio>`**, **`<PCU>`** — physics/perf.
- **`<DamageEffectName>` / `<DamagedSound>`** — damage FX.

## Blocks table — TypeId ↔ SBC definition (xsi:type)

Blocks using **CubeBlock Definition** need NO `xsi:type`. Others require the
listed definition. "EntityComp." = conveyor via EntityComponents;
"Inherent" = conveyor built into the type.

| TypeId | SBC definition | Notes |
|---|---|---|
| CubeBlock | CubeBlock Definition | Decorative |
| EmissiveBlock | CubeBlock Definition | |
| TerminalBlock | CubeBlock Definition | |
| ControlPanel | CubeBlock Definition | vanilla uses TerminalBlock instead |
| FunctionalBlock | FunctionalBlock Definition | base on/off; Farm Plot, Algae Farm |
| ExhaustBlock | ExhaustBlock Definition | |
| HeatVentBlock | HeatVentBlock Definition | |
| Ladder2 | CubeBlock Definition | climbable |
| Kitchen | Kitchen Definition | decorative |
| Passage | CubeBlock Definition | decorative |
| Planter | Planter Definition | decorative |
| Door | Door Definition | 2 subparts slide out |
| AdvancedDoor | AdvancedDoor Definition | highly customizable |
| AirtightHangarDoor | AirtightHangarDoor Definition | stacked sliding subparts |
| AirtightSlideDoor | AirtightSlideDoor Definition | bus-door style |
| Conveyor | CubeBlock Definition | conveyor hub |
| ConveyorConnector | CubeBlock Definition | conveyor tube |
| ConveyorSorter | ConveyorSorter Definition | |
| ShipConnector | ShipConnector Definition | |
| Collector | PoweredCargoContainer Definition | |
| CargoContainer | CargoContainer Definition | |
| OxygenFarm | OxygenFarm Definition | |
| OxygenGenerator | OxygenGenerator Definition | O2/H2 generator |
| OxygenTank | GasTank Definition | hydrogen tanks too |
| AirVent | AirVent Definition | |
| BatteryBlock | BatteryBlock Definition | |
| HydrogenEngine | HydrogenEngine Definition | |
| Reactor | Reactor Definition | |
| SolarPanel | SolarPanel Definition | |
| WindTurbine | WindTurbine Definition | |
| InteriorLight | LightingBlock Definition | point light |
| ReflectorLight | ReflectorBlock Definition | spotlight |
| Searchlight | Searchlight Definition | light on a turret |
| ButtonPanel | ButtonPanel Definition | |
| TextPanel | TextPanel Definition | single LCD |
| LCDPanelsBlock | LCDPanelsBlock Definition | |
| Cockpit | Cockpit Definition | seats, couches, anything you sit in |
| CryoChamber | CryoChamber Definition | |
| Gyro | Gyro Definition | |
| Thrust | Thrust Definition | |
| LandingGear | LandingGear Definition | |
| Parachute | Parachute Definition | |
| JumpDrive | JumpDrive Definition | |
| CameraBlock | CameraBlock Definition | |
| RemoteControl | RemoteControl Definition | |
| BasicMissionBlock | BasicMissionBlock Definition | AI Basic (Task) |
| FlightMovementBlock | FlightMovementBlock Definition | |
| PathRecorderBlock | PathRecorderBlock Definition | |
| OffensiveCombatBlock | OffensiveCombatBlock Definition | |
| DefensiveCombatBlock | DefensiveCombatBlock Definition | |
| InteriorTurret | LargeTurretBase Definition | ignore "Large" in name |
| LargeGatlingTurret | LargeTurretBase Definition | |
| LargeMissileTurret | LargeTurretBase Definition | |
| SmallGatlingGun | WeaponBlock Definition | ignore "Small" in name |
| SmallMissileLauncher | WeaponBlock Definition | |
| SmallMissileLauncherReload | WeaponBlock Definition | |
| Warhead | Warhead Definition | |
| Decoy | Decoy Definition | |
| TargetDummyBlock | TargetDummyBlock Definition | |
| TurretControlBlock | TurretControlBlock Definition | custom turret controller |
| Beacon | Beacon Definition | |
| RadioAntenna | RadioAntenna Definition | |
| LaserAntenna | LaserAntenna Definition | |
| TransponderBlock | TransponderBlock Definition | Action Relay |
| BroadcastController | BroadcastController Definition | |
| EmotionControllerBlock | EmotionControllerBlock Definition | |
| EventControllerBlock | EventControllerBlock Definition | |
| TimerBlock | TimerBlock Definition | |
| SensorBlock | SensorBlock Definition | |
| MyProgrammableBlock | ProgrammableBlock Definition | |
| SoundBlock | SoundBlock Definition | |
| Jukebox | Jukebox Definition | |
| ExtendedPistonBase | ExtendedPistonBase Definition | |
| PistonTop | CubeBlock Definition | piston head |
| MotorStator | MotorStator Definition | rotors & hinges |
| MotorRotor | CubeBlock Definition | rotor/hinge top |
| MotorAdvancedStator | MotorAdvancedStator Definition | |
| MotorAdvancedRotor | CubeBlock Definition | |
| MotorSuspension | MotorSuspension Definition | |
| Wheel | CubeBlock Definition | suspension/standalone wheels |
| RealWheel | CubeBlock Definition | unused by vanilla |
| OreDetector | OreDetector Definition | |
| Drill | ShipDrill Definition | |
| ShipGrinder | ShipGrinder Definition | |
| ShipWelder | ShipWelder Definition | |
| Projector | Projector Definition | buildable or miniature |
| MergeBlock | MergeBlock Definition | |
| Refinery | Refinery Definition | |
| Assembler | Assembler Definition | |
| UpgradeModule | UpgradeModule Definition | |
| SurvivalKit | SurvivalKit Definition | |
| MedicalRoom | MedicalRoom Definition | |
| GravityGenerator | GravityGenerator Definition | |
| GravityGeneratorSphere | GravityGeneratorSphere Definition | |
| VirtualMass | VirtualMass Definition | artificial mass |
| SpaceBall | SpaceBall Definition | |
| SafeZoneBlock | SafeZoneBlock Definition | |
| StoreBlock | StoreBlock Definition | |
| VendingMachine | VendingMachine Definition | |
| ContractBlock | ContractBlock Definition | |

## Minimal well-formed CubeBlock example

```xml
<?xml version="1.0" encoding="utf-8"?>
<Definitions xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
             xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <CubeBlocks>
    <Definition xsi:type="MyObjectBuilder_CubeBlockDefinition">
      <Id>
        <TypeId>CubeBlock</TypeId>
        <SubtypeId>MyCustomArmorWall</SubtypeId>
      </Id>
      <DisplayName>Custom Heavy Armor Wall</DisplayName>
      <Description>A thick structural wall.</Description>
      <Icon>Textures\Icons\MyCustomBlock_Icon.dds</Icon>
      <CubeSize>Large</CubeSize>
      <BlockTopology>TriangleMesh</BlockTopology>
      <Size x="1" y="1" z="1" />
      <ModelOffset x="0" y="0" z="0" />
      <Model>Models\Cues\MyCustomBlock_Large.mwm</Model>
      <Components>
        <Component Subtype="SteelPlate" Count="100" />
        <Component Subtype="MetalGrid" Count="30" />
        <Component Subtype="Construction" Count="20" />
        <Component Subtype="SteelPlate" Count="50" />
      </Components>
      <CriticalComponent Subtype="SteelPlate" Index="1" />
      <BuildProgressModels>
        <Model BuildPercentUpperBound="0.33" File="Models\Cues\MyBlock_BS1.mwm" />
        <Model BuildPercentUpperBound="0.66" File="Models\Cues\MyBlock_BS2.mwm" />
      </BuildProgressModels>
      <BlockPairName>CustomArmorWall</BlockPairName>
      <EdgeType>Heavy</EdgeType>
      <BuildTimeSeconds>15</BuildTimeSeconds>
      <MountPoints>
        <MountPoint Side="Back"   StartX="0" StartY="0"   EndX="1" EndY="1" />
        <MountPoint Side="Bottom" StartX="0" StartY="0"   EndX="1" EndY="0.2" />
        <MountPoint Side="Top"    StartX="0" StartY="0.8" EndX="1" EndY="1" />
      </MountPoints>
      <DeformationRatio>0.2</DeformationRatio>
      <PCU>1</PCU>
    </Definition>
  </CubeBlocks>
</Definitions>
```
