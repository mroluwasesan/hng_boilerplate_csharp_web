﻿using CSharpFunctionalExtensions;
using Hng.Application.Features.PaymentIntegrations.Paystack.Dtos.Requests;
using Hng.Application.Features.PaymentIntegrations.Paystack.Services;
using Hng.Domain.Entities;
using Hng.Domain.Enums;
using Hng.Infrastructure.Repository.Interface;
using Hng.Infrastructure.Utilities.StringKeys;
using MediatR;

namespace Hng.Application.Features.PaymentIntegrations.Paystack.Handlers.Queries
{
    public class VerifyTransactionQueryHandler : IRequestHandler<VerifyTransactionQuery, Result<string>>
    {
        private readonly PaystackClient _paystackClient;
        private readonly PaystackApiKeys _apiKey;

        public VerifyTransactionQueryHandler(
            PaystackClient paystackClient,
            PaystackApiKeys apiKey)
        {
            _paystackClient = paystackClient;
            _apiKey = apiKey;
        }

        public async Task<Result<string>> Handle(VerifyTransactionQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Reference))
                return Result.Failure<string>("Reference cannot be null!");

            var verifyRequest = new VerifyTransactionRequest(request.Reference) { BusinessAuthorizationToken = _apiKey.SecretKey };

            var verifyResponse = await _paystackClient.VerifyTransaction(verifyRequest);

            if (verifyResponse.IsSuccess && verifyResponse.Value.status
                    && verifyResponse.Value.data.status == PaystackResponseStatus.success.ToString())
            {
                return Result.Success("Success");
            }

            return Result.Failure<string>("Verification Failed");
        }
    }
}
