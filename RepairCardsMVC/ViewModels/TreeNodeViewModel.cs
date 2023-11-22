namespace RepairCardsMVC.ViewModels
{
    public class TreeNodeViewModel
    {
        public int Id { get; set; }
        public string ParentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Count { get; set; }
        public string Route { get; set; }
        public bool IsChecked { get; set; }
        public bool? HasChangedComposition { get; set; }
        public bool? IsOvercoatingRequired { get; set; }
    }
}
