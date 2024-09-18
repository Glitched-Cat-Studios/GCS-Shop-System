using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlitchedCatStudios.Wardrobe;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GlitchedCatStudios.ShopSystem
{
    public class GcsShopPurchaseButton : MonoBehaviour
    {
        [Header("This package was made by Glitched Cat Studios!\nPlease give credits!")]
        [Space]
        [SerializeField] private GcsShopPurchaseBoard manager;
        [Space]
        public GcsShopSystemButtonType type;
        public string handTag = "HandTag";

        internal void OnTriggerEnter(Collider other)
        {
            if (GcsShopSystemManager.instance.currentSelected.price == int.MaxValue || string.IsNullOrEmpty(GcsShopSystemManager.instance.currentSelected.name) || manager.isPurchasing)
                return;

            if (other.CompareTag(handTag))
            {
                switch (manager.stage)
                {
                    case GcsShopPurchaseBoard.GcsShopSystemStage.Option1:
                        switch (type)
                        {
                            case GcsShopSystemButtonType.Choice1:
                                manager.OptionOneChoiceOne();
                                break;

                            case GcsShopSystemButtonType.Choice2:
                                manager.OptionOneChoiceTwo();
                                break;
                        }
                        break;

                    case GcsShopPurchaseBoard.GcsShopSystemStage.Option2:
                        switch (type)
                        {
                            case GcsShopSystemButtonType.Choice1:
                                manager.OptionTwoChoiceOne();
                                break;

                            case GcsShopSystemButtonType.Choice2:
                                manager.OptionTwoChoiceTwo();
                                break;
                        }
                        break;
                }
            }
        }

        public enum GcsShopSystemButtonType
        {
            Choice1,
            Choice2
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GcsShopPurchaseButton))]
    public class GcsShopPurchaseButtonEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GcsShopPurchaseButton manager = (GcsShopPurchaseButton)target;

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
