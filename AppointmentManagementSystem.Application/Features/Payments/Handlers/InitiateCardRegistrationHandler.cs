using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Payments.Commands;
using AppointmentManagementSystem.Application.Shared;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AppointmentManagementSystem.Application.Features.Payments.Handlers
{
    public class InitiateCardRegistrationHandler : IRequestHandler<InitiateCardRegistrationCommand, Result<CardRegistrationResponseDto>>
    {
        private readonly IPayTRService _paytrService;
        private readonly ILogger<InitiateCardRegistrationHandler> _logger;

        public InitiateCardRegistrationHandler(IPayTRService paytrService, ILogger<InitiateCardRegistrationHandler> logger)
        {
            _paytrService = paytrService;
            _logger = logger;
        }

        public async Task<Result<CardRegistrationResponseDto>> Handle(InitiateCardRegistrationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // PayTR merchant_oid sadece alfanumerik kabul ediyor (tire, nokta vb. yok)
                var merchantOid = $"REG{request.BusinessId}{Guid.NewGuid().ToString("N").Substring(0, 8)}";

                var result = await _paytrService.InitiateCardRegistrationAsync(
                    request.Email,
                    request.UserIp,
                    merchantOid
                );

                if (result.Success)
                {
                    var response = new CardRegistrationResponseDto
                    {
                        Success = true,
                        IFrameToken = result.IFrameToken,
                        IFrameUrl = result.IFrameUrl
                    };

                    return Result<CardRegistrationResponseDto>.SuccessResult(response);
                }
                else
                {
                    return Result<CardRegistrationResponseDto>.FailureResult(result.ErrorMessage ?? "Card registration failed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in InitiateCardRegistrationHandler");
                return Result<CardRegistrationResponseDto>.FailureResult("An error occurred during card registration");
            }
        }
    }
}