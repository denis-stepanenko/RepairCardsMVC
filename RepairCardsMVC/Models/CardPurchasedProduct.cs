namespace RepairCardsMVC.Models
{
    public class CardPurchasedProduct
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }

        public Card Card { get; set; }

        public List<CardPurchasedProductOperation> Operations { get; set; }
    }
}
