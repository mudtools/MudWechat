// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Pay;

/// <summary>
/// 查询商户号详情响应体（<c>/cgi-bin/externalpay/getmerchant</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class GetPayMerchantResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置微信支付商户号（不超过 32 字节）。
    /// </summary>
    [JsonPropertyName("mch_id")]
    public string? MchId { get; set; }

    /// <summary>
    /// 获取或设置商户号全称（不超过 50 字节）。
    /// </summary>
    [JsonPropertyName("merchant_name")]
    public string? MerchantName { get; set; }

    /// <summary>
    /// 获取或设置绑定状态：1 - 申请中、2 - 已绑定、3 - 已撤销。
    /// </summary>
    [JsonPropertyName("bind_status")]
    public int? BindStatus { get; set; }

    /// <summary>
    /// 获取或设置使用范围（仅已绑定时返回）。
    /// </summary>
    [JsonPropertyName("allow_use_scope")]
    public PayMerchantUseScope? AllowUseScope { get; set; }
}

/// <summary>
/// 商户号使用范围（<see cref="GetPayMerchantResponse.AllowUseScope"/> /
/// <see cref="SetPayMerchantUseScopeRequest.AllowUseScope"/>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayMerchantUseScope
{
    /// <summary>
    /// 获取或设置使用范围内的企业用户 ID 列表。
    /// </summary>
    [JsonPropertyName("user")]
    public List<string>? User { get; set; }

    /// <summary>
    /// 获取或设置使用范围内的部门 ID 列表。
    /// </summary>
    [JsonPropertyName("partyid")]
    public List<int>? PartyId { get; set; }

    /// <summary>
    /// 获取或设置使用范围内的标签 ID 列表。
    /// </summary>
    [JsonPropertyName("tagid")]
    public List<int>? TagId { get; set; }
}
