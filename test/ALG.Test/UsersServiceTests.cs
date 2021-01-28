using ALG.Application.Settings;
using ALG.Application.Users;
using ALG.Application.Users.Dto;
using ALG.Core.Users;
using ALG.EntityFrameworkCore;
using ALG.EntityFrameworkCore.EntityFrameworkCore.Repositories;
using ALG.EntityFrameworkCore.EntityFrameworkCore.Seed;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Xunit;

namespace ALG.Test
{
    public class UsersServiceTests
    {
        private readonly IUsersService _usersService;
        private readonly IUsersRepository _usersRepository;

        #region "Init"

        public UsersServiceTests()
        {
            //create InMemory DbContext
            var builder = new DbContextOptionsBuilder<AlgDbContext>();
            builder.UseInMemoryDatabase("UsersServiceTests")
                   .EnableSensitiveDataLogging();
            var options = builder.Options;
            var context = new AlgDbContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            //init UsersService
            _usersRepository = new UsersRepository(context);
            var config = new MapperConfiguration(c =>
            {
                c.AddProfile<UsersMapProfile>();
            });
            var mapper = config.CreateMapper();

            var settings = new JWTSettings
            {
                Secret = "REPLACE THIT WITH YOUR OWN SECRET STRING",
                TokenValidHrs = 10
            };
            _usersService = new UsersService(_usersRepository, mapper,
                            Microsoft.Extensions.Options.Options.Create(settings));

            //seed with initial data
            new UsersSeeds(context).Seed();
        }

        #endregion

        #region "Login"

        [Fact]
        public async void UserShouldLoginAsyncSuccess()
        {
            //Arrange
            var credentialsDto = new CredentialsDto
            {
                Email = "john.dow@gmail.com",
                Password = "111"
            };

            //Act
            var authenticatedDto = await _usersService.LoginAsync(credentialsDto);
            var testUser = await _usersRepository.GetUserByEmailAsync(credentialsDto.Email);

            //Assert
            Assert.NotNull(authenticatedDto);
            Assert.NotNull(testUser);
            Assert.Equal(authenticatedDto.Id, testUser.Id);
            Assert.Equal(authenticatedDto.Name, testUser.Name);
            Assert.Equal(authenticatedDto.Role, testUser.Role);
        }

        [Fact]
        public async void UserNotFoundShouldNotLoginAsyncFailure()
        {
            //Arrange
            var credentialsDto = new CredentialsDto
            {
                Email = "xxx@gmail.com",
                Password = "111"
            };

            //Act
            var authenticatedDto = await _usersService.LoginAsync(credentialsDto);

            //Assert
            Assert.Null(authenticatedDto);
        }

        [Fact]
        public async void UserBadPasswordShouldNotLoginAsyncFailure()
        {
            //Arrange
            var credentialsDto = new CredentialsDto
            {
                Email = "john.dow@gmail.com",
                Password = "xxx"
            };

            //Act
            var authenticatedDto = await _usersService.LoginAsync(credentialsDto);

            //Assert
            Assert.Null(authenticatedDto);
        }

        #endregion

        #region "CredentialsDtoValidator"

        [Fact]
        public void CredentialsDtoValidatorValid()
        {
            //Arrange
            var credentialsDto = new CredentialsDto
            {
                Email = "john.dow@gmail.com",
                Password = "111"
            };

            //Act
            var validationResult = new CredentialsDtoValidator().Validate(credentialsDto);

            //Assert
            Assert.Empty(validationResult.Errors);
        }

        [Fact]
        public void CredentialsDtoValidatorEmptyEmailPasswordNotValid()
        {
            //Arrange
            var credentialsDto = new CredentialsDto {};

            //Act
            var validationResult = new CredentialsDtoValidator().Validate(credentialsDto);

            //Assert
            Assert.Equal(2, validationResult.Errors.Count);
            Assert.True(validationResult.Errors.Count(x => x.PropertyName == "Email") == 1);
            Assert.True(validationResult.Errors.Count(x => x.PropertyName == "Password") == 1);
        }

        [Fact]
        public void CredentialsDtoValidatorBadEmailFormatNotValid()
        {
            //Arrange
            var credentialsDto = new CredentialsDto
            {
                Email = "not an Email",
                Password = "111"
            };

            //Act
            var validationResult = new CredentialsDtoValidator().Validate(credentialsDto);

            //Assert
            Assert.Single(validationResult.Errors);
            Assert.True(validationResult.Errors.Count(x => x.PropertyName == "Email") == 1);
        }

        #endregion

    }
}
