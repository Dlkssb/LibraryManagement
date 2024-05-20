using LibraryManagement.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.IRepositories
{
    public interface IAuthenticationService
    {
        Task<User> GetByIdAsync(string userId);
    }

   
}
