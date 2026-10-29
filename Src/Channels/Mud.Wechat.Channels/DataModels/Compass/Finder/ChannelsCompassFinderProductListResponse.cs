// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Compass;

/// <summary>获取带货商品列表（<c>getfinderproductlist</c>，罗盘达人版）响应。</summary>
/// <remarks>
/// <b>官方文档矛盾照录</b>：<c>product_list</c> 参数表标注为 objarray，官方返回示例为单对象——按参数表（列表）建模；
/// 元素与 <c>getfinderproductdata</c> 的 <c>product_info</c> 字段集一致 ⇒ 共用 <see cref="ChannelsCompassFinderProductInfo"/>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassFinderProductListResponse : ChannelsResponse
{
    /// <summary>获取或设置带货商品列表（官方 <c>product_list</c>）。</summary>
    [JsonPropertyName("product_list")]
    public List<ChannelsCompassFinderProductInfo>? ProductList { get; set; }
}
