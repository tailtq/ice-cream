namespace App.ViewModels.Client.Cart
{
    public class CartItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Quantity { get; set; }
        public int RemainQuantity { get; set; }
        public decimal Price { get; set; }
        public double Discount { get; set; }
        public string Image { get; set; }

        public decimal GetPriceAfterDiscount()
        {
            return Price - Price * (decimal) Discount / 100;
        }
    }
}