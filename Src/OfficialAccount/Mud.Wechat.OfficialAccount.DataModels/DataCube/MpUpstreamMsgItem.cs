// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.DataCube;

/// <summary>
/// 消息发送概况数据条目（官方 <c>getupstreammsg</c> 的 list 元素；week/month/hour 三端点响应<b>完全同构</b>
/// ⇒ 共用本 DTO——分时端点的 ref_hour 仅出现在官方示例、字段表未列（官方文档缺口，照录不建模））。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpUpstreamMsgItem
{
    /// <summary>获取或设置数据的日期（官方 <c>ref_date</c>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>获取或设置消息类型（官方 <c>msg_type</c>：1 文字 / 2 图片 / 3 语音 / 4 视频 / 6 第三方应用消息（链接消息））。</summary>
    [JsonPropertyName("msg_type")]
    public int? MsgType { get; set; }

    /// <summary>获取或设置上行发送消息的用户数（官方 <c>msg_user</c>）。</summary>
    [JsonPropertyName("msg_user")]
    public int? MsgUser { get; set; }

    /// <summary>获取或设置上行发送消息的消息总数（官方 <c>msg_count</c>）。</summary>
    [JsonPropertyName("msg_count")]
    public int? MsgCount { get; set; }
}
