// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Compass;

/// <summary>带货达人商品佣金数据（官方 <c>data</c> 对象，<c>getshopfinderproductlist</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopFinderProductCommission
{
    /// <summary>获取或设置佣金率（官方 <c>commission_ratio</c>）。</summary>
    [JsonPropertyName("commission_ratio")]
    public double? CommissionRatio { get; set; }

    /// <summary>获取或设置成交金额（官方 <c>pay_gmv</c>，单位：分，参数表类型 number）。</summary>
    [JsonPropertyName("pay_gmv")]
    public long? PayGmv { get; set; }
}
