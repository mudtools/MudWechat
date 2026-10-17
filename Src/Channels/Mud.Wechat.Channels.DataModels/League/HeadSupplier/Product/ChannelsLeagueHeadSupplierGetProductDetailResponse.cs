// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>
/// 机构侧商品基础详情查询响应（官方 <c>headsupplier/productdetail/get</c> 返回体）。
/// </summary>
/// <remarks>
/// 官方返回参数表的根字段为 <c>item</c>，而同页「代码示例」的返回体根字段写成 <c>product</c> ⇒ 二者不一致，
/// 本仓<b>以参数表为权威</b>建模为 <c>item</c>（示例差异在守卫 CLG5 留注，不改官方字段名）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierGetProductDetailResponse : ChannelsResponse
{
    /// <summary>获取或设置商品详情（官方 <c>item</c>）。</summary>
    [JsonPropertyName("item")]
    public ChannelsLeagueHeadSupplierProductDetailItem? Item { get; set; }
}
