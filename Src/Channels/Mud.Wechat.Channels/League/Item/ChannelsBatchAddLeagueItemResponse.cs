// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>批量新增联盟商品（<c>item/batchadd</c>）响应。</summary>
/// <remarks>
/// <b>官方文档矛盾照录</b>：响应参数表字段为 <c>promotion_plan_id</c>（推广计划 id），代码示例写作
/// <c>info_id</c>/<c>ratio</c>——SDK 按参数表建模（官方变更日志 2026-06-17：新增 promotion_plan_id、删除 info_id）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsBatchAddLeagueItemResponse : ChannelsResponse
{
    /// <summary>获取或设置逐商品添加结果（官方 <c>result_info_list</c>）。</summary>
    [JsonPropertyName("result_info_list")]
    public List<ChannelsLeagueItemResultInfo>? ResultInfoList { get; set; }
}
