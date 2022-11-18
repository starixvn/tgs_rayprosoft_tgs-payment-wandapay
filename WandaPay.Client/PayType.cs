using Tgs.Payment.WandaPay;
namespace Tgs.Payment.WandaPay.Client;

public static class PayType
{
	public static readonly string BankCard = "bankcard";
	public static readonly string MiniBankCard = "m_bankcard";
	public static readonly string AliPayQrCode = "alipay.qr";
	public static readonly string AliPayH5 = "alipay.h5";
	public static readonly string WechatQrCode = "wechat.qr";
	public static readonly string WechatH5 = "wechat.h5";
	public static readonly string Usdt = "USDT";
	public static readonly string IndiaUpi = "india_upi";
	public static readonly string IndiaPro = "india_pro";
	public static readonly string BrasilPix = "brasil_pix";
	public static readonly string ThaiOrCode = "thaiqr";
	public static readonly string Trx = "TRX";
}
