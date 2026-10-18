// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopAfterSale;

/// <summary>
/// 纠纷处理流转记录（<c>flow_info.list[]</c> 元素）。
/// </summary>
/// <remarks>
/// <para><b><c>time</c> 是字符串</b>：官方示例给 <c>"1647431667908"</c>（毫秒时间戳的字符串形态）⇒</para>
/// <para>本类照示例建模为 <c>string</c>，不做本地转数字。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAfterSale")]
public class WxaShopComplaintUserRecord
{
    /// <summary>事件编号（官方未发布枚举值）。</summary>
    [JsonPropertyName("event")]
    public long? Event { get; set; }

    /// <summary>事件描述。</summary>
    [JsonPropertyName("event_describe")]
    public string? EventDescribe { get; set; }

    /// <summary>事件材料，见 <see cref="WxaShopComplaintMaterialInfo"/>。</summary>
    [JsonPropertyName("material_info")]
    public WxaShopComplaintMaterialInfo? MaterialInfo { get; set; }

    /// <summary>事件发生时纠纷单状态（官方未发布枚举值）。</summary>
    [JsonPropertyName("state")]
    public long? State { get; set; }

    /// <summary>状态描述。</summary>
    [JsonPropertyName("state_describe")]
    public string? StateDescribe { get; set; }

    /// <summary>事件时间（官方示例为毫秒时间戳字符串）。</summary>
    [JsonPropertyName("time")]
    public string? Time { get; set; }
}
