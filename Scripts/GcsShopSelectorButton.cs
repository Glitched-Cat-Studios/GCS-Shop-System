using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GlitchedCatStudios.ShopSystem
{
    public class GcsShopSelectorButton : MonoBehaviour
    {
        [Header("This package was made by Glitched Cat Studios!\nPlease give credits!")]
        [Space]
        public TextMeshPro nameText;
        public TextMeshPro priceText;
        [Space]
        public string handTag = "HandTag";

        [SerializeField, HideInInspector] internal GcsShopSystemSlot GcsCurrentSlot = null;

        private void Start()
        {
            GcsCurrentSlot = null;

            nameText.text = string.Empty;
            priceText.text = string.Empty;
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
            GetComponent<Renderer>().material = GcsShopSystemManager.instance.currentSelected == GcsCurrentSlot ? GcsShopSystemManager.instance.selected : GcsShopSystemManager.instance.unselected;
            
            if (GcsCurrentSlot != null)
            {
                nameText.text = GcsCurrentSlot.name;
                priceText.text = GcsCurrentSlot.price.ToString();
            }
            else
            {
                nameText.text = string.Empty;
                priceText.text = string.Empty;
            }
        }

        internal void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(handTag))
            {
                GcsShopSystemManager.instance.currentSelected = GcsCurrentSlot;

                GcsShopSystemManager.GcsShopRefreshButton.Invoke();
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GcsShopSelectorButton))]
    public class GcsShopSelectorButtonEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GcsShopSelectorButton manager = (GcsShopSelectorButton)target;

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
