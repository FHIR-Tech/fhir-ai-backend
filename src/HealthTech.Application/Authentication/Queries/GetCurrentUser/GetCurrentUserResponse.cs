using HealthTech.Application.Authentication.DTOs;
using HealthTech.Application.Common.Base;

namespace HealthTech.Application.Authentication.Queries.GetCurrentUser;

public record GetCurrentUserResponse : BaseResponse
{
    public UserInfoDto? User { get; init; }
}
