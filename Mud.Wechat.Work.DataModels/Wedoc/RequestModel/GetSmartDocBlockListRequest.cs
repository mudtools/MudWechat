// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 获取内容块列表请求体（<c>/cgi-bin/wedoc/smartdoc/get_block_list</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class GetSmartDocBlockListRequest
{
    /// <summary>获取或设置智能文档的文档 ID（官方 <c>docid</c>，必填）。</summary>
    [JsonPropertyName("docid")]
    public string? Docid { get; set; }

    /// <summary>获取或设置页面 ID（官方 <c>page_id</c>，必填）。</summary>
    [JsonPropertyName("page_id")]
    public string? PageId { get; set; }

    /// <summary>获取或设置需要获取的 Block ID 列表（官方 <c>ids</c>，非必填）；传入时仅获取指定 Block。</summary>
    [JsonPropertyName("ids")]
    public List<string>? Ids { get; set; }

    /// <summary>获取或设置分批起始点（官方 <c>start</c>，非必填，最小值 <c>0</c>）。</summary>
    [JsonPropertyName("start")]
    public int? Start { get; set; }

    /// <summary>
    /// 获取或设置分批大小（官方 <c>limit</c>，非必填）。
    /// <para>官方业务限制：最大值为 <c>200</c>，默认值为 <c>200</c>。</para>
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}
