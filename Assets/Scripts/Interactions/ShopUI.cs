using Assets.Scripts.Items;
using System.Collections.Generic;

namespace Assets.Scripts.Interactions
{
    public enum EShopMode
    {
        buy,
        sell,
        rebuy
    }

    public class ShopUI
    {
        EShopMode userMode = EShopMode.buy;
        int numElemsPerPage = 10;
        int page = 0;
        PraeItem[,] pagedItems; // switching the mode should adjust this 3D array

        List<PraeItem> soldItems;

        Inventory buyer;
        Inventory seller;

        public void SetInventories(Inventory buyer, Inventory Seller)
        {
            if (buyer == null || Seller == null)
                throw new System.NullReferenceException("buyer or seller inventory set to NULL!");
            SetUpInventoryElements();
        }

        public void SetUpInventoryElements()
        {
            userMode = EShopMode.buy;
            if (seller.itemCount != 0)
                pagedItems = new PraeItem[seller.itemCount / numElemsPerPage, numElemsPerPage];
            else
                pagedItems = new PraeItem[1, numElemsPerPage];

            // TODO: set currency label here

            SetUpPage(0);
        }

        public void SetUpPage(int p)
        {
            UnityEngine.Debug.Assert(p >= 0 && p < pagedItems.Length, "Invalid page id to set in ShopUI!");

            for (int i=0; i<numElemsPerPage; ++i)
            {
                //TODO: set item elements of ui here
            }

            page = p;
        }
    }
}
