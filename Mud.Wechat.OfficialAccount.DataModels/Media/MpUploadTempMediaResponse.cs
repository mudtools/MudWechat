// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Media;

/// <summary>
/// 新增临时素材（<c>media/upload</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：成功响应含 <c>type</c>/<c>media_id</c>/<c>created_at</c> 三字段，
/// 不含 <c>errcode</c> ⇒ 判错契约经基底缺省 0 视为成功。
/// </para>
/// <para>
/// <b>临时素材有效期 3 天</b>：官方「注意事项」原文「媒体文件在微信后台保存时间为 3 天，
/// 3 天后 media_id 失效」⇒ 调用方须在有效期内消费（多媒体消息 ≤ 10 条的使用上限另见官方「注意事项」）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Media")]
public class MpUploadTempMediaResponse : MpResponse
{
    /// <summary>获取或设置媒体文件类型（官方 <c>type</c>：<c>image</c>/<c>voice</c>/<c>video</c>/<c>thumb</c>）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>获取或设置媒体文件上传后获取的唯一标识（官方 <c>media_id</c>，3 天内有效且可复用）。</summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }

    /// <summary>获取或设置媒体文件上传时间戳（官方 <c>created_at</c>，秒级 Unix 时间）。</summary>
    [JsonPropertyName("created_at")]
    public long CreatedAt { get; set; }
}
