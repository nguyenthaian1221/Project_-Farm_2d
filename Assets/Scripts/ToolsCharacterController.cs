﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using System.Linq;

public class ToolsCharacterController : MonoBehaviour
{
    PlayerControl character;
    Rigidbody2D rgbd2d;
    [SerializeField] MarkerManager markerManager;
    [SerializeField] TileMapReadController tileMapReadController;
    [SerializeField] CropsReadController cropsReadController;
    [SerializeField] float maxDistance = 2f;
    [SerializeField] CropsManager cropsManager;
    [SerializeField] TileData plowableTiles;
    [SerializeField] TileData toMowTiles;
    [SerializeField] TileData toSeedTiles;
    [SerializeField] TileData waterableTiles;
    InventoryController inventoryController;
    ToolbarController toolbarController;
    [SerializeField] GameObject toolbarPanel;
    GameObject shop;

    [SerializeField] float offsetDistance = 1f;
    [SerializeField] float sizeOfInteractableArea = 1.2f;

    private static int cornAmount = 3;
    private static int parsleyAmount = 1;
    private static int potatoAmount = 1;
    private static int strawberryAmount = 1;
    private static int tomatoAmount = 1;


    Vector3Int selectedTilePosition;
    Vector3Int selectedCropPosition;
    bool selectable;

    public static Dictionary<Vector2Int, TileData> fields;
    public static Dictionary<Vector2Int, CropData> crops;


    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<PlayerControl>();
        rgbd2d = GetComponent<Rigidbody2D>();
        fields = new Dictionary<Vector2Int, TileData>();
        crops = new Dictionary<Vector2Int, CropData>();
        toolbarController = GetComponent<ToolbarController>();
        inventoryController = GetComponent<InventoryController>();

