﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering;
using static Unity.Mathematics.math;
using int2 = Unity.Mathematics.int2;

public class InventoryPanel : MonoBehaviour
{
    public ActionGameManager GameManager;
    public ContextMenu ContextMenu;
    public TextMeshProUGUI Title;
    public Button Dropdown;
    public GridLayoutGroup Grid;
    public Sprite[] NodeBackgroundTextures;
    public Sprite[] NodeTextures;
    public Prototype NodePrototype;
    public RawImage TemperatureDisplay;
    public Texture2D TemperatureColor;
    public ExponentialLerp TemperatureColorCurve;
    public ExponentialLerp TemperatureAlphaCurve;
    public float MaxTemperature;
    public bool FitToContent;
    
    Subject<InventoryEventData> _onBeginDrag;
    Subject<InventoryEventData> _onDrag;
    Subject<InventoryEventData> _onEndDrag;
    Subject<InventoryEventData> _onClick;
    Subject<InventoryEventData> _onPointerEnter;
    Subject<InventoryEventData> _onPointerExit;

    private Entity _displayedEntity;
    private EquippedCargoBay _displayedCargo;
    private Texture2D _temperatureTexture;
    private RectTransform _firstRect;

    public EquippedItem FakeEquipment;
    public ItemInstance FakeItem;
    public Shape FakeOccupancy;
    public Shape IgnoreOccupancy;
    public List<GameObject> EmptyCells = new List<GameObject>();
    public Dictionary<int2, InventoryCell> CellInstances = new Dictionary<int2, InventoryCell>();
    
    private int2[] _offsets = {
        int2(0, 1),
        int2(1, 0),
        int2(0, -1),
        int2(-1, 0),
        int2(1, 1),
        int2(1, -1),
        int2(-1, -1),
        int2(-1, 1)
    };

    private void Start()
    {
        if(Dropdown)
        {
            Dropdown.onClick.AddListener(() =>
            {
                ContextMenu.Clear();
                foreach (var ship in GameManager.AvailableShips())
                {
                    if (ship.CargoBays.Any())
                    {
                        ContextMenu.AddDropdown(ship.Name,
                            ship.CargoBays
                                .Select<EquippedCargoBay, (string text, Action action, bool enabled)>((bay, index) =>
                                    ($"Bay {index}", () => Display(bay), true))
                                .Prepend(("Equipment", () => Display(ship), true)));
                    }
                    else ContextMenu.AddOption(ship.Name, () => Display(ship));
                }

                foreach (var bay in GameManager.AvailableCargoBays())
                {
                    ContextMenu.AddOption(bay.Name, () => Display(bay));
                }

                ContextMenu.Show();
            });
        }
    }

    private void Update()
    {
        if (_displayedEntity != null)
        {
            var hullData = GameManager.ItemManager.GetData(_displayedEntity.Hull) as HullData;
            for(var x = 0; x < _temperatureTexture.width; x++)
            {
                for (var y = 0; y < _temperatureTexture.height; y++)
                {
                    if (hullData.Shape[int2(x-1, y-1)])
                    {
                        var temp = _displayedEntity.Temperature[x - 1, y - 1] / MaxTemperature;
                        var color = TemperatureColor.GetPixelBilinear(TemperatureColorCurve.Evaluate(temp), 0);
                        color.a = TemperatureAlphaCurve.Evaluate(temp);
                        _temperatureTexture.SetPixel(x,y,color);
                    }
                    else
                        _temperatureTexture.SetPixel(x,y,new Color(0,0,0,0));
                }
            }
            _temperatureTexture.Apply();
            TemperatureDisplay.rectTransform.anchoredPosition = _firstRect.anchoredPosition - Vector2.one * Grid.cellSize * 1.5f;
        }
    }

    public void Clear()
    {
        _displayedCargo = null;
        _displayedEntity = null;
        
        foreach(var empty in EmptyCells)
            Destroy(empty);
        EmptyCells.Clear();
        
        foreach(var node in CellInstances.Values)
            node.GetComponent<Prototype>().ReturnToPool();
        CellInstances.Clear();
        
        if(Title)
            Title.text = "None";
    }

