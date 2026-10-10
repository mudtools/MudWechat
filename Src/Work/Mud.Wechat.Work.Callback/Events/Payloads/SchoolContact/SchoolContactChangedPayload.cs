// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work.Abstractions.Callback.Payloads;

namespace Mud.Wechat.Work.Callback.Events.Payloads;

/// <summary>
/// 家校通讯录变更事件载荷（<b>结构族</b>：官方事件键 <c>change_school_contact</c>；
/// 官方 92032 成员变更 / 92052 部门变更（企业自建）· 92051 成员变更 / 92050 部门变更（第三方，套件信封）·
/// 96716 成员变更 / 96717 部门变更（服务商代开发））。
/// </summary>
/// <remarks>
/// <para>
/// <b>族事件值即事件键</b>：成员事件的 <c>ChangeType</c> <c>subscribe</c>/<c>unsubscribe</c> 与
/// 消息与事件族（官方 90240）的事件键同名（成员关注应用 vs 家长关注家校通知），逐 <c>ChangeType</c> 键
/// 无法消歧 ⇒ 事件键为族事件值，具体类别由信封 <c>ChangeType</c> 判别（ADR-1 结构族模型，同
/// <c>ExternalContactChangedPayload</c>）。<c>ChangeType</c> 闭合值域 11 类：
/// 成员 <c>create_student</c>/<c>update_student</c>/<c>delete_student</c>/<c>create_parent</c>/
/// <c>update_parent</c>/<c>delete_parent</c>、关注 <c>subscribe</c>/<c>unsubscribe</c>、
/// 部门 <c>create_department</c>/<c>update_department</c>/<c>delete_department</c>。
/// </para>
/// <para>
/// <b>双信封同键</b>：企业自建/代开发的应用数据回调为 <c>Event</c> 信封；第三方应用的指令回调 URL
/// 为套件信封（<c>SuiteId</c>/<c>AuthCorpId</c>/<c>InfoType</c>/<c>TimeStamp</c>，官方 92050/92051）。
/// 两种信封产出同一事件键 ⇒ 一份载荷覆盖三模式（外层 <c>SuiteId</c>/<c>AuthCorpId</c> 由信封承载，不入载荷）。
/// </para>
/// <para>
/// <b>权限分层（处理器不得假设必有值）</b>：自建应用须配置到「家校沟通-可调用接口的应用」或
/// 「家校沟通-配置-家长可使用的应用」并开启接收事件开关；代开发应用须「家校沟通→家校通讯录读取权限」；
/// 第三方应用须「家校沟通」使用权限（读取家长信息不含电话 + 给家长发通知）或「使用和编辑」权限
/// （编辑家校通讯录 + 给家长发通知）。
/// </para>
/// <para>
/// <b>官方文档拼写不一致（以实际推送为准）</b>：部门变更事件官方参数说明表写
/// <c>create_department</c>/<c>update_department</c>/<c>delete_department</c>，而官方 92052/96717 的
/// XML/JSON 示例拼写为 <c>create_deparmtment</c> 等（m/t 换位）；第三方 92050 与批量事件 97281 的示例
/// 均为正确拼写。处理器按 <c>evt.ChangeType</c> 判别部门类别时建议对两种拼写做实测确认后取舍。
/// </para>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/92032">path 92032 成员变更事件（企业自建）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/92052">path 92052 部门变更事件（企业自建）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/92051">path 92051 成员变更事件（第三方，套件信封）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/92050">path 92050 部门变更事件（第三方，套件信封）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/96716">path 96716 成员变更事件（服务商代开发）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/96717">path 96717 部门变更事件（服务商代开发）</see>。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredEvent = WechatCallbackEventTypes.ChangeSchoolContact,
    RequiredFamily = WechatCallbackEventFamily.SchoolContactChange,
    SupportedAppTypes = WechatAppTypeSet.Internal | WechatAppTypeSet.Provider,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] { WechatCallbackEventTypes.ChangeSchoolContact })]
[WechatCallbackContract(
    RequiredEvent = WechatCallbackEventTypes.ChangeSchoolContact,
    RequiredFamily = WechatCallbackEventFamily.SchoolContactChange,
    SupportedAppTypes = WechatAppTypeSet.ThirdParty,
    RequiredChannel = WechatCallbackChannel.Suite,
    EventTypes = new[] { WechatCallbackEventTypes.ChangeSchoolContact })]
public sealed partial class SchoolContactChangedPayload : WechatCallbackPayload
{
    /// <summary>
    /// 变更对象 id（官方 <c>Id</c>）：学生/家长的家校通讯录 userid（成员与关注事件命中），
    /// 或家校通讯录部门 id（部门事件命中，值形如 <c>1</c>）。
    /// </summary>
    [PayloadField("Id")]
    public string? Id { get; set; }

    /// <summary>
    /// 变更后的新 id（官方 <c>NewId</c>）：<b>仅第三方套件信封的 <c>update_student</c>/<c>update_parent</c>
    /// 事件、且 userid 被修改时携带</b>（官方 92051「只在 userid 被修改时回调」）；
    /// 企业自建/代开发报文与其它 <c>ChangeType</c> 均无该节点 ⇒ <see langword="null"/>。
    /// </summary>
    [PayloadField("NewId")]
    public string? NewId { get; set; }
}
