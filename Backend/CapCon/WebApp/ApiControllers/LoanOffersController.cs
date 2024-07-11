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
    public class LoanOffersController : ControllerBase
    {
        private readonly IAppBLL _bll;
        private readonly UserManager<AppUser> _userManager;
        private readonly PublicDTOBllMapper<App.DTO.v1_0.LoanOffer, App.BLL.DTO.LoanOffer> _mapper;
        private readonly PublicDTOBllMapper<App.DTO.v1_0.Loan, App.BLL.DTO.Loan> _loanMapper;
        private readonly ILogger<LoanOffersController>? _logger;



        public LoanOffersController(IAppBLL bll, UserManager<AppUser> userManager, IMapper mapper, ILogger<LoanOffersController>? logger)
        {
            _bll = bll;
            _userManager = userManager;
            _mapper = new PublicDTOBllMapper<App.DTO.v1_0.LoanOffer, App.BLL.DTO.LoanOffer>(mapper);
            _loanMapper = new PublicDTOBllMapper<App.DTO.v1_0.Loan, App.BLL.DTO.Loan>(mapper);
            _logger = logger;

        }

        /// <summary>
        /// Gets all user's loan offers.
        /// </summary>
        /// <returns>A list of loan offers.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<App.DTO.v1_0.LoanOffer>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<IEnumerable<LoanOffer>>> GetLoanOffers()
        {
            var res = (await _bll.LoanOffers.GetAllLoanOffersIncludingAsync(
                    Guid.Parse(_userManager.GetUserId(User))))
                .Select((c) => _mapper.Map(c));
            return Ok(res);
        }

        /// <summary>
        /// Gets a loan offer by its ID.
        /// </summary>
        /// <returns>The loan offer if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(App.DTO.v1_0.LoanOffer), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<LoanOffer>> GetLoanOffer(Guid id)
        {
            var loanOffer = await _bll.LoanOffers.FirstOrDefaultLoanOfferIncludingAsync(id);

            if (loanOffer == null)
            {
                return NotFound( new RestApiErrorResponse()
                {
                    Status = HttpStatusCode.NotFound,
                    Error = "Loan offer not found!"
                });
            }
            
            var loanOfferDTO = _mapper.Map(loanOffer);

            return Ok(loanOfferDTO);
        }

        /// <summary>
        /// Updates a loan offer.
        /// </summary>
        /// <returns>NoContent if the update is successful; otherwise, BadRequest or NotFound.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> PutLoanOffer(Guid id, LoanOffer loanOfferDto)
        {
            if (id != loanOfferDto.Id)
            {
                return BadRequest( new RestApiErrorResponse()
                {
                    Status = HttpStatusCode.NotFound,
                    Error = "Loan offer id mismatch!"
                });
            }
            
            try
            {
                var loanOffer = _mapper.Map(loanOfferDto);
                _bll.LoanOffers.Update(loanOffer);
                await _bll.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoanOfferExists(id))
                {
                    return NotFound( new RestApiErrorResponse()
                    {
                        Status = HttpStatusCode.NotFound,
                        Error = "Loan offer not found!"
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
        /// Creates a new loan offer.
        /// </summary>
        /// <returns>The created loan offer.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(App.DTO.v1_0.LoanOffer), (int) HttpStatusCode.Created)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<LoanOffer>> PostLoanOffer(LoanOffer loanOfferDto)
        {
            var loanOffer = _mapper.Map(loanOfferDto);
            loanOffer!.CreatedAt = DateTime.Now.ToUniversalTime();
            loanOffer.Id = Guid.NewGuid();
            _bll.LoanOffers.Add(loanOffer);
            await _bll.SaveChangesAsync();

            return CreatedAtAction("GetLoanOffer", new { id = loanOffer.Id }, loanOffer);
        }
        
        /// <summary>
        /// Accepts a loan offer.
        /// </summary>
        /// <returns>The accepted loan.</returns>
        [HttpPost("AcceptOffer")]
        [ProducesResponseType(typeof(App.DTO.v1_0.Loan), (int) HttpStatusCode.Created)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.BadRequest)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<Loan>> AcceptLoanOffer([FromBody]LoanOffer loanOfferDto)
        {
            var loanOffer = await _bll.LoanOffers.FirstOrDefaultAsync(loanOfferDto.Id);
            if (loanOffer == null)
            {
                return BadRequest(new RestApiErrorResponse()
                {
                    Status = HttpStatusCode.BadRequest,
                    Error = "Loan offer not found!"
                });
            }

            loanOffer.Active = false;
            _bll.LoanOffers.Update(loanOffer);

            var loanRequest = await _bll.LoanRequests.FirstOrDefaultAsync(loanOffer.LoanRequestId.Value);
            loanRequest!.Active = false;
            _bll.LoanRequests.Update(loanRequest);

            
            var loan = new App.BLL.DTO.Loan()
            {
                 AppUserId = loanOffer.AppUserId,
                 CompanyId = loanRequest.CompanyId,
                 Active = false,
                 Amount = loanOffer.Amount,
                 Interest = loanOffer.Interest,
                 Period = loanOffer.Period,
                 StartDate = DateTime.Now.ToUniversalTime(),
                 Comment = "From request: " + loanRequest.Comment + "\n" + "From offer: " + loanOffer.Comment
            };
            
            loan.Id= Guid.NewGuid();
            
            var l = _bll.Loans.Add(loan);

            await _bll.SaveChangesAsync();
            
            l = await _bll.Loans.FirstOrDefaultLoanIncludingAsync(l.Id);

            return Ok(_loanMapper.Map(l));

        }

        /// <summary>
        /// Deletes a loan offer by its ID.
        /// </summary>
        /// <returns>NoContent if the deletion is successful; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> DeleteLoanOffer(Guid id)
        {
            var loanOffer = await _bll.LoanOffers.FirstOrDefaultAsync(id);
            if (loanOffer == null)
            {
                return NotFound(new RestApiErrorResponse()
                {
                    Status = HttpStatusCode.NotFound,
                    Error = "Loan offer not found!"
                });
            }

            _bll.LoanOffers.Remove(loanOffer);
            await _bll.SaveChangesAsync();

            return NoContent();
        }

        private bool LoanOfferExists(Guid id)
        {
            return _bll.LoanOffers.Exists(id);
        }
    }
}
