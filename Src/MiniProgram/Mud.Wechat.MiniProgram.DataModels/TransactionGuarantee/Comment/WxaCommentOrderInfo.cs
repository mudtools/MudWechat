// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>
/// 评价关联的商家订单信息（<c>orderInfo</c>，列表侧 <c>commentList[].orderInfo</c> 与详情侧
/// <c>info.content.orderInfo</c> 参数表<b>完全一致</b> ⇒ 共用本类型）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaCommentOrderInfo
{
    /// <summary>
    /// 商户单号（<c>busiOrderId</c>，即商户系统内部订单号，只能是数字、大小写字母 <c>_-*</c>
    /// 且同一商户号下唯一；对应微信支付 APIv3 的 <c>out_trade_no</c>）。
    /// </summary>
    [JsonPropertyName("busiOrderId")]
    public string? BusiOrderId { get; set; }
}
