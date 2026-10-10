// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedrive;

/// <summary>
/// 上传文件请求体（<c>/cgi-bin/wedrive/file_upload</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：spaceid/fatherid 和 selected_ticket 必须填且仅填其中一组参数；
/// file_base64_content 文件大小上限 10M，超过请改用文件分块上传接口。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedrive")]
public class UploadWedriveFileRequest
{
    /// <summary>获取或设置空间 spaceid（与 selected_ticket 二选一：填 spaceid/fatherid 组时不填 selected_ticket）。</summary>
    [JsonPropertyName("spaceid")]
    public string? Spaceid { get; set; }

    /// <summary>获取或设置父目录 fileid（根目录时为空间 spaceid；与 selected_ticket 二选一）。</summary>
    [JsonPropertyName("fatherid")]
    public string? Fatherid { get; set; }

    /// <summary>获取或设置微盘和文件选择器 jsapi 返回的 selectedTicket（填此项则不需要填 spaceid/fatherid）。</summary>
    [JsonPropertyName("selected_ticket")]
    public string? SelectedTicket { get; set; }

    /// <summary>获取或设置文件名字（官方必填；最多 255 个字符，英文算 1 个，汉字算 2 个）。</summary>
    [JsonPropertyName("file_name")]
    public string? FileName { get; set; }

    /// <summary>
    /// 获取或设置文件内容 Base64（官方必填；只填文件内容的 Base64，不添加
    /// 如 <c>data:application/x-javascript;base64</c> 的数据类型描述信息；文件大小上限 10M）。
    /// </summary>
    [JsonPropertyName("file_base64_content")]
    public string? FileBase64Content { get; set; }
}
