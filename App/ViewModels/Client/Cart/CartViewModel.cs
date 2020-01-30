using System.Collections.Generic;

namespace App.ViewModels.Client.Cart
{
    public class CartViewModel
    {
        public List<CartItemViewModel> ListItem { get; set; }
        public double Total { get; set; }
        public int Quantity { get; set; }
        public double ShippingFee { get; set; }
        public double PaymentSum
        {
            get
            {
                return Total - ShippingFee;
            }
        }

        public CartViewModel()
        {
            Total = 0;
            ListItem = new List<CartItemViewModel>();
        }

        public void UpdateItem(CartItemViewModel cartItem)
        {
            bool isFound = false;
            foreach (var item in ListItem)
            {
                if (item.Id == cartItem.Id)
                {
                    isFound = true;
                    item.Quantity += cartItem.Quantity;
                }
            }
            if (!isFound)
            {
                ListItem.Add(cartItem);
            }

            Total += (double)cartItem.GetPriceAfterDiscount() * cartItem.Quantity;
            Quantity += cartItem.Quantity;
        }

        public CartItemViewModel GetItem(int id)
        {
            foreach (var item in ListItem)
            {
                if (item.Id == id)
                {
                    return item;
                }
            }
            return null;
        }

        public void RecalculateTotalAndQuantity()
        {
            Quantity = 0;
            Total = 0;
            foreach (var item in ListItem)
            {
                Quantity += item.Quantity;
                Total += (double)item.GetPriceAfterDiscount() * item.Quantity;
            }
        }

        public bool HasInvalidQuantity()
        {
            bool result = false;

            foreach (var item in ListItem)
            {
                if (item.Quantity > item.RemainQuantity)
                {
                    result = true;
                }
            }

            return result;
        }
    }
}