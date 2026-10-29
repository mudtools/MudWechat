// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 查询订单列表应答（<c>/shop/order/get_list</c>）。
/// </summary>

[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopOrderGetListResponse : WxaShopOrderListResponse
{
    /// <summary>官方<b>回包示例</b>使用 <c>data</c> 承载订单数组，与返回参数表的 <c>orders[]</c> 不一致 ⇒ 继承 <see cref="WxaShopOrderListResponse.Orders"/> 之外额外保留本键。</summary>
    [JsonPropertyName("data")]
    public List<WxaShopOrder>? Data { get; set; }
}
