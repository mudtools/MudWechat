// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 「申请品牌信息」（请求侧 <c>Application</c>）。
/// </summary>
/// <remarks>
/// <para><b><c>audit_info</c> 仅在申请类型为「知名品牌（<c>1</c>）」时必填</b>；
/// 申请类型 <c>2</c>「已接入小店商品组件」不需要品牌材料。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_famousbrandapply.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OrderShipping")]
public class WxaFamousBrandApplication
{
    /// <summary>apply_for（官方类型 <c>number</c>，必填）：品牌申请类型枚举值。取值见 <see cref="WxaFamousBrandApplyForValues"/></summary>
    [JsonPropertyName("apply_for")]
    public long? ApplyFor { get; set; }

    /// <summary>audit_info（官方类型 <c>object</c>）：申请品牌信息，当申请类型为知名品牌时必填</summary>
    [JsonPropertyName("audit_info")]
    public WxaFamousBrandAuditInfo? AuditInfo { get; set; }
}
