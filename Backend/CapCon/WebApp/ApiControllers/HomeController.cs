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

namespace WebApp.ApiControllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class HomeController : ControllerBase
    {
        private readonly IAppBLL _bll;
        private readonly UserManager<AppUser> _userManager;
        private readonly PublicDTOBllMapper<App.DTO.v1_0.UserCompany, App.BLL.DTO.UserCompany> _userCompanyMapper;
        private readonly PublicDTOBllMapper<App.DTO.v1_0.Loan, App.BLL.DTO.Loan> _loanMapper;
        private readonly PublicDTOBllMapper<App.DTO.v1_0.LoanOffer, App.BLL.DTO.LoanOffer> _loanOfferMapper;
        private readonly ILogger<HomeController>? _logger;


        public HomeController(IAppBLL bll, UserManager<AppUser> userManager, IMapper autoMapper,
            ILogger<HomeController>? logger)
        {
            _bll = bll;
            _userManager = userManager;
            _logger = logger;
            _userCompanyMapper =
                new PublicDTOBllMapper<App.DTO.v1_0.UserCompany, App.BLL.DTO.UserCompany>(autoMapper);
            _loanMapper =
                new PublicDTOBllMapper<App.DTO.v1_0.Loan, App.BLL.DTO.Loan>(autoMapper);
            _loanOfferMapper =
                new PublicDTOBllMapper<App.DTO.v1_0.LoanOffer, App.BLL.DTO.LoanOffer>(autoMapper);
        }
        
        /// <summary>
        /// Returns homepage data for user.
        /// </summary>
        /// <returns>Data for homepage relevant to user.</returns>
        [HttpGet]
        [ProducesResponseType<HomePageData>((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.NotFound)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<HomePageData>> GetCompanies()
        {
            
            var userEmailClaim = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
            if (userEmailClaim == null)
            {
                return NotFound(new RestApiErrorResponse {Error = "JWT does not contain email claim", Status = HttpStatusCode.NotFound});
            }
    
            var userEmail = userEmailClaim.Value;
            var user = await _userManager.FindByEmailAsync(userEmail);

            var userId = _userManager.GetUserId(User);
            
            if (userId == null)
            {
                return BadRequest(
                    new RestApiErrorResponse()
                    {
                        Status = HttpStatusCode.BadRequest,
                        Error = "User not found!"
                    }
                );
            }
            
            var userCompanies = (await _bll.UserCompanies.GetAllAsync(Guid.Parse(userId)))
                .Select(uc => _userCompanyMapper.Map(uc)!)
                .ToList();
            
            var userLoans = (await _bll.Loans.GetAllAsync(Guid.Parse(userId)))
                .Select(l => _loanMapper.Map(l)!)
                .ToList();

            var userLoanOffers = (await _bll.LoanOffers.GetAllAsync(Guid.Parse(userId)))
                .Select(lo => _loanOfferMapper.Map(lo)!)
                .ToList();
            
            return Ok(new HomePageData()
            {
                UserCompanies = userCompanies,
                UserLoans = userLoans,
                UserLoanOffers = userLoanOffers
            });
        }


    }
}