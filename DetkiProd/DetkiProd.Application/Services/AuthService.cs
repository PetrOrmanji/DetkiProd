using AutoMapper;
using DetkiProd.Application.DTOs;
using DetkiProd.Application.Interfaces;
using DetkiProd.Domain.Entities;
using DetkiProd.Domain.Exceptions;
using DetkiProd.Domain.Repositories;
using DetkiProd.Domain.Security;

namespace DetkiProd.Application.Services;

public class AuthService : IAuthService
{
    private readonly IDetkiProdUserRepository _userRepository;
    private readonly IDetkiProdUserRoleRepository _userRoleRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly IMapper _mapper;

    public AuthService(
        IDetkiProdUserRepository userRepository,
        IDetkiProdUserRoleRepository userRoleRepository,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider,
        IMapper mapper)
    {
        _userRepository = userRepository ?? throw new ArgumentException("User Repository cannot be null", nameof(userRepository));
        _userRoleRepository = userRoleRepository ?? throw new ArgumentException("User Role Repository cannot be null", nameof(userRoleRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentException("Password Hasher cannot be null", nameof(passwordHasher));
        _jwtProvider = jwtProvider ?? throw new ArgumentException("Jwt Provider cannot be null", nameof(jwtProvider));
        _mapper = mapper ?? throw new ArgumentException("Mapper cannot be null", nameof(mapper));
    }

    public async Task<string> LoginAsync(string login, string password)
    {
        var user = await _userRepository.GetByLoginAsync(login);
        if (user == null)
        {
            throw new InvalidCredentialsException();
        }

        var isPasswordCorrect = _passwordHasher.Verify(password, user.PasswordHash!);
        if (!isPasswordCorrect)
        {
            throw new InvalidCredentialsException();
        }

        var jwtToken = _jwtProvider.Generate(user.Id, user.Login!, user.Role!.Name);
        return jwtToken;
    }

    public async Task<string> RegisterAsAdminAsync(string login, string password)
    {
        var user = await _userRepository.GetByLoginAsync(login);
        if (user != null)
        {
            throw new UserAlreadyExistsException(login);
        }

        var role = Domain.Enums.UserRole.Admin;
        var userRole = await _userRoleRepository.GetByRoleAsync(role);
        if (userRole == null)
        {
            throw new UserRoleNotFoundException(role);
        }

        var newUser = DetkiProdUser.CreateBaseUser(login, _passwordHasher.Hash(password), userRole);

        await _userRepository.AddAsync(newUser);

        var jwtToken = _jwtProvider.Generate(newUser.Id, newUser.Login!, role);
        return jwtToken;
    }

    public async Task<DetkiProdUserDto> RegisterTelegramUserAsAdminAsync(long telegramUserId)
    {
        var user = await _userRepository.GetByTelegramUserIdAsync(telegramUserId);
        if (user != null)
        {
            throw new UserAlreadyExistsException(telegramUserId);
        }

        var role = Domain.Enums.UserRole.Admin;
        var userRole = await _userRoleRepository.GetByRoleAsync(role);
        if (userRole == null)
        {
            throw new UserRoleNotFoundException(role);
        }

        var newUser = DetkiProdUser.CreateTelegramUser(telegramUserId, userRole);

        await _userRepository.AddAsync(newUser);

        return _mapper.Map<DetkiProdUserDto>(newUser);
    }
}
