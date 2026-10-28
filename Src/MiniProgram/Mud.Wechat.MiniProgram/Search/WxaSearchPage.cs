// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Search;

/// <summary>搜一搜数据推送的页面信息（<c>pages[]</c>）。</summary>
/// <remarks>字段以官方页面为准；SDK 不做本地校验。</remarks>
[HttpJsonSerializable(SerializerClassName = "Search")]
public class WxaSearchPage
{
    /// <summary>页面路径（<c>path</c>，必填）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>页面参数（<c>query</c>，如 <c>foo=1</c>）。</summary>
    [JsonPropertyName("query")]
    public string? Query { get; set; }

    /// <summary>页面结构化数据列表（<c>data_list</c>），见 <see cref="WxaSearchPageData"/>。</summary>
    [JsonPropertyName("data_list")]
    public List<WxaSearchPageData>? DataList { get; set; }
}
