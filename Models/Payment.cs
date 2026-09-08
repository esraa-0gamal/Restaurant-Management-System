using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.Models
{
    public class Payment
    {
        [Key]
        public    int PaymentId { get; set; }

        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }= DateTime.Now;

     
        [Required, StringLength(50)]
        public string PaymentMethod { get; set; }= "Cash";
        public string Status { get; set; } = "Paid";
        public int OrderId { get; set; }
       public Order? Order { get; set; }



    }
}
