﻿using System;
using System.Collections.Generic;
using System.Linq;
using MessagePack;
using Newtonsoft.Json;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Random = Unity.Mathematics.Random;

[MessagePackObject, JsonObject(MemberSerialization.OptIn)]
public class FactoryData : BehaviorData
{
    [InspectableField, JsonProperty("toolingTime"), Key(1)]
    public PerformanceStat ToolingTime = new PerformanceStat();

    [InspectableField, JsonProperty("automation"), Key(2)]
    public int AutomationPoints;
    
    [InspectableDatabaseLink(typeof(PersonalityAttribute)), JsonProperty("productionProfile"), Key(3)]  
    public Dictionary<Guid, float> ProductionProfile = new Dictionary<Guid, float>();
    
    public override IBehavior CreateInstance(GameContext context, Entity entity, Gear item)
    {
        return new Factory(context, this, entity, item);
    }
}

public class Factory : IBehavior, IPersistentBehavior
{
    public float ProductionQuality;
    public double RetoolingTime;
    public string ItemName;
    public Guid ItemUnderConstruction;
    public bool Active;
    
    public Entity Entity { get; }
    public Gear Item { get; }
    public GameContext Context { get; }

    public BehaviorData Data => _data;
    
    public float ToolingTime { get; private set; }
    
    public Guid Blueprint
    {
        get => _blueprint;
        set
        {
            if (value != _blueprint)
            {
                _blueprint = value;
                Active = false;
                if(value != Guid.Empty)
                {
                    ItemName = Context.Cache.Get<ItemData>(Context.Cache.Get<BlueprintData>(value).Item).Name;
                    RetoolingTime = ToolingTime;
                    _retooling = true;
                }
                Item.Change();
            }
        }
    }
    
    private FactoryData _data;
    
    private Guid _blueprint;
    private int _assignedPopulation;
    private bool _retooling = false;
    private float _currentProductionQuality;

    public Factory(GameContext context, FactoryData data, Entity entity, Gear item)
    {
        Context = context;
        _data = data;
        Entity = entity;
        Item = item;
    }

    public bool Update(float delta)
    {
        ToolingTime = Context.Evaluate(_data.ToolingTime, Item, Entity);
        var blueprint = Context.Cache.Get<BlueprintData>(_blueprint);
        if (blueprint == null)
            return false;

        if (ItemUnderConstruction != Guid.Empty)
        {
            Entity.IncompleteCargo[ItemUnderConstruction] =
                Entity.IncompleteCargo[ItemUnderConstruction] - delta * (_assignedPopulation + _data.AutomationPoints) /
                pow(lerp(blueprint.QualityFloor, 1, saturate(_currentProductionQuality)),
                    blueprint.ProductionExponent);
            
            if(Entity.IncompleteCargo[ItemUnderConstruction] < 0)
            {
                var item = Context.Cache.Get<ItemInstance>(ItemUnderConstruction);
                if (item is SimpleCommodity simpleCommodity)
                    Entity.AddCargo(simpleCommodity);
                else if (item is CraftedItemInstance craftedItemInstance)
                    Entity.AddCargo(craftedItemInstance);
                Entity.IncompleteCargo.Remove(ItemUnderConstruction);
                ItemUnderConstruction = Guid.Empty;
                Item.Change();
            }

            return true;
        }
        
        if (RetoolingTime > 0)
        {
            RetoolingTime -= delta;
            return false;
        }

        if (_retooling)
        {
            _retooling = false;
            Item.Change();
        }

        if (Active)
        {
            // Applying exponents to two random numbers and adding them produces a range of interesting probability distributions for quality
            ItemUnderConstruction = Entity.Build(blueprint, blueprint.Quality *
                pow(ProductionQuality, blueprint.QualityExponent) *
                (pow(Context.Random.NextFloat(), blueprint.RandomExponent) +
                 pow(Context.Random.NextFloat(), blueprint.RandomExponent)) / 2, ItemName);
        
            if(ItemUnderConstruction!=Guid.Empty)
                Item.Change();
        }
        
        return false;
    }

    // TODO Update Factory Persistence
    public PersistentBehaviorData Store()
    {
        return new FactoryPersistence
        {
            Blueprint = _blueprint,
            RetoolingTime = RetoolingTime,
            AssignedPopulation = _assignedPopulation,
            ProductionQuality = ProductionQuality
        };
    }

    public void Restore(PersistentBehaviorData data)
    {
        var factoryPersistence = data as FactoryPersistence;
        _blueprint = factoryPersistence.Blueprint;
        RetoolingTime = factoryPersistence.RetoolingTime;
        _assignedPopulation = factoryPersistence.AssignedPopulation;
        ProductionQuality = factoryPersistence.ProductionQuality;
    }
}

[MessagePackObject, JsonObject(MemberSerialization.OptIn)]
public class FactoryPersistence : PersistentBehaviorData
{
    [JsonProperty("blueprint"), Key(0)] public Guid Blueprint;
    [JsonProperty("retoolingTime"), Key(2)] public double RetoolingTime;
    [JsonProperty("assignedPopulation"), Key(5)] public int AssignedPopulation;
    [JsonProperty("productionQuality"), Key(6)] public float ProductionQuality;
}