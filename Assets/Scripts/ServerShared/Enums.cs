/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;

public enum ItemRotation
{
    None = 0,
    CounterClockwise = 1,
    Reversed = 2,
    Clockwise = 3
}

public enum HitType
{
    Armor,
    Hardpoint,
    Gear,
    Thermal
}

public enum LauncherCaliber
{
    Micro,
    Missile,
    Fighter
}

public enum HardpointType
{
    Hull,
    Tool,
    Thermal,
    Thruster,
    WarpThruster,
    Reactor,
    Radiator,
    Shield,
    Sensors,
    Energy,
    Ballistic,
    Launcher,
    ControlModule
}

public enum WeaponType
{
    ElectromagneticallyPropelled,
    ExplosivelyPropelled,
    Laser,
    Electrostatic,
    ParticleProjection,
    Missile,
    MicroMissile,
    SplitMissile,
    Mine,
    Jet
}

public enum WeaponRange
{
    Melee,
    Short,
    Medium,
    Long
}

public enum WeaponCaliber
{
    Small,
    Medium,
    Large,
    ExtraLarge
}

[Flags]
public enum WeaponFireType
{
    None = 0,
    Direct = 1 << 0,
    Guided = 1 << 1,
    Seeking = 1 << 2,
    Continuous = 1 << 3,
    Charged = 1 << 4
}

[Flags]
public enum WeaponModifiers
{
    None = 0,
    Airburst = 1 << 0,
    Incendiary = 1 << 1,
    ArmorPenetrating = 1 << 2,
    NegativeEntropy = 1 << 3,
    RapidFire = 1 << 4,
    Burst = 1 << 5,
    Cluster = 1 << 6
}

public enum DamageType
{
    Kinetic,
    Corrosive,
    Electric,
    Thermal,
    Optical,
    Ionizing
}

[Flags]
public enum BodyType
{
    None = 0,
    Asteroid = 1 << 0,
    Planetoid = 1 << 1,
    Planet = 1 << 2,
    GasGiant = 1 << 3,
    Sun = 1 << 4
}

public enum HullType
{
    Ship,
    Station,
    Turret
}

public enum MegaPlacementType
{
    Mass = 0,
    Planets = 1,
    Resources = 2,
    Connected = 3,
    Isolated = 4
}

public enum SimpleCommodityCategory
{
    Minerals = 0,
    Metals = 1,
    Alloys = 2,
    Compounds = 3,
    Organics = 4,
    Ammo = 5,
    Consumer = 6
}

public enum CompoundCommodityCategory
{
    Wearables = 0,
    Consumables = 1,
    Luxuries = 2,
    Tools = 3,
    Manufacturing = 4,
    Assemblies = 5
}