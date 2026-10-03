// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Pay;

/// <summary>
/// 交易账单申请请求体（<c>/cgi-bin/miniapppay/get_bill</c>，交易账单域）。
/// <para>官方业务限制：仅支持三个月内的账单下载申请。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class GetPayTradeBillRequest
{
    /// <summary>
    /// 获取或设置账单日期（官方必填，10 个字符）：格式 yyyy-MM-dd；
    /// 仅支持三个月内的账单下载申请。
    /// </summary>
    [JsonPropertyName("bill_date")]
    public string? BillDate { get; set; }

    /// <summary>
    /// 获取或设置商户号（官方必填，8~32 个字符）：用于下载某个商户下的交易或退款数据。
    /// </summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>
    /// 获取或设置账单类型（官方可选，不填则默认 ALL）：ALL - 返回当日所有订单信息
    /// （不含充值退款订单）、SUCCESS - 返回当日成功支付的订单（不含充值退款订单）、
    /// REFUND - 返回当日退款订单（不含充值退款订单）。
    /// </summary>
    [JsonPropertyName("bill_type")]
    public string? BillType { get; set; }

    /// <summary>
    /// 获取或设置压缩类型（官方可选，不填则默认是数据流）：GZIP - 返回格式为 .gzip 的压缩包账单。
    /// </summary>
    [JsonPropertyName("tar_type")]
    public string? TarType { get; set; }
}
