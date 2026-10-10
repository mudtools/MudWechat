// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.MsgAudit;

/// <summary>
/// 获取会话内容存档开启成员列表响应体（<c>/cgi-bin/msgaudit/get_permit_user_list</c>）。
/// <para>
/// 官方业务限制：开启范围可设为成员、部门、标签；接口会将部门/标签打散为全部成员
/// userid 返回，且仅含实际生效成员——开启范围超出购买人数时，不包含超容后不生效的 userid。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MsgAudit")]
public class GetMsgAuditPermitUserListResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置设置在开启范围内的成员的 userid 列表。
    /// </summary>
    [JsonPropertyName("ids")]
    public List<string>? Ids { get; set; }
}
