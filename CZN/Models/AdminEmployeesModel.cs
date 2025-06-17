using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZN.Models
{
    public class AdminEmployeesModel
    {
        public int EmployeeID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string InternalPhone { get; set; }
        public string CityPhone { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }
        public bool IsAdmin { get; set; }
        public string Username { get; set; }
        public string DepartmentAddress { get; set; }
        public string DistrictName { get; set; }
        public bool IsLocked { get; set; }
        public bool CanBeLocked { get; set; }
    }
}
