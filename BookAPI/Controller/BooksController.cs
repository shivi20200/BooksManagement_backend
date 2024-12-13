
using BookAPI.Data;
using BookAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  //  [Authorize]
    [AllowAnonymous]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            // Retrieve all books from the database
            return await _context.Books.ToListAsync();
        }

        // GET: api/books/{isbn}
        [HttpGet("{isbn}")]
        [AllowAnonymous]
        public async Task<ActionResult<Book>> GetBook(string isbn)
        {
            // Find a book by ISBN
            var book = await _context.Books.FirstOrDefaultAsync(b => b.ISBN == isbn);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        // POST: api/books
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<Book>> AddBook(Book book)
        {
            // Add a new book to the database
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // Return the created book with its location
            return CreatedAtAction(nameof(GetBook), new { isbn = book.ISBN }, book);
        }

        // PUT: api/books/{isbn}
        [HttpPut("{isbn}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateBook(string isbn, Book updatedBook)
        {
            // Find the book by ISBN
            var book = await _context.Books.FirstOrDefaultAsync(b => b.ISBN == isbn);

            if (book == null)
            {
                return NotFound();
            }

            // Update the book's properties
            book.Title = updatedBook.Title;
            book.Author = updatedBook.Author;
            book.PublicationYear = updatedBook.PublicationYear;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/books/{isbn}
        [HttpDelete("{isbn}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteBook(string isbn)
        {
            // Find the book by ISBN
            var book = await _context.Books.FirstOrDefaultAsync(b => b.ISBN == isbn);

            if (book == null)
            {
                return NotFound();
            }

            // Remove the book from the database
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
