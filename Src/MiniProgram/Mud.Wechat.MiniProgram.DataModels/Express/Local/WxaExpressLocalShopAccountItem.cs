// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>已绑定的商家签约账号项（<c>shop_list</c> 元素，<c>shop/get</c> 响应）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalShopAccountItem
{
    /// <summary>商家 ID（<c>shopid</c>）。</summary>
    [JsonPropertyName("shopid")]
    public string? ShopId { get; set; }

    /// <summary>配送公司 ID（<c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>审核状态（<c>audit_result</c>；<c>0</c> 审核通过，<c>1</c> 审核中，<c>2</c> 审核不通过）。</summary>
    [JsonPropertyName("audit_result")]
    public int AuditResult { get; set; }
}
