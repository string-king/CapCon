using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using App.Contracts.BLL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain.Identity;
using App.DTO.v1_0;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WebApp.Helpers;

namespace WebApp.ApiControllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserCompaniesController : ControllerBase
    {
        private readonly IAppBLL _bll;
        private readonly UserManager<AppUser> _userManager;
        private readonly PublicDTOBllMapper<App.DTO.v1_0.UserCompany, App.BLL.DTO.UserCompany> _mapper;
        private readonly PublicDTOBllMapper<App.DTO.v1_0.UserCompanyForCompany, App.BLL.DTO.UserCompany> _mapperSimple;


        public UserCompaniesController(IAppBLL bll, UserManager<AppUser> userManager, IMapper mapper)
        {
            _bll = bll;
            _userManager = userManager;
            _mapper = new PublicDTOBllMapper<App.DTO.v1_0.UserCompany, App.BLL.DTO.UserCompany>(mapper);
            _mapperSimple = new PublicDTOBllMapper<App.DTO.v1_0.UserCompanyForCompany, App.BLL.DTO.UserCompany>(mapper);
        }

        /// <summary>
        /// Gets all user's companies.
        /// </summary>
        /// <returns>A list of user companies.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<App.DTO.v1_0.UserCompany>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<IEnumerable<App.DTO.v1_0.UserCompany>>> GetUserCompanies()
        {
            var res = (await _bll.UserCompanies.GetAllUserCompaniesIncludingAsync(
                Guid.Parse(_userManager.GetUserId(User)))
                ).Select((uc) => _mapper.Map(uc));
            return Ok(res);
        }
        
        /// <summary>
        /// Gets all usercompanies.
        /// </summary>
        /// <returns>A list of user companies.</returns>
        [HttpGet("All")]
        [ProducesResponseType(typeof(IEnumerable<App.DTO.v1_0.UserCompany>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<IEnumerable<App.DTO.v1_0.UserCompany>>> GetAllUserCompanies()
        {
            var res = (await _bll.UserCompanies.
                    GetAllUserCompaniesIncludingAsync()).Select((uc) => _mapper.Map(uc));
            return Ok(res);
        }

        /// <summary>
        /// Gets a user company by its ID.
        /// </summary>
        /// <returns>The user company if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(App.DTO.v1_0.UserCompany), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<App.DTO.v1_0.UserCompany>> GetUserCompany(Guid id)
        {
            var userCompany = await _bll.UserCompanies.FirstOrDefaultUserCompanyIncludingAsync(id);

            if (userCompany == null)
            {
                return NotFound(new RestApiErrorResponse()
                {
                    Status = HttpStatusCode.NotFound,
                    Error = "UserCompany not found!"
                });
            }
            
            var userCompanyDto = _mapper.Map(userCompany);

            return Ok(userCompanyDto);
        }

        /// <summary>
        /// Updates a user company.
        /// </summary>
        /// <returns>NoContent if the update is successful; otherwise, BadRequest or NotFound.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> PutUserCompany(Guid id, App.DTO.v1_0.UserCompanyForCompany userCompanyDto)
        {
            /*if (id != userCompanyDto.Id)
            {
                return BadRequest();
            }*/

            try
            {
                var uc = _bll.UserCompanies.FirstOrDefault(userCompanyDto.Id);
                var userCompany = _mapperSimple.Map(userCompanyDto);
                uc!.Role = userCompany!.Role;
                _bll.UserCompanies.Update(uc);
                await _bll.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserCompanyExists(id))
                {
                    return NotFound(new RestApiErrorResponse()
                    {
                        Status = HttpStatusCode.NotFound,
                        Error = "UserCompany not found!"
                    });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Creates a new user company.
        /// </summary>
        /// <returns>The created user company.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(App.DTO.v1_0.UserCompany), (int) HttpStatusCode.Created)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<App.DTO.v1_0.UserCompany>> PostUserCompany(App.DTO.v1_0.UserCompany userCompanyDto)
        {
            var userCompany = _mapper.Map(userCompanyDto);
            _bll.UserCompanies.Add(userCompany);
            await _bll.SaveChangesAsync();

            return CreatedAtAction("GetUserCompany", new { id = userCompany.Id }, userCompany);
        }
        
        /// <summary>
        /// Adds a new member to company.
        /// </summary>
        /// <returns>New member usercompany.</returns>
        [HttpPost("AddNewMember")]
        [ProducesResponseType<App.DTO.v1_0.UserCompany>((int) HttpStatusCode.Created)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int) HttpStatusCode.BadRequest)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<App.DTO.v1_0.UserCompany>> AddNewMemberByEmail(App.DTO.v1_0.NewMember newMemberDto)
        {
            var appUser = await _userManager.FindByEmailAsync(newMemberDto.Email);
            if (appUser == null)
            {
                return BadRequest(new RestApiErrorResponse()
                {
                    Status = HttpStatusCode.BadRequest,
                    Error = $"User with email {newMemberDto.Email} not found!"
                });
            }

            var userCompany = new App.DTO.v1_0.UserCompany()
            {
                AppUserId = appUser.Id,
                CompanyId = newMemberDto.CompanyId,
                Role = newMemberDto.Role
            };
            var uc = _mapper.Map(userCompany);
            _bll.UserCompanies.Add(uc);
            await _bll.SaveChangesAsync();

            return CreatedAtAction("GetUserCompany", new { id = userCompany.Id }, userCompany);
        }

        /// <summary>
        /// Deletes a user company by its ID.
        /// </summary>
        /// <returns>NoContent if the deletion is successful; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> DeleteUserCompany(Guid id)
        {
            var userCompany = await _bll.UserCompanies.FirstOrDefaultAsync(id);
            if (userCompany == null)
            {
                return NotFound(new RestApiErrorResponse()
                {
                    Status = HttpStatusCode.NotFound,
                    Error = "UserCompany not found!"
                });
            }

            _bll.UserCompanies.Remove(userCompany);
            await _bll.SaveChangesAsync();

            return NoContent();
        }

        private bool UserCompanyExists(Guid id)
        {
            return _bll.UserCompanies.Exists(id);
        }
    }
}
