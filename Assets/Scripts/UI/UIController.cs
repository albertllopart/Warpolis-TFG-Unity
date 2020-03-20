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

        transform.Find("Tile_info").GetComponent<TileInfo>().UpdateInfo();
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

    void EnableTileInfo()
    {
        transform.Find("Tile_info").gameObject.SetActive(true);
    }

    void DisableTileInfo()
    {
        transform.Find("Tile_info").gameObject.SetActive(false);
    }

    void EnableMenuOptions()
    {
        DisableTileInfo();

        transform.Find("Menu_options").gameObject.SetActive(true);
        transform.Find("Menu_options").GetComponent<MenuOptionsController>().MyOnEnable();
    }

    void DisableMenuOptions()
    {
        EnableTileInfo();

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

    void SubscribeToEvents()
    {
        GameObject.Find("Gameplay Controller").GetComponent<GameplayController>().enableMenuOptions.AddListener(EnableMenuOptions);
        GameObject.Find("Gameplay Controller").GetComponent<GameplayController>().disableMenuOptions.AddListener(DisableMenuOptions);
        GameObject.Find("Gameplay Controller").GetComponent<GameplayController>().enableMenuShop.AddListener(EnableMenuShop);
        GameObject.Find("Gameplay Controller").GetComponent<GameplayController>().disableMenuShop.AddListener(DisableMenuShop);
    }
}
