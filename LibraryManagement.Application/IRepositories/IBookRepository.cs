using LibraryManagement.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.IRepositories
{
    public interface IBookRepository
    {
        Task<Book> GetByISBNAsync(string isbn);
        Task AddAsync(Book book);
        Task UpdateAsync(Book book);
        Task DeleteAsync(string isbn);
        Task<List<Book>> GetAllAsync();
    }
}
