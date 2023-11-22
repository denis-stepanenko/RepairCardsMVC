namespace RepairCardsMVC.Models
{
    public class CardOwnProduct
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public string? Route { get; set; }
        public int? ParentId { get; set; }
        public bool? HasChangedComposition { get; set; }
        public bool? IsOvercoatingRequired { get; set; }

        public Card Card { get; set; }

        public List<CardOwnProductOperation> Operations { get; set; }
        public List<CardOwnProductRepairOperation> RepairOperations { get; set; }
    }
}
