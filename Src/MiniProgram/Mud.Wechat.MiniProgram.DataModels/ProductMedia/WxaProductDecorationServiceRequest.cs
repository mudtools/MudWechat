// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductMedia;

/// <summary>
/// 装修服务启停请求体（<c>POST /product/decoration/decoration_service</c>）。
/// </summary>
/// <remarks>
/// <para><b>两段式生效</b>：调用后微信会下发模板消息给小商店管理员，管理员确认后方可发布
/// （官方原文）⇒ 返回成功不代表已生效。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductMedia")]
public class WxaProductDecorationServiceRequest
{
    /// <summary>1 启用 / 2 停用，见 <see cref="WxaProductDecorationSwitchStatuses"/>。</summary>
    [JsonPropertyName("switch_status")]
    public int? SwitchStatus { get; set; }

    /// <summary>服务 id（必填）。</summary>
    [JsonPropertyName("service_id")]
    public long? ServiceId { get; set; }
}
