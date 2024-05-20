using LibraryManagement.Application.IRepositories;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Features.Book.Commands
{
    public class DeleteBookCommand : IRequest<LibraryManagement.Domain.Models.Book>
    {
        public string ISBN { get; set; }


        public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand, LibraryManagement.Domain.Models.Book>
        {
            private readonly IBookRepository _repository;
            private readonly IMemoryCache _cache;
            private const string CacheKeyPrefix = "Book_";

            public DeleteBookCommandHandler(IBookRepository repository, IMemoryCache cache)
            {
                _repository = repository;
                _cache = cache;
            }

            public async Task<LibraryManagement.Domain.Models.Book> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
            {
                var book = await _repository.GetByISBNAsync(request.ISBN);
                if (book != null)
                {
                    await _repository.DeleteAsync(request.ISBN);
                    var cacheKey = $"{CacheKeyPrefix}{request.ISBN}";
                    _cache.Remove(cacheKey);  // Invalidate the cache
                }

                return book;
            }
        }

    }


}
