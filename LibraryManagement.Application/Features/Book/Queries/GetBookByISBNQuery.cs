using LibraryManagement.Application.IRepositories;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Features.Book.Queries
{
    public class GetBookByISBNQuery : IRequest<LibraryManagement.Domain.Models.Book>
    {
        public string ISBN { get; set; }


        public class GetBookByISBNQueryHandler : IRequestHandler<GetBookByISBNQuery, LibraryManagement.Domain.Models.Book>
        {
            private readonly IBookRepository _repository;
            private readonly IMemoryCache _cache;
            private const string CacheKeyPrefix = "Book_";

            public GetBookByISBNQueryHandler(IBookRepository repository, IMemoryCache cache)
            {
                _repository = repository;
                _cache = cache;
            }

            public async Task<LibraryManagement.Domain.Models.Book> Handle(GetBookByISBNQuery request, CancellationToken cancellationToken)
            {
                var cacheKey = $"{CacheKeyPrefix}{request.ISBN}";

                if (!_cache.TryGetValue(cacheKey, out LibraryManagement.Domain.Models.Book book))
                {
                    book = await _repository.GetByISBNAsync(request.ISBN);

                    if (book != null)
                    {
                        var cacheOptions = new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromMinutes(2))  // Adjust the expiration time as needed
                            .SetAbsoluteExpiration(TimeSpan.FromHours(1));  // Set a maximum lifetime for the cache entry

                        _cache.Set(cacheKey, book, cacheOptions);
                    }
                }

                return book;
            }
        }
    }
}
