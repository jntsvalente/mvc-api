using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Api.Extensions;
using Web.Api.Interfaces;
using Web.Api.Models;
using Web.Api.ViewModels;

namespace Web.Api.Controllers;
public class AccountController : ControllerBase
{
    [HttpPost("v1/accounts/register")]
    public async Task<IActionResult> Register(
        [FromBody] AccountViewModel model,        
        [FromServices] IApiDataContext context,
        [FromServices] IPasswordHasherService passwordHasher,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));
        
        try
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = model.Email,
                PasswordHash = passwordHasher.Hash(model.Password)
            };

            await context.Users.AddAsync(user, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = Role.MemberId
            };

            await context.UserRoles.AddAsync(userRole, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return Ok(new ResultViewModel<dynamic>(new
            {
                user = user.Email
            }));
        }
        catch (DbUpdateException)
        {
            return StatusCode(400, new ResultViewModel<string>("8895D - This e-mail is already used"));
        }
    }

    [HttpPost("v1/accounts/login")]
    public async Task<IActionResult> Login(
        [FromBody] AccountViewModel model,
        [FromServices] IApiDataContext context,
        [FromServices] IPasswordHasherService passwordHasher,
        [FromServices] ITokenService tokenService,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

        try
        {
            var user = await context
                .Users
                .Include(x => x.Roles)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Email == model.Email, cancellationToken);

            if (user == null)
                return StatusCode(400, new ResultViewModel<string>("User or password is invalid"));

            if (!passwordHasher.Verify(model.Password, user.PasswordHash))
                return StatusCode(400, new ResultViewModel<string>("User or password is invalid"));

            var token = await tokenService.CreateToken(user, cancellationToken);
            return Ok(new ResultViewModel<string>(token, null));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("25D85 - Internal server failure"));
        }
    }

    [HttpGet("v1/accounts/users")]
    public async Task<IActionResult> GetUsersAsync(
        [FromServices] IApiDataContext context,
        CancellationToken cancellationToken)
    {
        try
        {
            var users = await context.UserRoles
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return Ok(new ResultViewModel<List<UserRole>>(users));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<List<UserRole>>("ER23D - Internal server failure"));
        }
    }

    [HttpPost("v1/accounts/add-role/{userId}")]
    public async Task<IActionResult> AddRole(
    [FromRoute] Guid userId,
    [FromBody] RoleIdViewModel model,
    [FromServices] IApiDataContext context,
    CancellationToken cancellationToken)
    {
        try
        {
            var user = await context.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Id == userId, cancellationToken);
            if (user == null)
                return NotFound(new ResultViewModel<string>("User not found"));

            var role = await context.Roles.AsNoTracking().SingleOrDefaultAsync(x => x.Id == model.RoleId, cancellationToken);
            if (role == null)
                return NotFound(new ResultViewModel<string>("Role not found"));

            var userRoleExists = await context
                .UserRoles
                .AsNoTracking()
                .AnyAsync(x => x.UserId == userId && x.RoleId == model.RoleId, cancellationToken);
            if (userRoleExists)
                return StatusCode(400, new ResultViewModel<string>("User already has this role"));

            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = model.RoleId
            };

            await context.UserRoles.AddAsync(userRole, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return Ok(new ResultViewModel<string>("Role added successfully"));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("WE652 - Internal server failure"));
        }
    }

    [HttpDelete("v1/accounts/delete-user/{userId}")]
    public async Task<IActionResult> DeleteUser(
        [FromRoute] Guid userId,
        [FromServices] IApiDataContext context,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await context.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Id == userId, cancellationToken);

            if (user == null)
                return NotFound(new ResultViewModel<User>("User not found"));

            context.Users.Remove(user);
            await context.SaveChangesAsync(cancellationToken);

            return Ok(new ResultViewModel<User>(user));
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, new ResultViewModel<User>("636FR - It was not possible to delete the user"));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<User>("PP852 - Internal server failure"));
        }
    }
}