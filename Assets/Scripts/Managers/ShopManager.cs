using Assets.Scripts.Items;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Managers
{
    public class ShopManager : MonoBehaviour
    {

        public enum EShopMode
        {
            unset,
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
            public int id;
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
                    _value.Set(item.value.G, item.value.K, item.value.T, item.amount);
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


        class UIButton
        {
            public Component button;
            public Image butImage;
            public Color origColor;
            public Color mouseOvercolor;
            public Color selectedColor;
            public Color selectedOverColor;

            public UIButton(Component but, Color mouseOver, Color selected, Color selectedOver)
            {
                button = but;
                butImage = but.GetComponent<Image>();
                origColor = butImage.color;
                mouseOvercolor = mouseOver;
                selectedColor = selected;
                selectedOverColor = selectedOver;
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
        Component panelItems;
        UIItem[] UIItems;
        UIItem selectedUIItem = null;
        UIItem mouseoverUIItem = null;

        Color mouseSelectColor = Color.blue;
        Color mouseSelectOverColor = Color.cyan;
        Color mouseOverColor = Color.yellow;

        Component panelTop;
        UIButton buttonSelectBuy, buttonSell, buttonRebuy, buttonClose;
        UIButton mouseOverModeButton, selectedModeButton;

        Component panelPageSetter;
        Text textPage;

        Component panelBottom;
        Button buttonBuy;
        UICurrency UIbuyerMoney;

        /* variables */
        EShopMode userMode = EShopMode.buy;
        public InteractionManager currendManager = null;
        int numElemsPerPage = 10;
        int page = 0;
        int maxPage = 1;
        PraeItem[,] pagedItems; // [page, itemsPerPage] switching the mode should adjust this 3D array
        List<PraeItem> soldItems;

        Inventory _buyer;
        Shop _seller;

        public void Awake()
        {
            Constants.ShopUI = gameObject;
            Text TG = null;
            Text TK = null;
            Text TT = null;

            // get important item elements
            UIItems = new UIItem[numElemsPerPage];
            HashSet<string> addedList = new HashSet<string>();
            int i = 0;
            foreach (Component g in gameObject.GetComponentsInChildren<Component>())
            {
                if (g.name.StartsWith("ItemElem") && !addedList.Contains(g.name))
                {
                    UIItems[i] = new UIItem(g);
                    UIItems[i].id = i;
                    addedList.Add(g.name);
                    ++i;
                }
                else if (g.name.Equals("MoneyLabel"))
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
                else if (g.name.Equals("PanelTop"))
                    panelTop = g;
                else if (g.name.Equals("PanelItems"))
                    panelItems = g;
                else if (g.name.Equals("PanelPageSetter"))
                    panelPageSetter = g;
                else if (g.name.Equals("PanelBottom"))
                    panelBottom = g;
                else if (g.name.Equals("ButtonClose"))
                    buttonClose = new UIButton(g, mouseOverColor, mouseSelectColor, mouseSelectOverColor);
                else if (g.name.Equals("ButtonSelectBuy"))
                    buttonSelectBuy = new UIButton(g, mouseOverColor, mouseSelectColor, mouseSelectOverColor);
                else if (g.name.Equals("ButtonSelectSell"))
                    buttonSell = new UIButton(g, mouseOverColor, mouseSelectColor, mouseSelectOverColor);
                else if (g.name.Equals("ButtonSelectRebuy"))
                    buttonRebuy = new UIButton(g, mouseOverColor, mouseSelectColor, mouseSelectOverColor);
                else if (g.name.Equals("TextPage"))
                    textPage = g.GetComponent<Text>();
                else if (g.name.Equals("ButtonPageLeft"))
                {
                    g.GetComponent<Button>().onClick.RemoveAllListeners();
                    g.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        if (page - 1 >= 0)
                            SetUpPage(--page);
                    });
                }
                else if (g.name.Equals("ButtonPageRight"))
                {
                    g.GetComponent<Button>().onClick.RemoveAllListeners();
                    g.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        if (page + 1 < maxPage)
                            SetUpPage(++page);
                    });
                }
                else if (g.name.Equals("ButtonBuy"))
                {
                    buttonBuy = g.GetComponent<Button>();
                    g.GetComponent<Button>().onClick.RemoveAllListeners(); // the buttons are found multiple times in the list of child components
                    g.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        switch(userMode)
                        {
                            case EShopMode.buy:
                                if (selectedUIItem != null) // an item icon can only be selected if a valid item is presend
                                    if (Input.GetKey(KeyCode.LeftControl))
                                        tryBuy(selectedUIItem, true);
                                    else
                                        tryBuy(selectedUIItem, false);
                                break;
                            case EShopMode.sell:
                                if (selectedUIItem != null)
                                    if (Input.GetKey(KeyCode.LeftControl))
                                        trySell(selectedUIItem, true);
                                    else
                                        trySell(selectedUIItem, false);
                                break;
                            case EShopMode.rebuy:
                                if (selectedUIItem != null)
                                    if (Input.GetKey(KeyCode.LeftControl))
                                        tryBuy(selectedUIItem, true);
                                    else
                                        tryBuy(selectedUIItem, false);
                                break;
                            default:
                                throw new System.NotImplementedException();
                        }

                        //TODO: apply amount selection screen during leftshift-click!
                    });
                }
            }
            UIbuyerMoney = new UICurrency(TG, TK, TT);
        }

        public void Start()
        {
            gameObject.SetActive(false);
        }

        void LateUpdate()
        {
            // handle mouse over animation
            // remove visual changes for buttons (except items)
            if (selectedModeButton != null)
                selectedModeButton.butImage.color = selectedModeButton.selectedColor;
            if (mouseOverModeButton != null && mouseOverModeButton != selectedModeButton)
                mouseOverModeButton.butImage.color = mouseOverModeButton.origColor;
            mouseOverModeButton = null;

            // remove visual changes for items
            if (selectedUIItem != null)
                selectedUIItem.Set(mouseSelectColor);
            if (mouseoverUIItem != null && mouseoverUIItem != selectedUIItem)
                mouseoverUIItem.ResetColor();
            mouseoverUIItem = null;

            if (RectTransformUtility.RectangleContainsScreenPoint((RectTransform)gameObject.transform, Input.mousePosition)) // ShopUI rect
            {
                if (RectTransformUtility.RectangleContainsScreenPoint((RectTransform)panelItems.transform, Input.mousePosition)) // PanelItems rect
                {
                    for (int i = 0; i < UIItems.Length; ++i)
                    {
                        if (RectTransformUtility.RectangleContainsScreenPoint((RectTransform)UIItems[i]._itemElem.transform, Input.mousePosition)) // items
                        {
                            // mouse is within item rect
                            if (Input.GetMouseButton(0))
                            {
                                if (UIItems[i]._hasItem && UIItems[i]._item.amount > 0)
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
                else if (RectTransformUtility.RectangleContainsScreenPoint((RectTransform)panelTop.transform, Input.mousePosition)) // top bar rect
                {
                    UIButton oldSelectButton = selectedModeButton;
                    if (RectTransformUtility.RectangleContainsScreenPoint((RectTransform)buttonClose.button.transform, Input.mousePosition))
                    {
                        if (Input.GetMouseButton(0))
                            currendManager.EndInteraction(); // end of shopping
                        else if (selectedModeButton == buttonClose)
                            selectedModeButton.butImage.color = selectedModeButton.selectedOverColor;
                        else
                            mouseOverModeButton = buttonClose;
                    }
                    else if (RectTransformUtility.RectangleContainsScreenPoint((RectTransform)buttonSelectBuy.button.transform, Input.mousePosition))
                    {
                        if (Input.GetMouseButton(0))
                            ChangeModeTo(EShopMode.buy);
                        else if (selectedModeButton == buttonSelectBuy)
                            selectedModeButton.butImage.color = selectedModeButton.selectedOverColor;
                        else
                            mouseOverModeButton = buttonSelectBuy;
                    }
                    else if (RectTransformUtility.RectangleContainsScreenPoint((RectTransform)buttonSell.button.transform, Input.mousePosition))
                    {
                        if (Input.GetMouseButton(0))
                            ChangeModeTo(EShopMode.sell);
                        else if (selectedModeButton == buttonSell)
                            selectedModeButton.butImage.color = selectedModeButton.selectedOverColor;
                        else
                            mouseOverModeButton = buttonSell;
                    }
                    else if (RectTransformUtility.RectangleContainsScreenPoint((RectTransform)buttonRebuy.button.transform, Input.mousePosition))
                    {
                        if (Input.GetMouseButton(0))
                            ChangeModeTo(EShopMode.rebuy);
                        else if (selectedModeButton == buttonRebuy)
                            selectedModeButton.butImage.color = selectedModeButton.selectedOverColor;
                        else
                            mouseOverModeButton = buttonRebuy;
                    }

                    if (mouseOverModeButton != null)
                        mouseOverModeButton.butImage.color = mouseOverModeButton.mouseOvercolor; // apply visual changes

                    if (selectedModeButton != oldSelectButton )
                    {
                        oldSelectButton.butImage.color = selectedModeButton.origColor;
                        selectedModeButton.butImage.color = selectedModeButton.selectedOverColor;
                    }
                }
            }
        }

        void ChangeModeTo(EShopMode newMode)
        {
            if (newMode == userMode)
                return;

            userMode = newMode;
            switch (newMode)
            {
                case EShopMode.buy:
                    buttonBuy.GetComponentInChildren<Text>().text = "Buy";
                    selectedModeButton = buttonSelectBuy;
                    break;
                case EShopMode.sell:
                    buttonBuy.GetComponentInChildren<Text>().text = "Sell";
                    selectedModeButton = buttonSell;
                    break;
                case EShopMode.rebuy:
                    buttonBuy.GetComponentInChildren<Text>().text = "Buys";
                    selectedModeButton = buttonRebuy;
                    break;
                default:
                    Debug.LogError("Feature not implemented!");
                    break;
            }

            calcPagedItems();
            SetUpPage(0);
        }

        void calcPagedItems()
        {
            switch(userMode)
            {
                case EShopMode.buy:
                    if (_seller.itemCount != 0)
                        pagedItems = new PraeItem[(_seller.itemCount / numElemsPerPage) + 1, numElemsPerPage];
                    else
                        pagedItems = new PraeItem[1, numElemsPerPage];
                    maxPage = pagedItems.Length / numElemsPerPage;

                    for (int i = 0; i < _seller.itemCount; ++i)
                        pagedItems[i / 10, i % 10] = _seller.items[i];
                    break;
                case EShopMode.sell:
                    if (_buyer.itemCount != 0)
                        pagedItems = new PraeItem[(_buyer.itemCount / numElemsPerPage) + 1, numElemsPerPage];
                    else
                        pagedItems = new PraeItem[1, numElemsPerPage];
                    maxPage = pagedItems.Length / numElemsPerPage;

                    for (int i = 0; i < _buyer.itemCount; ++i)
                        pagedItems[i / 10, i % 10] = _buyer.items[i];
                    break;
                case EShopMode.rebuy:
                    if (_seller.boughtItemCount != 0)
                        pagedItems = new PraeItem[(_seller.boughtItemCount / numElemsPerPage) + 1, numElemsPerPage];
                    else
                        pagedItems = new PraeItem[1, numElemsPerPage];
                    maxPage = pagedItems.Length / numElemsPerPage;

                    for (int i = 0; i < _seller.boughtItemCount; ++i)
                        pagedItems[i / 10, i % 10] = _seller.bougthItems[i];
                    break;
                default:
                    Debug.LogError("Feature not implemented!");
                    break;
            }
        }

        bool tryBuy(UIItem uitem, bool stack)
        {
            PraeItem pitem = uitem._item;
            int amount = (stack) ? System.Math.Min(pitem.amount, pitem.stackSize) : 1;
            float itemWeight = pitem.weightSingle * amount;

            // weight constraint
            if (_buyer.weight + itemWeight > _buyer.maxWeight)
            {
                Debug.Log("Cannot add item: inventory weight constraint violated!");
                return false;
            }

            // money constraint
            if (!_buyer.money.CanPay(uitem._item.value))
            {
                Debug.Log("Insufficient money!");
                return false;
            }

            // it can fit based on size
            int rest = _buyer.AddItem(new PraeItem(ref pitem, amount), true, false);
            Debug.Assert(rest == 0, "Adding item to inventory was unsuccessful!");
            uitem._item.amount -= (amount - rest);
            uitem.Set(uitem._item); // update labels
            if (uitem._item.amount <= 0)
            {
                selectedUIItem.ResetColor();
                selectedUIItem = null; // deselect item
                // NOTE: this is by far not the most efficient way of doing this - but whats the point in optimising shops anyway
                if (userMode == EShopMode.buy)
                    _seller.RemoveItem(page * 10 + uitem.id);
                else if (userMode == EShopMode.rebuy)
                    _seller.RemoveBoughtItem(page * 10 + uitem.id);
                else
                    Debug.LogError("Cannot buy item in wrong mode: " + userMode);

                calcPagedItems();   // refresh PagedItems
                if (page > maxPage)
                    page = maxPage;
                SetUpPage(page);    // update UI
            }

            // pay
            _buyer.money.Pay(uitem._item.value);
            _seller.money.Add(uitem._item.value);
            UIbuyerMoney.Set(_buyer.G, _buyer.K, _buyer.T);

            return true;
        }

        bool trySell(UIItem uitem, bool stack)
        {
            PraeItem pitem = uitem._item;
            int amount = (stack) ? System.Math.Min(pitem.amount, pitem.stackSize) : 1;
            float itemWeight = pitem.weightSingle * amount;

            // money constraint of shop
            if (!_seller.money.CanPay(uitem._item.value))
            {
                Debug.Log("Shop has insufficient money!");
                return false;
            }

            _seller.AddBoughtItem(new PraeItem(ref pitem, amount));
            uitem._item.amount -= amount;
            uitem.Set(uitem._item); // update labels
            if (uitem._item.amount <= 0)
            {
                selectedUIItem.ResetColor();
                selectedUIItem = null; // deselect item
                // NOTE: this is by far not the most efficient way of doing this - but whats the point in optimising shops anyway
                _buyer.RemoveItem(page * 10 + uitem.id);
                calcPagedItems();
                if (page > maxPage)
                    page = maxPage;
                SetUpPage(page);
            }

            // gain
            _seller.money.Pay(uitem._item.value);
            _buyer.money.Add(uitem._item.value);
            UIbuyerMoney.Set(_buyer.G, _buyer.K, _buyer.T);

            return true;
        }

        public void SetInventories(Inventory buyer, Shop seller)
        {
            if (buyer == null || seller == null)
                throw new System.NullReferenceException("buyer or seller inventory set to NULL!");
            _buyer = buyer;
            _seller = seller;
            userMode = EShopMode.unset;
            ChangeModeTo(EShopMode.buy);
            UIbuyerMoney.Set(_buyer.G, _buyer.K, _buyer.T);
            SetUpPage(0);
        }

        public void SetUpPage(int p)
        {
            Debug.Assert(p >= 0 && p < pagedItems.Length, "Invalid page id to set in ShopUI!");
            textPage.text = (page + 1) + "/" + maxPage;

            for (int i = 0; i < numElemsPerPage; ++i)
                UIItems[i].Set(pagedItems[p, i]);
            
            page = p;
        }
    }
}
