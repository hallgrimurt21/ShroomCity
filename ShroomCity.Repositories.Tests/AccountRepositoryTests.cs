namespace ShroomCity.Repositories.Tests;

public class AccountRepositoryTests
{
    [Fact]
    public async Task RegisterShouldCreateNewUserWhenInputModelIsValid()
    {
        // Arrange
        var mockContext = new Mock<ShroomCityDbContext>();
        var mockTokenRepository = new Mock<ITokenRepository>();
        var mockDbSetUsers = new Mock<DbSet<User>>();
        var mockDbSetRoles = new Mock<DbSet<Role>>();
        var inputModel = new RegisterInputModel
        {
            FullName = "Test User",
            EmailAddress = "test@example.com",
            Password = "TestPassword",
            Bio = "Test Bio",
            PasswordConfirmation = "TestPassword"
        };
        var analystRole = new Role { Name = RoleConstants.Analyst, Permissions = new List<Permission>() };
        var roles = new List<Role> { analystRole }.AsQueryable();

        mockDbSetRoles.As<IQueryable<Role>>().Setup(m => m.Provider).Returns(roles.Provider);
        mockDbSetRoles.As<IQueryable<Role>>().Setup(m => m.Expression).Returns(roles.Expression);
        mockDbSetRoles.As<IQueryable<Role>>().Setup(m => m.ElementType).Returns(roles.ElementType);
        mockDbSetRoles.As<IQueryable<Role>>().Setup(m => m.GetEnumerator()).Returns(roles.GetEnumerator());

        mockContext.Setup(c => c.Users).Returns(mockDbSetUsers.Object);
        mockContext.Setup(c => c.Roles).Returns(mockDbSetRoles.Object);
        mockTokenRepository.Setup(x => x.CreateToken()).ReturnsAsync(1);
        var accountRepository = new AccountRepository(mockTokenRepository.Object, mockContext.Object);

        // Act
        var result = await accountRepository.Register(inputModel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(inputModel.FullName, result.Name);
        Assert.Equal(inputModel.EmailAddress, result.EmailAddress);
        Assert.Equal(1, result.TokenId);
        mockDbSetUsers.Verify(u => u.Add(It.IsAny<User>()), Times.Once);
        mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    // [Fact]
    // public async Task RegisterShouldReturnNullWhenUserAlreadyExists()
    // {
    //     // Arrange
    //     var mockContext = new Mock<ShroomCityDbContext>();
    //     var mockTokenRepository = new Mock<ITokenRepository>();
    //     var mockDbSetUsers = new Mock<DbSet<User>>();
    //     var inputModel = new RegisterInputModel
    //     {
    //         FullName = "Test User",
    //         EmailAddress = "test@example.com",
    //         Password = "TestPassword",
    //         Bio = "Test Bio",
    //         PasswordConfirmation = "TestPassword"
    //     };
    //     var existingUser = new User { EmailAddress = "test@example.com" };
    //     mockDbSetUsers.Setup(u => u.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(existingUser);
    //     mockContext.Setup(c => c.Users).Returns(mockDbSetUsers.Object);
    //     var accountRepository = new AccountRepository(mockContext.Object, mockTokenRepository.Object);

    //     // Act
    //     var result = await accountRepository.Register(inputModel);

    //     // Assert
    //     Assert.Null(result);
    // }
    // [Fact]
    // public async Task SignInShouldReturnNullWhenUserDoesNotExist()
    // {
    //     // Arrange
    //     var mockContext = new Mock<ShroomCityDbContext>();
    //     var mockTokenRepository = new Mock<ITokenRepository>();
    //     var mockDbSetUsers = new Mock<DbSet<User>>();
    //     var inputModel = new LoginInputModel
    //     {
    //         EmailAddress = "test@example.com",
    //         Password = "TestPassword"
    //     };
    //     mockDbSetUsers.Setup(u => u.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync((User)null);
    //     mockContext.Setup(c => c.Users).Returns(mockDbSetUsers.Object);
    //     var accountRepository = new AccountRepository(mockContext.Object, mockTokenRepository.Object);

    //     // Act
    //     var result = await accountRepository.SignIn(inputModel);

    //     // Assert
    //     Assert.Null(result);
    // }
}
