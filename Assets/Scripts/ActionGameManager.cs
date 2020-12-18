﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cinemachine;
using MessagePack;
using TMPro;
using UniRx;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Unity.Mathematics.math;
using float2 = Unity.Mathematics.float2;

public class ActionGameManager : MonoBehaviour
{
    public GameSettings Settings;
    public SectorRenderer SectorRenderer;
    // public MapView MapView;
    public CinemachineVirtualCamera TopDownCamera;
    public CinemachineVirtualCamera FollowCamera;
    public GameObject GameplayUI;
    public MenuPanel Menu;
    public InventoryMenu Inventory;
    
    //public PlayerInput Input;
    
    private DirectoryInfo _filePath;
    private bool _editMode;
    private float _time;
    private AetheriaInput _input;
    private int _zoomLevelIndex;
    public List<Ship> PlayerShips { get; } = new List<Ship>();
    
    public DatabaseCache ItemData { get; private set; }
    public ItemManager ItemManager { get; private set; }
    public Zone Zone { get; private set; }

    void Start()
    {
        ConsoleController.MessageReceiver = this;
        _filePath = new DirectoryInfo(Application.dataPath).Parent.CreateSubdirectory("GameData");
        ItemData = new DatabaseCache();
        ItemData.Load(_filePath.FullName);
        ItemManager = new ItemManager(ItemData, Settings.GameplaySettings, Debug.Log);

        var zoneFile = Path.Combine(_filePath.FullName, "Home.zone");
        
        // If the game has already been run, there will be a Home file containing a ZonePack; if not, generate one
        var zonePack = File.Exists(zoneFile) ? 
            MessagePackSerializer.Deserialize<ZonePack>(File.ReadAllBytes(zoneFile)): 
            ZoneGenerator.GenerateZone(
                settings: Settings.ZoneSettings,
                // mapLayers: Context.MapLayers,
                // resources: Context.Resources,
                mass: Settings.DefaultZoneMass,
                radius: Settings.DefaultZoneRadius
            );
        
        Zone = new Zone(Settings.PlanetSettings, zonePack);
        SectorRenderer.LoadZone(Zone);

        _input = new AetheriaInput();
        _input.Player.Enable();
        _input.Global.Enable();
        _input.UI.Enable();

        _zoomLevelIndex = Settings.DefaultMinimapZoom;
        _input.Player.MinimapZoom.performed += context =>
        {
            _zoomLevelIndex = (_zoomLevelIndex + 1) % Settings.MinimapZoomLevels.Length;
            SectorRenderer.MinimapDistance = Settings.MinimapZoomLevels[_zoomLevelIndex];
        };

        _input.Global.MapToggle.performed += context =>
        {
            if (Menu.gameObject.activeSelf && Menu.CurrentTab == MenuTab.Map)
            {
                Menu.gameObject.SetActive(false);
                return;
            }
            
            Menu.ShowTab(MenuTab.Map);
        };

        _input.Global.Inventory.performed += context =>
        {
            if (Menu.gameObject.activeSelf && Menu.CurrentTab == MenuTab.Inventory)
            {
                Menu.gameObject.SetActive(false);
                return;
            }

            Menu.ShowTab(MenuTab.Inventory);
        };

        ConsoleController.AddCommand("editmode", _ => ToggleEditMode());
        ConsoleController.AddCommand("savezone", args =>
        {
            if (args.Length > 0)
                Zone.Data.Name = args[0];
            SaveZone();
        });
        
        var stationType = ItemData.GetAll<HullData>().First(x=>x.HullType==HullType.Station);
        var stationHull = ItemManager.CreateInstance(stationType, 0, 1) as EquippableItem;
        var stationParent = Zone.PlanetInstances.Values.OrderByDescending(p => p.BodyData.Mass.Value).ElementAt(2);
        var parentOrbit = stationParent.Orbit.Data.ID;
        var stationPos = Zone.GetOrbitPosition(parentOrbit) + 
            normalize(ItemManager.Random.NextFloat2() - float2(.5f, .5f)) * ItemManager.Random.NextFloat() * stationParent.GravityWellRadius.Value * .75f;
        var stationOrbit = Zone.CreateOrbit(parentOrbit, stationPos);
        var station = new OrbitalEntity(ItemManager, Zone, stationHull, stationOrbit.ID);
        Zone.Entities.Add(station);
        
        var hullType = ItemData.GetAll<HullData>().First(x=>x.HullType==HullType.Ship);
        var shipHull = ItemManager.CreateInstance(hullType, 0, 1) as EquippableItem;
        var ship = new Ship(ItemManager, Zone, shipHull);
        
        // MapButton.onClick.AddListener(() =>
        // {
        //     if (_currentMenuTabButton == MapPanel) return;
        //     
        //     _currentMenuTabButton.GetComponent<TextMeshProUGUI>().color = Color.white;
        //     _currentMenuTabButton = MapButton;
        // });
        //
        //InventoryPanel.Display(ItemManager, entity);
    }

    public void SaveZone() => File.WriteAllBytes(
        Path.Combine(_filePath.FullName, $"{Zone.Data.Name}.zone"), MessagePackSerializer.Serialize(Zone.Pack()));

    public void ToggleEditMode()
    {
        _editMode = !_editMode;
        FollowCamera.gameObject.SetActive(!_editMode);
        TopDownCamera.gameObject.SetActive(_editMode);
    }

    void Update()
    {
        if(!_editMode)
        {
            _time += Time.deltaTime;
            ItemManager.Time = Time.time;
        }
        Zone.Update(_time, Time.deltaTime);
        SectorRenderer.Time = _time;
    }
}
