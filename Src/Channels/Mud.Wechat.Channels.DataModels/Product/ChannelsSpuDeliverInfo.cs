// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>
/// SPU 维度预售规则（官方 <c>spu_deliver_info</c> 对象：spu 维度配置全部 sku 预售规则）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsSpuDeliverInfo
{
    /// <summary>获取或设置是否生效（官方 <c>is_spu_range</c>：0 不生效，1 生效）。</summary>
    [JsonPropertyName("is_spu_range")]
    public int? IsSpuRange { get; set; }

    /// <summary>获取或设置 spu 维度下的 sku 预售配置（官方 <c>sku_deliver_info</c>）。</summary>
    [JsonPropertyName("sku_deliver_info")]
    public ChannelsSkuDeliverInfo? SkuDeliverInfo { get; set; }
}
