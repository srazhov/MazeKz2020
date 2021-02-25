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
using Microsoft.AspNetCore.Routing;
using WebMaze.DbStuff.Model.UserAccount;
using WebMaze.Models.Transactions;
using WebMaze.Services;

namespace WebMaze.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private IMapper mapper;

        private readonly HttpClient httpClient;

        public TransactionsController(IMapper mapper, LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            this.mapper = mapper;

            var uri = $"{httpContextAccessor.HttpContext.Request.Scheme}://" +
                      $"{httpContextAccessor.HttpContext.Request.Host}" +
                      linkGenerator.GetPathByAction("GetTransactions", "TransactionsApi");
            
            httpClient = new HttpClient { BaseAddress = new Uri(uri) };
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
            return View("_MakeTransactionPartial", new TransactionViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Make(TransactionViewModel transactionViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("_MakeTransactionPartial", transactionViewModel);
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

            return View("_MakeTransactionPartial", transactionViewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
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
