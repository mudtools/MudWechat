// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 「申请品牌信息（响应侧）」，<c>get_status</c> 的 <c>application</c>。
/// </summary>
/// <remarks>
/// <para><b>响应侧的 <c>status</c> 与 <c>progress.status</c> 取值集相同</b>（同为
/// <see cref="WxaFamousBrandApplyStatusValues"/>），官方给了两处而非一处 ⇒ 读取时以
/// <c>progress.status</c> 为主、<c>application.status</c> 作交叉校验。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_getfamousbrandapplystatus.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OrderShipping")]
public class WxaFamousBrandApplicationInfo
{
    /// <summary>apply_for（官方类型 <c>number</c>）：申请类型。取值见 <see cref="WxaFamousBrandApplyForValues"/></summary>
    [JsonPropertyName("apply_for")]
    public long? ApplyFor { get; set; }

    /// <summary>audit_info（官方类型 <c>object</c>）：申请品牌信息（响应侧只有审核原因，见 <see cref="WxaFamousBrandAuditResult"/>）</summary>
    [JsonPropertyName("audit_info")]
    public WxaFamousBrandAuditResult? AuditInfo { get; set; }

    /// <summary>status（官方类型 <c>number</c>）：品牌申请状态。取值见 <see cref="WxaFamousBrandApplyStatusValues"/></summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }
}
