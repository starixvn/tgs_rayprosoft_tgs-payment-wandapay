namespace Tgs.Payment.WandaPay;

public class WandaPayOptions
{
    public required string CallbackBaseUrl { get; set; }

    public required Uri PaymentApiUrl { get; set; }

    public required Uri PaymentServiceBaseUrl { get; set; }
}
