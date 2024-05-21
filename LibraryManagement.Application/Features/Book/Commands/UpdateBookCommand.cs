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
    public class UpdateBookCommand : IRequest<Response<string>>
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int PublishedYear { get; set; }
    }

    public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, Response<string>>
    {
        private readonly IBookRepository _repository;
        private readonly IMemoryCache _cache;
        private const string CacheKeyPrefix = "Book_";

        public UpdateBookCommandHandler(IBookRepository repository, IMemoryCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<Response<string>> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            var book = new LibraryManagement.Domain.Models.Book
            {
                ISBN = request.ISBN,
                Title = request.Title,
                Author = request.Author,
                PublishedYear = request.PublishedYear
            };

            await _repository.UpdateAsync(book);

            var cacheKey = $"{CacheKeyPrefix}{book.ISBN}";
            _cache.Remove(cacheKey);  // Invalidate the cache

            return new Response<string>() { Data = book.ISBN.ToString(), Success = true, Message = "Updated " };
        }
    }
}
