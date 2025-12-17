namespace Artix.API.Core.DomainService.Services.OTP;

using System.Text.Json;
using Contract.Features.OTPs.Commands;
using Contract.Features.Users.Client.Queries.GetVerifyOTPAuth;
using Contract.Primitives.Infra.Identity;
using Contract.Primitives.Infra.Redis;
using Domain.Entities.OTP;
using Domain.Entities.OTP.Enums;
using Domain.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

