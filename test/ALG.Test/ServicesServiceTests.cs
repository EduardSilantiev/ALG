using ALG.Application;
using ALG.Application.Application.Settings;
using ALG.Application.Services;
using ALG.Application.Services.Dto;
using ALG.Core.Services;
using ALG.Core.Users;
using ALG.EntityFrameworkCore;
using ALG.EntityFrameworkCore.EntityFrameworkCore.Repositories;
using ALG.EntityFrameworkCore.EntityFrameworkCore.Seed;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ALG.Test
{
    public class ServicesServiceTests
    {
        private readonly IServicesService _servicesService;
        private readonly IServicesRepository _servicesRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly PagingSettings _settings;
        private readonly IMapper _mapper;

        #region "Init"

        public ServicesServiceTests()
        {
            //create InMemory DbContext
            var builder = new DbContextOptionsBuilder<AlgDbContext>();
            builder.UseInMemoryDatabase("ServicesServiceTests")
                   .EnableSensitiveDataLogging();
            var options = builder.Options;
            var context = new AlgDbContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            _usersRepository = new UsersRepository(context);

            //create UsersService
            _servicesRepository = new ServicesRepository(context);
            var config = new MapperConfiguration(c =>
            {
                c.AddProfile<ServicesMapProfile>();
            });
            _mapper = config.CreateMapper();

            _settings = new PagingSettings
            {
                PageSize = 4
            };
            _servicesService = new ServicesService(_servicesRepository, _mapper,
                            Microsoft.Extensions.Options.Options.Create(_settings));

            //seed with initial data
            new UsersSeeds(context).Seed();
            new ServicesSeeds(context).Seed();
        }

        #endregion

        #region "Service List"

        [Fact]
        public async void AllServicesAllDefaultsShouldBeGotSuccess()
        {
            //Arrange
            var getServicesListDto = new GetServicesListDto { };
            var initServices = InitDefaults.GetInitialServices();

            //Act
            var services = await _servicesService.GetAllServicesAsync(Guid.NewGuid(), getServicesListDto);

            //Assert
            Assert.Equal(services.TotalItems, initServices.Count);
            Assert.Equal(services.Items.Count, _settings.PageSize);
            Assert.Equal(services.PageSize, _settings.PageSize);
            Assert.Equal(1, services.CurrentPage);
            Assert.Equal((int)Math.Ceiling(services.TotalItems / (double)services.PageSize), services.TotalPages);
        }

        [Fact]
        public async void AllServicesShouldBeGotSuccess()
        {
            //Arrange
            var initServices = InitDefaults.GetInitialServices();
            
            var getServicesListDto = new GetServicesListDto
            {
                 PageSize = initServices.Count()
            };
            
            //Act
            var services = await _servicesService.GetAllServicesAsync(Guid.NewGuid(), getServicesListDto);

            //Assert
            Assert.Equal(services.TotalItems, initServices.Count);
            Assert.Equal(services.Items.Count, getServicesListDto.PageSize);
            Assert.Equal(services.PageSize, getServicesListDto.PageSize);
            Assert.Equal(1, services.CurrentPage);
            Assert.Equal((int)Math.Ceiling(services.TotalItems / (double)services.PageSize), services.TotalPages);
            AssertServices(initServices, services.Items);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async void CurrentPageServicesShouldBeGotSuccess(int currentPage)
        {
            //Arrange
            var getServicesListDto = new GetServicesListDto
            {
                 CurrentPage = currentPage,
                 PageSize = 3
            };
            var initServices = InitDefaults.GetInitialServices();

            //Act
            var services = await _servicesService.GetAllServicesAsync(Guid.NewGuid(), getServicesListDto);

            //Assert
            Assert.Equal(services.TotalItems, initServices.Count);
            Assert.Equal(services.PageSize, getServicesListDto.PageSize);
            Assert.Equal(currentPage, services.CurrentPage);
            Assert.Equal((int)Math.Ceiling(services.TotalItems / (double)getServicesListDto.PageSize), services.TotalPages);
        }

        [Fact]
        public async void ApplyFilterServicesShouldBeGotSuccess()
        {
            //Arrange
            var getServicesListDto = new GetServicesListDto
            {
                Filter = "ic"
            };

            var initServicesFiltered = InitDefaults.GetInitialServices()
                        .Where(s => s.Name.Contains(getServicesListDto.Filter))
                        .ToList();

            //Act
            var services = await _servicesService.GetAllServicesAsync(Guid.NewGuid(), getServicesListDto);

            //Assert
            Assert.Equal(initServicesFiltered.Count(), services.TotalItems);
            AssertServices(initServicesFiltered, services.Items);
        }

        [Fact]
        public async void ApplyInvalidFilterServicesShouldBeGotSuccess()
        {
            //Arrange
            var getServicesListDto = new GetServicesListDto
            {
                Filter = "some invalid filter"
            };

            //Act
            var services = await _servicesService.GetAllServicesAsync(Guid.NewGuid(), getServicesListDto);

            //Assert
            Assert.Equal(0, services.TotalItems);
            Assert.Empty(services.Items);
        }

        #endregion

        #region "Bonus can be activated"

        [Fact]
        public async void BonusCanBeActivatedSuccess()
        {
            //Arrange
            var testUser = await _usersRepository.GetUserByEmailAsync("john.dow@gmail.com");

            var getServicesListDto = new GetServicesListDto { PageSize = 100 };
            var allServices = await _servicesService.GetAllServicesAsync(testUser.Id, getServicesListDto);

            var activateBonusDto = new ActivateBonusDto
            {
                ServiceId = allServices.Items.First().Id,
                Promocode = "itpromocode"
            };

            //Act
            var canBeProcessedDto = await _servicesService.BonusCanBeActivatedAsync(testUser.Id, activateBonusDto);

            //Assert
            Assert.True(canBeProcessedDto.CanBeProcessed);
        }

        [Fact]
        public async void BonusCanBeActivatedServiceNotFoundFailure()
        {
            //Arrange
            var activateBonusDto = new ActivateBonusDto
            {
                ServiceId = Guid.NewGuid(),
                Promocode = "itpromocode"
            };

            //Act
            var canBeProcessedDto = await _servicesService.BonusCanBeActivatedAsync(Guid.NewGuid(), activateBonusDto);

            //Assert
            Assert.False(canBeProcessedDto.CanBeProcessed);
            Assert.Equal((int)AppConsts.HttpStatusCodes.Status400BadRequest, canBeProcessedDto.StatusCode);
            Assert.Equal($"Service is not found by id={activateBonusDto.ServiceId}.", canBeProcessedDto.RejectionReason);
        }

        [Fact]
        public async void BonusCanBeActivatedInvalidPromocodeFailure()
        {
            //Arrange
            var testUser = await _usersRepository.GetUserByEmailAsync("john.dow@gmail.com");

            var getServicesListDto = new GetServicesListDto { PageSize = 100 };
            var allServices = await _servicesService.GetAllServicesAsync(testUser.Id, getServicesListDto);

            var activateBonusDto = new ActivateBonusDto
            {
                ServiceId = allServices.Items.First().Id,
                Promocode = "invalid promocode"
            };

            //Act
            var canBeProcessedDto = await _servicesService.BonusCanBeActivatedAsync(testUser.Id, activateBonusDto);

            //Assert
            Assert.False(canBeProcessedDto.CanBeProcessed);
            Assert.Equal((int)AppConsts.HttpStatusCodes.Status400BadRequest, canBeProcessedDto.StatusCode);
            Assert.Equal($"Promocode '{activateBonusDto.Promocode}' is not valid for the service.", canBeProcessedDto.RejectionReason);
        }

        [Fact]
        public async void BonusCanBeActivatedAlreadyActivatedFailure()
        {
            //Arrange
            var testUser = await _usersRepository.GetUserByEmailAsync("john.dow@gmail.com");

            var getServicesListDto = new GetServicesListDto { PageSize = 100 };
            var allServices = await _servicesService.GetAllServicesAsync(testUser.Id, getServicesListDto);

            var activateBonusDto = new ActivateBonusDto
            {
                ServiceId = allServices.Items.First().Id,
                Promocode = "itpromocode"
            };

            await _servicesService.ActivateBonusAsync(testUser.Id, activateBonusDto);

            var canBeProcessedDto = await _servicesService.BonusCanBeActivatedAsync(testUser.Id, activateBonusDto);

            //Assert
            Assert.False(canBeProcessedDto.CanBeProcessed);
            Assert.Equal((int)AppConsts.HttpStatusCodes.Status409Conflict, canBeProcessedDto.StatusCode);
            Assert.Equal($"Bonus is already activated for the service.", canBeProcessedDto.RejectionReason);
        }

        #endregion

        #region "Activate bonus"

        [Fact]
        public async void BonusShouldBeActivatedSuccess()
        {
            //Arrange
            var testUser = await _usersRepository.GetUserByEmailAsync("john.dow@gmail.com");

            var getServicesListDto = new GetServicesListDto {  PageSize = 100};
            var allServices = await _servicesService.GetAllServicesAsync(testUser.Id, getServicesListDto);

            var activateBonusDto = new ActivateBonusDto
            {
                ServiceId = allServices.Items.First().Id,
                Promocode = "itpromocode"
            };

            //Act
            await _servicesService.ActivateBonusAsync(testUser.Id, activateBonusDto);

            allServices = await _servicesService.GetAllServicesAsync(testUser.Id, getServicesListDto);
            var service = allServices.Items.Single(s => s.Activated);

            //Assert
            Assert.Equal(service.Id, activateBonusDto.ServiceId);
        }

        #endregion

        #region "ActivateBonusDtoValidator"

        [Fact]
        public void ActivateBonusDtoValidatorValid()
        {
            //Arrange
            var activateBonusDto = new ActivateBonusDto
            {
                ServiceId = Guid.NewGuid(),
                Promocode = "itpromocode"
            };

            //Act
            var validationResult = new ActivateBonusDtoValidator().Validate(activateBonusDto);

            //Assert
            Assert.Empty(validationResult.Errors);
        }

        [Fact]
        public void CredentialsDtoValidatorEmptyServiceIdPromocodeNotValid()
        {
            //Arrange
            var activateBonusDto = new ActivateBonusDto { };

            //Act
            var validationResult = new ActivateBonusDtoValidator().Validate(activateBonusDto);

            //Assert
            Assert.Equal(2, validationResult.Errors.Count);
            Assert.True(validationResult.Errors.Count(x => x.PropertyName == "ServiceId") == 1);
            Assert.True(validationResult.Errors.Count(x => x.PropertyName == "Promocode") == 1);
        }

        #endregion

        #region "Private methods"

        private void AssertServices(ICollection<Service> initServices, ICollection<ServiceListItemDto> serviceListItemDtos)
        {
            Assert.Equal(initServices.Count(), serviceListItemDtos.Count());

            var initServicesDto = _mapper.Map<ICollection<ServiceListItemDto>>(initServices);

            var result = serviceListItemDtos.Except(initServicesDto, new ServiceListItemDtoComparer()).ToList();
            Assert.Empty(result);
            
            result = initServicesDto.Except(serviceListItemDtos, new ServiceListItemDtoComparer()).ToList();
            Assert.Empty(result);
        }

        public class ServiceListItemDtoComparer : IEqualityComparer<ServiceListItemDto>
        {
            public bool Equals(ServiceListItemDto x, ServiceListItemDto y) =>
                x.Name.Equals(y.Name) && x.Description.Equals(y.Description) ;

            public int GetHashCode(ServiceListItemDto obj) =>
                obj == null ? 0 : obj.Id.GetHashCode() ^ obj.Id.GetHashCode();
        }

        #endregion

    }
}
