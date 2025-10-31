using LiberiaDriveMVC.Models;
using Microsoft.AspNetCore.Identity;

namespace LiberiaDriveMVC.Services;

public interface IUserService
{
    Task<Usuario?> GetActiveUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> IsUsernameOrEmailTakenAsync(string username, string email, CancellationToken cancellationToken = default);
    Task<Usuario> CreateClientUserAsync(string username, string email, string password, CancellationToken cancellationToken = default);
    PasswordVerificationResult VerifyPassword(Usuario user, string password);
}
