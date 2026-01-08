using Xunit;
using Moq;
using FluentAssertions;
using Docosoft.Application.Services;
using Docosoft.Application.Interfaces;
using Docosoft.Application.Dtos.Users;
using Docosoft.Domain.Entities;
using Docosoft.Domain.Repositories;

namespace Docosoft.Application.Tests.Services
{

    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasherService> _passwordHasherMock;
        private readonly UserService _userService;
        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasherService>();

            _userService = new UserService(
                _userRepositoryMock.Object,
                _passwordHasherMock.Object);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldCreateUser_WithHashedPassword()
        {
            // mock dto
            var dto = new CreateUserDto
            {
                FirstName = "Phill",
                LastName = "Dumphy",
                Email = "phill.dumphy@email.com",
                Password = "testing123",
                BirthDate = new DateTime(1975, 06, 25),
                PhoneNumber = "+353 086 469 8963"
            };

            _passwordHasherMock
                .Setup(x => x.HashPassword(dto.Password))
                .Returns("hashed-password");

            _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .ReturnsAsync((User user) => user);

            //testing to create new
            var result = await _userService.CreateUserAsync(dto);

            // asserting restults from tests
            result.Should().NotBeNull();
            result.Email.Should().Be(dto.Email);
            result.FullName.Should().Be("Phill Dumphy");

            _userRepositoryMock.Verify(
                x => x.AddAsync(It.Is<User>(u =>
                    u.PasswordHash == "hashed-password")),
                Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            //non existing user
            var userId = Guid.NewGuid();

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync((User?)null);

            var result = await _userService.UpdateUserAsync(userId, new UpdateUserDto());

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser_WhenExists()
        {

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Claire",
                LastName = "Dumphy",
                Email = "claire.dumphy@email.com",
                CreatedAt = DateTime.UtcNow,
                BirthDate = new DateTime(1977, 02, 15),
                IsActive = true,
                PasswordHash = "3213DDADWR£32SD£DSADHJ5465wD£dasWDC56",
                PhoneNumber = "+353 154510245",
                UpdatedAt = DateTime.UtcNow.AddDays(-150)
            };

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(user.Id))
                .ReturnsAsync(user);


            var result = await _userService.GetByIdAsync(user.Id);


            result.Should().NotBeNull();
            result!.Email.Should().Be(user.Email);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldReturnTrue_WhenUserExists()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Alex",
                LastName = "Dumphy",
                Email = "alex.dumphy@email.com",
                CreatedAt = DateTime.UtcNow.AddYears(-1).AddDays(-86),
                BirthDate = new DateTime(1999, 07, 16),
                IsActive = true,
                PasswordHash = "DASD£DSADHJ5465wD£dasWDC56",
                PhoneNumber = "+353 0987624412",
                UpdatedAt = DateTime.UtcNow.AddDays(-200)
            };

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(user.Id))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(x => x.DeleteAsync(user))
                .Returns(Task.CompletedTask);

            var result = await _userService.DeleteUserAsync(user.Id);

            result.Should().BeTrue();

            _userRepositoryMock.Verify(
                x => x.DeleteAsync(user),
                Times.Once);
        }


        [Fact]
        public async Task GetByEmailAsync_ShouldReturnUser_WhenExists()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Luke",
                LastName = "Dumphy",
                Email = "luke.dumphy@email.com",
                CreatedAt = DateTime.UtcNow.AddYears(-2).AddDays(-156),
                BirthDate = new DateTime(2002, 02, 28),
                IsActive = true,
                PasswordHash = "DASD6565JTY4AS£DSADHJ5465wD£DASWDC56",
                PhoneNumber = "+353 0987645892",
                UpdatedAt = DateTime.UtcNow.AddDays(-3)
            };

            _userRepositoryMock
                .Setup(x => x.GetByEmailAsync(user.Email))
                .ReturnsAsync(user);

            var result = await _userService.GetByEmailAsync(user.Email);

            result.Should().NotBeNull();
            result!.Email.Should().Be(user.Email);
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnUsers()
        {
            var users = new List<User>
    {
        new User
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Smith",
            Email = "john.smith@email.com",
            CreatedAt = DateTime.UtcNow,
            BirthDate = new DateTime(2002, 02, 28),
            IsActive = true,
            PasswordHash = "AWEKLR6565JTY4AS£DSADHJ5465wD£DASWDC56",
            PhoneNumber = "+353 0987645692",
            UpdatedAt = DateTime.UtcNow.AddDays(-3)
        },
        new User
        {
            Id = Guid.NewGuid(),
            FirstName = "Johnny",
            LastName = "Segel",
            Email = "johnny.segel@email.com",
            CreatedAt = DateTime.UtcNow,
            BirthDate = new DateTime(2002, 02, 28),
            IsActive = true,
            PasswordHash = "45DSWSD%&6565JTY4AS£DSADHJ5465wD£DASWDC56",
            PhoneNumber = "+353 0987641529",
             UpdatedAt = DateTime.UtcNow.AddDays(-3)
        }
    };

            _userRepositoryMock
                .Setup(x => x.GetByNameAsync("John"))
                .ReturnsAsync(users);

            var result = await _userService.GetByNameAsync("John");

            result.Should().HaveCount(2);
            result[0].FullName.Should().Contain("John");
        }

        [Fact]
        public async Task SearchAsync_ShouldReturnFilteredUsers_WithPagination()
        {

            var users = new List<User>
                {
                    new User
                     {
                         Id = Guid.NewGuid(),
                         FirstName = "Claire",
                         LastName = "Dumphy",
                         Email = "claire.dumphy@email.com",
                         CreatedAt = DateTime.UtcNow,
                         BirthDate = new DateTime(1977, 02, 15),
                         IsActive = true,
                         PasswordHash = "3213DDADWR£32SD£DSADHJ5465wD£dasWDC56",
                         PhoneNumber = "+353 154510245",
                         UpdatedAt = DateTime.UtcNow.AddDays(-150)
                    },
                    new User
                     {
                        Id = Guid.NewGuid(),
                        FirstName = "Luke",
                        LastName = "Dumphy",
                        Email = "luke.dumphy@email.com",
                        CreatedAt = DateTime.UtcNow.AddYears(-2).AddDays(-156),
                        BirthDate = new DateTime(2002, 02, 28),
                        IsActive = true,
                        PasswordHash = "DASD6565JTY4AS£DSADHJ5465wD£DASWDC56",
                        PhoneNumber = "+353 0987645892",
                        UpdatedAt = DateTime.UtcNow.AddDays(-3)
                    },
                    new User
                    {
                        Id = Guid.NewGuid(),
                        FirstName = "Alex",
                        LastName = "Dumphy",
                        Email = "alex.dumphy@email.com",
                        CreatedAt = DateTime.UtcNow.AddYears(-1).AddDays(-86),
                        BirthDate = new DateTime(1999, 07, 16),
                        IsActive = true,
                        PasswordHash = "DASD£DSADHJ5465wD£dasWDC56",
                        PhoneNumber = "+353 0987624412",
                        UpdatedAt = DateTime.UtcNow.AddDays(-200)
                    }
                };

            _userRepositoryMock
                .Setup(x => x.SearchAsync("Dumphy", 0, 3))
                .ReturnsAsync(users);

            var result = await _userService.SearchAsync("Dumphy", 0, 3);

            result.Should().HaveCount(3);
            result[0].Email.Should().Be("claire.dumphy@email.com");
            result[1].FullName.ToLower().Should().Contain("luke");
            result[2].PhoneNumber.Should().Contain("+353 0987624412");
            

        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllUsers()
        {
            var users = new List<User>
                {
                    new User
                     {
                         Id = Guid.NewGuid(),
                         FirstName = "Claire",
                         LastName = "Dumphy",
                         Email = "claire.dumphy@email.com",
                         CreatedAt = DateTime.UtcNow,
                         BirthDate = new DateTime(1977, 02, 15),
                         IsActive = true,
                         PasswordHash = "3213DDADWR£32SD£DSADHJ5465wD£dasWDC56",
                         PhoneNumber = "+353 154510245",
                         UpdatedAt = DateTime.UtcNow.AddDays(-150)
                    },
                    new User
                     {
                        Id = Guid.NewGuid(),
                        FirstName = "Luke",
                        LastName = "Dumphy",
                        Email = "luke.dumphy@email.com",
                        CreatedAt = DateTime.UtcNow.AddYears(-2).AddDays(-156),
                        BirthDate = new DateTime(2002, 02, 28),
                        IsActive = true,
                        PasswordHash = "DASD6565JTY4AS£DSADHJ5465wD£DASWDC56",
                        PhoneNumber = "+353 0987645892",
                        UpdatedAt = DateTime.UtcNow.AddDays(-3)
                    },
                    new User
                    {
                        Id = Guid.NewGuid(),
                        FirstName = "Alex",
                        LastName = "Dumphy",
                        Email = "alex.dumphy@email.com",
                        CreatedAt = DateTime.UtcNow.AddYears(-1).AddDays(-86),
                        BirthDate = new DateTime(1999, 07, 16),
                        IsActive = true,
                        PasswordHash = "DASD£DSADHJ5465wD£dasWDC56",
                        PhoneNumber = "+353 0987624412",
                        UpdatedAt = DateTime.UtcNow.AddDays(-200)
                    }
                };

            _userRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(users);


            var result = await _userService.GetAllAsync();

            result.Should().HaveCount(3);
        }


    }
}
