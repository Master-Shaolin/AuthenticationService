using ClubMembership_Authentication.gRPC;
using ClubMembership_Authentication.Infrastructure.Repositories;
using Grpc.Core;

namespace ClubMembership_Authentication.Application.Services
{
    public class UserServiceGrpc(IUserRepository userRepository) : UserService.UserServiceBase
    {
        private readonly IUserRepository _userRepository = userRepository;

        public override async Task<ValidateUserResponse> ValidateUser(ValidateUserRequest request, ServerCallContext context)
        {
            var userExists = await _userRepository.ExistsAsync(Guid.Parse(request.UserId));
            return new ValidateUserResponse { Exists = userExists };
        }
    }
}