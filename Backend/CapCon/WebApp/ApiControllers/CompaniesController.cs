using System.Net;
using App.Contracts.BLL;
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
using Company = App.BLL.DTO.Company;

namespace WebApp.ApiControllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CompaniesController : ControllerBase
    {
        private readonly IAppBLL _bll;
        private readonly UserManager<AppUser> _userManager;
        private readonly PublicDTOBllMapper<App.DTO.v1_0.Company, App.BLL.DTO.Company> _mapper;
        private readonly ILogger<CompaniesController>? _logger;


        public CompaniesController(IAppBLL bll, UserManager<AppUser> userManager, IMapper autoMapper, ILogger<CompaniesController>? logger)
        {
            _bll = bll;
            _userManager = userManager;
            _logger = logger;
            _mapper = new PublicDTOBllMapper<App.DTO.v1_0.Company, App.BLL.DTO.Company>(autoMapper);
        }

        /// <summary>
        /// Returns all companies visible to the user.
        /// </summary>
        /// <returns>List of contests.</returns>
        [HttpGet]
        [ProducesResponseType<IEnumerable<App.DTO.v1_0.Company>>((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<IEnumerable<App.DTO.v1_0.Company>>> GetCompanies()
        {
            /*var res = (await _bll.Companies.GetAllSortedAsync(
                    Guid.Parse(_userManager.GetUserId(User))))
                .Select((c) => _mapper.Map(c));*/
            
            var res = (await _bll.Companies.GetAllCompaniesIncludingAsync())
                .Select(x => _mapper.Map(x)!)
                .ToList();
            return Ok(res);
        }

        /// <summary>
        /// Gets a company by its ID.
        /// </summary>
        /// <param name="id">The ID of the company.</param>
        /// <returns>The company if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(App.DTO.v1_0.Company), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<App.DTO.v1_0.Company>> GetCompany(Guid id)
        {
            var company = await _bll.Companies.FirstOrDefaultCompanyIncludingAsync(id);
            if (company == null)
            {
                return NotFound(new RestApiErrorResponse()
                {
                    Status = HttpStatusCode.NotFound,
                    Error = "Company not found"
                });
            }
            
            var companyDto = _mapper.Map(company);

            foreach (var uc in companyDto!.UserCompanies!)
            {
                _logger.LogInformation(uc.AppUserId.ToString());
            }

            return Ok(companyDto);
        }

        /// <summary>
        /// Updates a company.
        /// </summary>
        /// <param name="id">The ID of the company to update.</param>
        /// <param name="companyDto">The updated company data.</param>
        /// <returns>OK if the update is successful; otherwise, BadRequest or NotFound.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Company), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> PutCompany(Guid id, App.DTO.v1_0.CompanySimple companyDto)
        {
            if (id != companyDto.Id)
            {
                return BadRequest(new RestApiErrorResponse()
                {
                    Status = HttpStatusCode.BadRequest,
                    Error = "Company ID mismatch"
                });
            }

            var found = await _bll.Companies.FirstOrDefaultAsync(companyDto.Id);
            if (found == default || found.Id == default)
            {
                return NotFound(new RestApiErrorResponse()
                {
                    Status = HttpStatusCode.NotFound,
                    Error = "Company not found"
                });
            }
            
            if (companyDto.Id == default ||
                string.IsNullOrEmpty(companyDto.CompanyName))
            {
                return BadRequest(new RestApiErrorResponse() {Error = "Companyname is empty!", Status = HttpStatusCode.BadRequest});
            }

            try
            {
                found.CompanyName = companyDto.CompanyName;
                var updated = _bll.Companies.Update(found);
                await _bll.SaveChangesAsync();
                return Ok(updated);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            
        }

        /// <summary>
        /// Creates a new company.
        /// </summary>
        /// <param name="company">The company to create.</param>
        /// <returns>The created company.</returns>
        [HttpPost]
        [ProducesResponseType<App.DTO.v1_0.CompanySimple>((int) HttpStatusCode.Created)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<App.DTO.v1_0.Company>> PostCompany(App.DTO.v1_0.CompanySimple companyDto)
        {
            var company = new Company()
            {
                CompanyName = companyDto.CompanyName
            };
            
            var c = await _bll.Companies.AddCompanyWithManagerAsync(company, Guid.Parse(_userManager.GetUserId(User)));
            await _bll.SaveChangesAsync();
            
            
            company.Id = c.Id;

            return CreatedAtAction("GetCompany", new
            {
                version = HttpContext.GetRequestedApiVersion()?.ToString(),
                id = c.Id,
            }, company);
        }

        /// <summary>
        /// Deletes a company by its ID.
        /// </summary>
        /// <param name="id">The ID of the company to delete.</param>
        /// <returns>NoContent if the deletion is successful; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            var company = await _bll.Companies.FirstOrDefaultAsync(id);
            if (company == null)
            {
                return NotFound(new RestApiErrorResponse()
                {
                    Status = HttpStatusCode.NotFound,
                    Error = "Company not found"
                });
            }
            
            

            _bll.Companies.Remove(company);
            await _bll.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Checks if a company exists.
        /// </summary>
        /// <param name="id">The ID of the company to check.</param>
        /// <returns>True if the company exists; otherwise, false.</returns>
        private bool CompanyExists(Guid id)
        {
            return _bll.Companies.Exists(id);
        }
    }
}
