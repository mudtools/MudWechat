// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>
/// 获取供货商基础信息（<c>get_supplier_base_info</c>）响应。
/// </summary>
/// <remarks>
/// 官方契约：POST + 请求体参数表为空（2026-10-17 逐页核验）；官方错误码 <c>44002</c>「请求内容为空」
/// 在场 ⇒ 无请求体形态以联调为准，SDK 不造表外字段。
/// <c>subject_type</c> 枚举：2 企业 / 2500 个人；<c>head_img</c> 个人主体无头像。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsGetSupplierBaseInfoResponse : ChannelsResponse
{
    /// <summary>获取或设置供货商名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置供货商头像（官方 <c>head_img</c>；个人主体无头像）。</summary>
    [JsonPropertyName("head_img")]
    public string? HeadImg { get; set; }

    /// <summary>获取或设置供货商主体类型（官方 <c>subject_type</c>；枚举 2 企业 / 2500 个人，照录官方值域）。</summary>
    [JsonPropertyName("subject_type")]
    public int? SubjectType { get; set; }

    /// <summary>获取或设置供货商可用的经营模式（官方 <c>business_mode</c>）。</summary>
    [JsonPropertyName("business_mode")]
    public ChannelsSupplierBusinessMode? BusinessMode { get; set; }
}
