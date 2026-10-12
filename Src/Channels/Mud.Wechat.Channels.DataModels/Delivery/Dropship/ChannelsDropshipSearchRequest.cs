// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>搜索代发单（<c>dropship/search</c>）请求体。</summary>
/// <remarks>官方契约：<b>POST</b> + JSON 请求体；<c>search_condition</c> 必填；<c>page_info.limit</c> &lt;= 10（本端点专属，严于其他端点的 100）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsDropshipSearchRequest
{
    /// <summary>获取或设置搜索条件（官方 <c>search_condition</c>，必填）。</summary>
    [JsonPropertyName("search_condition")]
    public ChannelsDropshipSearchCondition SearchCondition { get; set; } = new();

    /// <summary>获取或设置分页参数（官方 <c>page_info</c>，选填，上一页请求返回）。</summary>
    [JsonPropertyName("page_info")]
    public ChannelsDeliveryPageInfo? PageInfo { get; set; }
}
