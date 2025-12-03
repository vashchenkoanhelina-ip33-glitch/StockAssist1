namespace StockAssist.Web.Models.ViewModels
{
    public class AdminReportsVm
    {
        public int TotalOrders { get; set; }
        public int Awaiting { get; set; }
        public int InProgress { get; set; }
        public int Completed { get; set; }
        public int Canceled { get; set; }
        public int PaidPayments { get; set; }
    }
}