using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Domain.Entities;
using AutoMapper;
using System;
using System.Linq;

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
                .ForMember(dest => dest.PhotoUrls, opt => opt.MapFrom(src => src.Photos
                    .Select(FormatImageData)
                    .Where(data => !string.IsNullOrEmpty(data))
                    .ToList()));

            CreateMap<CreateBusinessDto, Business>();

            // Service mappings
            CreateMap<Service, ServiceDto>()
                .ForMember(dest => dest.BusinessName, opt => opt.MapFrom(src => src.Business.Name))
                .ForMember(dest => dest.PhotoUrls, opt => opt.MapFrom(src => src.Photos
                    .Select(FormatImageData)
                    .Where(data => !string.IsNullOrEmpty(data))
                    .ToList()));

            CreateMap<CreateServiceDto, Service>();

            // Employee mappings
            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.BusinessName, opt => opt.MapFrom(src => src.Business.Name))
                .ForMember(dest => dest.PhotoUrls, opt => opt.MapFrom(src => src.Photos
                    .Select(FormatImageData)
                    .Where(data => !string.IsNullOrEmpty(data))
                    .ToList()))
                .ForMember(dest => dest.DocumentUrls, opt => opt.MapFrom(src => src.Documents.Select(d => d.Base64Data ?? "").ToList()));

            CreateMap<CreateEmployeeDto, Employee>();

            // Appointment mappings
            CreateMap<AppointmentServiceItem, AppointmentServiceDto>();

            CreateMap<Appointment, AppointmentDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : ""))
                .ForMember(dest => dest.BusinessName, opt => opt.MapFrom(src => src.Business != null ? src.Business.Name : ""))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.Name : null))
                .ForMember(dest => dest.Services, opt => opt.MapFrom(src => src.AppointmentServices))
                .ForMember(dest => dest.ServiceIds, opt => opt.MapFrom(src => src.AppointmentServices.Select(s => s.ServiceId).ToList()))
                .ForMember(dest => dest.ServiceId, opt => opt.MapFrom(src => src.AppointmentServices.Select(s => s.ServiceId).FirstOrDefault()))
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.AppointmentServices.Any()
                    ? string.Join(", ", src.AppointmentServices.Select(s => s.ServiceName))
                    : src.Service != null ? src.Service.Name : ""))
                .ForMember(dest => dest.ServicesSummary, opt => opt.MapFrom(src => src.AppointmentServices.Any()
                    ? string.Join(", ", src.AppointmentServices.Select(s => s.ServiceName))
                    : string.Empty))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.AppointmentServices.Sum(s => s.Price)))
                .ForMember(dest => dest.TotalDurationMinutes, opt => opt.MapFrom(src => src.AppointmentServices.Sum(s => s.DurationMinutes)))
                .ForMember(dest => dest.PhotoUrls, opt => opt.MapFrom(src => src.Photos != null
                    ? src.Photos.Select(FormatImageData)
                        .Where(data => !string.IsNullOrEmpty(data))
                        .ToList()
                    : new List<string>()));

            CreateMap<CreateAppointmentDto, Appointment>()
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Business, opt => opt.Ignore())
                .ForMember(dest => dest.Employee, opt => opt.Ignore())
                .ForMember(dest => dest.Service, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Photos, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.EndTime, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByBusinessUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Rating, opt => opt.Ignore())
                .ForMember(dest => dest.Review, opt => opt.Ignore())
                .ForMember(dest => dest.Photos, opt => opt.Ignore())
                .ForMember(dest => dest.RatingDate, opt => opt.Ignore());

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
            CreateMap<BusinessPhoto, PhotoDto>()
                .ForMember(dest => dest.Base64Data, opt => opt.MapFrom(src => FormatImageData(src)));

            CreateMap<EmployeePhoto, PhotoDto>()
                .ForMember(dest => dest.Base64Data, opt => opt.MapFrom(src => FormatImageData(src)));

            CreateMap<ServicePhoto, PhotoDto>()
                .ForMember(dest => dest.Base64Data, opt => opt.MapFrom(src => FormatImageData(src)));

            CreateMap<AppointmentPhoto, PhotoDto>()
                .ForMember(dest => dest.Base64Data, opt => opt.MapFrom(src => FormatImageData(src)));

            // Document mappings
            CreateMap<EmployeeDocument, DocumentDto>();
        }

        private static string FormatImageData(Photo? photo)
        {
            if (photo == null)
            {
                return string.Empty;
            }

            var base64 = photo.Base64Data?.Trim();

            if (string.IsNullOrWhiteSpace(base64))
            {
                return string.Empty;
            }

            if (base64.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
            {
                return base64;
            }

            var contentType = string.IsNullOrWhiteSpace(photo.ContentType)
                ? "image/jpeg"
                : photo.ContentType;

            return $"data:{contentType};base64,{base64}";
        }
    }
}
