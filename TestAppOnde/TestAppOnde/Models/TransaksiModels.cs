namespace TestAppOnde.Models
{
    public class TransaksiModels
    {
        public int Id { get; set; }
        public string Invoice { get; set; }
        public int Harga { get; set; }
        public int Discount { get; set; }
        public string User { get; set; }
        public string Status { get; set; }
        public DateTime Tgl_Transaksi { get; set; }
    }
}
