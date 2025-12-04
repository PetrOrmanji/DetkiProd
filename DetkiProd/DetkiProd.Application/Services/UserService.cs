using AutoMapper;
using DetkiProd.Application.DTOs;
using DetkiProd.Application.Interfaces;
using DetkiProd.Domain.Exceptions;
using DetkiProd.Domain.Repositories;
using DetkiProd.Domain.Security;

namespace DetkiProd.Application.Services;

public class UserService : IUserService
{
    private readonly IDetkiProdUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(
        IDetkiProdUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IMapper mapper)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<List<DetkiProdUserDto>> GetUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return _mapper.Map<List<DetkiProdUserDto>>(users);
    }

    public async Task<DetkiProdUserDto> GetUserByTelegramIdAsync(long telegramUserId)
    {
        var user = await _userRepository.GetByTelegramUserIdAsync(telegramUserId);

        return _mapper.Map<DetkiProdUserDto>(user);
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new UserNotFoundException(userId);
        }

        await _userRepository.DeleteByIdAsync(userId);
    }
}