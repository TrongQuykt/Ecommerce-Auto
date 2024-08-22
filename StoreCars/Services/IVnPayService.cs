using StoreCars.ViewModel;

namespace StoreCars.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModels PaymentExecute(IQueryCollection collections);
    }
}
