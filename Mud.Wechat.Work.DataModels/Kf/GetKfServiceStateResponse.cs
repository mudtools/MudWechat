// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 获取会话状态响应体（<c>/cgi-bin/kf/service_state/get</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class GetKfServiceStateResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置当前会话状态（0 - 未处理，1 - 由智能助手接待，2 - 待接入池排队中，
    /// 3 - 由人工接待，4 - 已结束/未开始）。
    /// </summary>
    [JsonPropertyName("service_state")]
    public int? ServiceState { get; set; }

    /// <summary>
    /// 获取或设置接待人员 userid（仅当会话状态为 3 时有效；第三方 / 代开发应用获取到的是密文 userid 即 open_userid）。
    /// </summary>
    [JsonPropertyName("servicer_userid")]
    public string? ServicerUserId { get; set; }
}
