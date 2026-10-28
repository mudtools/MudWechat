// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Window;

/// <summary>查询团购优惠列表（<c>getlocalproductlist</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsGetLocallifeProductListRequest
{
    /// <summary>获取或设置商品状态（官方 <c>status</c>，选填；不填默认拉全部商品，不包含回收站）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置每页数量（官方 <c>page_size</c>，选填，默认 10，不超过 30）。</summary>
    [JsonPropertyName("page_size")]
    public int? PageSize { get; set; }

    /// <summary>
    /// 获取或设置翻页上下文（官方 <c>next_key</c>，选填；由上次请求返回，传入时会从上次返回的结果往后翻一页，不传默认获取第一页数据）。
    /// </summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}
