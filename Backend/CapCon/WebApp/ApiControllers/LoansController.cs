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
    public class LoansController : ControllerBase
    {
        private readonly IAppBLL _bll;
        private readonly UserManager<AppUser> _userManager;
        private readonly PublicDTOBllMapper<App.DTO.v1_0.Loan, App.BLL.DTO.Loan> _mapper;
        public LoansController(IAppBLL bll, UserManager<AppUser> userManager, IMapper mapper)
        {
            _bll = bll;
            _userManager = userManager;
            _mapper = new PublicDTOBllMapper<App.DTO.v1_0.Loan, App.BLL.DTO.Loan>(mapper);
        }

        /// <summary>
        /// Gets all user's loans.
        /// </summary>
        /// <returns>A list of loans.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<App.DTO.v1_0.Loan>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<IEnumerable<Loan>>> GetLoans()
        {
            
            var res = (await _bll.Loans.GetAllLoansIncludingAsync(
                Guid.Parse(_userManager.GetUserId(User)))
                ).Select((l) => _mapper.Map(l));
            return Ok(res);
        }

        /// <summary>
        /// Gets a loan by its ID.
        /// </summary>
        /// <returns>The loan if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(App.DTO.v1_0.Loan), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<Loan>> GetLoan(Guid id)
        {
            var loan = await _bll.Loans.FirstOrDefaultLoanIncludingAsync(id);

            if (loan == null)
            {
                return NotFound(new RestApiErrorResponse()
                {
                    Status = HttpStatusCode.NotFound,
                    Error = "Loan not found!"
                });
            }
            
            var loanDTO = _mapper.Map(loan);

            return Ok(loanDTO);
        }

        /// <summary>
        /// Updates a loan.
        /// </summary>
        /// <returns>NoContent if the update is successful; otherwise, BadRequest or NotFound.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> PutLoan(Guid id, Loan loanDto)
        {
            if (id != loanDto.Id)
            {
                return BadRequest(new RestApiErrorResponse()
                {
                    Status = HttpStatusCode.BadRequest,
                    Error = "Loan id mismatch!"
                });
            }

            try
            {
                var loan = _mapper.Map(loanDto);
                _bll.Loans.Update(loan);
                await _bll.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoanExists(id))
                {
                    return NotFound(new RestApiErrorResponse()
                    {
                        Status = HttpStatusCode.NotFound,
                        Error = "Loan not found!"
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
        /// Creates a new loan.
        /// </summary>
        /// <returns>The created loan.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(App.DTO.v1_0.Loan), (int) HttpStatusCode.Created)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<Loan>> PostLoan(Loan loanDto)
        {
            var loan = _mapper.Map(loanDto);
            _bll.Loans.Add(loan);
            await _bll.SaveChangesAsync();

            return CreatedAtAction("GetLoan", new { id = loan.Id }, loan);
        }

        /// <summary>
        /// Deletes a loan by its ID.
        /// </summary>
        /// <returns>NoContent if the deletion is successful; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> DeleteLoan(Guid id)
        {
            var loan = await _bll.Loans.FirstOrDefaultAsync(id);
            if (loan == null)
            {
                return NotFound(new RestApiErrorResponse()
                {
                    Status = HttpStatusCode.NotFound,
                    Error = "Loan not found!"
                });
            }

            _bll.Loans.Remove(loan);
            await _bll.SaveChangesAsync();

            return NoContent();
        }

        private bool LoanExists(Guid id)
        {
            return _bll.Loans.Exists(id);
        }
    }
}
