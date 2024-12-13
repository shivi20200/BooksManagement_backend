
using NUnit.Framework;
using Moq;
using BookAPI.Services;
using BookAPI.Data;
using BookAPI.DTO;
using BookAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace BookAPI.Tests
{
    [TestFixture]
    public class AuthServiceTests
    {
        private Mock<IConfiguration> _mockConfiguration;
        private ApplicationDbContext _context;
        private AuthService _authService;

        [SetUp]
        public void Setup()
        {
            _mockConfiguration = new Mock<IConfiguration>();

            // Mock JWT settings
            _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("TestSecretKey123456789");
            _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("BookAPI");
            _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("BookAPIUsers");

            // Use In-Memory Database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "AuthTestDb")
                .Options;

            _context = new ApplicationDbContext(options);

            _authService = new AuthService(_mockConfiguration.Object, _context);
        }

        [TearDown]
        public void Teardown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task RegisterAsync_ValidUser_RegistersSuccessfully()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Username = "testuser",
                Email = "testuser@gmail.com",
                Password = "password123"
            };

            // Act
            var result = await _authService.RegisterAsync(request);

            // Assert
            Assert.IsNotNull(result);  // Check if result is not null
            Assert.AreEqual("testuser", result.Username);  // Check if the username is correct
        }

        [Test]
        public void HashPassword_ValidPassword_ReturnsHash()
        {
            // Arrange
            var password = "password123";

            // Act
            var hash = _authService.HashPassword(password);

            // Assert
            Assert.IsNotNull(hash);  // Check if the hash is not null
            Assert.IsTrue(hash.Contains(":")); // Hash should include salt
        }
    }
}
