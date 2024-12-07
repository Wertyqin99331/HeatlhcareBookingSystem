using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Data.Models.User;
using UserService.Services.Token;

namespace UserService.Grpc;

public class UserGrpcService(
    UserManager<Doctor> doctorManager,
    UserManager<Patient> patientManager,
    UserManager<User> userManager,
    IJwtTokenService jwtTokenService,
    UserDbContext dbContext,
    RoleManager<IdentityRole<Guid>> roleManager) : UserService.UserServiceBase
{
    public override async Task<RegisterDoctorResponse> RegisterDoctor(RegisterDoctorRequest request,
        ServerCallContext context)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            var birthDate = DateOnly.FromDateTime(request.DateOfBirth.ToDateTime());
            var doctorResult = Doctor.Create(request.Email, request.FirstName, request.Surname,
                birthDate);
            if (doctorResult.IsFailure)
            {
                return new RegisterDoctorResponse
                {
                    Error = new ErrorResponse
                    {
                        Status = 404,
                        Title = "Failed to create a doctor",
                        Detail = doctorResult.Error
                    }
                };
            }

            var registerResult = await doctorManager.CreateAsync(doctorResult.Value, request.Password);
            if (!registerResult.Succeeded)
            {
                return new RegisterDoctorResponse
                {
                    Error = new ErrorResponse
                    {
                        Status = 404,
                        Title = "Failed to register a doctor",
                        Detail = string.Join(", ", registerResult.Errors.Select(e => e.Description))
                    }
                };
            }

            var doctorRole = await roleManager.FindByNameAsync(UserRole.Doctor.ToString());
            if (doctorRole is null)
            {
                return new RegisterDoctorResponse
                {
                    Error = new ErrorResponse
                    {
                        Status = 404,
                        Title = "Failed to register a doctor",
                        Detail = "Failed to find doctor role"
                    }
                };
            }

            var addToRoleResult = await doctorManager.AddToRoleAsync(doctorResult.Value, UserRole.Doctor.ToString());
            if (!addToRoleResult.Succeeded)
            {
                return new RegisterDoctorResponse
                {
                    Error = new ErrorResponse()
                    {
                        Status = 500,
                        Title = "Failed to register a doctor",
                        Detail = string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))
                    }
                };

            }

            await transaction.CommitAsync();

            return new RegisterDoctorResponse()
            {
                Success = new Empty()
            };
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();

            return new RegisterDoctorResponse
            {
                Error = new ErrorResponse()
                {
                    Status = 500,
                    Title = "Some db error",
                    Detail = "Failed to commit transaction"
                }
            };
        }
    }

    public override async Task<RegisterPatientResponse> RegisterPatient(RegisterPatientRequest request,
        ServerCallContext context)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            var birthDate = DateOnly.FromDateTime(request.DateOfBirth.ToDateTime());
            var patientResult = Patient.Create(request.Email, request.FirstName, request.Surname,
                birthDate);
            if (patientResult.IsFailure)
            {
                return new RegisterPatientResponse
                {
                    Error = new ErrorResponse
                    {
                        Status = 404,
                        Title = "Failed to create a patient account",
                        Detail = patientResult.Error
                    }
                };
            }

            var registerResult = await patientManager.CreateAsync(patientResult.Value, request.Password);
            if (!registerResult.Succeeded)
            {
                return new RegisterPatientResponse
                {
                    Error = new ErrorResponse
                    {
                        Status = 404,
                        Title = "Failed to register a patient",
                        Detail = string.Join(", ", registerResult.Errors.Select(e => e.Description))
                    }
                };
            }
            
            var patientRole = await roleManager.FindByNameAsync(UserRole.Patient.ToString());
            if (patientRole is null)
            {
                return new RegisterPatientResponse()
                {
                    Error = new ErrorResponse
                    {
                        Status = 404,
                        Title = "Failed to register a patient",
                        Detail = "Failed to find patient role"
                    }
                };
            }

            var addToRoleResult = await patientManager.AddToRoleAsync(patientResult.Value, UserRole.Patient.ToString());
            if (!addToRoleResult.Succeeded)
            {
                return new RegisterPatientResponse()
                {
                    Error = new ErrorResponse()
                    {
                        Status = 500,
                        Title = "Failed to add patient role to a patient",
                        Detail = string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))
                    }
                };

            }

            await transaction.CommitAsync();

            return new RegisterPatientResponse()
            {
                Success = new Empty()
            };
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            
            return new RegisterPatientResponse()
            {
                Error = new ErrorResponse()
                {
                    Status = 500,
                    Title = "Some db error",
                    Detail = "Failed to commit transaction"
                }
            };
        }
    }

    public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return new LoginResponse()
            {
                Error = new ErrorResponse()
                {
                    Status = 404,
                    Title = "Bad request",
                    Detail = "User with this email is not found"
                }
            };
        }

        var loginResult = await userManager.CheckPasswordAsync(user, request.Password);
        if (!loginResult)
        {
            return new LoginResponse()
            {
                Error = new ErrorResponse()
                {
                    Status = 404,
                    Title = "Bad request",
                    Detail = "Invalid password"
                }
            };
        }

        var roles = await userManager.GetRolesAsync(user);
        if (roles.Count == 0)
        {
            return new LoginResponse()
            {
                Error = new ErrorResponse
                {
                    Status = 404,
                    Title = "Bad request",
                    Detail = "User has no roles"
                }
            };
        }

        var token = jwtTokenService.GenerateToken(user.Id, roles);

        var loginResponse = new LoginResponse()
        {
            Success = new SuccessLoginResponse()
            {
                Token = token
            }
        };
        loginResponse.Success.Roles.AddRange(roles);

        return loginResponse;
    }
}