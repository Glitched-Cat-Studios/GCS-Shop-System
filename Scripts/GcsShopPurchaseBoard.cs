using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GlitchedCatStudios.ShopSystem
{
    public class GcsShopPurchaseBoard : MonoBehaviour
    {
        [Header("This package was made by Glitched Cat Studios!\nPlease give credits!")]
        [Space]
        public TextMeshPro bodyText;
        public TextMeshPro choice1;
        public TextMeshPro choice2;
        public TextMeshPro itemIdText;
        public GcsShopSystemStage stage = GcsShopSystemStage.Option1;

        private string gcspurchase = "Would you like you purchase this item?";
        private string gcsconfirm = "Are you sure?";
        private string gcscurrency = "Insufficient funds.";
        private string gcsown = "You already own this item.";

        private string yes = "YES\r\n|\r\n\\/";
        private string no = "NO\r\n|\r\n\\/";

        internal bool isPurchasing = false;

        private void OnEnable()
        {
            GcsShopSystemManager.GcsShopRefreshButton += RefreshItemId;
        }

        private void OnDisable()
        {
            GcsShopSystemManager.GcsShopRefreshButton -= RefreshItemId;
        }

        private void RefreshItemId()
        {
            if (GcsShopSystemManager.instance.currentSelected.price == int.MaxValue || string.IsNullOrEmpty(GcsShopSystemManager.instance.currentSelected.name))
            {
                stage = GcsShopSystemStage.Option1;

                bodyText.text = gcspurchase;
                choice1.text = yes;
                choice2.text = no;

                itemIdText.text = string.Empty;
            }
            else
            {
                itemIdText.text = GcsShopSystemManager.instance.currentSelected.name;
            }
        }

        #region Button Callbacks
        internal void OptionOneChoiceOne()
        {
            //Move to Option Two
            stage = GcsShopSystemStage.Option2;

            bodyText.text = gcsconfirm;
            choice1.text = no;
            choice2.text = yes;
        }

        internal void OptionOneChoiceTwo()
        {
            //Cancel or Do Nothing
        }

        internal void OptionTwoChoiceOne()
        {
            //Send back to choice one

            stage = GcsShopSystemStage.Option1;

            bodyText.text = gcspurchase;
            choice1.text = yes;
            choice2.text = no;
        }

        internal void OptionTwoChoiceTwo()
        {
            //Buy the item, unselect the item, and go back

            isPurchasing = true;

            GcsShopSystemManager.instance.PurchaseItem(result =>
            {
                StartCoroutine(UpdateBoardText(result));
            });
        }

        private IEnumerator UpdateBoardText(PurchaseItemCallback callback)
        {
            switch (callback)
            {
                case PurchaseItemCallback.Success:
                    bodyText.text = $"Purchased {GcsShopSystemManager.instance.currentSelected.name} successfully!";
                    break;
                case PurchaseItemCallback.InsufficientFunds:
                    bodyText.text = $"Insufficient funds while buying {GcsShopSystemManager.instance.currentSelected.name}.";
                    break;
                case PurchaseItemCallback.AlreadyOwn:
                    bodyText.text = $"You already own {GcsShopSystemManager.instance.currentSelected.name}.";
                    break;
                case PurchaseItemCallback.Other:
                    bodyText.text = $"An error occured while purchasing {GcsShopSystemManager.instance.currentSelected.name}. Please try again later.";
                    break;
            }

            yield return new WaitForSeconds(5);

            stage = GcsShopSystemStage.Option1;
            bodyText.text = gcspurchase;
            choice1.text = yes;
            choice2.text = no;

            GcsShopSystemManager.instance.currentSelected = new GcsShopSystemSlot()
            {
                name = "Empty",
                price = int.MaxValue
            };

            isPurchasing = false;
        }

        #endregion


        public enum GcsShopSystemStage
        {
            Option1,
            Option2
        }
    }
}
