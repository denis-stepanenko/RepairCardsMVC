using RepairCardsMVC.Models;

namespace RepairCardsMVC
{
    class TreeBuilder
    {
        private IEnumerable<Relation> _relations;

        public TreeProduct Get(IEnumerable<Relation> relations)
        {
            _relations = relations;

            var roots = GetChildren(null);
            Build(roots);
            CalculateCountAll(roots);

            return roots.FirstOrDefault();
        }

        void Build(List<TreeProduct> products)
        {
            foreach (var product in products)
            {
                var children = GetChildren(product);
                children.ForEach(x => x.Parent = product);
                product.Children.AddRange(children);

                Build(product.Children);
            }
        }

        void CalculateCountAll(List<TreeProduct> products)
        {
            if (products.Count == 0) return;

            products.ForEach(x => x.CountAll = GetCountAll(x));

            var children = products.SelectMany(x => x.Children).ToList();

            CalculateCountAll(children);
        }

        decimal GetCountAll(TreeProduct product)
        {
            if (product == null) return 1;

            return product.Count * (1 + product.TechWaste) * GetCountAll(product.Parent);
        }

        List<TreeProduct> GetChildren(TreeProduct product)
        {
            return _relations.Where(x => x.ParentCode == product?.Code && x.ParentType == product?.Type)
                            .Select(x => new TreeProduct
                            {
                                Code = x.Code,
                                Name = x.Name,
                                Count = x.Count,
                                TechWaste = x.TechWaste,
                                AssemblyDepartment = x.AssemblyDepartment?.PadLeft(3, '0'),
                                IsAssembly = x.IsAssembly,
                                TypeName = x.TypeName,
                                Type = x.Type,
                                Departments = GetDepartments(x),
                                Route = string.Join(" ", GetDepartments(x))
                            })
                            .ToList();
        }

        static List<string> GetDepartments(Relation item)
        {
            var departments = item.Route.Replace("  ", " ").Split(' ').ToList();

            return departments;
        }

        public Task<TreeProduct> GetAsync(IEnumerable<Relation> relations) => Task.Run(() => Get(relations));
    }
}
