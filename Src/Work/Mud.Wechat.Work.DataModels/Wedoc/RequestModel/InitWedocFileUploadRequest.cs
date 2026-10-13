// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 文件分块上传初始化请求体（<c>/cgi-bin/wedoc/file_upload_init</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：分块上传三端点（init / part / finish）由官方同一个文档页承载；
/// <c>block_sha</c> 为按分块顺序排列的文件分块 sha 值数组，与 <c>size</c>、<c>file_name</c> 一同官方必填。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class InitWedocFileUploadRequest
{
    /// <summary>获取或设置文档 docid（官方必填；文件上传至该文档，官方口径「通过新建文档接口创建后获得」）。</summary>
    [JsonPropertyName("docid")]
    public string Docid { get; set; } = string.Empty;

    /// <summary>获取或设置文件名字（官方必填）。</summary>
    [JsonPropertyName("file_name")]
    public string FileName { get; set; } = string.Empty;

    /// <summary>获取或设置文件大小（单位：字节，官方必填；官方标注<b>最大支持 20G</b>）。</summary>
    [JsonPropertyName("size")]
    public ulong Size { get; set; }

    /// <summary>获取或设置文件分块<b>累积</b> sha 值列表（官方必填；分块大小固定为 2M，按分块顺序填入数组，
    /// 每块取「依次 sha1_update 后的当前 state 十六进制」，最后一块取 sha1_final 的整文件 sha1，算法见接口文档附录）。</summary>
    [JsonPropertyName("block_sha")]
    public List<string> BlockSha { get; set; } = new List<string>();
}
