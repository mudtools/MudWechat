// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedrive;

/// <summary>
/// 获取文件列表响应体（<c>/cgi-bin/wedrive/file_list</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedrive")]
public class GetWedriveFileListResponse : WechatWorkResponse
{
    /// <summary>获取或设置列表是否还有内容（true 为需要继续分批拉取）。</summary>
    [JsonPropertyName("has_more")]
    public bool? HasMore { get; set; }

    /// <summary>获取或设置下次分批拉取对应的请求参数 start 值。</summary>
    [JsonPropertyName("next_start")]
    public ulong? NextStart { get; set; }

    /// <summary>获取或设置文件信息列表（对象内含 item 数组，详见 <see cref="WedriveFileList"/>）。</summary>
    [JsonPropertyName("file_list")]
    public WedriveFileList? FileList { get; set; }
}