        shop = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(g => g.CompareTag("UIshop"));
    }

    // Update is called once per frame
    void Update()
    {
        SelectTile();
        CanSelectCheck();
        Marker();
        if (Input.GetMouseButtonDown(0)) // lewy przycisk myszki
        {
            if (!inventoryController.isOpen) //you can use tools only if inventory is closed
            {
                if (UseToolWorld() == true)
                {
                    return;
                }
                UseTool();
            }

        }
    }

    private bool CastRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        if (hit)
        {
            //Debug.Log(hit.collider.gameObject.name);
            if (hit.collider.gameObject.name.Contains("Tree"))
            {
                return true;
            }
            if (hit.collider.gameObject.name.Contains("CampFire"))
            {
                return true;
            }
            if (hit.collider.gameObject.name.Contains("Chest"))
            {
                return true;
            }
        }
        return false;
    }
    private bool CastRayPlayer()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        if (hit)
        {
            if (hit.collider.gameObject.name.Contains("Player"))
            {
                return true;
            }
        }
        return false;
    }
            private void SelectTile()
    {
        selectedTilePosition = tileMapReadController.GetGridPosition(Input.mousePosition, true);
        TileBase tileBase = tileMapReadController.GetTileBase(selectedTilePosition);
        try
        {
            TileData tileData = tileMapReadController.GetTileData(tileBase);
            if (!(tileData is null))
            {
                if (!fields.ContainsKey((Vector2Int)selectedTilePosition))
                {
                    fields.Add((Vector2Int)selectedTilePosition, tileData);
                }
                else
                {
                    fields[(Vector2Int)selectedTilePosition] = tileData;
                }
            }
        }
        catch
        {
            return;
        }

        selectedCropPosition = cropsReadController.GetGridPosition(Input.mousePosition, true);
        TileBase cropBase = cropsReadController.GetTileBase(selectedTilePosition);
        try
        {
            CropData cropData = cropsReadController.GetCropData(cropBase);
            if (!(cropData is null))
            {
                if (!crops.ContainsKey((Vector2Int)selectedTilePosition))
                {
                    crops.Add((Vector2Int)selectedTilePosition, cropData);
                }
                else
                {
                    crops[(Vector2Int)selectedTilePosition] = cropData;
                }
            }
        }
        catch
        {
            return;
        }

    }

    void CanSelectCheck()
    {
        if (Time.timeScale == 0) //if game paused
            return;

        Vector2 characterPosition = transform.position;
        Vector2 cameraPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        selectable = Vector2.Distance(characterPosition, cameraPosition) < maxDistance;
        markerManager.Show(selectable);
    }

    private void Marker()
    {
        markerManager.markedCellPosition = selectedTilePosition;
    }

    // interacting with physical objects in the world
    private bool UseToolWorld()
    {
        if (Time.timeScale == 0)
            return false;

        // CUTTING TREE
        Vector2 position = rgbd2d.position + character.lastMotionVector * offsetDistance;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, sizeOfInteractableArea);


        foreach (Collider2D collidor in colliders)
        {
            ToolHit hitTree = collidor.GetComponent<ToolHit>();
            CampFireHit hitFire = collidor.GetComponent<CampFireHit>();
            ChestHit hitChest = collidor.GetComponent<ChestHit>();
            PlayerHit hitPlayer = collidor.GetComponent<PlayerHit>();

            if (hitTree != null && toolbarController.GetItem != null &&
                toolbarController.GetItem.Name == "Axe" && CastRay() == true)
            {
                hitTree.Hit();
                // Debug.Log("we can hit");
                return true;
            }
            if (hitFire != null && toolbarController.GetItem != null &&
                toolbarController.GetItem.Name == "Wood" && CastRay() == true)
            {
                hitFire.Hit();
                return true;
            }
            if (hitChest != null && CastRay() == true)
            {
                hitChest.Hit();
                return true;
            }
            if (hitPlayer != null && CastRayPlayer() == true && (toolbarController.GetItem.Name == "Food_Corn" || toolbarController.GetItem.Name == "Food_Parsley"
                    || toolbarController.GetItem.Name == "Food_Potato" || toolbarController.GetItem.Name == "Food_Strawberry" || toolbarController.GetItem.Name == "Food_Tomato"))
            {
                //Debug.Log(toolbarController.GetItem.Name);
                hitPlayer.Hit();
                return true;
            }
        }

        return false;
    }

    private void RefreshToolbar()
    {
        toolbarPanel.SetActive(!toolbarPanel.activeInHierarchy);
        toolbarPanel.SetActive(true);
    }

    private void UseTool()
    {
        if (Time.timeScale == 0) //if game paused - return
            return;

        // when sth is present on the grid but you can't plant there
        if (selectable == true && toolbarController.GetItem != null)
        {
            TileBase tileBase = tileMapReadController.GetTileBase(selectedTilePosition);
            TileData tileData = tileMapReadController.GetTileData(tileBase);
            //TileData cropData = cropsReadController.GetTileData(tileBase);

            /*if (toolbarController.GetItem.Name == "WateringCan" && fields[(Vector2Int)selectedTilePosition].watered) //if you are using watering can - play sound of water
                FindObjectOfType<SoundManager>().Play("Water");*/

            if (tileData != plowableTiles && tileData != toMowTiles && tileData != toSeedTiles && tileData != waterableTiles) //if tile doesn't have any ability
            {
                return;
            }


            //if there is no plant on tile
            if (crops[(Vector2Int)selectedTilePosition].noPlant)
            {
                //usage of tools if tile has suitable ability
                if (fields[(Vector2Int)selectedTilePosition].ableToMow && toolbarController.GetItem.Name == "Shovel"
                    && !shop.activeSelf)
                {
                    cropsManager.Mow(selectedTilePosition);
                }
                else if (fields[(Vector2Int)selectedTilePosition].plowable && toolbarController.GetItem.Name == "Hoe")
                {
                    cropsManager.Plow(selectedTilePosition);
                }
                else if (fields[(Vector2Int)selectedTilePosition].ableToSeed && toolbarController.GetItem.isSeed == true)
                {
                    switch (toolbarController.GetItem.Name) //depending on what seed you have chosen
                    {
                        case "Seeds_Corn":
                            // Checking whether we have more than 4 seeds to seed
                            if (GameManager.instance.inventoryContainer.slots[toolbarController.selectedTool].count >= 4)
                            {
                                cropsManager.SeedCrop(selectedTilePosition, "corn");
                                GameManager.instance.inventoryContainer.RemoveItem(toolbarController.GetItem, cornAmount);   // Deletes 4 seeds
                            }
                        break;
                        case "Seeds_Parsley":
                            // Checking whether we have more than 3 seeds to seed
                            if (GameManager.instance.inventoryContainer.slots[toolbarController.selectedTool].count >= 3)
                            {
                                cropsManager.SeedCrop(selectedTilePosition, "parsley");
                                GameManager.instance.inventoryContainer.RemoveItem(toolbarController.GetItem, parsleyAmount);  // Deletes 3 seeds
                            }
                        break;
                        case "Seeds_Potato":
                            // Checking whether we have more than 1 seed to seed
                            if (GameManager.instance.inventoryContainer.slots[toolbarController.selectedTool].count >= 1)
                            {
                                cropsManager.SeedCrop(selectedTilePosition, "potato");
                                GameManager.instance.inventoryContainer.RemoveItem(toolbarController.GetItem, potatoAmount);   // Deletes 1 seed
                            }
                        break;
                        case "Seeds_Strawberry":
                            // Checking whether we have more than 6 seeds to seed
                            if (GameManager.instance.inventoryContainer.slots[toolbarController.selectedTool].count >= 6)
                            {
                                cropsManager.SeedCrop(selectedTilePosition, "strawberry");
                                GameManager.instance.inventoryContainer.RemoveItem(toolbarController.GetItem, strawberryAmount); // Deletes 6 seeds
                            }
                        break;
                        case "Seeds_Tomato":
                            // Checking whether we have more than 3 seeds to seed
                            if (GameManager.instance.inventoryContainer.slots[toolbarController.selectedTool].count >= 3)
                            {
                                cropsManager.SeedCrop(selectedTilePosition, "tomato");
                                GameManager.instance.inventoryContainer.RemoveItem(toolbarController.GetItem, tomatoAmount);   // Deletes 3 seeds
                            }
                        break;
                    }

                    // Refreshing the count of seeds
                    RefreshToolbar();
                }               
            }

            //usage of tools if there is a planted tile
            else if (crops[(Vector2Int)selectedTilePosition].planted && fields[(Vector2Int)selectedTilePosition].waterable && toolbarController.GetItem.Name == "WateringCan")
            {
                cropsManager.Water(selectedTilePosition);
                FindObjectOfType<SoundManager>().Play("Water");
            }

            else if (crops[(Vector2Int)selectedTilePosition].collectibleCorn && toolbarController.GetItem.Name == "Bag")
            {
                cropsManager.Collect(selectedTilePosition, "corn");
                foreach (ItemSlot itemSlot in GameManager.instance.allItemsContainer.slots)
                {
                    if (itemSlot.item.Name == "Food_Corn")
                    {
                        GameManager.instance.inventoryContainer.Add(itemSlot.item, cornAmount);
                        RefreshToolbar();
                        break;
                    }
                }
                //Debug.Log("dodajemy corn");

            }
            else if (crops[(Vector2Int)selectedTilePosition].collectibleParsley && toolbarController.GetItem.Name == "Bag")
            {
                cropsManager.Collect(selectedTilePosition, "parsley");
                foreach (ItemSlot itemSlot in GameManager.instance.allItemsContainer.slots)
                {
                    if (itemSlot.item.Name == "Food_Parsley")
                    {
                        GameManager.instance.inventoryContainer.Add(itemSlot.item, parsleyAmount);
                        RefreshToolbar();
                        break;
                    }
                }
                //Debug.Log("dodajemy parsley");
            }
            else if (crops[(Vector2Int)selectedTilePosition].collectiblePotato && toolbarController.GetItem.Name == "Bag")
            {
                cropsManager.Collect(selectedTilePosition, "potato");
                foreach (ItemSlot itemSlot in GameManager.instance.allItemsContainer.slots)
                {
                    if (itemSlot.item.Name == "Food_Potato")
                    {
                        GameManager.instance.inventoryContainer.Add(itemSlot.item, potatoAmount);
                        RefreshToolbar();
                        break;
                    }
                }
                //Debug.Log("dodajemy potato");
            }
            else if (crops[(Vector2Int)selectedTilePosition].collectibleStrawberry && toolbarController.GetItem.Name == "Bag")
            {
                cropsManager.Collect(selectedTilePosition, "strawberry");
                foreach (ItemSlot itemSlot in GameManager.instance.allItemsContainer.slots)
                {
                    if (itemSlot.item.Name == "Food_Strawberry")
                    {
                        GameManager.instance.inventoryContainer.Add(itemSlot.item, strawberryAmount);
                        RefreshToolbar();
                        break;
                    }
                }
                //Debug.Log("dodajemy strawberry");
            }
            else if (crops[(Vector2Int)selectedTilePosition].collectibleTomato && toolbarController.GetItem.Name == "Bag")
            {
                cropsManager.Collect(selectedTilePosition, "tomato");
                foreach (ItemSlot itemSlot in GameManager.instance.allItemsContainer.slots)
                {
                    if (itemSlot.item.Name == "Food_Tomato")
                    {
                        GameManager.instance.inventoryContainer.Add(itemSlot.item, tomatoAmount);
                        RefreshToolbar();
                        break;
                    }
                }
                //Debug.Log("dodajemy tomato");
            }

        }
    }
}
