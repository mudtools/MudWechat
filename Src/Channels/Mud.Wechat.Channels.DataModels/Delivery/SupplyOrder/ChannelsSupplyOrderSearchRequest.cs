// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>搜索供货单（<c>supplyorder/search</c>）请求体。</summary>
/// <remarks>
/// <para>官方契约：<b>POST</b> + JSON 请求体；<c>search_condition</c> 与 <c>page_info</c> 官方参数表均标必填；
/// <c>search_condition</c> 各字段必须至少设置一个，否则接口报错。</para>
/// <para>
/// <b>官方文档矛盾照录</b>：官方请求示例中出现 <c>tel_last4</c>/<c>user_name</c> 两字段，
/// 但参数表未列出——SDK 按参数表建模，不登记表外字段。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSupplyOrderSearchRequest
{
    /// <summary>获取或设置搜索条件（官方 <c>search_condition</c>，必填，至少设置一个字段）。</summary>
    [JsonPropertyName("search_condition")]
    public ChannelsSupplyOrderSearchCondition SearchCondition { get; set; } = new();

    /// <summary>获取或设置分页参数（官方 <c>page_info</c>，必填，上一页请求返回）。</summary>
    [JsonPropertyName("page_info")]
    public ChannelsDeliveryPageInfo PageInfo { get; set; } = new();
}
