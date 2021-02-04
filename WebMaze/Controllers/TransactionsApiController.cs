using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebMaze.DbStuff.Model.UserAccount;
using WebMaze.Models.Transactions;
using WebMaze.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebMaze.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    public class TransactionsApiController : ControllerBase
    {
        private TransactionService transactionService;

        private UserService userService;

        private IMapper mapper;

        public TransactionsApiController(TransactionService transactionService, UserService userService, IMapper mapper)
        {
            this.transactionService = transactionService;
            this.userService = userService;
            this.mapper = mapper;
        }

        // GET: api/transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionViewModel>>> GetTransactions([FromQuery] string userLogin)
        {
            List<Transaction> transactions;

            if (!string.IsNullOrWhiteSpace(userLogin))
            {
                transactions = await transactionService.GetUserTransactionsAsync(userLogin);
            }
            else
            {
                transactions = await transactionService.GetTransactionsAsync();
            }

            var transactionViewModels = mapper.Map<List<TransactionViewModel>>(transactions);

            return transactionViewModels;
        }

        // GET api/transactions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionViewModel>> GetTransaction(long id)
        {
            var transaction = await transactionService.GetTransactionByIdAsync(id);

            if (transaction == null)
            {
                return NotFound($"Transaction with ID = {id} not found");
            }

            var transactionViewModel = mapper.Map<TransactionViewModel>(transaction);

            return transactionViewModel;
        }

        // POST api/transactions
        [HttpPost]
        public async Task<ActionResult<TransactionViewModel>> PostTransaction(TransactionViewModel transactionViewModel)
        {
            // Exclude property from binding.
            transactionViewModel.Id = 0;

            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values.SelectMany(modelStateEntry => modelStateEntry.Errors.Select(b => b.ErrorMessage)).ToList();
                return BadRequest(errorMessages);
            }

            var sender = userService.FindByLogin(transactionViewModel.SenderLogin);
            var recipient = userService.FindByLogin(transactionViewModel.RecipientLogin);

            if (sender == null || recipient == null)
            {
                var notFoundUserLogin = sender == null ? transactionViewModel.SenderLogin : transactionViewModel.RecipientLogin;
                return BadRequest(new List<string>() { $"User with Login = {notFoundUserLogin} not found" });
            }

            var transaction = mapper.Map<Transaction>(transactionViewModel);
            transaction.Sender = sender;
            transaction.Recipient = recipient;
            transaction.Date = DateTime.Now;

            var operationResult = await transactionService.CreateTransaction(transaction);

            if (!operationResult.Succeeded)
            {
                return BadRequest(operationResult.Errors);
            }

            transactionViewModel = mapper.Map<TransactionViewModel>(transaction);

            return CreatedAtAction("GetTransaction", new { id = transactionViewModel.Id }, transactionViewModel);
        }

        // PUT api/transactions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransaction(long id, TransactionViewModel transactionViewModel)
        {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values.SelectMany(modelStateEntry => modelStateEntry.Errors.Select(b => b.ErrorMessage)).ToList();
                return BadRequest(errorMessages);
            }

            if (id != transactionViewModel.Id)
            {
                return BadRequest(new List<string>() { "Transaction ID mismatch" });
            }

            var transaction = await transactionService.GetTransactionByIdAsync(id);

            if (transaction == null)
            {
                return NotFound(new List<string>() { $"Transaction with ID = {id} not found" });
            }

            if (transaction.Sender.Login != transactionViewModel.SenderLogin ||
                transaction.Recipient.Login != transactionViewModel.RecipientLogin)
            {
                return BadRequest(new List<string>() { "Cannot change transaction participants" });
            }

            transaction.Description = transactionViewModel.Description;
            transaction.Category = transactionViewModel.Category;
            transaction.Type = transactionViewModel.Type;

            await transactionService.UpdateTransaction(transaction);

            return NoContent();
        }

        // DELETE api/transactions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await transactionService.TransactionExistsAsync(id))
            {
                return NotFound(new List<string>() { $"Certificate with ID = {id} not found" });
            }

            await transactionService.DeleteTransactionAsync(id);

            return NoContent();
        }
    }
}
