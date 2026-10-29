// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Qic;

/// <summary>质检收发货地址（官方 <c>delivery_address</c> / <c>return_address</c> 对象，字段集一致 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Qic")]
public class ChannelsInspectAddress
{
    /// <summary>获取或设置联系人（官方 <c>contact_name</c>）。</summary>
    [JsonPropertyName("contact_name")]
    public string? ContactName { get; set; }

    /// <summary>获取或设置联系电话（官方 <c>contact_phone</c>）。</summary>
    [JsonPropertyName("contact_phone")]
    public string? ContactPhone { get; set; }

    /// <summary>获取或设置省（官方 <c>province</c>）。</summary>
    [JsonPropertyName("province")]
    public string? Province { get; set; }

    /// <summary>获取或设置市（官方 <c>city</c>）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>获取或设置区（官方 <c>county</c>）。</summary>
    [JsonPropertyName("county")]
    public string? County { get; set; }

    /// <summary>获取或设置详细地址（官方 <c>detail</c>）。</summary>
    [JsonPropertyName("detail")]
    public string? Detail { get; set; }
}
