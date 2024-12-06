using Grpc.Core;
using UserService.Data.Models.User;

namespace UserService.GrpcServices;

public class UserService : global::UserService.UserService.UserServiceBase
{
    public override Task<RegisterDoctorResponse> RegisterDoctor(RegisterDoctorRequest request, ServerCallContext context)
    {
        var birthDate = DateOnly.FromDateTime(request.DateOfBirth.ToDateTime())
        var doctorResult = Doctor.Create(request.Email, request.FirstName, request.Surname,
            birthDate);
        if (doctorResult.IsFailure)
    }
}