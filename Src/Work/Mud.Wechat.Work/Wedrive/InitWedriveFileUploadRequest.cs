// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedrive;

/// <summary>
/// 分块上传初始化请求体（<c>/cgi-bin/wedrive/file_upload_init</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：文件按 2M（2097152 字节）固定分块；block_sha 为按分块顺序的文件分块累积 sha1；
/// size 最大支持 20G；spaceid/fatherid 和 selected_ticket 必须填且仅填其中一组参数。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedrive")]
public class InitWedriveFileUploadRequest
{
    /// <summary>获取或设置空间 spaceid（与 selected_ticket 二选一：填 spaceid/fatherid 组时不填 selected_ticket）。</summary>
    [JsonPropertyName("spaceid")]
    public string? Spaceid { get; set; }

    /// <summary>获取或设置当前目录的 fileid（根目录时为空间 spaceid；与 selected_ticket 二选一）。</summary>
    [JsonPropertyName("fatherid")]
    public string? Fatherid { get; set; }

    /// <summary>获取或设置微盘和文件选择器 jsapi 返回的 selectedTicket（填此项则不需要填 spaceid/fatherid）。</summary>
    [JsonPropertyName("selected_ticket")]
    public string? SelectedTicket { get; set; }

    /// <summary>获取或设置文件名字（官方必填）。</summary>
    [JsonPropertyName("file_name")]
    public string? FileName { get; set; }

    /// <summary>获取或设置文件大小（官方必填；最大支持 20G）。</summary>
    [JsonPropertyName("size")]
    public ulong? Size { get; set; }

    /// <summary>获取或设置文件分块累积 sha 值（官方必填；按分块顺序填入数组）。</summary>
    [JsonPropertyName("block_sha")]
    public List<string>? BlockSha { get; set; }

    /// <summary>获取或设置文件创建完成时是否推送企业微信卡片（默认 false，即默认推送卡片）。</summary>
    [JsonPropertyName("skip_push_card")]
    public bool? SkipPushCard { get; set; }
}
