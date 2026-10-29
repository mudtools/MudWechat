// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopAfterSale;

/// <summary>
/// 获取纠纷单请求体（<c>POST /shop/complaint/get</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方表标「选填」</b>：<c>complaint_order_id</c> 是唯一请求字段且表中「是否必填」列为「否」，</para>
/// <para>与请求示例必带该键矛盾 ⇒ 照表建模为可空、不擅自收紧。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAfterSale")]
public class WxaShopComplaintGetRequest
{
    /// <summary>纠纷单号（官方表标选填）。</summary>
    [JsonPropertyName("complaint_order_id")]
    public long? ComplaintOrderId { get; set; }
}
