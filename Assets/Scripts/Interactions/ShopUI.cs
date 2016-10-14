using Assets.Scripts.Items;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Interactions
{
    public class ShopUI : MonoBehaviour
    {

        public enum EShopMode
        {
            buy,
            sell,
            rebuy
        }


        /**
         * stores important information of item ui component 
         */
        class UIItem
        {
            public Component _itemElem;
            Image _itemElemBorderImage;
            Color _origColor;
            public Button   _but;
            public PraeItem _item;
            public Sprite   _icon;
            public UICurrency _value;
            public bool _hasItem = false;

            /**
             * empty slot constructor
             */
            public UIItem(Component itemElem)
            {
                Text TG = null, TK = null, TT = null, TN = null;
                foreach (Text t in itemElem.GetComponentsInChildren<Text>())
                {
                    if (t.name.Equals("Text_G"))
                        TG = t;
                    else if (t.name.Equals("Text_K"))
                        TK = t;
                    else if (t.name.Equals("Text_T"))
                        TT = t;
                    else if (t.name.Equals("Text_N"))
                        TN = t;
                }

                _itemElem = itemElem;
                _itemElemBorderImage = itemElem.GetComponent<Image>();
                _origColor = _itemElemBorderImage.color;
                _but = itemElem.GetComponent<Button>();
                _value = new UICurrency(TG, TK, TT, TN);
            }

            /**
             * use the value of the item
             */
            public void Set(PraeItem item)
            {
                if (item != null)
                {
                    _item = item;
                    _icon = item.icon;
                    _value.Set(item.value.G, item.value.K, item.value.T);
                    _hasItem = true;
                }
                else
                {
                    _value.SetEmpty();
                    _hasItem = false;
                }
            }

            public void Set(PraeItem item, int g, int k, int t)
            {
                _item = item;
                _icon = item.icon;
                _value.Set(g, k, t);
            }

            public void Set(Color borderColor)
            {
                _itemElemBorderImage.color = borderColor;
            }

            public void ResetColor()
            {
                _itemElemBorderImage.color = _origColor;
            }
        }


        class UICurrency
        {
            const int ignore = -1;
            Text G;
            Text K;
            Text T;
            Text Amount;

            public UICurrency(Text TG, Text TK, Text TT, Text TAmount = null)
            {
                if (TG == null || TK == null || TT == null)
                    throw new System.NullReferenceException("CurrencyLabel texts cannot be null!");
                G = TG;
                K = TK;
                T = TT;
                Amount = TAmount;
                Set(0, 0, 0);
            }

            /**
             * full constructor
             */
            public UICurrency(Text TG, Text TK, Text TT, int g, int k, int t, Text TAmount = null, int amount = ignore) : this(TG, TK, TT, TAmount)
            {
                Set(g, k, t, amount);
            }

            public void Set(int g, int k, int t, int amount = ignore)
            {
                G.text = "G: " + g;
                K.text = "K: " + k;
                T.text = "T: " + t;
                if (amount != -1)
                    Amount.text = "N: " + amount;
            }

            public void SetEmpty()
            {
                G.text = "";
                K.text = "";
                T.text = "";
                if (Amount != null)
                    Amount.text = "";
            }
        }

        /* ui elements */
        UICurrency UIbuyerMoney;
        UIItem[] UIItems;
        UIItem selectedUIItem = null;
        UIItem mouseoverUIItem = null;
        Color mouseSelectColor = Color.blue;
        Color mouseSelectOverColor = Color.cyan;
        Color mouseOverColor = Color.yellow;

        /* variables */
        EShopMode userMode = EShopMode.buy;
        int numElemsPerPage = 10;
        int page = 0;
        PraeItem[,] pagedItems; // [page, itemsPerPage] switching the mode should adjust this 3D array
        List<PraeItem> soldItems;

        Inventory _buyer;
        Inventory _seller;

        public void Awake()
        {
            Constants.ShopUI = gameObject;

            // get the currency text
            Component[] components = gameObject.GetComponentsInChildren<Component>();
            Text TG = null;
            Text TK = null;
            Text TT = null;
            foreach (Component g in components)
            {
                if (g.name.Equals("MoneyLabel"))
                {
                    foreach (Text t in g.GetComponentsInChildren<Text>())
                    {
                        if (t.name.Equals("Text_G"))
                            TG = t;
                        else if (t.name.Equals("Text_K"))
                            TK = t;
                        else if (t.name.Equals("Text_T"))
                            TT = t;
                        else
                            Debug.Log("MoneyLabel contained unexpected text objects? " + t.name);
                    }
                    break;
                }
            }

            UIbuyerMoney = new UICurrency(TG, TK, TT);

            // get important item elements
            UIItems = new UIItem[numElemsPerPage];
            HashSet<string> addedList = new HashSet<string>();
            int i = 0;
            foreach (Component g in components)
            {
                if (g.name.StartsWith("ItemElem") && !addedList.Contains(g.name))
                {
                    UIItems[i] = new UIItem(g);
                    addedList.Add(g.name);
                    ++i;
                }
            }

        }

        public void Start()
        {
            gameObject.SetActive(false);
        }

        void LateUpdate()
        {
            // handle mouse over animation
            Debug.Log((RectTransform)(gameObject.transform));
            if (RectTransformUtility.RectangleContainsScreenPoint((RectTransform)gameObject.transform, Input.mousePosition))
            {
                for (int i=0; i<UIItems.Length; ++i)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint((RectTransform)UIItems[i]._itemElem.transform, Input.mousePosition))
                    {
                        // mouse is within item rect
                        if (Input.GetMouseButton(0))
                        {
                            if (UIItems[i]._hasItem)
                            {
                                /* item was clicked on with left mouse button => selected */
                                if (selectedUIItem != UIItems[i] && selectedUIItem != null)
                                    selectedUIItem.ResetColor(); // a different item was selected previously

                                selectedUIItem = UIItems[i];
                                selectedUIItem.Set(mouseSelectColor);
                            }
                        }
                        else
                        {
                            /* mouse is only hovering over the item */
                            // restore old visual for the previously highlighted item
                            if (mouseoverUIItem != null && mouseoverUIItem != UIItems[i] && mouseoverUIItem != selectedUIItem) // mouse hovers over a new item
                                mouseoverUIItem.ResetColor();
                            else if (selectedUIItem != null && mouseoverUIItem == selectedUIItem) // the previous item could still be selected
                                selectedUIItem.Set(mouseSelectColor);

                            // set colors for new hover item
                            mouseoverUIItem = UIItems[i];
                            UIItems[i].Set((selectedUIItem == mouseoverUIItem) ? mouseSelectOverColor : mouseOverColor);
                            break;
                        }
                    }
                }
            }
            else
            {
                // handle remaining mouse exit visuals
                if (mouseoverUIItem != null && mouseoverUIItem != selectedUIItem)
                    mouseoverUIItem.ResetColor();
                else if (selectedUIItem != null && mouseoverUIItem == selectedUIItem)
                    selectedUIItem.Set(mouseSelectColor);
            }
        }

        public void SetInventories(Inventory buyer, Inventory seller)
        {
            if (buyer == null || seller == null)
                throw new System.NullReferenceException("buyer or seller inventory set to NULL!");
            this._buyer = buyer;
            this._seller = seller;
            SetUpInventoryElements();
        }

        public void SetUpInventoryElements()
        {
            userMode = EShopMode.buy;
            if (_seller.itemCount != 0)
                pagedItems = new PraeItem[_seller.itemCount / numElemsPerPage, numElemsPerPage];
            else
                pagedItems = new PraeItem[1, numElemsPerPage];

            for (int i = 0; i < _seller.itemCount; ++i)
                pagedItems[i / 10, i % 10] = _seller.items[i];

            UIbuyerMoney.Set(_buyer.G, _buyer.K, _buyer.T);

            SetUpPage(0);
        }

        public void SetUpPage(int p)
        {
            Debug.Assert(p >= 0 && p < pagedItems.Length, "Invalid page id to set in ShopUI!");

            for (int i = 0; i < numElemsPerPage; ++i)
            {
                UIItems[i].Set(pagedItems[p, i]);
            }

            page = p;
        }
    }
}
