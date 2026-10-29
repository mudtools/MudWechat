// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>
/// 供货商视角搜索代发单（<c>dropship/supplier/search</c>）请求。
/// </summary>
/// <remarks>
/// 官方要求 <c>search_condition</c> 与 <c>page_info</c> 均必填，且 <c>search_condition</c> 至少设置一个字段，否则接口报错；
/// SDK 不做本地字段数拦截（由官方错误码表达）。响应与商家侧同形，复用 <see cref="ChannelsDropshipOrderListResponse"/>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSupplierDropshipSearchRequest
{
    /// <summary>获取或设置搜索条件（官方 <c>search_condition</c>，必填且至少设置一个字段）。</summary>
    [JsonPropertyName("search_condition")]
    public ChannelsSupplierDropshipSearchCondition SearchCondition { get; set; } = new();

    /// <summary>获取或设置分页参数（官方 <c>page_info</c>，必填；<c>limit</c> &lt;= 100）。</summary>
    [JsonPropertyName("page_info")]
    public ChannelsDeliveryPageInfo? PageInfo { get; set; }
}