    public void Display(Entity entity)
    {
        Clear();

        _displayedEntity = entity;
        _firstRect = null;

        if(Title)
            Title.text = entity.Name;

        var hullData = GameManager.ItemManager.GetData(entity.Hull) as HullData;
        if (FitToContent)
        {
            var gridRect = Grid.GetComponent<RectTransform>();
            var rect = gridRect.rect;
            Grid.cellSize = Vector2.one * (int) min(rect.width / (hullData.Shape.Width + 1), rect.height / (hullData.Shape.Height + 1));
        }
        _temperatureTexture = new Texture2D(
            hullData.Shape.Width+2, 
            hullData.Shape.Height+2, 
            TextureFormat.RGBA32, 
            false, false);
        TemperatureDisplay.gameObject.SetActive(true);
        TemperatureDisplay.texture = _temperatureTexture;
        var tempRect = TemperatureDisplay.rectTransform;
        tempRect.sizeDelta = Grid.cellSize * new Vector2(hullData.Shape.Width + 2, hullData.Shape.Height + 2);
        //tempRect.anchoredPosition = 
        //FakeOccupancy = new Shape(hullData.Shape.Width, hullData.Shape.Height);
        //IgnoreOccupancy = new Shape(hullData.Shape.Width, hullData.Shape.Height);
        Grid.constraintCount = hullData.Shape.Width;
        foreach (var v in hullData.Shape.AllCoordinates)
        {
            if (!hullData.Shape[v])
            {
                var empty = new GameObject("Empty Node", typeof(RectTransform));
                empty.transform.SetParent(Grid.transform);
                if (!_firstRect)
                    _firstRect = empty.GetComponent<RectTransform>();
                EmptyCells.Add(empty);
            }
            else
            {
                var cell = NodePrototype.Instantiate<InventoryCell>();
                if (!_firstRect)
                    _firstRect = cell.GetComponent<RectTransform>();
                cell.PointerClickTrigger.OnPointerClickAsObservable()
                    .Subscribe(data => _onClick?.OnNext(new InventoryEntityEventData(data, v, entity)));
                cell.BeginDragTrigger.OnBeginDragAsObservable()
                    .Subscribe(data => _onBeginDrag?.OnNext(new InventoryEntityEventData(data, v, entity)));
                cell.DragTrigger.OnDragAsObservable()
                    .Subscribe(data => _onDrag?.OnNext(new InventoryEntityEventData(data, v, entity)));
                cell.EndDragTrigger.OnEndDragAsObservable()
                    .Subscribe(data => _onEndDrag?.OnNext(new InventoryEntityEventData(data, v, entity)));
                cell.PointerEnterTrigger.OnPointerEnterAsObservable()
                    .Subscribe(data => _onPointerEnter?.OnNext(new InventoryEntityEventData(data, v, entity)));
                cell.PointerExitTrigger.OnPointerExitAsObservable()
                    .Subscribe(data => _onPointerExit?.OnNext(new InventoryEntityEventData(data, v, entity)));
                CellInstances.Add(v, cell);
            }
        }
        RefreshCells(CellInstances.Keys);
    }

    public void Display(EquippedCargoBay cargo)
    {
        Clear();
        
        _displayedCargo = cargo;
        TemperatureDisplay.gameObject.SetActive(false);

        if(Title)
            Title.text = cargo.Name;
        
        // FakeOccupancy = new Shape(cargo.Data.InteriorShape.Width, cargo.Data.InteriorShape.Height);
        // IgnoreOccupancy = new Shape(cargo.Data.InteriorShape.Width, cargo.Data.InteriorShape.Height);
        Grid.constraintCount = cargo.Data.InteriorShape.Width;
        foreach (var v in cargo.Data.InteriorShape.AllCoordinates)
        {
            if (!cargo.Data.InteriorShape[v])
            {
                var empty = new GameObject("Empty Node", typeof(RectTransform));
                empty.transform.SetParent(Grid.transform);
                EmptyCells.Add(empty);
            }
            else
            {
                var cell = NodePrototype.Instantiate<InventoryCell>();
                cell.PointerClickTrigger.OnPointerClickAsObservable()
                    .Subscribe(data => _onClick?.OnNext(new InventoryCargoEventData(data, v, cargo)));
                cell.BeginDragTrigger.OnBeginDragAsObservable()
                    .Subscribe(data => _onBeginDrag?.OnNext(new InventoryCargoEventData(data, v, cargo)));
                cell.DragTrigger.OnDragAsObservable()
                    .Subscribe(data => _onDrag?.OnNext(new InventoryCargoEventData(data, v, cargo)));
                cell.EndDragTrigger.OnEndDragAsObservable()
                    .Subscribe(data => _onEndDrag?.OnNext(new InventoryCargoEventData(data, v, cargo)));
                cell.PointerEnterTrigger.OnPointerEnterAsObservable()
                    .Subscribe(data => _onPointerEnter?.OnNext(new InventoryCargoEventData(data, v, cargo)));
                cell.PointerExitTrigger.OnPointerExitAsObservable()
                    .Subscribe(data => _onPointerExit?.OnNext(new InventoryCargoEventData(data, v, cargo)));
                CellInstances.Add(v, cell);
            }
        }
        RefreshCells();
    }

