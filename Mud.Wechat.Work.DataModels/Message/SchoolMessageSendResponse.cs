// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 发送「学校通知」响应体（<c>/cgi-bin/externalcontact/message/send</c>，官方文档 91609）。
/// <para>如果部分接收人无权限或不存在，发送仍然执行，但会返回无效的部分。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class SchoolMessageSendResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置无效/无权限的家长 userid 列表。
    /// </summary>
    [JsonPropertyName("invalid_parent_userid")]
    public List<string>? InvalidParentUserId { get; set; }

    /// <summary>
    /// 获取或设置无效/无权限的学生 userid 列表。
    /// </summary>
    [JsonPropertyName("invalid_student_userid")]
    public List<string>? InvalidStudentUserId { get; set; }

    /// <summary>
    /// 获取或设置无效/无权限的部门列表。
    /// </summary>
    [JsonPropertyName("invalid_party")]
    public List<string>? InvalidParty { get; set; }
}
