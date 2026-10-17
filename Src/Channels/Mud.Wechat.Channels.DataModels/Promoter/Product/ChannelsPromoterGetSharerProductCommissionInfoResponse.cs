// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>获取推客的某个商品的推广分佣比例（<c>promoter/get_sharer_product_commission_info</c>）响应。</summary>
/// <remarks>官方响应示例的 <c>commission_ratio</c> 为 10，与参数表的 100000-900000 量纲不符——按参数表量纲建模，示例照录。</remarks>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterGetSharerProductCommissionInfoResponse : ChannelsResponse
{
    /// <summary>获取或设置分佣比例（官方 <c>commission_ratio</c>，范围 100000-900000，代表 10%-90%）。</summary>
    [JsonPropertyName("commission_ratio")]
    public long? CommissionRatio { get; set; }

    /// <summary>获取或设置是否设置了这个商品的佣金率（官方 <c>is_set</c>）。</summary>
    [JsonPropertyName("is_set")]
    public bool? IsSet { get; set; }
}