    public void RefreshCells()
    {
        RefreshCells(CellInstances.Keys);
    }

    public void RefreshCells(IEnumerable<int2> cells)
    {
        if (_displayedEntity != null)
        {
            foreach (var v in cells)
            {
                if(!CellInstances.ContainsKey(v)) continue;
                
                var hullData = GameManager.ItemManager.GetData(_displayedEntity.Hull) as HullData;
            
                var spriteIndex = 0;
                var interior = hullData.InteriorCells[v];
                var item = FakeOccupancy?[v]??false ? FakeEquipment : IgnoreOccupancy?[v]??false ? null : _displayedEntity.GearOccupancy[v.x, v.y];
                var hardpoint = _displayedEntity.Hardpoints[v.x, v.y];

                bool HardpointMatch(int2 offset)
                {
                    var v2 = v + offset;
                    return !(
                        hullData.Shape[v2] &&
                        _displayedEntity.Hardpoints[v2.x, v2.y] == hardpoint &&
                        (FakeOccupancy?[v2]??false ? FakeEquipment : IgnoreOccupancy?[v2]??false ? null : _displayedEntity.GearOccupancy[v2.x, v2.y]) == item
                    );
                }

                bool NoHardpointMatch(int2 offset)
                {
                    var v2 = v + offset;
                    return !(
                        hullData.Shape[v2] && (
                            !interior && !hullData.InteriorCells[v2] && _displayedEntity.Hardpoints[v2.x, v2.y] == null ||
                            interior && item != null && 
                            (FakeOccupancy?[v2]??false ? FakeEquipment : IgnoreOccupancy?[v2]??false ? null : _displayedEntity.GearOccupancy[v2.x, v2.y]) == item
                        )
                    );
                }

                if (hardpoint != null)
                {
                    for(int i = 0; i < 8; i++)
                        if (HardpointMatch(_offsets[i]))
                            spriteIndex += 1 << i;
                }
                else
                {
                    for(int i = 0; i < 8; i++)
                        if (NoHardpointMatch(_offsets[i]))
                            spriteIndex += 1 << i;
                }

                var bgSpriteIndex = spriteIndex;

                if (item != null)
                    spriteIndex += 1 << 8;
            
                CellInstances[v].Background.sprite = NodeBackgroundTextures[bgSpriteIndex];
                CellInstances[v].Icon.sprite = NodeTextures[spriteIndex];
                CellInstances[v].Icon.color = GetColor(v);
            }
        }
        else if(_displayedCargo != null)
        {
            foreach (var v in cells)
            {
                if(!CellInstances.ContainsKey(v)) continue;
                
                var spriteIndex = 0;
                var item = FakeOccupancy?[v]??false ? FakeItem : IgnoreOccupancy?[v]??false ? null : _displayedCargo.Occupancy[v.x, v.y];

                bool ItemMatch(int2 offset)
                {
                    var v2 = v + offset;
                    return !_displayedCargo.Data.InteriorShape[v2] || item == null ||
                           (FakeOccupancy?[v2]??false ? FakeItem : IgnoreOccupancy?[v2]??false ? null : _displayedCargo.Occupancy[v2.x, v2.y]) != item;
                }

                for(int i = 0; i < 8; i++)
                    if (ItemMatch(_offsets[i]))
                        spriteIndex += 1 << i;
                
                var bgSpriteIndex = spriteIndex;

                if (item != null)
                    spriteIndex += 1 << 8;
            
                CellInstances[v].Background.sprite = NodeBackgroundTextures[bgSpriteIndex];
                CellInstances[v].Icon.sprite = NodeTextures[spriteIndex];
                CellInstances[v].Icon.color = GetColor(v);
            }
        }
    }

