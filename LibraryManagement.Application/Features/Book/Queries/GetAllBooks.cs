using LibraryManagement.Application.IRepositories;
using LibraryManagement.Application.Responses;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Features.Book.Queries
{
    public class GetAllBooks : IRequest<Response<List<LibraryManagement.Domain.Models.Book>>>
    {
        public class GetBookByAllQueryHandler : IRequestHandler<GetAllBooks,Response< List<LibraryManagement.Domain.Models.Book>>>
        {
           
            private readonly IBookRepository _repository;
            private readonly IMemoryCache _cache;
            private const string CacheKeyPrefix = "Book_";

            public GetBookByAllQueryHandler(IBookRepository repository, IMemoryCache cache)
            {
                _repository = repository;
                _cache = cache;
            }
            public async Task<Response<List<LibraryManagement.Domain.Models.Book>>> Handle(GetAllBooks request, CancellationToken cancellationToken)
            {
                

                
                  var  books = await _repository.GetAllAsync();

                return new Response<List<LibraryManagement.Domain.Models.Book>>() { Data = books, Success = true, Message = "Done" };


                  
                

                
            }
        }
    }
}
