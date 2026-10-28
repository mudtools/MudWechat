// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 微信小店回调载荷 DTO —— 平台客服工单族（工单创建 / 工单超时）。
// 事件键：ec_platform_kf_create_mch_ticket、ec_platform_kf_ticket_timeout。
// 注意：两事件官方参数表仅列 request_data 一行，信封公共字段（ToUserName 等）仅见于官方示例
// —— 按公共信封 + request_data 建模；request_data 字段集相同（仅 ticket_id），共用嵌套类。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Callback。

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>平台客服工单请求体（官方 <c>request_data</c>，创建/超时两事件字段集相同，共用本类）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsPlatformKfTicketInfo
{
    /// <summary>获取或设置工单 id（官方 <c>ticket_id</c>）。</summary>
    [JsonPropertyName("ticket_id")]
    public string? TicketId { get; set; }
}
