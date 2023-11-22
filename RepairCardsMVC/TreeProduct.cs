namespace RepairCardsMVC
{
    public class TreeProduct
    {
        public TreeProduct()
        {
            Children = new List<TreeProduct>();
        }

		public int Id { get; set; }
		public string ParentId { get; set; }
		public string Code { get; set; }
        public decimal Count { get; set; }
        public decimal CountAll { get; set; }
        public decimal TechWaste { get; set; }
        public string Name { get; set; }
        public string AssemblyDepartment { get; set; }
        public int Type { get; set; }
        public string TypeName { get; set; }
        public bool IsAssembly { get; set; }
		public bool? IsOvercoatingRequired { get; set; }
		public bool? HasChangedComposition { get; set; }
		public string? Route { get; set; }
        public bool IsChecked { get; set; }

        public List<string> Departments { get; set; }

        public TreeProduct Parent { get; set; }
        public List<TreeProduct> Children { get; set; }
    }
}
