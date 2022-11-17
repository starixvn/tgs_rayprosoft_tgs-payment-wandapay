namespace Tgs.Payment.WandaPay;

public class WandaApiException : Exception
{
	public WandaApiException(string? message) : base(message)
	{
	}
}
