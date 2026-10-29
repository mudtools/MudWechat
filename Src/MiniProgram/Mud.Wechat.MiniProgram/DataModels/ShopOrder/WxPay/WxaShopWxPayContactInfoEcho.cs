// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 进件回显的超级管理员信息（<c>/shop/wxpay/get</c> 与 <c>get_info</c> 读侧）。
/// </summary>

[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopWxPayContactInfoEcho
{
    /// <summary>超级管理员邮箱（个人敏感信息，不得进日志）。</summary>
    [JsonPropertyName("contact_email")]
    public string? ContactEmail { get; set; }

    /// <summary>超级管理员证件号码（个人敏感信息，不得进日志）。</summary>
    [JsonPropertyName("contact_id_card_number")]
    public string? ContactIdCardNumber { get; set; }

    /// <summary>超级管理员姓名（个人敏感信息，不得进日志）。</summary>
    [JsonPropertyName("contact_name")]
    public string? ContactName { get; set; }

    /// <summary>超级管理员类型：<c>65</c> 经营者 / 法人、<c>66</c> 经办人。</summary>
    [JsonPropertyName("contact_type")]
    public string? ContactType { get; set; }

    /// <summary>超级管理员手机（个人敏感信息，不得进日志）。</summary>
    [JsonPropertyName("mobile_phone")]
    public string? MobilePhone { get; set; }
}
