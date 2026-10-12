// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Media;

/// <summary>
/// 上传永久素材（<c>material/add_material</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<c>media_id</c> 恒返回；<c>url</c> <b>仅图片素材返回</b>——
/// 官方「注意事项」原文「永久图片素材新增后，将带有 URL 返回给开发者，开发者可以在腾讯系域名内使用
/// （腾讯系域名外使用，图片将被屏蔽）」。视频示例中 <c>url</c> 为空串 ⇒ 非图片类型不得以
/// <c>url</c> 有值作任何判断。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Media")]
public class MpAddMaterialResponse : MpResponse
{
    /// <summary>获取或设置新增的永久素材 media_id（官方 <c>media_id</c>）。</summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }

    /// <summary>获取或设置图片素材 URL（官方 <c>url</c>，<b>仅图片素材返回</b>；腾讯系域名内可用）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
