// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Media;

/// <summary>
/// 生成异步上传任务请求体（<c>/cgi-bin/media/upload_by_url</c>）。
/// <para>
/// 五个字段均为官方必填；任务完成时官方会向应用回调地址推送
/// <c>upload_media_job_finish</c> 事件（仅携带 JobId，不含具体结果）。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Media")]
public class UploadMediaByUrlRequest
{
    /// <summary>
    /// 获取或设置场景值（官方必填）：1 - 客户联系入群欢迎语素材（目前仅支持 1）。
    /// </summary>
    [JsonPropertyName("scene")]
    public int? Scene { get; set; }

    /// <summary>
    /// 获取或设置媒体文件类型（官方必填）：目前仅支持 video - 视频、file - 普通文件，不超过 32 字节。
    /// <para>视频仅支持 MP4 格式；图片（image）与语音（voice）暂不支持。</para>
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// 获取或设置文件名（官方必填，不超过 128 字节），控制使用该 media_id 发消息时的展示名。
    /// </summary>
    [JsonPropertyName("filename")]
    public string? FileName { get; set; }

    /// <summary>
    /// 获取或设置文件 CDN URL（官方必填，不超过 1024 字节）。
    /// <para>url 要求支持 Range 分块下载；腾讯云 COS 链接需设置为「公有读」权限。</para>
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// 获取或设置文件 md5 值（官方必填，不超过 32 字节），用于校验下载内容一致性（不一致返回 301019）。
    /// </summary>
    [JsonPropertyName("md5")]
    public string? Md5 { get; set; }
}
