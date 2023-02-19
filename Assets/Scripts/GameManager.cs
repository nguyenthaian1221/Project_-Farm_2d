using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject player;
    public ItemContainer inventoryContainer;
    public ItemContainer allItemsContainer;
    public SeedContainer allSeedsContainer;
    public DragAndDropController dragAndDropController;

    public ToolbarController toolbarControllerGlobal;




    private bool isWinFunctionCalled = false;
    private bool isHackFunctionCalled = false;

    private void Update()
    {
        if (Input.GetKey(KeyCode.H) && Input.GetKey(KeyCode.W) && !isWinFunctionCalled)
        {
            isWinFunctionCalled = true;
            Application.LoadLevel(4);
        }


        if (Input.GetKey(KeyCode.H) && Input.GetKey(KeyCode.M) && !isHackFunctionCalled)
        {
            isHackFunctionCalled = true;
            player.GetComponent<ToolsCharacterController>().HACK9999GAME();
        }


    }



}
