﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MessagePack;
using Newtonsoft.Json;
using Unity.Mathematics;
using static Unity.Mathematics.math;

[MessagePackObject, JsonObject(MemberSerialization.OptIn), EntityTypeRestriction(HullType.Ship), Order(-100)]
public class PatrolControllerData : ControllerData
{
    public override IBehavior CreateInstance(GameContext context, Entity entity, Gear item)
    {
        return new PatrolController(context, this, entity, item);
    }
}

public class PatrolController : IBehavior, IController, IInitializableBehavior
{
    public TaskType TaskType => TaskType.None;
    public bool Available => false;
    public Zone Zone => _entity.Zone;
    public BehaviorData Data => _data;
    
    private PatrolControllerData _data;
    private GameContext _context;
    private Entity _entity;
    private Gear _item;
    private Locomotion _locomotion;
    private Guid _targetOrbit;
    
    public PatrolController(GameContext context, PatrolControllerData data, Entity entity, Gear item)
    {
        _context = context;
        _data = data;
        _entity = entity;
        _item = item;
    }
    
    public void Initialize()
    {
        _locomotion = new Locomotion(_context, _entity, _data);
        RandomTarget();
    }

    public bool Update(float delta)
    {
        _locomotion.Objective = Zone.GetOrbitPosition(_targetOrbit);
        _locomotion.Update(delta);
        
        if(length(_entity.Position - _locomotion.Objective) < _data.TargetDistance)
            RandomTarget();
        
        return true;
    }

    public void AssignTask(Guid task)
    {
        throw new NotImplementedException();
    }
    
    private void RandomTarget()
    {
        _targetOrbit = Zone.Planets.Keys.ToArray()[_context.Random.NextInt(Zone.Planets.Count)];
    }
}
