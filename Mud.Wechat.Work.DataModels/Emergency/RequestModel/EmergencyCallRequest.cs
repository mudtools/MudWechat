// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Emergency;

/// <summary>
/// 发起语音电话请求体（<c>/cgi-bin/pstncc/call</c>，紧急通知域）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Emergency")]
public class EmergencyCallRequest
{
    /// <summary>获取或设置需要呼叫的成员 userid 列表（官方必填且不能为空）。</summary>
    /// <remarks>
    /// <para>
    /// 官方参数名为单数 <c>callee_userid</c>、值为 userid 字符串数组（批量被呼叫），
    /// 照抄官方字段名；区别于获取接听状态端点的单数字符串形态。
    /// </para>
    /// </remarks>
    [JsonPropertyName("callee_userid")]
    public List<string>? CalleeUserid { get; set; }
}
