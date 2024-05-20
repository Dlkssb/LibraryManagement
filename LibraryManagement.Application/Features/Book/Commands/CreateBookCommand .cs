using MediatR;
using LibraryManagement.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagement.Application.IRepositories;
using LibraryManagement.Application.Responses;

namespace LibraryManagement.Application.Features.Book.Commands
{
    public class CreateBookCommand : IRequest<Response<LibraryManagement.Domain.Models.Book>>
    {
       // public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int PublishedYear { get; set; }
    }

    public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, Response<LibraryManagement.Domain.Models.Book>>
    {
        private readonly IBookRepository _repository;

        public CreateBookCommandHandler(IBookRepository repository)
        {
            _repository = repository;
        }

        public async Task<Response<LibraryManagement.Domain.Models.Book>> Handle(CreateBookCommand request, CancellationToken cancellationToken)
        {
            var book = new LibraryManagement.Domain.Models.Book
            {
                
                Title = request.Title,
                Author = request.Author,
                PublishedYear = request.PublishedYear
            };

            await _repository.AddAsync(book);

            return new Response<Domain.Models.Book>() { Data = book ,Success=true,Message="Created Sucssefully"};
        }
    }
}
