// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 发送「学校通知」请求基类（<c>/cgi-bin/externalcontact/message/send</c>，官方文档 91609）。
/// <para>学校通过此接口给家长/学生发送学校通知；学校管理员需要将应用配置在「家长可使用的应用」才可调用。</para>
/// <para>id 转译仅文本、图文、mpnews、小程序四类消息的部分字段支持；本类仅承载信封公共字段，由各消息类型请求子类继承。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class SchoolMessageSendRequest
{
    /// <summary>
    /// 获取或设置指定发送对象：0 - 发送给家长（默认），1 - 发送给学生，2 - 发送给家长和学生。
    /// </summary>
    [JsonPropertyName("recv_scope")]
    public int? RecvScope { get; set; }

    /// <summary>
    /// 获取或设置家校通讯录家长列表（最多支持 1000 个）：recv_scope 为 0 或 2 时表示发送给对应的家长，
    /// recv_scope 为 1 时忽略本字段。
    /// </summary>
    [JsonPropertyName("to_parent_userid")]
    public List<string>? ToParentUserId { get; set; }

    /// <summary>
    /// 获取或设置家校通讯录学生列表（最多支持 1000 个）：recv_scope 为 0 时表示发送给学生的所有家长，
    /// recv_scope 为 1 时表示发送给学生，recv_scope 为 2 时表示发送给学生和学生的所有家长。
    /// </summary>
    [JsonPropertyName("to_student_userid")]
    public List<string>? ToStudentUserId { get; set; }

    /// <summary>
    /// 获取或设置家校通讯录部门列表（最多支持 100 个）：recv_scope 为 0 时表示发送给班级的所有家长，
    /// recv_scope 为 1 时表示发送给班级的所有学生，recv_scope 为 2 时表示发送给班级的所有学生和家长。
    /// </summary>
    [JsonPropertyName("to_party")]
    public List<string>? ToParty { get; set; }

    /// <summary>
    /// 获取或设置是否向全校发送：1 表示本字段生效，0 表示本字段无效（默认）。
    /// recv_scope 为 0 时表示发送给学校的所有家长，recv_scope 为 1 时表示发送给学校的所有学生，
    /// recv_scope 为 2 时表示发送给学校的所有学生和家长。
    /// </summary>
    [JsonPropertyName("toall")]
    public int? ToAll { get; set; }

    /// <summary>
    /// 获取或设置企业应用的 id，整型（官方必填）。
    /// </summary>
    [JsonPropertyName("agentid")]
    public int? AgentId { get; set; }

    /// <summary>
    /// 获取或设置是否开启 id 转译：0 - 否（默认），1 - 是（图片、语音、视频、文件消息不支持）。
    /// </summary>
    [JsonPropertyName("enable_id_trans")]
    public int? EnableIdTrans { get; set; }

    /// <summary>
    /// 获取或设置是否开启重复消息检查：0 - 否（默认），1 - 是。
    /// </summary>
    [JsonPropertyName("enable_duplicate_check")]
    public int? EnableDuplicateCheck { get; set; }

    /// <summary>
    /// 获取或设置重复消息检查的时间间隔，默认 1800s，最大不超过 4 小时。
    /// </summary>
    [JsonPropertyName("duplicate_check_interval")]
    public int? DuplicateCheckInterval { get; set; }
}
