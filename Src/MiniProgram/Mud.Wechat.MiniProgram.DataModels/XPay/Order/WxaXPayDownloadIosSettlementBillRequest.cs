// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「下载虚拟支付 iOS 月结账单」请求体（<c>POST /xpay/download_ios_settlement_bill</c>）。
/// </summary>
/// <remarks>
/// <para><b>粒度是「月」</b>（<c>YYYYMM</c>）而非日，与 <see cref="WxaXPayDownloadBillRequest"/> 的 <c>YYYYMMDD</c> 区间不同形 ⇒ 两支不共用请求类型。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_download_ios_settlement_bill.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayDownloadIosSettlementBillRequest
{
    /// <summary>start_month（官方类型 <c>string</c>，<b>必填</b>）：开始月份，格式 YYYYMM</summary>
    [JsonPropertyName("start_month")]
    public string? StartMonth { get; set; }

    /// <summary>end_month（官方类型 <c>string</c>，<b>必填</b>）：结束月份，格式 YYYYMM</summary>
    [JsonPropertyName("end_month")]
    public string? EndMonth { get; set; }
}
