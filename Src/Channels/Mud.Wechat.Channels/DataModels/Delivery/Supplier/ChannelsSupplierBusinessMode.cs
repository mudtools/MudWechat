// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>
/// 供货商可用的经营模式（官方 <c>business_mode</c>，<c>get_supplier_base_info</c> 应答子对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSupplierBusinessMode
{
    /// <summary>获取或设置是否可用代发模式（官方 <c>is_dropshipping</c>；个人主体 / 企业主体均可使用）。</summary>
    [JsonPropertyName("is_dropshipping")]
    public bool? IsDropshipping { get; set; }

    /// <summary>获取或设置是否可用供货模式（官方 <c>is_supply</c>；仅企业主体可用）。</summary>
    [JsonPropertyName("is_supply")]
    public bool? IsSupply { get; set; }
}
