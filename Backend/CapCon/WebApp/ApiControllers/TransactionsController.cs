using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using App.BLL.DTO.Enums;
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
using NuGet.Protocol;
using WebApp.Helpers;

namespace WebApp.ApiControllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TransactionsController : ControllerBase
    {
        private readonly IAppBLL _bll;
        private readonly UserManager<AppUser> _userManager;
        private readonly PublicDTOBllMapper<App.DTO.v1_0.Transaction, App.BLL.DTO.Transaction> _mapper;
        private readonly ILogger<TransactionsController>? _logger;


        public TransactionsController(IAppBLL bll, UserManager<AppUser> userManager, IMapper mapper, ILogger<TransactionsController>? logger)
        {
            _bll = bll;
            _userManager = userManager;
            _mapper = new PublicDTOBllMapper<Transaction, App.BLL.DTO.Transaction>(mapper);
            _logger = logger;
        }

        // GET: api/Transactions
        /// <summary>
        /// Gets all user's transactions.
        /// </summary>
        /// <returns>A list of transactions.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<App.DTO.v1_0.Transaction>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            var res = (await _bll.Transactions.GetAllAsync(
                Guid.Parse(_userManager.GetUserId(User)))
                ).Select((t) => _mapper.Map(t));
            return Ok(res);
        }

        /// <summary>
        /// Gets a transaction by its ID.
        /// </summary>
        /// <returns>The transaction if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(App.DTO.v1_0.Transaction), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<Transaction>> GetTransaction(Guid id)
        {
            var transaction = await _bll.Transactions.FirstOrDefaultAsync(id);

            if (transaction == null)
            {
                return NotFound(new RestApiErrorResponse()
                {
                    Status = HttpStatusCode.NotFound,
                    Error = "Transaction not found!"
                });
            }
            
            var transactionDTO = _mapper.Map(transaction);

            return Ok(transactionDTO);
        }

        /// <summary>
        /// Updates a transaction.
        /// </summary>
        /// <returns>NoContent if the update is successful; otherwise, BadRequest or NotFound.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> PutTransaction(Guid id, Transaction transactionDto)
        {
            if (id != transactionDto.Id)
            {
                return BadRequest(new RestApiErrorResponse()
                {
                    Status = HttpStatusCode.BadRequest,
                    Error = "Transaction id mismatch!"
                });
            }
            
            try
            {
                var transaction = _mapper.Map(transactionDto);
                _bll.Transactions.Update(transaction);
                await _bll.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id))
                {
                    return NotFound(new RestApiErrorResponse()
                    {
                        Status = HttpStatusCode.NotFound,
                        Error = "Transaction not found!"
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
        /// Creates a new transaction.
        /// </summary>
        /// <returns>The created transaction.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(App.DTO.v1_0.Transaction), (int) HttpStatusCode.Created)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int) HttpStatusCode.BadRequest)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<Transaction>> PostTransaction(Transaction transactionDto)
        {
            transactionDto.Id = Guid.NewGuid();
            
            try
            {
                var transaction = await _bll.AddTransactionToLoanAsync(transactionDto.LoanId, _mapper.Map(transactionDto), Guid.Parse(_userManager.GetUserId(User)));
                return CreatedAtAction("GetTransaction", new { id = transaction!.Id }, transaction);
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new RestApiErrorResponse()
                    {
                        Status = HttpStatusCode.BadRequest,
                        Error = ex.Message
                    }
                );
            }
        }

        /// <summary>
        /// Deletes a transaction by its ID.
        /// </summary>
        /// <returns>NoContent if the deletion is successful; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(RestApiErrorResponse), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> DeleteTransaction(Guid id)
        {
            var transaction = await _bll.Transactions.FirstOrDefaultAsync(id);
            if (transaction == null)
            {
                return NotFound(new RestApiErrorResponse()
                {
                    Status = HttpStatusCode.NotFound,
                    Error = "Transaction not found!"
                });
            }

            _bll.Transactions.Remove(transaction);
            await _bll.SaveChangesAsync();

            return NoContent();
        }

        private bool TransactionExists(Guid id)
        {
            return _bll.Transactions.Exists(id);
        }
    }
}
