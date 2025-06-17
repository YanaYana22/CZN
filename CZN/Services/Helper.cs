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

            return context.Employees
                .AsEnumerable()
                .Select(employee => new AdminEmployeesModel
                {
                    EmployeeID = employee.EmployeeID,
                    LastName = employee.LastName,
                    FirstName = employee.FirstName,
                    MiddleName = employee.MiddleName,
                    Department = context.Departments.FirstOrDefault(d => d.DepartmentID == employee.DepartmentID)?.Name,
                    Position = context.Positions.FirstOrDefault(p => p.PositionID == employee.PositionID)?.Title,
                    InternalPhone = context.ContactInfo.FirstOrDefault(c => c.EmployeeID == employee.EmployeeID)?.InternalPhone,
                    CityPhone = context.ContactInfo.FirstOrDefault(c => c.EmployeeID == employee.EmployeeID)?.CityPhone,
                    MobilePhone = context.ContactInfo.FirstOrDefault(c => c.EmployeeID == employee.EmployeeID)?.MobilePhone,
                    Email = context.ContactInfo.FirstOrDefault(c => c.EmployeeID == employee.EmployeeID)?.Email,
                    Notes = context.ContactInfo.FirstOrDefault(c => c.EmployeeID == employee.EmployeeID)?.Notes,
                    IsAdmin = context.Users.Any(u => u.EmployeeID == employee.EmployeeID && u.RoleID == 1),
                    Username = context.Users.FirstOrDefault(u => u.EmployeeID == employee.EmployeeID)?.Username
                })
                .ToList();
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
