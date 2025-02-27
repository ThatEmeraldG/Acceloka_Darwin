using Acceloka.Entities;
using Acceloka.Models;
using Microsoft.EntityFrameworkCore;
namespace Acceloka.Services
{
    public class TransactionService
    {
        private readonly AccelokaContext _db;
        private readonly ILogger<TransactionService> _logger;
        public TransactionService(AccelokaContext db, ILogger<TransactionService> logger)
        {
            _db = db;
            _logger = logger;
        }

        // GET transaction
        public async Task<List<TransactionModel>> GetAllTransactions()
        {
            _logger.LogInformation("Fetching all transactions...");

            var datas = await _db.Transactions
                .Select(Q => new TransactionModel
                {
                    TransactionId = Q.TransactionId,
                    TotalPrice = Q.TotalPrice,
                    TotalPayment = Q.TotalPayment,
                    PaymentMethod = Q.PaymentMethod,
                    TransactionDate = Q.TransactionDate,
                    CreatedBy = Q.CreatedBy,
                    CreatedAt = Q.CreatedAt,
                    UpdatedBy = Q.UpdatedBy,
                    UpdatedAt = Q.UpdatedAt
                }).ToListAsync();

            _logger.LogInformation("Successfully fetched {Count} transactions", datas.Count);
            return datas;
        }

        public async Task<TransactionModel> GetTransactionById(int id)
        {
            _logger.LogInformation("Fetching transaction {TransactionId}...", id);

            var data = await _db.Transactions.FirstOrDefaultAsync(x => x.TransactionId == id);
            if (data == null)
            {
                _logger.LogWarning("Transaction {TransactionId} not found", id);
                return null;
            }

            var transaction = new TransactionModel
            {
                TransactionId = data.TransactionId,
                TotalPrice = data.TotalPrice,
                TotalPayment = data.TotalPayment,
                PaymentMethod = data.PaymentMethod,
                TransactionDate = data.TransactionDate,
                CreatedBy = data.CreatedBy,
                CreatedAt = data.CreatedAt,
                UpdatedBy = data.UpdatedBy,
                UpdatedAt = data.UpdatedAt
            };

            _logger.LogInformation("Successfully fetched transaction with ID: {TransactionId}", id);
            return transaction;
        }
    }
}