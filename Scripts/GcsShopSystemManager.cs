using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlitchedCatStudios.Wardrobe;
using PlayFab;
using PlayFab.ClientModels;

namespace GlitchedCatStudios.ShopSystem
{
    [HelpURL("https://discord.gg/SQSCRT3eEC")]

    public class GcsShopSystemManager : MonoBehaviour
    {
        public static GcsShopSystemManager instance;
        public static Action GcsShopRefreshButton;

        [Header("This package was made by Glitched Cat Studios!\nPlease give credits!")]
        [Space]
        [Header("Settings")]
        [Space]
        [SerializeField] private string catalogName = "Cosmetics";
        [SerializeField] private string currencyCode = "HS";
        [Header("Materials")]
        [Space]
        public Material selected;
        public Material unselected;

        [Header("Don't mess with")]
        [Space]
        [SerializeField] internal GcsShopSelectorButton[] selectorButtons;

        [SerializeField, HideInInspector] internal GcsShopSystemSlot currentSelected;


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.LogError("Two GcsShopSystemManagers cannot be in the same scene.");
                gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            GcsShopRefreshButton.Invoke();
        }

        internal void PurchaseItem(Action<PurchaseItemCallback> callback)
        {
            PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            (inventoryResult) =>
            {
                bool ownsItem = inventoryResult.Inventory.Exists(item => item.ItemId == currentSelected.name);
                if (ownsItem)
                {
                    callback.Invoke(PurchaseItemCallback.AlreadyOwn);
                    return;
                }

                PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest()
                {
                    CatalogVersion = catalogName,
                    VirtualCurrency = currencyCode,
                    ItemId = currentSelected.name,
                    Price = currentSelected.price
                },
                (result) =>
                {
                    Debug.Log("Purchase successful!");
                    callback.Invoke(PurchaseItemCallback.Success);
                    GcsShopRefreshButton.Invoke();
                    currentSelected = null;
                    GcsWardrobeManager.instance.ReloadWardrobe();
                },
                (error) =>
                {
                    Debug.LogError(error.GenerateErrorReport());

                    if (error.Error == PlayFabErrorCode.InsufficientFunds)
                        callback.Invoke(PurchaseItemCallback.InsufficientFunds);
                    else
                        callback.Invoke(PurchaseItemCallback.Other);
                });
            },
            (error) =>
            {
                Debug.LogError(error.GenerateErrorReport());
                callback.Invoke(PurchaseItemCallback.Other);
            });
        }

        internal void AddToCart(GcsShopSystemSlot item)
        {
            bool itemAdded = false;

            for (int i = 0; i < selectorButtons.Length; i++)
            {
                if (selectorButtons[i].GcsCurrentSlot == null)
                {
                    selectorButtons[i].GcsCurrentSlot = item;
                    itemAdded = true;
                    break;
                }
            }

            if (!itemAdded)
            {
                selectorButtons[selectorButtons.Length - 1].GcsCurrentSlot = item;
            }

            GcsShopRefreshButton.Invoke();
        }



        internal void RemoveFromCart(GcsShopSystemSlot item)
        {
            for (int i = 0; i < selectorButtons.Length; i++)
            {
                if (selectorButtons[i].GcsCurrentSlot == item)
                {
                    selectorButtons[i].GcsCurrentSlot = null;

                    if (currentSelected == item)
                        currentSelected = new GcsShopSystemSlot()
                        {
                            name = "Empty",
                            price = int.MaxValue
                        };

                    GcsShopRefreshButton.Invoke();
                    break;
                }
            }
        }
    }

    [Serializable]
    public class GcsShopSystemSlot
    {
        [Header("The PlayFab Item ID")]
        public string name;
        [Space]
        [Header("The price of the item")]
        public int price;
    }

    internal enum PurchaseItemCallback
    {
        Success,
        InsufficientFunds,
        AlreadyOwn,
        Other
    }
}
