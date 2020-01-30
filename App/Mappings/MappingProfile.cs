using App.Models;
using App.ViewModels.Admin.Book;
using App.ViewModels.Admin.BookCategory;
using App.ViewModels.Admin.Flavor;
using App.ViewModels.Admin.Staff;
using App.ViewModels.Client.Customer;
using AutoMapper;

namespace App.Mappings
{
    public class MappingProfile
    {
        public static IMapper mapper;

        public static void RegisterMapping()
        {
            var mapperConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<UpdateProfileViewModel, Staff>();
                config.CreateMap<Staff, UpdateProfileViewModel>();
                config.CreateMap<BookCategory, BookCategoryViewModel>();
                config.CreateMap<BookCategoryViewModel, BookCategory>();
                config.CreateMap<BookViewModel, Book>();
                config.CreateMap<Book, BookViewModel>();
                config.CreateMap<BookViewModel, Book>();
                config.CreateMap<Staff, UpdateStaffViewModel>();
                config.CreateMap<Customer, CustomerUpdateProfileViewModel>();
                config.CreateMap<Customer, CustomerChangePasswordViewModel>();
                config.CreateMap<Customer, CheckoutViewModel>();
                config.CreateMap<FlavorViewModel, Flavor>();
                config.CreateMap<Flavor, FlavorViewModel>();
            });

            mapper = mapperConfig.CreateMapper();
        }
    }
}