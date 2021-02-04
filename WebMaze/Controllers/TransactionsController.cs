using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebMaze.DbStuff.Model.UserAccount;
using WebMaze.Models.Transactions;
using WebMaze.Services;

namespace WebMaze.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private TransactionService transactionService;

        private IMapper mapper;

        private readonly HttpClient httpClient;

        public TransactionsController(TransactionService transactionService, IMapper mapper)
        {
            this.transactionService = transactionService;
            this.mapper = mapper;
            httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:44302/api/transactions/") };
        }

        public async Task<IActionResult> Index()
        {
            var userLogin = User.Identity.Name;
            var responseString = await httpClient.GetStringAsync($"?userLogin={userLogin}");
            var certificatesViewModels = responseString.DeserializeCaseInsensitive<List<TransactionViewModel>>();

            return View(certificatesViewModels);
        }

        public IActionResult Make()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Make(TransactionViewModel transactionViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(transactionViewModel);
            }

            transactionViewModel.SenderLogin = User.Identity.Name;
            transactionViewModel.Type = TransactionType.Transfer;

            var transactionJson = new StringContent(JsonSerializer.Serialize(transactionViewModel), Encoding.UTF8,
                "application/json");

            using var httpResponse = await httpClient.PostAsync("", transactionJson);

            if (httpResponse.StatusCode == HttpStatusCode.Created)
            {
                return RedirectToAction(nameof(Index));
            }

            var responseString = await httpResponse.Content.ReadAsStringAsync();
            var errorMessages = responseString.DeserializeCaseInsensitive<List<string>>();
            errorMessages.ForEach(errorMessage => ModelState.AddModelError(string.Empty, errorMessage));

            return View(transactionViewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            //var transaction = await transactionService.GetTransactionByIdAsync(id);

            var responseString = await httpClient.GetStringAsync(id.ToString());
            var transactionViewModel = responseString.DeserializeCaseInsensitive<TransactionViewModel>();

            return View(transactionViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, TransactionViewModel transactionViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(transactionViewModel);
            }

            var certificateJson = new StringContent(JsonSerializer.Serialize(transactionViewModel), Encoding.UTF8,
                "application/json");

            using var httpResponse = await httpClient.PutAsync(transactionViewModel.Id.ToString(), certificateJson);

            if (httpResponse.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var responseString = await httpResponse.Content.ReadAsStringAsync();
            var errorMessages = responseString.DeserializeCaseInsensitive<List<string>>();
            errorMessages.ForEach(errorMessage => ModelState.AddModelError(string.Empty, errorMessage));

            return View(transactionViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            using var httpResponse = await httpClient.DeleteAsync(id.ToString());

            return RedirectToAction(nameof(Index));
        }
    }
}
