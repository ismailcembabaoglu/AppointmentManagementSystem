using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Domain.Entities;
using AutoMapper;

namespace AppointmentManagementSystem.Application.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Category mappings
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.BusinessCount, opt => opt.MapFrom(src => src.Businesses.Count));

            CreateMap<CreateCategoryDto, Category>(); // BU SATIR EKLENDİ

            // Business mappings
            CreateMap<Business, BusinessDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.PhotoUrls, opt => opt.MapFrom(src => src.Photos.Select(p => p.FilePath ?? "").ToList()));

            CreateMap<CreateBusinessDto, Business>();

            // Service mappings
            CreateMap<Service, ServiceDto>()
                .ForMember(dest => dest.BusinessName, opt => opt.MapFrom(src => src.Business.Name))
                .ForMember(dest => dest.PhotoUrls, opt => opt.MapFrom(src => src.Photos.Select(p => p.FilePath ?? "").ToList()));

            CreateMap<CreateServiceDto, Service>();

            // Employee mappings
            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.BusinessName, opt => opt.MapFrom(src => src.Business.Name))
                .ForMember(dest => dest.PhotoUrls, opt => opt.MapFrom(src => src.Photos.Select(p => p.FilePath ?? "").ToList()))
                .ForMember(dest => dest.DocumentUrls, opt => opt.MapFrom(src => src.Documents.Select(d => d.FilePath ?? "").ToList()));

            CreateMap<CreateEmployeeDto, Employee>();

            // Appointment mappings
            CreateMap<Appointment, AppointmentDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
                .ForMember(dest => dest.BusinessName, opt => opt.MapFrom(src => src.Business.Name))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.Name : null))
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service.Name))
                .ForMember(dest => dest.PhotoUrls, opt => opt.MapFrom(src => src.Photos.Select(p => p.FilePath ?? "").ToList()));

            CreateMap<CreateAppointmentDto, Appointment>();

            // User mappings
            CreateMap<User, UserDto>();

            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // Password hashing ayrı yapılır

            // BusinessUser mappings
            CreateMap<BusinessUser, BusinessUserDto>()
                .ForMember(dest => dest.BusinessName, opt => opt.MapFrom(src => src.Business.Name))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name));

            CreateMap<CreateBusinessUserDto, BusinessUser>(); // BU SATIR EKLENDİ

            // Photo mappings
            CreateMap<BusinessPhoto, PhotoDto>(); // BU SATIR EKLENDİ
            CreateMap<EmployeePhoto, PhotoDto>(); // BU SATIR EKLENDİ
            CreateMap<ServicePhoto, PhotoDto>(); // BU SATIR EKLENDİ
            CreateMap<AppointmentPhoto, PhotoDto>(); // BU SATIR EKLENDİ

            // Document mappings
            CreateMap<EmployeeDocument, DocumentDto>(); // BU SATIR EKLENDİ
                                                        // BusinessUser mappings
            CreateMap<BusinessUser, BusinessUserDto>()
                .ForMember(dest => dest.BusinessName, opt => opt.MapFrom(src => src.Business.Name))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name));

            CreateMap<CreateBusinessUserDto, BusinessUser>();

        }
    }
}
