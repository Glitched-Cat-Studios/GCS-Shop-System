using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GlitchedCatStudios.ShopSystem
{
    public class GcsShopPurchase : MonoBehaviour
    {
        [Header("This package was made by Glitched Cat Studios!\nPlease give credits!")]
        [Space]
        public TextMeshPro statusText;
        public TextMeshPro itemIdText;
        public TextMeshPro priceText;
        [Space]
        [SerializeField] internal GcsShopSystemSlot itemInfo = new GcsShopSystemSlot();
        [Space(20)]
        public string handTag = "HandTag";

        private void Start()
        {
            itemIdText.text = itemInfo.name;
            priceText.text = itemInfo.price.ToString();
        }


        internal void OnTriggerEnter(Collider other)
        {
            if (CheckIfInCart(itemInfo))
            {
                GcsShopSystemManager.instance.RemoveFromCart(itemInfo);
                Debug.Log("Removing from cart");
            }
            else
            {
                GcsShopSystemManager.instance.AddToCart(itemInfo);

                Debug.Log("Adding to cart");
            }
        }

        private void OnEnable()
        {
            GcsShopSystemManager.GcsShopRefreshButton += CheckButton;
        }
        private void OnDisable()
        {
            GcsShopSystemManager.GcsShopRefreshButton -= CheckButton;
        }

        private void CheckButton()
        {
            GetComponent<Renderer>().material = CheckIfInCart(itemInfo) ? GcsShopSystemManager.instance.selected : GcsShopSystemManager.instance.unselected;
            statusText.text = CheckIfInCart(itemInfo) ? "REMOVE FROM CART" : "ADD TO CART";
        }

        private bool CheckIfInCart(GcsShopSystemSlot item)
        {
            for (int i = 0; i < GcsShopSystemManager.instance.selectorButtons.Length; i++)
            {
                if (GcsShopSystemManager.instance.selectorButtons[i].GcsCurrentSlot == item)
                {
                    return true;
                }
            }

            return false;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GcsShopPurchase))]
    public class GcsShopPurchaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GcsShopPurchase manager = (GcsShopPurchase)target;

            GUILayout.Space(15);

            if (GUILayout.Button("Test Button Press"))
            {
                GameObject Hand = GameObject.FindGameObjectWithTag(manager.handTag);
                Collider collider = Hand.GetComponent<Collider>();

                manager.OnTriggerEnter(collider);
            }
        }
    }
#endif
}
