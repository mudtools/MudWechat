// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Invoice;

/// <summary>
/// 电子发票结构化信息（批量查询电子发票响应 <c>item_list</c> 数组元素；
/// 字段面与「查询电子发票」响应顶层一致，但不含 errcode / errmsg 信封字段）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class InvoiceInfoItem
{
    /// <summary>
    /// 获取或设置发票 id（官方必返回）。
    /// </summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>
    /// 获取或设置发票有效期起始时间（Unix 时间戳，秒；官方必返回）。
    /// </summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>
    /// 获取或设置发票有效期结束时间（Unix 时间戳，秒；官方必返回）。
    /// </summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>
    /// 获取或设置用户标识（openid，官方必返回）。
    /// </summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>
    /// 获取或设置发票类型（如「广东增值税普通发票」，官方必返回）。
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// 获取或设置发票的收款方（官方必返回）。
    /// </summary>
    [JsonPropertyName("payee")]
    public string? Payee { get; set; }

    /// <summary>
    /// 获取或设置发票详情（官方必返回）。
    /// </summary>
    [JsonPropertyName("detail")]
    public string? Detail { get; set; }

    /// <summary>
    /// 获取或设置发票的用户信息（官方必返回）。
    /// </summary>
    [JsonPropertyName("user_info")]
    public InvoiceUserInfo? UserInfo { get; set; }
}
