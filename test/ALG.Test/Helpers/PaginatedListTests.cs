using ALG.Application.Application.Settings;
using ALG.Application.Helpers.Paging;
using ALG.Application.Helpers.Paging.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ALG.Test.Helpers
{
    public class PaginatedListTests
    {
        private readonly TestDbContext _context;
        private readonly IOptions<PagingSettings> _settings;
        private readonly ICollection<TestModel> _items;
        private const int _testItemsCount = 10;

        #region "Init"

        public PaginatedListTests()
        {
            //create InMemory DbContext
            DbContextOptions<TestDbContext> options;
            var builder = new DbContextOptionsBuilder<TestDbContext>();
            builder.UseInMemoryDatabase("PaginatedListTests")
                   .EnableSensitiveDataLogging();
            options = builder.Options;
            _context = new TestDbContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            var settings = new PagingSettings
            {
                PageSize = 20
            };
            _settings = Microsoft.Extensions.Options.Options.Create(settings);


            _items = new List<TestModel>();
            for (int i = 0; i < _testItemsCount; i++)
                _items.Add(new TestModel() { Id = Guid.NewGuid(), Name = "test" + i });

            for (int i = 0; i < _testItemsCount; i++)
                _items.Add(new TestModel() { Id = Guid.NewGuid(), Name = "name" + i });

            _context.AddRange(_items);
            _context.SaveChanges();
        }

        #endregion

        #region "GetPaginatedListDtoValidator methods"

        [Fact]
        public void GetPaginatedListDtoValidatorEmptyCorrectIsValid()
        {
            // Arrange
            var getPaginatedListDto = new GetPaginatedListDto
            {
            };

            //Act
            var validationResult = new GetPaginatedListDtoValidator().Validate(getPaginatedListDto);

            // Assert
            Assert.Empty(validationResult.Errors);
        }

        [Fact]
        public void GetPaginatedListDtoValidatorCorrectIsValid()
        {
            // Arrange
            var getPaginatedListDto = new GetPaginatedListDto
            {
                CurrentPage = 1,
                PageSize = 20
            };

            //Act
            var validationResult = new GetPaginatedListDtoValidator().Validate(getPaginatedListDto);

            // Assert
            Assert.Empty(validationResult.Errors);
        }

        [Fact]
        public void GetPaginatedListDtoValidatorIncorrectIsNotValid()
        {
            // Arrange
            var getPaginatedListDto = new GetPaginatedListDto
            {
                CurrentPage = -1,
                PageSize = -2
            };

            //Act
            var validationResult = new GetPaginatedListDtoValidator().Validate(getPaginatedListDto);

            // Assert
            Assert.True(validationResult.Errors.Count == 2);
            Assert.True(validationResult.Errors.Count(x => x.PropertyName == "CurrentPage") == 1);
            Assert.True(validationResult.Errors.Count(x => x.PropertyName == "PageSize") == 1);
        }

        #endregion

        #region "GetPaginatedListDto methodes"

        [Fact]
        public async Task GetAllItemsCorrectValid()
        {
            // Arrange
            var getPaginatedListDto = new GetPaginatedListDto()
            {
                CurrentPage = 1,
                PageSize = 10
            };

            //Act
            var testModelListDto = await GetAllItemsFilteredAsync(getPaginatedListDto);

            // Assert
            Assert.True(testModelListDto.CurrentPage == getPaginatedListDto.CurrentPage);
            Assert.True(testModelListDto.PageSize == getPaginatedListDto.PageSize);
            Assert.True(testModelListDto.TotalItems == _items.Count);
        }

        [Fact]
        public async Task GetAllItemsEmptyDtoCorrectValid()
        {
            // Arrange
            var getPaginatedListDto = new GetPaginatedListDto()
            {
            };

            //Act
            var testModelListDto = await GetAllItemsFilteredAsync(getPaginatedListDto);

            // Assert
            Assert.True(testModelListDto.CurrentPage == 1);
            Assert.True(testModelListDto.PageSize == _settings.Value.PageSize);
            Assert.True(testModelListDto.TotalItems == _items.Count);
            Assert.True(testModelListDto.IsFirst == true);
            Assert.True(testModelListDto.IsLast == true);
        }

        [Fact]
        public async Task GetAllItemsLastPageCorrectValid()
        {
            // Arrange
            var getPaginatedListDto = new GetPaginatedListDto()
            {
                CurrentPage = 2,
                PageSize = 10
            };

            //Act
            var testModelListDto = await GetAllItemsFilteredAsync(getPaginatedListDto);

            // Assert
            Assert.True(testModelListDto.CurrentPage == 2);
            Assert.True(testModelListDto.PageSize == getPaginatedListDto.PageSize);
            Assert.True(testModelListDto.TotalItems == _items.Count);
            Assert.True(testModelListDto.IsFirst == false);
            Assert.True(testModelListDto.IsLast == true);
        }


        [Fact]
        public async Task GetAllItemsEmptyDtoEmptyListCorrectValid()
        {
            // Arrange
            var getPaginatedListDto = new GetPaginatedListDto()
            {
            };

            _context.RemoveRange(_items);
            _context.SaveChanges();

            //Act
            var testModelListDto = await GetAllItemsFilteredAsync(getPaginatedListDto);

            // Assert
            Assert.True(testModelListDto.CurrentPage == 1);
            Assert.True(testModelListDto.PageSize == _settings.Value.PageSize);
            Assert.True(testModelListDto.TotalItems == 0);
            Assert.True(testModelListDto.IsFirst == true);
            Assert.True(testModelListDto.IsLast == true);
        }

        [Fact]
        public async Task GetAllItemsDefaultEmptyListCorrectValid()
        {
            // Arrange
            var getPaginatedListDto = new GetPaginatedListDto()
            {
                CurrentPage = 0,
                PageSize = 0
            };

            _context.RemoveRange(_items);
            _context.SaveChanges();

            //Act
            var testModelListDto = await GetAllItemsFilteredAsync(getPaginatedListDto);

            // Assert
            Assert.True(testModelListDto.CurrentPage == 1);
            Assert.True(testModelListDto.PageSize == _settings.Value.PageSize);
            Assert.True(testModelListDto.TotalItems == 0);
            Assert.True(testModelListDto.IsFirst == true);
            Assert.True(testModelListDto.IsLast == true);
        }

        [Fact]
        public async Task GetAllItemsPage2EmptyListCorrectValid()
        {
            // Arrange
            var getPaginatedListDto = new GetPaginatedListDto()
            {
                CurrentPage = 2
            };

            _context.RemoveRange(_items);
            _context.SaveChanges();

            //Act
            var testModelListDto = await GetAllItemsFilteredAsync(getPaginatedListDto);

            // Assert
            Assert.True(testModelListDto.CurrentPage == 2);
            Assert.True(testModelListDto.PageSize == _settings.Value.PageSize);
            Assert.True(testModelListDto.TotalItems == 0);
            Assert.True(testModelListDto.IsFirst == false);
            Assert.True(testModelListDto.IsLast == true);

        }

        [Fact]
        public async Task GetAllItemsPageSize0EmptyListCorrectValid()
        {
            // Arrange
            var getPaginatedListDto = new GetPaginatedListDto()
            {
                CurrentPage = 1,
                PageSize = 0
            };
            _settings.Value.PageSize = 0;

            _context.RemoveRange(_items);
            _context.SaveChanges();

            //Act
            var testModelListDto = await GetAllItemsFilteredAsync(getPaginatedListDto);

            // Assert
            Assert.True(testModelListDto.CurrentPage == 1);
            Assert.True(testModelListDto.PageSize == _settings.Value.PageSize);
            Assert.True(testModelListDto.TotalItems == 0);
            Assert.True(testModelListDto.TotalPages == 0);
            Assert.True(testModelListDto.IsFirst == true);
            Assert.True(testModelListDto.IsLast == true);
        }

        [Fact]
        public async Task GetAllItemsPageSize0CorrectValid()
        {
            // Arrange
            var getPaginatedListDto = new GetPaginatedListDto()
            {
                CurrentPage = 1,
                PageSize = 0
            };
            _settings.Value.PageSize = 0;

            //Act
            var testModelListDto = await GetAllItemsFilteredAsync(getPaginatedListDto);

            // Assert
            Assert.True(testModelListDto.CurrentPage == 1);
            Assert.True(testModelListDto.PageSize == _settings.Value.PageSize);
            Assert.True(testModelListDto.TotalItems == _items.Count());
            Assert.True(testModelListDto.Items.Count() == 0);
            Assert.True(testModelListDto.IsFirst == true);
            Assert.True(testModelListDto.IsLast == true);
        }

        [Fact]
        public async Task GetAllItemsDefaultsCorrectValid()
        {
            // Arrange
            var getPaginatedListDto = new GetPaginatedListDto()
            {
                CurrentPage = 0,
                PageSize = 0
            };

            //Act
            var testModelListDto = await GetAllItemsFilteredAsync(getPaginatedListDto);

            // Assert
            Assert.True(testModelListDto.CurrentPage == 1);
            Assert.True(testModelListDto.PageSize == _settings.Value.PageSize);
            Assert.True(testModelListDto.TotalItems == _items.Count);
            Assert.True(testModelListDto.IsFirst == true);
            Assert.True(testModelListDto.IsLast == true);
        }

        [Fact]
        public async Task GetAllItemsPagesizeAllCorrectValid()
        {
            // Arrange
            var getPaginatedListDto = new GetPaginatedListDto()
            {
                CurrentPage = 1,
                PageSize = -1
            };

            //Act
            var testModelListDto = await GetAllItemsFilteredAsync(getPaginatedListDto);

            // Assert
            Assert.True(testModelListDto.CurrentPage == 1);
            Assert.True(testModelListDto.TotalItems == _items.Count);
            Assert.True(testModelListDto.PageSize == testModelListDto.TotalItems);
            Assert.True(testModelListDto.IsFirst == true);
            Assert.True(testModelListDto.IsLast == true);
        }

        [Fact]
        public async Task GetAllItemsPageSize6CurrPage1CorrectValid()
        {
            // Arrange
            var getPaginatedListDto = new GetPaginatedListDto()
            {
                CurrentPage = 1,
                PageSize = 6
            };

            //Act
            var testModelListDto = await GetAllItemsFilteredAsync(getPaginatedListDto);

            // Assert
            Assert.True(testModelListDto.TotalPages == 4);
            Assert.True(testModelListDto.Items.Count == getPaginatedListDto.PageSize);

            //Check with formula
            Assert.True(testModelListDto.TotalPages == (int)Math.Ceiling((_items.Count) / (double)testModelListDto.PageSize));
            if (testModelListDto.CurrentPage < testModelListDto.TotalPages)
                Assert.True(testModelListDto.Items.Count == testModelListDto.PageSize);
            else
                Assert.True(testModelListDto.Items.Count == _items.Count - ((testModelListDto.TotalPages - 1) * testModelListDto.PageSize));
            Assert.True(testModelListDto.IsFirst == true);
            Assert.True(testModelListDto.IsLast == false);
        }

        [Fact]
        public async Task GetAllItemsPageSize6CurrPage4CorrectValid()
        {
            // Arrange
            var getPaginatedListDto = new GetPaginatedListDto()
            {
                CurrentPage = 4,
                PageSize = 6
            };

            //Act
            var testModelListDto = await GetAllItemsFilteredAsync(getPaginatedListDto);

            // Assert
            Assert.True(testModelListDto.TotalPages == 4);
            Assert.True(testModelListDto.Items.Count == 2);

            //Check with formula
            Assert.True(testModelListDto.TotalPages == (int)Math.Ceiling((_items.Count) / (double)testModelListDto.PageSize));
            if (testModelListDto.CurrentPage < testModelListDto.TotalPages)
                Assert.True(testModelListDto.Items.Count == testModelListDto.PageSize);
            else
                Assert.True(testModelListDto.Items.Count == _items.Count - ((testModelListDto.TotalPages - 1) * testModelListDto.PageSize));
            Assert.True(testModelListDto.IsFirst == false);
            Assert.True(testModelListDto.IsLast == true);
        }

        [Fact]
        public async Task GetAllItemsAllPageSizesAllCurrPagesCorrectValid()
        {
            for (int pageSize = 1; pageSize <= _items.Count; pageSize++)
            {
                for (int currentPage = _items.Count; currentPage >= 1; currentPage--)
                {
                    // Arrange
                    var getPaginatedListDto = new GetPaginatedListDto()
                    {
                        CurrentPage = currentPage,
                        PageSize = pageSize
                    };

                    //Act
                    var testModelListDto = await GetAllItemsFilteredAsync(getPaginatedListDto);

                    // Assert
                    Assert.True(testModelListDto.TotalPages == (int)Math.Ceiling((_items.Count) / (double)testModelListDto.PageSize));
                    if (testModelListDto.CurrentPage < testModelListDto.TotalPages)
                        Assert.True(testModelListDto.Items.Count == testModelListDto.PageSize);
                    else if (testModelListDto.CurrentPage > testModelListDto.TotalPages)
                        Assert.True(testModelListDto.Items.Count == 0);
                    else
                        Assert.True(testModelListDto.Items.Count == _items.Count -
                                ((testModelListDto.TotalPages - 1) * testModelListDto.PageSize));
                }
            }
        }

        #endregion

        #region "Filter methodes"

        [Fact]
        public async Task GetAllItemsFilteredCorrectValid()
        {
            // Arrange
            var getPaginatedListDto = new GetPaginatedListDto()
            {
                Filter = "test",
                CurrentPage = 1,
                PageSize = 10
            };

            //Act
            var testModelListDto = await GetAllItemsFilteredAsync(getPaginatedListDto);

            // Assert
            Assert.True(testModelListDto.TotalItems == _testItemsCount);
            Assert.True(testModelListDto.TotalPages == 1);
            Assert.True(testModelListDto.IsFirst == true);
            Assert.True(testModelListDto.IsLast == true);
        }

        #endregion

        #region "Private methodes"

        private IQueryable<TestModel> GetAllItemsAsync(string filter)
        {
            IQueryable<TestModel> items = _context.TestModels.AsNoTracking();

            if (filter != null && filter.Length > 0)
                items = items.Where(e => e.Name.Contains(filter));

            return items;
        }

        private async Task<TestModelListDto> GetAllItemsFilteredAsync(GetPaginatedListDto getPaginatedListDto)
        {
            var testModels = await PaginatedList<TestModel>
                     .FromIQueryable(GetAllItemsAsync(getPaginatedListDto.Filter),
                                    getPaginatedListDto.CurrentPage,
                                    getPaginatedListDto.PageSize == 0 ? _settings.Value.PageSize : getPaginatedListDto.PageSize);

            var testModelListDto = new TestModelListDto(testModels);
            return testModelListDto;
        }

        #endregion

        #region "Private classes"

        private class TestModelListDto : PagingDto
        {
            public ICollection<TestModel> Items { get; set; }

            public TestModelListDto(PaginatedList<TestModel> paginatedList) :
                    base(paginatedList.CurrentPage, paginatedList.PageSize, paginatedList.TotalItems, paginatedList.TotalPages)
            {
                this.Items = paginatedList;
            }

        }

        private class TestDbContext : DbContext
        {
            public DbSet<TestModel> TestModels { get; set; }

            public TestDbContext(DbContextOptions<TestDbContext> options)
                : base(options) { }
        }

        private class TestModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        #endregion
    }
}
