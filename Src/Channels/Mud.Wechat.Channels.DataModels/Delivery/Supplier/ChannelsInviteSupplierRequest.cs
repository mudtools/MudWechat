// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>申请关联供货商（<c>invite_supplier</c>）请求体。</summary>
/// <remarks>
/// 官方契约：<b>POST</b> + JSON 请求体；<c>service_id</c> 在第三方平台代商家调用时<b>必填</b>
/// （平台将校验该 ServiceID 与实际调用方的一致性）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsInviteSupplierRequest
{
    /// <summary>获取或设置申请关联的供货商的 appid（官方 <c>supplier_appid</c>，必填）。</summary>
    [JsonPropertyName("supplier_appid")]
    public string SupplierAppid { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置第三方服务商 ServiceID（官方 <c>service_id</c>，选填）——
    /// 第三方平台代商家调用时必填，且平台将校验该 ServiceID 与实际调用方的一致性。
    /// </summary>
    [JsonPropertyName("service_id")]
    public long? ServiceId { get; set; }

    /// <summary>
    /// 获取或设置第三方服务商备注信息（官方 <c>remark</c>，选填）——应清晰说明具体的调用场景
    /// （官方建议示例：「由店铺成员 xxx 手动发起确认」「XX 货源平台『一件代发』供货商 xxx」）。
    /// </summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }
}
