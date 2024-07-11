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
    public class LoanRequestsController : ControllerBase
    {
        private readonly IAppBLL _bll;
        private readonly UserManager<AppUser> _userManager;
        private readonly PublicDTOBllMapper<App.DTO.v1_0.LoanRequest, App.BLL.DTO.LoanRequest> _mapper;
        private readonly PublicDTOBllMapper<App.DTO.v1_0.LoanRequestSimple, App.BLL.DTO.LoanRequest> _mapperSimple;


        public LoanRequestsController(IAppBLL bll, UserManager<AppUser> userManager, IMapper mapper)
        {
            _bll = bll;
            _userManager = userManager;
            _mapperSimple = new PublicDTOBllMapper<LoanRequestSimple, App.BLL.DTO.LoanRequest>(mapper);
            _mapper = new PublicDTOBllMapper<LoanRequest, App.BLL.DTO.LoanRequest>(mapper);
        }

        
        /// <summary>
        /// Gets all active loan requests.
        /// </summary>
        /// <returns>A list of loan requests.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<App.DTO.v1_0.LoanRequest>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<IEnumerable<LoanRequest>>> GetLoanRequests()
        {
            var res = await _bll.LoanRequests.GetAllActiveLoanRequestsIncludingAsync();
            var loanRequestDTOs = res.Select(x => _mapper.Map(x));
            return Ok(loanRequestDTOs);
        }
        
        /// <summary>
        /// Gets all loan requests.
        /// </summary>
        /// <returns>A list of loan requests.</returns>
        [HttpGet("All")]
        [ProducesResponseType(typeof(IEnumerable<App.DTO.v1_0.LoanRequest>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<IEnumerable<LoanRequest>>> GetAllLoanRequests()
        {
            var res = await _bll.LoanRequests.GetAllLoanRequestsIncludingAsync();
            var loanRequestDTOs = res.Select(x => _mapper.Map(x));
            return Ok(loanRequestDTOs);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(App.DTO.v1_0.LoanRequest), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<LoanRequest>> GetLoanRequest(Guid id)
        {
            var loanRequest = await _bll.LoanRequests.FirstOrDefaultLoanRequestIncludingAsync(id);

            if (loanRequest == null)
            {
                return NotFound(new RestApiErrorResponse()
                {
                    Status = HttpStatusCode.NotFound,
                    Error = "Loan request not found!"
                });
            }
            
            var loanRequestDTO = _mapper.Map(loanRequest);

            return Ok(loanRequestDTO);
        }

        /// <summary>
        /// Updates a loan request.
        /// </summary>
        /// <returns>NoContent if the update is successful; otherwise, BadRequest or NotFound.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> PutLoanRequest(Guid id, LoanRequest loanRequestDto)
        {
            if (id != loanRequestDto.Id)
            {
                return BadRequest((new RestApiErrorResponse()
                {
                    Status = HttpStatusCode.BadRequest,
                    Error = "Loan request id mismatch!"
                }) );
            }

            try
            {
                var loanRequest = _mapper.Map(loanRequestDto);
                _bll.LoanRequests.Update(loanRequest);
                await _bll.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoanRequestExists(id))
                {
                    return NotFound(new RestApiErrorResponse()
                    {
                        Status = HttpStatusCode.NotFound,
                        Error = "Loan request not found!"
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
        /// Creates a new loan request.
        /// </summary>
        /// <returns>The created loan request.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(App.DTO.v1_0.LoanRequestSimple), (int) HttpStatusCode.Created)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<LoanRequestSimple>> PostLoanRequest(LoanRequestSimple loanRequestDto)
        {
            var loanRequest = _mapperSimple.Map(loanRequestDto);
            loanRequest!.CreatedAt = DateTime.Now.ToUniversalTime();
            loanRequest.Id = Guid.NewGuid();
            var lr = _bll.LoanRequests.Add(loanRequest);
            await _bll.SaveChangesAsync();

            return CreatedAtAction("GetLoanRequest", new { id = lr.Id }, loanRequest);
        }

        /// <summary>
        /// Deletes a loan request by its ID.
        /// </summary>
        /// <returns>NoContent if the deletion is successful; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> DeleteLoanRequest(Guid id)
        {
            var loanRequest = await _bll.LoanRequests.FirstOrDefaultAsync(id);
            if (loanRequest == null)
            {
                return NotFound(new RestApiErrorResponse()
                {
                    Status = HttpStatusCode.NotFound,
                    Error = "Loan request not found!"
                });
            }

            _bll.LoanRequests.Remove(loanRequest);
            await _bll.SaveChangesAsync();

            return NoContent();
        }

        private bool LoanRequestExists(Guid id)
        {
            return _bll.LoanRequests.Exists(id);
        }
    }
}
