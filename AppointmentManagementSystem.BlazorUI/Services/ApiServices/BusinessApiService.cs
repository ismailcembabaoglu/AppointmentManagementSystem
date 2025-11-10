using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.BlazorUI.Models;
using Blazored.LocalStorage;
using System.Net.Http.Json;

namespace AppointmentManagementSystem.BlazorUI.Services.ApiServices
{
    public class BusinessApiService : BaseApiService, IBusinessApiService
    {
        public BusinessApiService(HttpClient httpClient, ILocalStorageService localStorage)
            : base(httpClient, localStorage)
        {
        }

        public async Task<ApiResponse<List<BusinessDto>>> GetAllBusinessesAsync(
            int? categoryId = null, 
            string? searchTerm = null,
            string? city = null,
            string? district = null,
            double? minRating = null)
        {
            try
            {
                var queryParams = new List<string>();
                
                if (categoryId.HasValue)
                    queryParams.Add($"categoryId={categoryId.Value}");
                if (!string.IsNullOrEmpty(searchTerm))
                    queryParams.Add($"search={Uri.EscapeDataString(searchTerm)}");
                if (!string.IsNullOrEmpty(city))
                    queryParams.Add($"city={Uri.EscapeDataString(city)}");
                if (!string.IsNullOrEmpty(district))
                    queryParams.Add($"district={Uri.EscapeDataString(district)}");
                if (minRating.HasValue)
                    queryParams.Add($"minRating={minRating.Value}");

                var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
                var request = await CreateRequestWithAuth(HttpMethod.Get, $"api/businesses{queryString}");
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<List<BusinessDto>>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<BusinessDto>> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<BusinessDto?>> GetBusinessByIdAsync(int id)
        {
            try
            {
                var request = await CreateRequestWithAuth(HttpMethod.Get, $"api/businesses/{id}");
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<BusinessDto?>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<BusinessDto?> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<List<ServiceDto>>> GetServicesByBusinessAsync(int businessId)
        {
            try
            {
                var request = await CreateRequestWithAuth(HttpMethod.Get, $"api/businesses/{businessId}/services");
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<List<ServiceDto>>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ServiceDto>> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<List<EmployeeDto>>> GetEmployeesByBusinessAsync(int businessId)
        {
            try
            {
                var request = await CreateRequestWithAuth(HttpMethod.Get, $"api/businesses/{businessId}/employees");
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<List<EmployeeDto>>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<EmployeeDto>> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<List<AppointmentDto>>> GetAppointmentsByBusinessAsync(int businessId)
        {
            try
            {
                var request = await CreateRequestWithAuth(HttpMethod.Get, $"api/businesses/{businessId}/appointments");
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<List<AppointmentDto>>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<AppointmentDto>> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<List<BusinessReviewDto>>> GetBusinessReviewsAsync(int businessId)
        {
            try
            {
                var request = await CreateRequestWithAuth(HttpMethod.Get, $"api/businesses/{businessId}/reviews");
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<List<BusinessReviewDto>>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<BusinessReviewDto>> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<BusinessDto>> CreateBusinessAsync(CreateBusinessDto createBusinessDto)
        {
            try
            {
                var request = await CreateRequestWithAuth(HttpMethod.Post, "api/businesses");
                request.Content = JsonContent.Create(createBusinessDto);
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<BusinessDto>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<BusinessDto> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<BusinessDto?>> UpdateBusinessAsync(int id, CreateBusinessDto updateBusinessDto)
        {
            try
            {
                var request = await CreateRequestWithAuth(HttpMethod.Put, $"api/businesses/{id}");
                request.Content = JsonContent.Create(updateBusinessDto);
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<BusinessDto?>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<BusinessDto?> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<bool>> DeleteBusinessAsync(int id)
        {
            try
            {
                var request = await CreateRequestWithAuth(HttpMethod.Delete, $"api/businesses/{id}");
                var response = await _httpClient.SendAsync(request);
                return await HandleApiResponse<bool>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = $"Hata: {ex.Message}" };
            }
        }
    }
}
