// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Search;

/// <summary>搜一搜数据推送请求体（<c>POST /wxa/search/wxaapi_submitpages</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>wxsearch/api_submitpages.html</c>。</para>
/// <para>
/// 将小程序内优质页面的<b>路径、参数与结构化数据</b>推送给微信搜索，供搜索结果展示；<c>pages</c> 为
/// 页面信息列表（必填，非空），每项含 <c>path</c>（页面路径）与 <c>query</c>（页面参数），
/// 结构化数据经 <c>data_list</c> 逐条表达（<c>@type</c> 为数据结构类型）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Search")]
public class WxaSearchSubmitPagesRequest
{
    /// <summary>小程序页面信息列表（<c>pages</c>，必填），见 <see cref="WxaSearchPage"/>。</summary>
    [JsonPropertyName("pages")]
    public List<WxaSearchPage>? Pages { get; set; }
}
