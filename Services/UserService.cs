using System;
using System.Security.Cryptography;
using System.Text;
using LiberiaDriveMVC.Data;
using LiberiaDriveMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LiberiaDriveMVC.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public UserService(ApplicationDbContext context, IPasswordHasher<Usuario> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<Usuario?> GetActiveUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _context.Usuario
            .Include(u => u.IdRolNavigation)
            .FirstOrDefaultAsync(u => u.NombreUsuario == username && (u.Estado ?? true), cancellationToken);
    }

    public Task<bool> IsUsernameOrEmailTakenAsync(string username, string email, CancellationToken cancellationToken = default)
    {
        return _context.Usuario.AnyAsync(
            u => u.NombreUsuario == username || u.Correo == email,
            cancellationToken);
    }

    public async Task<Usuario> CreateClientUserAsync(string username, string email, string password, CancellationToken cancellationToken = default)
    {
        var clientRole = await _context.Rol
            .FirstOrDefaultAsync(r => r.NombreRol == "Cliente", cancellationToken);

        if (clientRole is null)
        {
            throw new InvalidOperationException("No se encontró el rol 'Cliente' en la base de datos.");
        }

        var user = new Usuario
        {
            NombreUsuario = username,
            Correo = email,
            IdRol = clientRole.IdRol,
            FechaRegistro = DateTime.UtcNow,
            Estado = true
        };

        user.ContrasenaHash = _passwordHasher.HashPassword(user, password);

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await _context.Usuario.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        return user;
    }

    public PasswordVerificationResult VerifyPassword(Usuario user, string password)
    {
        if (string.IsNullOrEmpty(user.ContrasenaHash))
        {
            return PasswordVerificationResult.Failed;
        }

        if (user.ContrasenaHash.StartsWith("AQAAAA", StringComparison.Ordinal))
        {
            return _passwordHasher.VerifyHashedPassword(user, user.ContrasenaHash, password);
        }

        var legacyHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
        return string.Equals(user.ContrasenaHash, legacyHash, StringComparison.Ordinal)
            ? PasswordVerificationResult.Success
            : PasswordVerificationResult.Failed;
    }
}
