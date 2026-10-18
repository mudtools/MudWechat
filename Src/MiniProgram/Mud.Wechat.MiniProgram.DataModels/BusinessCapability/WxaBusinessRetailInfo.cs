// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.BusinessCapability;

/// <summary>
/// 门店信息查询结果条目。
/// </summary>
/// <remarks>
/// <para><b>官方字段表与示例结构不一致（诚实留档）</b>：字段表把门店字段平铺列出，回包示例却包在
/// <c>info</c> 数组内 ⇒ 本 SDK 按示例建模为数组元素，并以 <c>WxaBusinessGetRetailInfoResponse.Info</c> 承载。</para>
/// <para><b>敏感面</b>：手机号、统一社会信用代码、法人/负责人姓名、openid 一律禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BusinessCapability")]
public class WxaBusinessRetailInfo
{
    /// <summary>门店联系人手机号（敏感）。</summary>
    [JsonPropertyName("mobile_phone")]
    public string? MobilePhone { get; set; }

    /// <summary>门店类型。</summary>
    [JsonPropertyName("retail_type")]
    public string? RetailType { get; set; }

    /// <summary>门店子类型。</summary>
    [JsonPropertyName("sub_retail_type")]
    public string? SubRetailType { get; set; }

    /// <summary>门店地址。</summary>
    [JsonPropertyName("retail_address")]
    public string? RetailAddress { get; set; }

    /// <summary>门店名称。</summary>
    [JsonPropertyName("retail_name")]
    public string? RetailName { get; set; }

    /// <summary>门店标识（官方未说明编码规则）。</summary>
    [JsonPropertyName("identification")]
    public string? Identification { get; set; }

    /// <summary>负责人姓名（敏感）。</summary>
    [JsonPropertyName("principal")]
    public string? Principal { get; set; }

    /// <summary>法人姓名（敏感）。</summary>
    [JsonPropertyName("legal_person_name")]
    public string? LegalPersonName { get; set; }

    /// <summary>门店成员 openid（敏感）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>角色（示例 1 管理员 / 2 员工，官方未发布完整枚举）。</summary>
    [JsonPropertyName("role")]
    public int? Role { get; set; }

    /// <summary>认证状态（示例 1 已认证，官方未发布完整枚举）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>认证时间（Unix 秒级时间戳）。</summary>
    [JsonPropertyName("auth_time")]
    public long? AuthTime { get; set; }

    /// <summary>授权时间（Unix 秒级时间戳）。</summary>
    [JsonPropertyName("grant_time")]
    public long? GrantTime { get; set; }

    /// <summary>经度。</summary>
    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }

    /// <summary>纬度。</summary>
    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }

    /// <summary>经营类目（官方未发布枚举）。</summary>
    [JsonPropertyName("business_type")]
    public List<int>? BusinessType { get; set; }

    /// <summary>其它经营类目。</summary>
    [JsonPropertyName("other_business_type")]
    public string? OtherBusinessType { get; set; }

    /// <summary>门店员工列表。</summary>
    [JsonPropertyName("staff_list")]
    public List<WxaBusinessRetailStaff>? StaffList { get; set; }
}
