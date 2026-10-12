// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.DataCube;

/// <summary>
/// 消息发送分布数据条目（官方 <c>getupstreammsgdist</c> 的 list 元素；week/month 两端点响应完全同构 ⇒ 共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpUpstreamMsgDistItem
{
    /// <summary>获取或设置数据的日期（官方 <c>ref_date</c>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>获取或设置当日发送消息量分布区间（官方 <c>count_interval</c>：0 代表 "0"、1 代表 "1-5"、2 代表 "6-10"、3 代表 "10 次以上"）。</summary>
    [JsonPropertyName("count_interval")]
    public int? CountInterval { get; set; }

    /// <summary>获取或设置上行发送消息的用户数（官方 <c>msg_user</c>）。</summary>
    [JsonPropertyName("msg_user")]
    public int? MsgUser { get; set; }
}
