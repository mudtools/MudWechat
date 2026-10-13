// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「下载广告金对应的商户订单信息」请求体（<c>POST /xpay/download_adverfunds_order</c>）。
/// </summary>
/// <remarks>
/// <para><b>轮询式生成（官方原文）</b>：「第一次调用触发生成下载url，可以间隔轮训来获取最终生成的下载url」⇒ 首次调用拿到空链接属预期，须轮询而非报错。</para>
/// <para><b>适用范围官方限定</b>：「当前仅支持通用赠送广告金对应订单下载」（<c>fund_type = 0</c>，见 <see cref="WxaXPayAdverFund"/>）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_download_adverfunds_order.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayDownloadAdverfundsOrderRequest
{
    /// <summary>fund_id（官方类型 <c>string</c>，<b>必填</b>）：广告金发放ID</summary>
    [JsonPropertyName("fund_id")]
    public string? FundId { get; set; }

    /// <summary>env（官方类型 <c>number</c>，<b>必填</b>）：0-正式环境 1-沙箱环境（仅作为签名校验，查询的结果都是正式环境的）</summary>
    [JsonPropertyName("env")]
    public long? Env { get; set; }
}
