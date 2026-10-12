// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.HomePage;

/// <summary>分类关联商品查询条件（官方 <c>req</c> 对象，<c>getclassificationproductlist</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsClassificationProductQuery
{
    /// <summary>获取或设置一级分类 ID（官方 <c>level_1_id</c>，必填；经「获取在店铺主页展示的商品分类」获取）。</summary>
    [JsonPropertyName("level_1_id")]
    public long Level1Id { get; set; }

    /// <summary>获取或设置二级分类 ID（官方 <c>level_2_id</c>，必填；经同一分类树接口获取）。</summary>
    [JsonPropertyName("level_2_id")]
    public long Level2Id { get; set; }

    /// <summary>获取或设置翻页上下文（官方 <c>page_context</c>，必填；由上次请求返回，首页传空串）。</summary>
    [JsonPropertyName("page_context")]
    public string PageContext { get; set; } = string.Empty;

    /// <summary>获取或设置每页数量（官方 <c>page_size</c>，必填）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }
}
