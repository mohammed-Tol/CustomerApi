namespace CustomersApi.Models
{
    public class Beneficiaries
    {      
        public int  BenefId { get; set; }
        public string BenefName { get; set; }   

        public  long? BenefAccount { get; set; }

        public string IFSC { get; set; }

        public bool isActive { get; set; }

        public int CustomerId { get; set; }

        public int BenefAccType { get; set; }
    }
}
