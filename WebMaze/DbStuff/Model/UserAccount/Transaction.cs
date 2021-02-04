using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebMaze.DbStuff.Model.UserAccount
{
    public class Transaction : BaseModel
    {
        [Required]
        public virtual DateTime Date { get; set; }

        public virtual string Description { get; set; }

        [Column(TypeName = "money")]
        public virtual decimal Amount { get; set; }

        public virtual TransactionCategory Category { get; set; }
        
        public virtual TransactionType Type { get; set; }

        public virtual CitizenUser Sender { get; set; }

        public virtual CitizenUser Recipient { get; set; }
    }

    public enum TransactionCategory
    {
        Fees,
        Shopping,
        Income,
        Education,
        Health,
        Transport,
        Entertainment,
        Food,
        Taxes,
        Rent,
        Pension,
        Donation
    }

    public enum TransactionType
    {
        Purchase,
        Transfer,
        Payment,
        Refund
    }
}