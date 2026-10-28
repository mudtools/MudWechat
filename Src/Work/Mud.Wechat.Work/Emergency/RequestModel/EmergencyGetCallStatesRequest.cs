// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Emergency;

/// <summary>
/// 获取接听状态请求体（<c>/cgi-bin/pstncc/getstates</c>，紧急通知域）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Emergency")]
public class EmergencyGetCallStatesRequest
{
    /// <summary>获取或设置被呼叫人 userid（官方必填且不能为空）。</summary>
    /// <remarks>
    /// <para>
    /// 本端点的 <c>callee_userid</c> 为单数字符串（单次查询指定单人），
    /// 区别于发起语音电话端点的字符串数组形态。
    /// </para>
    /// </remarks>
    [JsonPropertyName("callee_userid")]
    public string? CalleeUserid { get; set; }

    /// <summary>获取或设置发起自动语音来电返回的 callid（官方必填且不能为空；仅支持查询七天内的 callid 状态）。</summary>
    [JsonPropertyName("callid")]
    public string? Callid { get; set; }
}
