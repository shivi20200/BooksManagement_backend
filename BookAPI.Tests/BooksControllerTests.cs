
using NUnit.Framework;
using BookAPI.Controllers;
using BookAPI.Data;
using BookAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPI.Tests
{
    [TestFixture]
    public class BooksControllerTests
    {
        private ApplicationDbContext _context;
        private BooksController _controller;

        [SetUp]
        public void Setup()
        {
            // Use In-Memory Database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "BookTestDb")
                .Options;

            _context = new ApplicationDbContext(options);

            // Seed Data
            _context.Books.AddRange(new List<Book>
            {
                new Book { ISBN = "111", Title = "Book 1", Author = "Author 1", PublicationYear = 2021 },
                new Book { ISBN = "222", Title = "Book 2", Author = "Author 2", PublicationYear = 2022 },
            });
            _context.SaveChanges();

            _controller = new BooksController(_context);
        }

        [TearDown]
        public void Teardown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetBooks_ReturnsAllBooks()
        {
            // Act
            var result = await _controller.GetBooks();

            // Assert
            var actionResult = result as ActionResult<IEnumerable<Book>>;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(2, actionResult.Value.Count());
        }

        [Test]
        public async Task GetBook_ValidISBN_ReturnsBook()
        {
            // Act
            var result = await _controller.GetBook("111");

            // Assert
            var actionResult = result as ActionResult<Book>;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual("Book 1", actionResult.Value.Title);
        }

        [Test]
        public async Task GetBook_InvalidISBN_ReturnsNotFound()
        {
            // Act
            var result = await _controller.GetBook("999");

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task AddBook_ValidBook_ReturnsCreatedBook()
        {
            // Arrange
            var newBook = new Book { ISBN = "333", Title = "Book 3", Author = "Author 3", PublicationYear = 2023 };

            // Act
            var result = await _controller.AddBook(newBook);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);
            var createdBook = (Book)((CreatedAtActionResult)result.Result).Value;
            Assert.AreEqual("Book 3", createdBook.Title);
        }

        [Test]
        public async Task UpdateBook_ValidISBN_UpdatesBook()
        {
            // Arrange
            var updatedBook = new Book { ISBN = "111", Title = "Updated Book", Author = "Updated Author", PublicationYear = 2025 };

            // Act
            var result = await _controller.UpdateBook("111", updatedBook);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result); // Ensure NoContentResult is returned after update

            // Fetch the updated book from the database
            var book = await _context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.ISBN == "111");
            Assert.NotNull(book); // Ensure the book exists in the database
            Assert.AreEqual("Updated Book", book.Title);
            Assert.AreEqual("Updated Author", book.Author); // Verify the author was updated
            Assert.AreEqual(2025, book.PublicationYear); 
        }
      

    }
}
