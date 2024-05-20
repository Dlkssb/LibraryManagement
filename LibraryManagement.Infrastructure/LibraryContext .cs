using LibraryManagement.Domain.Models;
using Microsoft.EntityFrameworkCore;
namespace LibraryManagement.Infrastructure;

public class LibraryContext : DbContext
{
    public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }
    public  DbSet<Book> Books { get; set; }
}
