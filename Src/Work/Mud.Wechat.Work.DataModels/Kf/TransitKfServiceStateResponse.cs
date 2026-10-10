// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 变更会话状态响应体（<c>/cgi-bin/kf/service_state/trans</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class TransitKfServiceStateResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置事件响应消息 code，可用于调用「发送欢迎语等事件响应消息」接口。
    /// <para>
    /// 会话初次变更为状态 2（待接入池）或 3（由人工接待）时返回回复语 code，
    /// 变更为状态 4（已结束）时返回结束语 code。
    /// </para>
    /// </summary>
    [JsonPropertyName("msg_code")]
    public string? MsgCode { get; set; }
}
