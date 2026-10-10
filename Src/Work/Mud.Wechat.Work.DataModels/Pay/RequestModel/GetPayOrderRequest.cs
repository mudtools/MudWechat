// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Pay;

/// <summary>
/// 查询订单请求体（<c>/cgi-bin/miniapppay/get_order</c>，普通支付域）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class GetPayOrderRequest
{
    /// <summary>
    /// 获取或设置商户号（官方必填，二级商户号，由企业微信生成并下发）。
    /// </summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>
    /// 获取或设置商户订单号（官方必填，6~32 个字符）：
    /// 商户系统内部订单号，只能是数字、大小写字母、<c>_-*</c> 且在同一个商户号下唯一。
    /// </summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }
}
