// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 「上传提审素材」（<c>uploadmedia</c>）应答。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/uploadMediaToCodeAudit.html"/>。
/// </summary>
/// <remarks>
/// 官方 <c>mediaid</c> 一栏标注类型为 <c>buffer</c>，实际应答为字符串（官方示例 <c>"xxxxxxxxxxxxxxxxx"</c>）⇒ 照形态取 <c>string</c>。
/// 官方原文：返回的 <c>mediaid</c> <b>有效期是三天</b>，过期需要重新上传。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpUploadMediaResponse : OpenPlatformResponse
{
    /// <summary>素材类型（官方 <c>type</c>，string：<c>image</c> / <c>video</c>）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>素材标识（官方 <c>mediaid</c>，<b>无下划线</b>；填入提审接口 <c>preview_info</c>）。</summary>
    [JsonPropertyName("mediaid")]
    public string? MediaId { get; set; }
}
