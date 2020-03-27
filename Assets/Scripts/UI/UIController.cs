using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    bool afterStart = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void AfterStart()
    {
        SubscribeToEvents();

        transform.Find("Tile_info").GetComponent<TileInfo>().UpdateInfo(GameObject.Find("Player").transform.position);

        transform.Find("Money_info").GetComponent<MoneyInfo>().AfterStart();

        afterStart = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (afterStart)
        {
            AfterStart();
        }
    }

    public void EnableTileInfo()
    {
        transform.Find("Tile_info").gameObject.SetActive(true);
    }

    void DisableTileInfo()
    {
        transform.Find("Tile_info").gameObject.SetActive(false);
    }

    public void EnableMoneyInfo()
    {
        transform.Find("Money_info").gameObject.SetActive(true);
    }

    void DisableMoneyInfo()
    {
        transform.Find("Money_info").gameObject.SetActive(false);
    }

    void EnableMenuOptions()
    {
        DisableTileInfo();
        DisableMoneyInfo();

        transform.Find("Menu_options").gameObject.SetActive(true);
        transform.Find("Menu_options").GetComponent<MenuOptionsController>().MyOnEnable();
    }

    void DisableMenuOptions()
    {
        EnableTileInfo();
        EnableMoneyInfo();

        transform.Find("Menu_options").GetComponent<MenuOptionsController>().MyOnDisable();
        transform.Find("Menu_options").gameObject.SetActive(false);
    }

    void EnableMenuShop()
    {
        DisableTileInfo();

        transform.Find("Menu_shop").gameObject.SetActive(true);
        transform.Find("Menu_shop").GetComponent<MenuShopController>().MyOnEnable();
    }

    void DisableMenuShop()
    {
        EnableTileInfo();

        transform.Find("Menu_shop").GetComponent<MenuShopController>().MyOnDisable();
        transform.Find("Menu_shop").gameObject.SetActive(false);
    }

    public void EnableMenuUnit(GameObject unit)
    {
        DisableTileInfo();

        transform.Find("Menu_unit").gameObject.SetActive(true);
        transform.Find("Menu_unit").GetComponent<MenuUnitController>().MyOnEnable(unit);
    }

    void DisableMenuUnit()
    {
        EnableTileInfo();
        EnableMoneyInfo();

        transform.Find("Tile_info").GetComponent<TileInfo>().UpdateInfo(GameObject.Find("Player").transform.position);

        MenuUnitController menu = transform.Find("Menu_unit").GetComponent<MenuUnitController>();
        menu.MyOnDisable();
        menu.gameObject.SetActive(false);
    }

    void ShowMenuUnit()
    {
        DisableTileInfo();

        GameObject menu = transform.Find("Menu_unit").gameObject;
        MenuUnitController menuController = menu.GetComponent<MenuUnitController>();

        menu.SetActive(true);
        menuController.selectedUnit.GetComponent<Unit>().OnMenu();
        menuController.selectedUnit.GetComponent<Unit>().UnsubscribeFromEvents();
    }

    void HideMenuUnit()
    {
        EnableTileInfo();

        transform.Find("Menu_unit").GetComponent<MenuUnitController>().MyOnHide();
        transform.Find("Menu_unit").gameObject.SetActive(false);
    }

    void CancelMenuUnit() // això es crida al cancel·lar el moviment
    {
        transform.Find("Menu_unit").GetComponent<MenuUnitController>().MyOnCancel();
        transform.Find("Menu_unit").gameObject.SetActive(false);

        EnableTileInfo();
        transform.Find("Tile_info").GetComponent<TileInfo>().UpdateInfo(GameObject.Find("Player").transform.position);
    }

    void SubscribeToEvents()
    {
        GameObject gameplay = GameObject.Find("Gameplay Controller");
        gameplay.GetComponent<GameplayController>().enableMenuOptions.AddListener(EnableMenuOptions);
        gameplay.GetComponent<GameplayController>().disableMenuOptions.AddListener(DisableMenuOptions);
        gameplay.GetComponent<GameplayController>().enableMenuShop.AddListener(EnableMenuShop);
        gameplay.GetComponent<GameplayController>().disableMenuShop.AddListener(DisableMenuShop);
        gameplay.GetComponent<GameplayController>().disableMenuUnit.AddListener(DisableMenuUnit);
        gameplay.GetComponent<GameplayController>().showMenuUnit.AddListener(ShowMenuUnit);
        gameplay.GetComponent<GameplayController>().hideMenuUnit.AddListener(HideMenuUnit);
        gameplay.GetComponent<GameplayController>().cancelMenuUnit.AddListener(CancelMenuUnit);
        gameplay.GetComponent<GameplayController>().enableMoneyInfo.AddListener(EnableMoneyInfo);
        gameplay.GetComponent<GameplayController>().disableMoneyInfo.AddListener(DisableMoneyInfo);
    }
}
