using CZN.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CZN.Services
{
    public class Helper
    {
        private static CZNEntities _context;

        public static CZNEntities GetContext()
        {
            if (_context == null)
            {
                _context = new CZNEntities();
            }
            return _context;
        }
        public static Users AuthUser(string username, string password)
        {
            try
            {
                string hashedPassword = Hash.HashPassword(password);

                var user = GetContext().Users.FirstOrDefault(u =>
                           u.Username == username &&
                           u.PasswordHash == hashedPassword);

                if (user != null)
                {
                    return new Users
                    {
                        UserID = user.UserID,
                        Username = user.Username,
                        PasswordHash = user.PasswordHash,
                        RoleID = user.RoleID,
                        EmployeeID = user.EmployeeID
                    };
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        public static bool IsAdmin(Users user)
        {
            return user?.RoleID == 1;
        }
        public static List<AdminEmployeesModel> GetEmployeesWithDetails()
        {
            var context = GetContext();

            return (from e in context.Employees
                    join d in context.Departments on e.DepartmentID equals d.DepartmentID
                    join dist in context.Districts on d.DistrictID equals dist.DistrictID
                    join p in context.Positions on e.PositionID equals p.PositionID
                    from c in context.ContactInfo.Where(ci => ci.EmployeeID == e.EmployeeID).DefaultIfEmpty()
                    from u in context.Users.Where(user => user.EmployeeID == e.EmployeeID).DefaultIfEmpty()
                    select new AdminEmployeesModel
                    {
                        EmployeeID = e.EmployeeID,
                        LastName = e.LastName,
                        FirstName = e.FirstName,
                        MiddleName = e.MiddleName,
                        Department = d.Name,
                        DepartmentAddress = d.Address,
                        DistrictName = dist.Name,
                        Position = p.Title,
                        InternalPhone = c.InternalPhone,
                        CityPhone = c.CityPhone,
                        MobilePhone = c.MobilePhone,
                        Email = c.Email,
                        Notes = c.Notes,
                        IsAdmin = u != null && u.RoleID == 1,
                        Username = u.Username
                    }).ToList();
        }
        public static bool SaveEmployee(AdminEmployeesModel model, string username = null, string password = null)
        {
            try
            {
                var context = GetContext();
                using (var transaction = context.Database.BeginTransaction())
                {
                    var employee = model.EmployeeID == 0
                        ? new Employees()
                        : context.Employees.FirstOrDefault(e => e.EmployeeID == model.EmployeeID);

                    if (employee == null) return false;

                    employee.LastName = model.LastName;
                    employee.FirstName = model.FirstName;
                    employee.MiddleName = model.MiddleName;
                    employee.DepartmentID = context.Departments.First(d => d.Name == model.Department).DepartmentID;
                    employee.PositionID = context.Positions.First(p => p.Title == model.Position).PositionID;

                    if (model.EmployeeID == 0)
                    {
                        context.Employees.Add(employee);
                    }
                    context.SaveChanges();

                    var contact = context.ContactInfo.FirstOrDefault(c => c.EmployeeID == employee.EmployeeID)
                                 ?? new ContactInfo { EmployeeID = employee.EmployeeID };

                    contact.InternalPhone = model.InternalPhone;
                    contact.CityPhone = model.CityPhone;
                    contact.MobilePhone = model.MobilePhone;
                    contact.Email = model.Email;
                    contact.Notes = model.Notes;

                    if (contact.ContactID == 0)
                    {
                        context.ContactInfo.Add(contact);
                    }
                    context.SaveChanges();

                    if (!string.IsNullOrEmpty(username))
                    {
                        var user = context.Users.FirstOrDefault(u => u.EmployeeID == employee.EmployeeID)
                                  ?? new Users { EmployeeID = employee.EmployeeID };

                        user.Username = username;
                        user.PasswordHash = Hash.HashPassword(password);
                        user.RoleID = model.IsAdmin ? 1 : 2;

                        if (user.UserID == 0)
                        {
                            context.Users.Add(user);
                        }
                        context.SaveChanges();
                    }

                    transaction.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка сохранения сотрудника: {ex.Message}");
                return false;
            }
        }

        public static bool DeleteEmployee(int employeeId)
        {
            try
            {
                var context = GetContext();
                using (var transaction = context.Database.BeginTransaction())
                {
                    var contacts = context.ContactInfo.Where(c => c.EmployeeID == employeeId).ToList();
                    context.ContactInfo.RemoveRange(contacts);

                    var users = context.Users.Where(u => u.EmployeeID == employeeId).ToList();
                    context.Users.RemoveRange(users);

                    var employee = context.Employees.FirstOrDefault(e => e.EmployeeID == employeeId);
                    if (employee != null)
                    {
                        context.Employees.Remove(employee);
                    }

                    context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка удаления сотрудника: {ex.Message}");
                return false;
            }
        }
    }
}
