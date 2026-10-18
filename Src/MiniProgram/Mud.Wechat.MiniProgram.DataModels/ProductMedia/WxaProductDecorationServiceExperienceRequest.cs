// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductMedia;

/// <summary>
/// 装修服务体验版请求体（<c>POST /product/decoration/decoration_service_experience</c>）。
/// </summary>
/// <remarks>
/// <para>官方说明：默认小商店所有相关人员均可打开体验版小商店；体验版没有缓存，
/// 不会影响线上版本（官方原文）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductMedia")]
public class WxaProductDecorationServiceExperienceRequest
{
    /// <summary>服务 id（必填）。</summary>
    [JsonPropertyName("service_id")]
    public long? ServiceId { get; set; }
}
