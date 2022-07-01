using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PeliculasAPI.PeliculasAPI.Common;
using PeliculasAPI.PeliculasAPI.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PeliculasAPI.Controllers
{
    [ApiController]
    [Route("api/cuenta")]
    public class CuentaController : _BaseController
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public CuentaController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            IConfiguration configuration, ApplicationDBContext context,
            IMapper mapper) : base(context, mapper)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("users")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<List<UserGet>>> Get([FromQuery] PaginationDTO pagination)
        {
            var queryable = context.Users.AsQueryable();
            queryable = queryable.OrderBy(x => x.Email);
            return await Get<IdentityUser, UserGet>(pagination);
        }

        [HttpPatch("AsignarRol")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> AsignarRol(EditarRol editarRol)
        {
            var user = await userManager.FindByIdAsync(editarRol.Id);
            if (user == null)
                return NotFound();
            await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, editarRol.NombreRol));
            return NoContent();
        }

        [HttpPatch("QuitarRol")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> QuitarRol(EditarRol editarRol)
        {
            var user = await userManager.FindByIdAsync(editarRol.Id);
            if (user == null)
                return NotFound();
            await userManager.RemoveClaimAsync(user, new Claim(ClaimTypes.Role, editarRol.NombreRol));
            return NoContent();
        }

        [HttpPost("renovarToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UserToken>> renovarToken()
        {
            var userInfo = new UserInfo
            {
                Email = HttpContext.User.Identity.Name
            };

            return await BuildToken(userInfo);
        }

        [HttpPost("signin")]
        public async Task<ActionResult<UserToken>> signin([FromBody] UserInfo model)
        {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
                return await BuildToken(model);
            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserToken>> signIn([FromBody] UserInfo model)
        {
            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
                return await BuildToken(model);
            return BadRequest();
        }

        private async Task<UserToken> BuildToken(UserInfo model)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, model.Email),
                new Claim(ClaimTypes.Email, model.Email)
            };

            var identityUser = await userManager.FindByEmailAsync(model.Email);

            claims.Add(new Claim(ClaimTypes.NameIdentifier, identityUser.Id));

            var claimsDb = await userManager.GetClaimsAsync(identityUser);

            claims.AddRange(claimsDb);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwt:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var exp = DateTime.UtcNow.AddYears(1);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: exp,
                signingCredentials: creds
                );
            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Exp = exp
            };
        }
    }
}