    public Color GetColor(int2 position, bool highlight = false)
    {
        if (_displayedEntity != null)
        {
            var hullData = GameManager.ItemManager.GetData(_displayedEntity.Hull) as HullData;
            var interior = hullData.InteriorCells[position];
            var item = FakeOccupancy?[position]??false ? FakeEquipment : IgnoreOccupancy?[position]??false ? null : _displayedEntity.GearOccupancy[position.x, position.y];
            var hardpoint = _displayedEntity.Hardpoints[position.x, position.y];

            if (hardpoint == null)
            {
                if(!interior)
                    return Color.white;
                
                if (item == null)
                {
                    return Color.white * .25f;
                }
                
                if(highlight)
                    return Color.white;
                
                return Color.white * .5f;
            }

            var tint = hardpoint.TintColor;
            
            if (!highlight || item == null)
                tint *= .7071f;

            return tint.ToColor();
        }
        else
        {
            var item = FakeOccupancy?[position] ?? false ? FakeItem : IgnoreOccupancy?[position] ?? false ? null : _displayedCargo.Occupancy[position.x, position.y];
        
            if (item == null)
                return Color.white * .25f;

            var c = float3(1);
            if (GameManager.ItemManager.GetData(item) is EquippableItemData equippable)
                c = HardpointData.GetColor(equippable.HardpointType);
            
            if(!highlight)
                c *= .7071f;

            return c.ToColor();
        }
    }

    public bool DropItem(ItemInstance item, int2 position)
    {
        if (_displayedEntity != null && item is EquippableItem equippableItem)
        {
            return _displayedEntity.TryEquip(equippableItem, position);
        }

        if(_displayedCargo != null)
        {
            return _displayedCargo.TryStore(item, position);
        }

        return false;
    }
    
    public UniRx.IObservable<InventoryEventData> OnClickAsObservable() => _onClick ?? (_onClick = new Subject<InventoryEventData>());
    public UniRx.IObservable<InventoryEventData> OnBeginDragAsObservable() => _onBeginDrag ?? (_onBeginDrag = new Subject<InventoryEventData>());
    public UniRx.IObservable<InventoryEventData> OnDragAsObservable() => _onDrag ?? (_onDrag = new Subject<InventoryEventData>());
    public UniRx.IObservable<InventoryEventData> OnEndDragAsObservable() => _onEndDrag ?? (_onEndDrag = new Subject<InventoryEventData>());
    public UniRx.IObservable<InventoryEventData> OnPointerEnterAsObservable() => _onPointerEnter ?? (_onPointerEnter = new Subject<InventoryEventData>());
    public UniRx.IObservable<InventoryEventData> OnPointerExitAsObservable() => _onPointerExit ?? (_onPointerExit = new Subject<InventoryEventData>());
}

public abstract class InventoryEventData
{
    protected InventoryEventData(PointerEventData pointerEventData, int2 position)
    {
        PointerEventData = pointerEventData;
        Position = position;
    }

    public PointerEventData PointerEventData { get; }
    public int2 Position { get; }
}

public class InventoryEntityEventData : InventoryEventData
{
    public InventoryEntityEventData(PointerEventData pointerEventData, int2 position, Entity entity) : 
        base(pointerEventData, position)
    {
        Entity = entity;
    }

    public Entity Entity { get; }
}

public class InventoryCargoEventData : InventoryEventData
{
    public InventoryCargoEventData(PointerEventData pointerEventData, int2 position, EquippedCargoBay cargoBay) : 
        base(pointerEventData, position)
    {
        CargoBay = cargoBay;
    }

    public EquippedCargoBay CargoBay { get; }
}
