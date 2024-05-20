using LibraryManagement.Application.Features.Book.Commands;
using LibraryManagement.Application.Features.Book.Queries;
using LibraryManagement.Application.Responses;
using LibraryManagement.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibraryManagement.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly IMediator _mediator;

    public BooksController(IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpGet("GetJWT/{userID}")]
    public async Task<ActionResult<string>> GetJWT(string userId)
    {

        


        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("ThisIsASecretKeyForJWTTokenGeneration"); // Replace with your secret key
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim("uid", userId),
                // Add more claims as needed
            }),
            Expires = DateTime.UtcNow.AddDays(1), // Token expiration time
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    [HttpGet("{isbn}")]
    public async Task<ActionResult<Response<Book>>> GetBook(string isbn)
    {
        var query = new GetBookByISBNQuery { ISBN = isbn };
        var book = await _mediator.Send(query);

        if (book == null)
        {
            return NotFound();
        }

        return book;
    }

    [HttpGet("books/all")]
    [Authorize]
    public async Task<ActionResult<Response<List<Book>>>> GetAllBooks()
    {
        return await _mediator.Send(new GetAllBooks());
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Book>> CreateBook(CreateBookCommand command)
    {
        var book = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetBook), new { isbn = book.Data.ISBN }, book);
    }

    [HttpPut("{isbn}")]
    [Authorize]
    public async Task<IActionResult> UpdateBook(string isbn, UpdateBookCommand command)
    {
        if (isbn != command.ISBN.ToString())
        {
            return BadRequest();
        }

        var updatedBook = await _mediator.Send(command);

        if (updatedBook == null)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{isbn}")]
    [Authorize]
    public async Task<IActionResult> DeleteBook(string isbn)
    {
        var command = new DeleteBookCommand { ISBN = new Guid(isbn) };
        var result = await _mediator.Send(command);

        if (result == null)
        {
            return NotFound();
        }

        return NoContent();
    }
}
