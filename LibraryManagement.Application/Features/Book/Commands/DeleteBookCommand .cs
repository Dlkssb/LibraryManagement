using LibraryManagement.Application.IRepositories;
using LibraryManagement.Application.Responses;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Features.Book.Commands
{
    public class DeleteBookCommand : IRequest<Response<LibraryManagement.Domain.Models.Book>>
    {
        public Guid ISBN { get; set; }


        public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand, Response<LibraryManagement.Domain.Models.Book>>
        {
            private readonly IBookRepository _repository;
            private readonly IMemoryCache _cache;
            private const string CacheKeyPrefix = "Book_";

            public DeleteBookCommandHandler(IBookRepository repository, IMemoryCache cache)
            {
                _repository = repository;
                _cache = cache;
            }

            public async Task<Response<LibraryManagement.Domain.Models.Book>> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
            {
                var book = await _repository.GetByISBNAsync(request.ISBN.ToString());
                if (book != null)
                {
                    await _repository.DeleteAsync(request.ISBN.ToString());
                    var cacheKey = $"{CacheKeyPrefix}{request.ISBN}";
                    _cache.Remove(cacheKey);  // Invalidate the cache
                }
                var res = new Response<LibraryManagement.Domain.Models.Book>()
                {
                    Data = book,
                    Success = true,
                    Message = "Deleted Succesfully"
                };

                return res;
            }
        }

    }


}
