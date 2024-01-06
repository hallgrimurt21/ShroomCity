namespace ShroomCity.Repositories.Tests;

public class AccountRepositoryTests
{
    [Fact]
    public async Task RegisterShouldCreateNewUserWhenInputModelIsValid()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ShroomCityDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        using var context = new ShroomCityDbContext(options);

        var mockTokenRepository = new Mock<ITokenRepository>();
        var inputModel = new RegisterInputModel
        {
            FullName = "Test User",
            EmailAddress = "test@example.com",
            Password = "TestPassword",
            Bio = "Test Bio",
            PasswordConfirmation = "TestPassword"
        };
        var analystRole = new Role { Name = RoleConstants.Analyst, Permissions = new List<Permission>() };
        context.Roles.Add(analystRole);
        await context.SaveChangesAsync();

        mockTokenRepository.Setup(x => x.CreateToken()).ReturnsAsync(1);
        var accountRepository = new AccountRepository(mockTokenRepository.Object, context);

        // Act
        var result = await accountRepository.Register(inputModel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(inputModel.FullName, result.Name);
        Assert.Equal(inputModel.EmailAddress, result.EmailAddress);
        Assert.Equal(1, result.TokenId);
        Assert.Single(context.Users);
        var createdUser = await context.Users.FirstOrDefaultAsync(u => u.EmailAddress == inputModel.EmailAddress);
        Assert.NotNull(createdUser);
        Assert.Equal(analystRole, createdUser.Role);
    }

    [Fact]
    public async Task RegisterShouldReturnNullWhenEmailAlreadyExists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ShroomCityDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabaseRegisterEmailExists")
            .Options;
        using var context = new ShroomCityDbContext(options);
        var mockTokenRepository = new Mock<ITokenRepository>();
        var inputModel = new RegisterInputModel
        {
            FullName = "Test User",
            EmailAddress = "test@example.com",
            Password = "TestPassword",
            Bio = "Test Bio",
            PasswordConfirmation = "TestPassword"
        };
        var user = new User
        {
            EmailAddress = inputModel.EmailAddress,
            HashedPassword = inputModel.Password,
            Name = "Test User",
            Role = new Role { Name = RoleConstants.Analyst, Permissions = new List<Permission>() }
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        mockTokenRepository.Setup(x => x.CreateToken()).ReturnsAsync(1);
        var accountRepository = new AccountRepository(mockTokenRepository.Object, context);

        // Act
        var result = await accountRepository.Register(inputModel);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SignInShouldReturnUserWhenCredentialsAreValid()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ShroomCityDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabaseSignIn")
            .Options;
        using var context = new ShroomCityDbContext(options);
        var mockTokenRepository = new Mock<ITokenRepository>();
        var inputModel = new LoginInputModel
        {
            EmailAddress = "test@example.com",
            Password = "TestPassword",
        };
        var user = new User
        {
            EmailAddress = inputModel.EmailAddress,
            HashedPassword = inputModel.Password,
            Name = "Test User",
            Role = new Role { Name = RoleConstants.Analyst, Permissions = new List<Permission>() }

        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        mockTokenRepository.Setup(x => x.CreateToken()).ReturnsAsync(1);
        var accountRepository = new AccountRepository(mockTokenRepository.Object, context);

        // Act
        var result = await accountRepository.SignIn(inputModel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(inputModel.EmailAddress, result.EmailAddress);
    }

    [Fact]
    public async Task SignInShouldReturnNullWhenUserDoesNotExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ShroomCityDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabaseSignInUserDoesNotExist")
            .Options;
        using var context = new ShroomCityDbContext(options);
        var mockTokenRepository = new Mock<ITokenRepository>();
        var inputModel = new LoginInputModel
        {
            EmailAddress = "nonexistent@example.com",
            Password = "NonExistentPassword",
        };
        var accountRepository = new AccountRepository(mockTokenRepository.Object, context);
        // Act
        var result = await accountRepository.SignIn(inputModel);
        // Assert
        Assert.Null(result);
    }
}
