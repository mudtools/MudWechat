// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.MsgAudit;

/// <summary>
/// 获取会话内容存档开启成员列表请求体（<c>/cgi-bin/msgaudit/get_permit_user_list</c>）。
/// <para>
/// 官方业务限制：access_token 必须由「会话内容存档」应用 secret 获取；
/// 调用频率不可超过 1000 次/分钟。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MsgAudit")]
public class GetMsgAuditPermitUserListRequest
{
    /// <summary>
    /// 获取或设置拉取对应版本的开启成员列表（官方可选）：1 - 办公版、2 - 服务版、3 - 企业版；
    /// 不填返回全量成员列表。
    /// </summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }
}
