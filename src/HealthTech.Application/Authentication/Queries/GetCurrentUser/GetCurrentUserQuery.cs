using MediatR;
using HealthTech.Application.Authentication.DTOs;

namespace HealthTech.Application.Authentication.Queries.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<GetCurrentUserResponse>;
