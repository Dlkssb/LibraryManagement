using LibraryManagement.Application.IRepositories;
using LibraryManagement.Domain.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Infrastructure.Repository
{
    public class MockAuthenticationService : IAuthenticationService
    {
        public async Task<User> GetByIdAsync(string userId)
        {
            // Simulate fetching a user by ID
            // For testing purposes, return a dummy user or null
            // You can customize this behavior based on your testing requirements
            return await Task.FromResult<User>(null);
        }
    }
}
