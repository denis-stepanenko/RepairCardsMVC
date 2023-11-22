using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace RepairCardsMVC.Models
{
    public class Operation
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите код")]
        [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "Код должен состоять из 6 цифр")]
        [Remote(action: "VerifyCode", controller: "Operation", AdditionalFields = nameof(Id), ErrorMessage = "Операция с таким номером уже существует.")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Введите наименование")]
        [MaxLength(500)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите трудоемкость")]
        public decimal Labor { get; set; }

        public decimal? Price { get; set; }
        public string? GroupName { get; set; }
        public int Department { get; set; }
        public string? UnitName { get; set; }
        public string? Description { get; set; }
        public bool IsInactive { get; set; }

        public string? ProductionOperationCode { get; set; }
        public ProductionOperation? ProductionOperation { get; set; }
    }
}
