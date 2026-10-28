// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Media;

/// <summary>
/// 异步上传任务执行明细（<see cref="GetUploadMediaByUrlResultResponse.Detail"/>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Media")]
public class MediaUploadJobDetail
{
    /// <summary>
    /// 获取或设置任务执行返回码：任务失败（status = 3）时返回非 0，其余情况返回 0。
    /// <para>常见错误码：830001 - url 非法（确认是否支持 Range 分块下载）、830003 - url 下载数据失败、
    /// 45001 - 文件大小超过限制（应在 5 字节 ~ 200M 范围内）、301019 - 文件 MD5 不匹配。</para>
    /// </summary>
    [JsonPropertyName("errcode")]
    public int? ErrorCode { get; set; }

    /// <summary>
    /// 获取或设置任务执行返回码的文本描述。
    /// </summary>
    [JsonPropertyName("errmsg")]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 获取或设置媒体文件唯一标识（任务完成 status = 2 时返回，三天内有效）。
    /// <para>注意：该 media_id 与普通「上传临时素材」接口的使用场景不通用，
    /// 目前适用于「获取临时素材」（超 20M 须 Range 分块下载）与入群欢迎语素材管理（scene = 1）。</para>
    /// </summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }

    /// <summary>
    /// 获取或设置媒体文件创建时间戳（任务完成 status = 2 时返回；Unix 时间戳，秒）。
    /// </summary>
    [JsonPropertyName("created_at")]
    public long? CreatedAt { get; set; }
}
