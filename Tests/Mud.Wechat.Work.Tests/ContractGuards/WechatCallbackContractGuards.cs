// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.RegularExpressions;
using Mud.HttpUtils.Attributes;
using Mud.HttpUtils.Payloads;
using Mud.Wechat.Work.Abstractions.Callback;
using Mud.Wechat.Work.Abstractions.Callback.Payloads;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Callback;
using Mud.Wechat.Work.Callback.Events.Payloads;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 回调域契约守卫（CB1~CB13，对齐《回调解决方案 v1》§7）：包依赖边界、官方事件键覆盖、
/// 内置授权族兜底处理器、事件 DTO 官方字段、回调凭据唯一来源、echo/被动应答协议、信封无 XML 依赖、
/// 加解密 32 块填充互操作（CB8）、指纹闸次序（CB9），以及「应用类型 × 回调通道」区分
/// （CB10~CB13：通道枚举 + 配置面、receiveid 三元分流、开放面合法性矩阵、分发器闸次序）。
/// </summary>
/// <remarks>
/// <b>v2.2 变更</b>：旧「逐事件 DTO」已收敛为 5 个<b>结构族载荷</b>（ADR-1），
/// 故 CB4 改写为按结构族断言，并新增 CB4b（官方 41 键全覆盖）、CB4c（事件键级开放面显式声明）、
/// CB4d（三模式无关性）。「元素名 ↔ 属性名」配对正确性改由上游生成器在编译期校验
/// （<c>PAYLOAD004/006/007</c>），逐字段取值由 <c>WechatCallbackPayloadReaderTests</c> 覆盖。
/// </remarks>
public class WechatCallbackContractGuards
{
    /// <summary>
    /// 官方事件键全集（授权 InfoType 6 + 通讯录 ChangeType 7 + 异步 Event 1 + 上下游 Event 1 + ChangeType 9
    /// + 安全管理 ChangeType 1（官方 100080）+ 微信客服 Event 2（官方 94670/97712 等三模式文档）
    /// + 客户联系/获客助手族事件值 5 + 消息与事件 path 90240 的 24 个 Event 键
    /// + 应用版本付费订单回调 InfoType 6 + 接口调用许可事件 4（官方 97195~97198，仅代开发）
    /// + 邮箱族事件值 2 + 文档族 ChangeType 5 + 智能表格族 ChangeType 6
    /// + 日程族 Event 5 + 会议族 ChangeType 30 + 家校沟通族事件值 2（官方 92032/92052/92050/92051/97281/96716/96717）
    /// + 会话内容存档 Event 1（官方 95039，仅自建）
    /// + 微盘族 Event 1 与 ChangeType 8 + 直播 Event 1 + OA 审批 Event 1，
    /// 合计 128）。
    /// </summary>
    /// <remarks>
    /// 客户联系/获客助手族（官方 92130/92277/96361/97299/97402/99485/98958）以<b>族事件值</b>为事件键：
    /// 其裸 <c>ChangeType</c>（create/update/delete 跨族同名、del_follow_user 跨族同名）无法作为消歧键，
    /// 具体类别由信封 <c>ChangeType</c> 判别。邮箱族同理（应用邮箱 <c>app_email_change</c> 与公共邮箱
    /// <c>public_email_change</c> 的 <c>ChangeType</c> 同为裸 <c>receive_email</c>，官方 97495/97517/97506/100180）。
    /// 家校沟通族同理（成员事件的 <c>subscribe</c>/<c>unsubscribe</c> 与消息与事件族 90240 的事件键同名，
    /// 官方 92032/92052/92050/92051/96716/96717 + 批量 97281）。
    /// </remarks>
    private static readonly (string Key, string Reason)[] OfficialEventKeys =
    {
        (WechatCallbackEventTypes.SuiteTicket, "授权族 suite_ticket"),
        (WechatCallbackEventTypes.CreateAuth, "授权族 create_auth"),
        (WechatCallbackEventTypes.ResetPermanentCode, "授权族 reset_permanent_code"),
        (WechatCallbackEventTypes.ChangeAuth, "授权族 change_auth"),
        (WechatCallbackEventTypes.CancelAuth, "授权族 cancel_auth"),
        (WechatCallbackEventTypes.DelAuth, "授权族 del_auth"),
        (WechatCallbackEventTypes.CreateUser, "通讯录族 create_user"),
        (WechatCallbackEventTypes.UpdateUser, "通讯录族 update_user"),
        (WechatCallbackEventTypes.DeleteUser, "通讯录族 delete_user"),
        (WechatCallbackEventTypes.CreateParty, "通讯录族 create_party"),
        (WechatCallbackEventTypes.UpdateParty, "通讯录族 update_party"),
        (WechatCallbackEventTypes.DeleteParty, "通讯录族 delete_party"),
        (WechatCallbackEventTypes.UpdateTag, "通讯录族 update_tag"),
        (WechatCallbackEventTypes.BatchJobResult, "异步任务族 batch_job_result"),
        (WechatCallbackEventTypes.ChangeChain, "上下游族 change_chain（95796）"),
        (WechatCallbackEventTypes.CreateChain, "上下游族 create_chain"),
        (WechatCallbackEventTypes.UpdateChain, "上下游族 update_chain"),
        (WechatCallbackEventTypes.DeleteChain, "上下游族 delete_chain"),
        (WechatCallbackEventTypes.CreateGroup, "上下游族 create_group"),
        (WechatCallbackEventTypes.UpdateGroup, "上下游族 update_group"),
        (WechatCallbackEventTypes.DeleteGroup, "上下游族 delete_group"),
        (WechatCallbackEventTypes.CorpJoin, "上下游族 corp_join"),
        (WechatCallbackEventTypes.UpdateCorp, "上下游族 update_corp"),
        (WechatCallbackEventTypes.RemoveCorp, "上下游族 remove_corp"),
        (WechatCallbackEventTypes.ChangeDomainIp, "安全管理族 change_domain_ip（100080，仅自建）"),
        (WechatCallbackEventTypes.KfMsgOrEvent, "微信客服族 kf_msg_or_event（94670/94699/96426）"),
        (WechatCallbackEventTypes.KfAccountAuthChange, "微信客服族 kf_account_auth_change（97712/97302/97713）"),
        (WechatCallbackEventTypes.ChangeExternalContact, "客户联系族 change_external_contact（92130/92277/96361）"),
        (WechatCallbackEventTypes.ChangeExternalChat, "客户联系族 change_external_chat"),
        (WechatCallbackEventTypes.ChangeExternalTag, "客户联系族 change_external_tag"),
        (WechatCallbackEventTypes.CustomerAcquisition, "获客助手族 customer_acquisition（97299/97402/99485/98958）"),
        (WechatCallbackEventTypes.CustomerAcquisitionPermitChange, "获客助手族 customer_acquisition_permit_change（92277）"),
        (WechatCallbackEventTypes.Subscribe, "消息与事件族 subscribe（90240）"),
        (WechatCallbackEventTypes.Unsubscribe, "消息与事件族 unsubscribe（90240）"),
        (WechatCallbackEventTypes.EnterAgent, "消息与事件族 enter_agent（90240）"),
        (WechatCallbackEventTypes.Location, "消息与事件族 LOCATION（90240，官方键值大写）"),
        (WechatCallbackEventTypes.Click, "消息与事件族 click（90240）"),
        (WechatCallbackEventTypes.View, "消息与事件族 view（90240）"),
        (WechatCallbackEventTypes.ViewMiniProgram, "消息与事件族 view_miniprogram（90240）"),
        (WechatCallbackEventTypes.ScanCodePush, "消息与事件族 scancode_push（90240）"),
        (WechatCallbackEventTypes.ScanCodeWaitMsg, "消息与事件族 scancode_waitmsg（90240）"),
        (WechatCallbackEventTypes.PicSysPhoto, "消息与事件族 pic_sysphoto（90240）"),
        (WechatCallbackEventTypes.PicPhotoOrAlbum, "消息与事件族 pic_photo_or_album（90240）"),
        (WechatCallbackEventTypes.PicWeixin, "消息与事件族 pic_weixin（90240）"),
        (WechatCallbackEventTypes.LocationSelect, "消息与事件族 location_select（90240）"),
        (WechatCallbackEventTypes.OpenApprovalChange, "消息与事件族 open_approval_change（90240）"),
        (WechatCallbackEventTypes.ShareAgentChange, "消息与事件族 share_agent_change（90240）"),
        (WechatCallbackEventTypes.ShareChainChange, "消息与事件族 share_chain_change（90240）"),
        (WechatCallbackEventTypes.TemplateCardEvent, "消息与事件族 template_card_event（90240）"),
        (WechatCallbackEventTypes.TemplateCardMenuEvent, "消息与事件族 template_card_menu_event（90240）"),
        (WechatCallbackEventTypes.InactiveAlert, "消息与事件族 inactive_alert（90240）"),
        (WechatCallbackEventTypes.CloseInactiveAgent, "消息与事件族 close_inactive_agent（90240）"),
        (WechatCallbackEventTypes.ReopenInactiveAgent, "消息与事件族 reopen_inactive_agent（90240）"),
        (WechatCallbackEventTypes.LowActiveAlert, "消息与事件族 low_active_alert（90240）"),
        (WechatCallbackEventTypes.LowActive, "消息与事件族 low_active（90240）"),
        (WechatCallbackEventTypes.ActiveRestored, "消息与事件族 active_restored（90240）"),
        (WechatCallbackEventTypes.OpenOrder, "应用版本付费订单回调族 open_order（91929）"),
        (WechatCallbackEventTypes.ChangeOrder, "应用版本付费订单回调族 change_order（91930）"),
        (WechatCallbackEventTypes.PayForAppSuccess, "应用版本付费订单回调族 pay_for_app_success（91931）"),
        (WechatCallbackEventTypes.Refund, "应用版本付费订单回调族 refund（91932）"),
        (WechatCallbackEventTypes.ChangeEditon, "应用版本付费订单回调族 change_editon（91933，官方拼写少一个字母 i）"),
        (WechatCallbackEventTypes.CancelOrder, "应用版本付费订单回调族 cancel_order（99353）"),
        (WechatCallbackEventTypes.UnlicensedNotify, "接口调用许可族 unlicensed_notify（97195，仅代开发）"),
        (WechatCallbackEventTypes.LicensePaySuccess, "接口调用许可族 license_pay_success（97196，仅代开发）"),
        (WechatCallbackEventTypes.LicenseRefund, "接口调用许可族 license_refund（97197，仅代开发）"),
        (WechatCallbackEventTypes.AutoActivate, "接口调用许可族 auto_activate（97198，仅代开发）"),
        (WechatCallbackEventTypes.AppEmailChange, "邮箱族 app_email_change（97495/97517/97506）"),
        (WechatCallbackEventTypes.PublicEmailChange, "邮箱族 public_email_change（100180，仅自建）"),
        (WechatCallbackEventTypes.DocMemberChange, "文档族 doc_member_change（97833/97839/97836）"),
        (WechatCallbackEventTypes.DeleteDoc, "文档族 delete_doc（97834/97840/97837）"),
        (WechatCallbackEventTypes.FormComplete, "文档族 form_complete（97835/97841/97838）"),
        (WechatCallbackEventTypes.DeleteForm, "文档族 delete_form（98095/98055/98097）"),
        (WechatCallbackEventTypes.FormSettingsChange, "文档族 form_settings_change（98096/98056/98098）"),
        (WechatCallbackEventTypes.AddFiled, "智能表格族 add_filed（100987/101016/101018，官方拼写 filed）"),
        (WechatCallbackEventTypes.UpdateFiled, "智能表格族 update_filed（同上，官方拼写 filed）"),
        (WechatCallbackEventTypes.DeleteFiled, "智能表格族 delete_filed（同上，官方拼写 filed）"),
        (WechatCallbackEventTypes.AddRecord, "智能表格族 add_record（100986/101017/101019）"),
        (WechatCallbackEventTypes.UpdateRecord, "智能表格族 update_record（同上）"),
        (WechatCallbackEventTypes.DeleteRecord, "智能表格族 delete_record（同上）"),
        (WechatCallbackEventTypes.DeleteCalendar, "日程族 delete_calendar（97728/97806/97771）"),
        (WechatCallbackEventTypes.ModifyCalendar, "日程族 modify_calendar（97730/97808/97772）"),
        (WechatCallbackEventTypes.ModifySchedule, "日程族 modify_schedule（97731/97809/97773）"),
        (WechatCallbackEventTypes.DeleteSchedule, "日程族 delete_schedule（97732/97810/97774）"),
        (WechatCallbackEventTypes.RespondSchedule, "日程族 respond_schedule（98111/98099/98110）"),
        (WechatCallbackEventTypes.ModifyMeeting, "会议族 modify_meeting（99081/97451/97459）"),
        (WechatCallbackEventTypes.CancelMeeting, "会议族 cancel_meeting（99082/97451/97459）"),
        (WechatCallbackEventTypes.MeetingStart, "会议族 meeting_start（98333，仅自建）"),
        (WechatCallbackEventTypes.MeetingEnd, "会议族 meeting_end（98337，仅自建）"),
        (WechatCallbackEventTypes.MeetingMuteAll, "会议族 meeting_mute_all（98341，仅自建）"),
        (WechatCallbackEventTypes.MeetingUnmuteAll, "会议族 meeting_unmute_all（98345，仅自建）"),
        (WechatCallbackEventTypes.JoinMeeting, "会议族 join_meeting（98348，仅自建）"),
        (WechatCallbackEventTypes.QuitMeeting, "会议族 quit_meeting（98352，仅自建）"),
        (WechatCallbackEventTypes.JoinMeetingBeforeHost, "会议族 join_meeting_before_host（98353，仅自建）"),
        (WechatCallbackEventTypes.JoinWaitingRoom, "会议族 join_waiting_room（98354，仅自建）"),
        (WechatCallbackEventTypes.OpenScreenShare, "会议族 open_screen_share（98395，仅自建）"),
        (WechatCallbackEventTypes.CloseScreenShare, "会议族 close_screen_share（98396，仅自建）"),
        (WechatCallbackEventTypes.QuitWaitingRoom, "会议族 quit_waiting_room（98355，仅自建）"),
        (WechatCallbackEventTypes.JoinFromMeetingRoom, "会议族 join_from_meeting_room（98393，仅自建）"),
        (WechatCallbackEventTypes.MoveToWaitingRoom, "会议族 move_to_waiting_room（98394，仅自建）"),
        (WechatCallbackEventTypes.RoleChange, "会议族 role_change（98397，仅自建）"),
        (WechatCallbackEventTypes.WebinarRoleChange, "会议族 webinar_role_change（98771，仅自建）"),
        (WechatCallbackEventTypes.WebinarWarmUpUpload, "会议族 webinar_warm_up_upload（98773，仅自建）"),
        (WechatCallbackEventTypes.PstnStatusUpdate, "会议族 pstn_status_update（98774，仅自建）"),
        (WechatCallbackEventTypes.MediumUpload, "会议族 medium_upload（98775，仅自建）"),
        (WechatCallbackEventTypes.StartRecording, "会议族 start_recording（98398，仅自建）"),
        (WechatCallbackEventTypes.PauseRecording, "会议族 pause_recording（98399，仅自建）"),
        (WechatCallbackEventTypes.ResumeRecording, "会议族 resume_recording（98400，仅自建）"),
        (WechatCallbackEventTypes.StopRecording, "会议族 stop_recording（98401，仅自建）"),
        (WechatCallbackEventTypes.RecordingComplete, "会议族 recording_complete（98402，仅自建）"),
        (WechatCallbackEventTypes.DeleteRecording, "会议族 delete_recording（98404，仅自建）"),
        (WechatCallbackEventTypes.Enroll, "会议族 enroll（98781，仅自建）"),
        (WechatCallbackEventTypes.CancelEnroll, "会议族 cancel_enroll（98782，仅自建）"),
        (WechatCallbackEventTypes.MeetingRoomResponse, "会议族 meeting_room_response（98783，仅自建）"),
        (WechatCallbackEventTypes.StartMeeting, "会议族 start_meeting（99648，meeting_statistics 族，仅自建）"),
        (WechatCallbackEventTypes.ChangeSchoolContact,
            "家校沟通族 change_school_contact（92032/92052/92050/92051/96716/96717，族事件值为键）"),
        (WechatCallbackEventTypes.ChangeSchoolContactBatch, "家校沟通族 change_school_contact_batch（97281，仅第三方套件信封）"),
        (WechatCallbackEventTypes.MsgAuditNotify, "会话内容存档族 msgaudit_notify（95039，仅自建）"),
        (WechatCallbackEventTypes.WedriveInsufficientCapacity, "微盘族 wedrive_insufficient_capacity（97898/97972/97932）"),
        (WechatCallbackEventTypes.DismissSpace, "微盘族 dismiss_space（97901/97976/97935）"),
        (WechatCallbackEventTypes.SpaceMemberChange, "微盘族 space_member_change（97902/97977/97936）"),
        (WechatCallbackEventTypes.SpaceSecuritySettingsChange, "微盘族 space_security_settings_change（97903/97978/97937）"),
        (WechatCallbackEventTypes.CreateFile, "微盘族 create_file（97900/97975/97934）"),
        (WechatCallbackEventTypes.RenameFile, "微盘族 rename_file（同上）"),
        (WechatCallbackEventTypes.UpdateFile, "微盘族 update_file（同上）"),
        (WechatCallbackEventTypes.DeleteFile, "微盘族 delete_file（同上）"),
        (WechatCallbackEventTypes.MoveFile, "微盘族 move_file（同上）"),
        (WechatCallbackEventTypes.LivingStatusChange, "直播族 living_status_change（94145/94308/96842）"),
        (WechatCallbackEventTypes.SysApprovalChange, "OA 审批族 sys_approval_change（91815/92633/96508）"),
    };

    // ---------------------------------------------------------------- CB1

    /// <summary>
    /// 契约守卫 CB1：Callback 包不得引用主包 <c>Work</c>（K-callback 依赖单向边）。
    /// </summary>
    [Fact]
    public void CallbackPackage_ShouldNotReferenceWorkProject()
    {
        var csprojPath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Work.Callback", "Mud.Wechat.Work.Callback.csproj");
        File.Exists(csprojPath).Should().BeTrue($"未找到回调包工程文件：{csprojPath}");

        var source = File.ReadAllText(csprojPath);

        source.Should().NotContain(
            $"\"..\\Mud.Wechat.Work\\Mud.Wechat.Work.csproj\"",
            "K-callback：Callback 只能依赖 Abstractions 与 DataModels");
        source.Should().Contain("Mud.Wechat.Work.Abstractions.csproj");
        source.Should().Contain("Mud.Wechat.Work.DataModels.csproj");
    }

    // ---------------------------------------------------------------- CB2

    /// <summary>
    /// 契约守卫 CB2：<see cref="WechatCallbackEventTypes"/> 常量必须覆盖官方事件键全集
    /// （授权 InfoType 6 + 通讯录 ChangeType 7 + 异步 Event 1 + 上下游 Event 1 与 ChangeType 9
    /// + 安全管理 ChangeType 1（官方 100080）+ 微信客服 Event 2（官方 94670/97712 等三模式文档）
    /// + 客户联系/获客助手族事件值 5 + 消息与事件 path 90240 的 24 个 Event 键
    /// + 应用版本付费订单回调 InfoType 6 + 接口调用许可事件 4（官方 97195~97198，仅代开发）
    /// + 邮箱族事件值 2 + 文档族 ChangeType 5 + 智能表格族 ChangeType 6
    /// + 日程族 Event 5 + 会议族 ChangeType 30 + 家校沟通族事件值 2 + 会话内容存档 Event 1
    /// + 微盘族 Event 1 与 ChangeType 8 + 直播 Event 1 + OA 审批 Event 1），
    /// 且 <see cref="WechatCallbackEvent.EventTypeKey"/> 判别优先级为
    /// 客户联系/获客族、邮箱族与家校沟通族「外层事件值」→ InfoType → ChangeType → Event
    /// （v1 方案 D4 + ADR-14 三模式键统一）。
    /// </summary>
    [Fact]
    public void CallbackEventTypeKeys_ShouldCoverOfficialEventFamilies()
    {
        var constants = typeof(WechatCallbackEventTypes)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(f => f.IsLiteral && f.FieldType == typeof(string))
            .Select(f => (string)f.GetRawConstantValue()!)
            .ToList();

        foreach (var (key, reason) in OfficialEventKeys)
        {
            constants.Should().Contain(key, $"官方事件键缺失（{reason}）；新增官方事件键须同批登记");
        }

        constants.Should().Contain(WechatCallbackEventTypes.ChangeContact, "通讯录变更事件信封值");
        constants.Should().Contain(WechatCallbackEventTypes.ChangeChain, "上下游变更事件信封值（95796）");
        constants.Should().Contain(WechatCallbackEventTypes.Security, "安全管理事件信封值（100080）");
        constants.Should().Contain(WechatCallbackEventTypes.DocChange, "文档族事件信封值（97833 等）");
        constants.Should().Contain(WechatCallbackEventTypes.SmartSheetChange, "智能表格族事件信封值（100986 等）");
        constants.Should().Contain(WechatCallbackEventTypes.MeetingChange, "会议族事件信封值（99081 等）");
        constants.Should().Contain(WechatCallbackEventTypes.MeetingStatistics, "会议统计族事件信封值（99648）");
        constants.Should().Contain(WechatCallbackEventTypes.WedriveSpaceChange, "微盘空间变更族事件信封值（97899 等）");
        constants.Should().Contain(WechatCallbackEventTypes.WedriveFileChange, "微盘文件变更族事件信封值（97900 等）");

        // EventTypeKey 判别优先级（D4）。
        new WechatCallbackEvent { InfoType = "suite_ticket", Event = "change_contact", ChangeType = "create_user" }
            .EventTypeKey.Should().Be("suite_ticket", "InfoType 优先");
        new WechatCallbackEvent { Event = "change_contact", ChangeType = "create_user" }
            .EventTypeKey.Should().Be("create_user", "ChangeType 次之");
        new WechatCallbackEvent { Event = "batch_job_result" }
            .EventTypeKey.Should().Be("batch_job_result", "Event 兜底");
        new WechatCallbackEvent().EventTypeKey.Should().BeEmpty("三者皆空 → 仅兜底处理器可见");

        // 客户联系/获客助手族以「外层事件值」为事件键（裸 ChangeType 跨族同名，无法逐键消歧）：
        // Event 信封取 Event 节点；第三方套件信封（92277 指令回调）无 Event 节点，回退 InfoType ⇒ 两信封同键。
        new WechatCallbackEvent { Event = "change_external_chat", ChangeType = "create" }
            .EventTypeKey.Should().Be("change_external_chat", "客户联系族以族事件值为键（create 与标签族同名）");
        new WechatCallbackEvent { InfoType = "change_external_contact", ChangeType = "add_external_contact" }
            .EventTypeKey.Should().Be("change_external_contact", "套件信封同样产出族事件值键（三模式键统一，ADR-14）");
        new WechatCallbackEvent { InfoType = "customer_acquisition", ChangeType = "del_follow_user" }
            .EventTypeKey.Should().Be("customer_acquisition", "获客助手族以族事件值为键（del_follow_user 与客户联系族同名）");
        new WechatCallbackEvent { InfoType = "customer_acquisition_permit_change" }
            .EventTypeKey.Should().Be("customer_acquisition_permit_change", "无 ChangeType 分组段 ⇒ 键为 InfoType 本身");

        // 邮箱族以「族事件值」为事件键（receive_email 与公共邮箱族同名，无法逐键消歧）。
        new WechatCallbackEvent { Event = "app_email_change", ChangeType = "receive_email" }
            .EventTypeKey.Should().Be("app_email_change", "邮箱族以族事件值为键（receive_email 与公共邮箱族同名）");
        new WechatCallbackEvent { Event = "public_email_change", ChangeType = "receive_email" }
            .EventTypeKey.Should().Be("public_email_change", "邮箱族以族事件值为键（receive_email 与应用邮箱族同名）");

        // 家校沟通族以「族事件值」为事件键（subscribe/unsubscribe 与消息与事件族 90240 的事件键同名，
        // 逐 ChangeType 键既无法消歧也会在契约注册表撞键）；第三方套件信封（92050/92051/97281 指令回调）
        // 无 Event 节点，外层事件值在 InfoType ⇒ 两信封同键。
        new WechatCallbackEvent { Event = "change_school_contact", ChangeType = "subscribe" }
            .EventTypeKey.Should().Be("change_school_contact", "家校沟通族以族事件值为键（subscribe 与 90240 消息族同名）");
        new WechatCallbackEvent { InfoType = "change_school_contact", ChangeType = "create_student" }
            .EventTypeKey.Should().Be("change_school_contact", "套件信封同样产出族事件值键（三模式键统一，ADR-14）");
        new WechatCallbackEvent { InfoType = "change_school_contact_batch" }
            .EventTypeKey.Should().Be("change_school_contact_batch", "批量变更事件键为 InfoType 本身（无 ChangeType 顶层分组段）");
        new WechatCallbackEvent { Event = "msgaudit_notify" }
            .EventTypeKey.Should().Be("msgaudit_notify", "会话内容存档事件无 InfoType/ChangeType 段 ⇒ Event 节点即事件键（逐键自指）");

        // 接口调用许可族（官方 97195~97198，仅代开发）：unlicensed_notify 为 Event 信封逐键自指，
        // 其余三键走套件信封（InfoType 非空）⇒ 归授权族，事件键为 InfoType 本身。
        new WechatCallbackEvent { Event = "unlicensed_notify" }
            .EventTypeKey.Should().Be("unlicensed_notify", "许可失效通知无 InfoType/ChangeType 段 ⇒ Event 节点即事件键（逐键自指）");
        new WechatCallbackEvent { Event = "unlicensed_notify" }
            .EventFamily.Should().Be(WechatCallbackEventFamily.Unknown, "许可失效通知不归既有族，族闸放行、键级开放面承载判定");
        new WechatCallbackEvent { InfoType = "license_pay_success" }
            .EventTypeKey.Should().Be("license_pay_success", "许可订单族事件键为 InfoType 本身（套件信封逐键自指）");
        new WechatCallbackEvent { InfoType = "auto_activate" }
            .EventFamily.Should().Be(WechatCallbackEventFamily.Authorization, "许可族套件信封（InfoType 非空）归授权族");

        // 事件族判别：客户联系/获客族的套件信封不得误判为授权族（InfoType 非空的历史口径仅适用授权族键）。
        new WechatCallbackEvent { InfoType = "change_external_contact", ChangeType = "add_external_contact" }
            .EventFamily.Should().Be(WechatCallbackEventFamily.ExternalContactChange);
        new WechatCallbackEvent { InfoType = "customer_acquisition", ChangeType = "balance_low" }
            .EventFamily.Should().Be(WechatCallbackEventFamily.CustomerAcquisition);
        new WechatCallbackEvent { InfoType = "change_auth" }
            .EventFamily.Should().Be(WechatCallbackEventFamily.Authorization, "授权族判别不受影响");
        new WechatCallbackEvent { InfoType = "change_school_contact", ChangeType = "create_student" }
            .EventFamily.Should().Be(WechatCallbackEventFamily.SchoolContactChange,
                "家校沟通族的套件信封 InfoType 承载族事件值，不得误判为授权族");
        new WechatCallbackEvent { Event = "change_school_contact", ChangeType = "create_department" }
            .EventFamily.Should().Be(WechatCallbackEventFamily.SchoolContactChange,
                "家校沟通族的 Event 信封按外层事件值归类");
        new WechatCallbackEvent { Event = "change_contact", ChangeType = "create_user" }
            .EventFamily.Should().Be(WechatCallbackEventFamily.ContactChange, "通讯录族判别不受影响");
        // 邮箱族无独立事件族：落 Unknown（族闸放行），键级开放面由契约声明承载（ADR-15）。
        new WechatCallbackEvent { Event = "app_email_change", ChangeType = "receive_email" }
            .EventFamily.Should().Be(WechatCallbackEventFamily.Unknown, "邮箱族不归类既有族，族闸不拦截");
        new WechatCallbackEvent { Event = "security", ChangeType = "change_domain_ip" }
            .EventFamily.Should().Be(WechatCallbackEventFamily.SecurityChange, "安全事件族按 Event=security 归类（100080）");
        new WechatCallbackEvent { Event = "security", ChangeType = "change_domain_ip" }
            .EventTypeKey.Should().Be("change_domain_ip", "安全事件族以 ChangeType 为事件键（信封 Event=security 是族前置条件）");
        new WechatCallbackEvent { Event = "kf_msg_or_event" }
            .EventFamily.Should().Be(WechatCallbackEventFamily.KfEvent, "微信客服族按 Event 值归类（94670）");
        new WechatCallbackEvent { Event = "kf_msg_or_event" }.EventTypeKey
            .Should().Be("kf_msg_or_event", "微信客服族无 InfoType/ChangeType 段 ⇒ Event 节点即事件键（逐键自指）");
        new WechatCallbackEvent { Event = "kf_account_auth_change" }
            .EventTypeKey.Should().Be("kf_account_auth_change", "授权变更事件键为 Event 值本身");
    }

    // ---------------------------------------------------------------- CB3

    /// <summary>
    /// 契约守卫 CB3：内置授权族处理器必须为「单类兜底」形态（v1 方案 D6）——实现
    /// <see cref="IWechatCallbackEventHandler"/> 且 <c>SupportedEventType</c> 返回空串；
    /// 授权族 6 个 InfoType 判别保留在信封；P0-3「只告警不删库」分支不回退（G9 同源）。
    /// </summary>
    [Fact]
    public void CallbackHandlers_ShouldRegisterBuiltinAuthorizationFamily()
    {
        var handlerPath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Work.Callback", "WechatCallbackHandler.cs");
        File.Exists(handlerPath).Should().BeTrue($"未找到回调处理器源码（勿移动文件位置，G9 按路径断言）：{handlerPath}");
        var handlerSource = File.ReadAllText(handlerPath);

        handlerSource.Should().Contain("IWechatCallbackEventHandler", "D6：内置处理器实现类型化处理器接口");
        handlerSource.Should().Contain("SupportedEventType => string.Empty", "D6：空键 = 兜底语义");
        handlerSource.Should().Contain("MatchAppKeysBySuiteId", "G9：清理范围恒为 SuiteId 命中集");
        handlerSource.Should().Contain("已跳过授权清理以避免误删其它套件授权", "G9：未命中只告警不删库");
        handlerSource.Should().NotContain("ResolveAppKeys(", "G9：不得回退「全部应用清理」");

        // 授权族 6 InfoType 判别落位信封（经 WechatCallbackEventTypes 常量名引用）。
        var envelopePath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Work.Abstractions", "Callback", "WechatCallbackEvent.cs");
        File.Exists(envelopePath).Should().BeTrue();
        var envelopeSource = File.ReadAllText(envelopePath);
        foreach (var keyName in new[]
                 {
                     nameof(WechatCallbackEventTypes.SuiteTicket), nameof(WechatCallbackEventTypes.CreateAuth),
                     nameof(WechatCallbackEventTypes.ResetPermanentCode), nameof(WechatCallbackEventTypes.ChangeAuth),
                     nameof(WechatCallbackEventTypes.CancelAuth), nameof(WechatCallbackEventTypes.DelAuth),
                 })
        {
            envelopeSource.Should().Contain(
                $"WechatCallbackEventTypes.{keyName}",
                $"信封判别必须覆盖授权族 InfoType：{keyName}");
        }
    }

    // ---------------------------------------------------------------- CB4

    /// <summary>
    /// 契约守卫 CB4（v2.2 改写）：<b>结构族载荷</b>必须暴露官方字段（v1 方案 §6 字段速查）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 旧 CB4 断言 11 个「逐事件 DTO」的属性名；v2.2 的 ADR-1 把它们收敛为 5 个<b>结构族载荷</b>
    /// （官方报文结构同一的事件键共用一个类型，具体类别由信封 <c>ChangeType</c> 判别）。
    /// 故本守卫改为按结构族断言，并<b>追加两项漂移校验</b>：
    /// </para>
    /// <list type="number">
    /// <item><description>载荷类型必须标注 <c>[PayloadContract]</c>（否则上游生成器不产出映射表）；</description></item>
    /// <item><description>载荷类型必须声明 <c>partial</c>（生成物是其 <c>partial</c> 成员）。</description></item>
    /// </list>
    /// <para>
    /// 「元素名 ↔ 属性名」的配对正确性<b>不再由本守卫承担</b>：上游生成器已在编译期校验（<c>PAYLOAD004/006/007</c>），
    /// 逐字段<b>取值</b>正确性由 <c>WechatCallbackPayloadReaderTests</c> 的官方样报文用例覆盖 —— 两者双向夹逼。
    /// </para>
    /// </remarks>
    [Fact]
    public void PayloadTypes_ShouldExposeOfficialFields()
    {
        AssertProperties(typeof(ContactUserChangedPayload), "create_user / update_user / delete_user",
            "UserId", "NewUserId", "Name", "DepartmentIds", "MainDepartmentId", "LeaderInDeptFlags",
            "DirectLeaderIds", "Position", "Mobile", "Gender", "Email", "BizMail", "Status", "Avatar",
            "Alias", "Telephone", "Address", "ExtAttr");
        AssertProperties(typeof(ContactPartyChangedPayload), "create_party / update_party / delete_party",
            "PartyId", "Name", "ParentId", "Order");
        AssertProperties(typeof(ContactTagChangedPayload), "update_tag",
            "TagId", "AddedUserIds", "RemovedUserIds", "AddedPartyIds", "RemovedPartyIds");
        AssertProperties(typeof(BatchJobCompletedPayload), "batch_job_result",
            "JobId", "JobType", "ErrCode", "ErrMsg");
        AssertProperties(typeof(ChainChangedPayload),
            "create_chain…remove_corp（9 键）", "ChainId", "GroupIds", "CorpIds");

        // 官方 path 90240 消息与事件（三模式文档正文一致 ⇒ 8 个结构族载荷覆盖 24 键）。
        AssertProperties(typeof(PlainEventPayload),
            "subscribe / enter_agent / click / share_agent_change / low_active 等 12 键（90240）",
            "EventKey", "AgentId");
        AssertProperties(typeof(AgentAlertPayload),
            "inactive_alert / low_active_alert", "EffectTime", "AgentId");
        AssertProperties(typeof(MenuScanCodePayload),
            "scancode_push / scancode_waitmsg", "EventKey", "ScanCodeInfo", "AgentId");
        AssertProperties(typeof(MenuPicPayload),
            "pic_sysphoto / pic_photo_or_album / pic_weixin", "EventKey", "SendPicsInfo", "AgentId");
        AssertProperties(typeof(MenuLocationSelectPayload),
            "location_select", "EventKey", "SendLocationInfo", "AgentId", "AppType");
        AssertProperties(typeof(LocationReportedPayload),
            "LOCATION", "Latitude", "Longitude", "Precision", "AgentId", "AppType");
        AssertProperties(typeof(ApprovalStatusChangedPayload),
            "open_approval_change（字段位于 ApprovalInfo 包装节点内，故不映射根级 AgentID）",
            "ThirdNo", "OpenSpName", "OpenTemplateId", "OpenSpStatus", "ApplyTime",
            "ApplyUserName", "ApplyUserId", "ApplyUserParty", "ApplyUserImage",
            "ApprovalNodes", "NotifyNodes", "ApproverStep");
        AssertProperties(typeof(TemplateCardEventPayload),
            "template_card_event / template_card_menu_event",
            "EventKey", "TaskId", "CardType", "ResponseCode", "AgentId", "SelectedItems");

        // 客户联系变更族 / 获客助手族（官方 92130/92277/96361/97299/99485/98958；族事件值为键）。
        AssertProperties(typeof(ExternalContactChangedPayload),
            "change_external_contact", "UserId", "ExternalUserId", "State", "WelcomeCode", "Source", "FailReason", "LinkId");
        AssertProperties(typeof(ExternalChatChangedPayload),
            "change_external_chat", "ChatId", "UpdateDetail", "JoinScene", "QuitScene",
            "MemberChangeCount", "MemberChangeList", "LastMemberVersion", "CurrentMemberVersion");
        AssertProperties(typeof(ExternalTagChangedPayload),
            "change_external_tag", "TagId", "TagType", "StrategyId");
        AssertProperties(typeof(CustomerAcquisitionPayload),
            "customer_acquisition（含 service_* / change_price 组件形态与 permit_change）",
            "LinkId", "State", "ExpireTime", "ExpireQuotaNum", "UserId", "ExternalUserId",
            "ChatSeq", "ChatKey", "OnceKey", "Price", "EffectiveTime");

        // 收银台·应用版本付费订单回调族（官方 91929~91933 / 99353 第三方 · 99387~99392 代开发；
        // 套件信封 InfoType 键，指令回调 URL）。
        AssertProperties(typeof(PayToolVersionOrderPayload),
            "open_order / change_order / pay_for_app_success / refund / change_editon / cancel_order（6 键）",
            "PaidCorpId", "OrderId", "OperatorId", "OldOrderId", "NewOrderId");

        // 接口调用许可族（官方 97195~97198，仅代开发；97196/97197 套件信封 InfoType 键，
        // 97195 为 Event 信封逐键自指；97198 的 AccountList 为根下重复同名兄弟元素）。
        AssertProperties(typeof(UnlicensedNotifyPayload), "unlicensed_notify（97195，信封外仅 AgentID）", "AgentId");
        AssertProperties(typeof(LicenseOrderPayload),
            "license_pay_success / license_refund（97196/97197）",
            "ServiceCorpId", "OrderId", "BuyerUserId", "OrderStatus");
        AssertProperties(typeof(LicenseAutoActivatePayload),
            "auto_activate（97198，AccountList 为根下重复同名兄弟元素）",
            "ServiceCorpId", "Scene", "AccountItems");

        // 安全管理族（官方 100080；信封外无业务字段 —— 断言意义在下方 payloadTypes 循环的契约/密封形态锁）。
        AssertProperties(typeof(SecurityDomainIpChangedPayload), "change_domain_ip（100080，信封外无业务字段）");

        // 微信客服族（官方 94670/94699/96426；外层通知仅 Token + OpenKfId，内容经 sync_msg 拉取）。
        AssertProperties(typeof(KfMsgOrEventPayload), "kf_msg_or_event（94670）", "Token", "OpenKfId");

        // 邮箱族（官方 97495/97517/97506 + 100180；族事件值为键）。
        AssertProperties(typeof(AppEmailChangedPayload), "app_email_change", "Amount");
        AssertProperties(typeof(PublicEmailChangedPayload), "public_email_change", "Id", "Amount");

        // 文档族（官方 doc_change 5 键）与智能表格族（smart_sheet_change 6 键）。
        AssertProperties(typeof(DocChangedPayload),
            "doc_member_change / delete_doc / form_complete / delete_form / form_settings_change（5 键）",
            "DocIds", "FormIds");
        AssertProperties(typeof(SmartSheetFieldChangedPayload),
            "add_filed / update_filed / delete_filed（官方拼写 filed）",
            "DocId", "SheetId", "FieldIds");
        AssertProperties(typeof(SmartSheetRecordChangedPayload),
            "add_record / update_record / delete_record",
            "DocId", "SheetId", "RecordIds");

        // 日程族（官方 97728/97730/97731/97732/98111 等；Event 节点即事件键）。
        AssertProperties(typeof(CalendarChangedPayload),
            "delete_calendar / modify_calendar", "CalId");
        AssertProperties(typeof(ScheduleChangedPayload),
            "modify_schedule / delete_schedule / respond_schedule", "CalId", "ScheduleId");

        // 会议族（官方 meeting_change 29 键 + meeting_statistics 1 键；modify/cancel 三模式开放，其余仅自建）。
        AssertProperties(typeof(MeetingChangedPayload),
            "modify_meeting / cancel_meeting / meeting_start / meeting_end / meeting_mute_all /" +
            " meeting_unmute_all / join_meeting / quit_meeting / join_meeting_before_host /" +
            " join_waiting_room / open_screen_share / close_screen_share / 云录制 6 键（18 键）",
            "FromUserTmpOpenId", "MeetingId");
        AssertProperties(typeof(MeetingEnrollPayload),
            "enroll / cancel_enroll", "FromUserTmpOpenId", "MeetingId", "EnrollId");
        AssertProperties(typeof(MeetingPstnStatusPayload),
            "pstn_status_update", "FromUserTmpOpenId", "MeetingId", "PstnStatus");
        AssertProperties(typeof(MeetingMemberChangedPayload),
            "quit_waiting_room / join_from_meeting_room / move_to_waiting_room / role_change /" +
            " webinar_role_change（5 键）",
            "FromUserTmpOpenId", "OperatedUser", "MeetingId");
        AssertProperties(typeof(MeetingWarmUpUploadPayload),
            "webinar_warm_up_upload", "MeetingId", "WarmUpInfo");
        AssertProperties(typeof(MeetingMediumUploadPayload),
            "medium_upload（UploadInfo 为根下重复同名兄弟元素）", "MeetingId", "AllUploadStatus", "UploadInfos");
        AssertProperties(typeof(MeetingRoomResponsePayload),
            "meeting_room_response", "MeetingId", "MeetingRoomId", "MraAddress", "RoomResponseStatus");
        AssertProperties(typeof(MeetingStatisticsPayload),
            "start_meeting（meeting_statistics 族）", "Status");

        // 家校沟通族（官方 92032/92052 自建 · 92050/92051 第三方套件 · 96716/96717 代开发 + 批量 97281）。
        AssertProperties(typeof(SchoolContactChangedPayload),
            "change_school_contact（成员 8 类 + 部门 3 类变更，族事件值为键）", "Id", "NewId");
        AssertProperties(typeof(SchoolContactBatchChangedPayload),
            "change_school_contact_batch（97281，ChangeList 为根下重复同名兄弟元素）", "ChangeItems");

        // 会话内容存档族（官方 95039，仅自建；信封外仅 AgentID）。
        AssertProperties(typeof(MsgAuditNotifyPayload), "msgaudit_notify（95039，信封外仅 AgentID）", "AgentId");

        // 微盘族（官方 97898~97903 / 97972~97978 / 97932~97937；三份文档逐字一致）。
        AssertProperties(typeof(WedriveInsufficientCapacityPayload),
            "wedrive_insufficient_capacity（信封外无业务字段）");
        AssertProperties(typeof(WedriveSpaceChangedPayload),
            "dismiss_space / space_member_change / space_security_settings_change（3 键）", "SpaceIds");
        AssertProperties(typeof(WedriveFileChangedPayload),
            "create_file / rename_file / update_file / delete_file / move_file（5 键）", "FileIds");

        // 直播族（官方 94145/94308/96842；三份 XML 逐字节一致）。
        AssertProperties(typeof(LivingStatusChangedPayload),
            "living_status_change", "LivingId", "Status", "AgentId");

        // OA 审批族（官方 91815/92633/96508；载荷在 ApprovalInfo 包装节点内）。
        AssertProperties(typeof(SysApprovalChangedPayload),
            "sys_approval_change（字段位于 ApprovalInfo 包装节点内）",
            "SpNoStr", "SpNo", "SpName", "SpStatus", "TemplateId", "ApplyTime", "Applyer",
            "SpRecords", "Notifyers", "ProcessList", "Comments", "StatuChangeEvent");

        var payloadTypes = new[]
        {
            typeof(ContactUserChangedPayload), typeof(ContactPartyChangedPayload),
            typeof(ContactTagChangedPayload), typeof(BatchJobCompletedPayload),
            typeof(ChainChangedPayload),
            typeof(PlainEventPayload), typeof(AgentAlertPayload),
            typeof(MenuScanCodePayload), typeof(MenuPicPayload),
            typeof(MenuLocationSelectPayload), typeof(LocationReportedPayload),
            typeof(ApprovalStatusChangedPayload), typeof(TemplateCardEventPayload),
            typeof(ExternalContactChangedPayload), typeof(ExternalChatChangedPayload),
            typeof(ExternalTagChangedPayload), typeof(CustomerAcquisitionPayload),
            typeof(PayToolVersionOrderPayload), typeof(SecurityDomainIpChangedPayload),
            typeof(KfMsgOrEventPayload),
            typeof(UnlicensedNotifyPayload), typeof(LicenseOrderPayload), typeof(LicenseAutoActivatePayload),
            typeof(AppEmailChangedPayload), typeof(PublicEmailChangedPayload),
            typeof(DocChangedPayload), typeof(SmartSheetFieldChangedPayload),
            typeof(SmartSheetRecordChangedPayload),
            typeof(CalendarChangedPayload), typeof(ScheduleChangedPayload),
            typeof(MeetingChangedPayload), typeof(MeetingEnrollPayload),
            typeof(MeetingPstnStatusPayload), typeof(MeetingMemberChangedPayload),
            typeof(MeetingWarmUpUploadPayload), typeof(MeetingMediumUploadPayload),
            typeof(MeetingRoomResponsePayload), typeof(MeetingStatisticsPayload),
            typeof(SchoolContactChangedPayload), typeof(SchoolContactBatchChangedPayload),
            typeof(MsgAuditNotifyPayload),
            typeof(WedriveInsufficientCapacityPayload), typeof(WedriveSpaceChangedPayload),
            typeof(WedriveFileChangedPayload), typeof(LivingStatusChangedPayload),
            typeof(SysApprovalChangedPayload),
        };

        foreach (var type in payloadTypes)
        {
            type.GetCustomAttributes(typeof(PayloadContractAttribute), inherit: false)
                .Should().NotBeEmpty(type.Name + " 必须标注 [PayloadContract]，否则上游生成器不产出映射表");

            // 生成物是 partial 成员 ⇒ 类型必须可分部声明（上游 PAYLOAD002 在编译期校验，此处为测试期冗余锁）。
            type.IsSealed.Should().BeTrue(type.Name + " 为密封载荷类型（非密封不影响 partial，此断言仅锁定既存形态）");
        }

        static void AssertProperties(Type payloadType, string eventName, params string[] expected)
        {
            var props = payloadType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(p => p.Name)
                .ToList();
            foreach (var field in expected)
            {
                props.Should().Contain(field, $"{payloadType.Name}（{eventName}）缺少官方字段 {field}");
            }
        }
    }

    /// <summary>
    /// 契约守卫 CB4b（v2.2 新增；P2 扩展）：官方契约表必须登记全部 120 个载荷事件键，且授权族键不登记；
    /// 并断言「生成物登记键集 == <c>[WechatCallbackContract]</c> 特性声明并集」（生成器漂移闸）。
    /// 双面锁定：官方清单（expectedKeys）是外部契约的权威锚点，特性一致性断言锁内部链条 ——
    /// 二者不得互替（同源即同向逃逸）。
    /// </summary>
    [Fact]
    public void OfficialPayloadContracts_ShouldCoverAllPayloadEventKeys()
    {
        var registry = new WechatPayloadContractRegistry();
        OfficialPayloadContracts.RegisterAll(registry);

        var expectedKeys = new[]
        {
            WechatCallbackEventTypes.CreateUser, WechatCallbackEventTypes.UpdateUser, WechatCallbackEventTypes.DeleteUser,
            WechatCallbackEventTypes.CreateParty, WechatCallbackEventTypes.UpdateParty, WechatCallbackEventTypes.DeleteParty,
            WechatCallbackEventTypes.UpdateTag,
            WechatCallbackEventTypes.BatchJobResult,
            WechatCallbackEventTypes.CreateChain, WechatCallbackEventTypes.UpdateChain, WechatCallbackEventTypes.DeleteChain,
            WechatCallbackEventTypes.CreateGroup, WechatCallbackEventTypes.UpdateGroup, WechatCallbackEventTypes.DeleteGroup,
            WechatCallbackEventTypes.CorpJoin, WechatCallbackEventTypes.UpdateCorp, WechatCallbackEventTypes.RemoveCorp,
            WechatCallbackEventTypes.ChangeDomainIp,
            WechatCallbackEventTypes.KfMsgOrEvent,

            // 客户联系变更族 / 获客助手族族事件值键（92130/92277/96361/97299/97402/99485/98958）。
            WechatCallbackEventTypes.ChangeExternalContact, WechatCallbackEventTypes.ChangeExternalChat,
            WechatCallbackEventTypes.ChangeExternalTag, WechatCallbackEventTypes.CustomerAcquisition,
            WechatCallbackEventTypes.CustomerAcquisitionPermitChange,

            // 官方 path 90240 消息与事件 24 键（三模式文档 90240/90376/96468 正文一致）。
            WechatCallbackEventTypes.Subscribe, WechatCallbackEventTypes.Unsubscribe,
            WechatCallbackEventTypes.EnterAgent, WechatCallbackEventTypes.Location,
            WechatCallbackEventTypes.Click, WechatCallbackEventTypes.View,
            WechatCallbackEventTypes.ViewMiniProgram, WechatCallbackEventTypes.ScanCodePush,
            WechatCallbackEventTypes.ScanCodeWaitMsg, WechatCallbackEventTypes.PicSysPhoto,
            WechatCallbackEventTypes.PicPhotoOrAlbum, WechatCallbackEventTypes.PicWeixin,
            WechatCallbackEventTypes.LocationSelect, WechatCallbackEventTypes.OpenApprovalChange,
            WechatCallbackEventTypes.ShareAgentChange, WechatCallbackEventTypes.ShareChainChange,
            WechatCallbackEventTypes.TemplateCardEvent, WechatCallbackEventTypes.TemplateCardMenuEvent,
            WechatCallbackEventTypes.InactiveAlert, WechatCallbackEventTypes.CloseInactiveAgent,
            WechatCallbackEventTypes.ReopenInactiveAgent, WechatCallbackEventTypes.LowActiveAlert,
            WechatCallbackEventTypes.LowActive, WechatCallbackEventTypes.ActiveRestored,

            // 收银台·应用版本付费订单回调族 6 键（91929~91933 / 99353 第三方 · 99387~99392 代开发，套件信封 InfoType）。
            WechatCallbackEventTypes.OpenOrder, WechatCallbackEventTypes.ChangeOrder,
            WechatCallbackEventTypes.PayForAppSuccess, WechatCallbackEventTypes.Refund,
            WechatCallbackEventTypes.ChangeEditon, WechatCallbackEventTypes.CancelOrder,

            // 接口调用许可族 4 键（97195~97198，仅代开发；97195 Event 信封逐键自指，其余套件信封 InfoType）。
            WechatCallbackEventTypes.UnlicensedNotify, WechatCallbackEventTypes.LicensePaySuccess,
            WechatCallbackEventTypes.LicenseRefund, WechatCallbackEventTypes.AutoActivate,

            // 邮箱族 2 键（97495/97517/97506 + 100180，族事件值为键）。
            WechatCallbackEventTypes.AppEmailChange, WechatCallbackEventTypes.PublicEmailChange,

            // 文档族 5 键（doc_change 的 ChangeType；97833~97835/98095/98096 等）。
            WechatCallbackEventTypes.DocMemberChange, WechatCallbackEventTypes.DeleteDoc,
            WechatCallbackEventTypes.FormComplete, WechatCallbackEventTypes.DeleteForm,
            WechatCallbackEventTypes.FormSettingsChange,

            // 智能表格族 6 键（smart_sheet_change 的 ChangeType；100986/100987 等）。
            WechatCallbackEventTypes.AddFiled, WechatCallbackEventTypes.UpdateFiled,
            WechatCallbackEventTypes.DeleteFiled, WechatCallbackEventTypes.AddRecord,
            WechatCallbackEventTypes.UpdateRecord, WechatCallbackEventTypes.DeleteRecord,

            // 日程族 5 键（Event 节点即事件键；97728/97730/97731/97732/98111 等）。
            WechatCallbackEventTypes.DeleteCalendar, WechatCallbackEventTypes.ModifyCalendar,
            WechatCallbackEventTypes.ModifySchedule, WechatCallbackEventTypes.DeleteSchedule,
            WechatCallbackEventTypes.RespondSchedule,

            // 会议族 30 键（meeting_change 的 ChangeType + meeting_statistics 的 start_meeting；
            // modify_meeting/cancel_meeting 三模式开放，其余仅自建）。
            WechatCallbackEventTypes.ModifyMeeting, WechatCallbackEventTypes.CancelMeeting,
            WechatCallbackEventTypes.MeetingStart, WechatCallbackEventTypes.MeetingEnd,
            WechatCallbackEventTypes.MeetingMuteAll, WechatCallbackEventTypes.MeetingUnmuteAll,
            WechatCallbackEventTypes.JoinMeeting, WechatCallbackEventTypes.QuitMeeting,
            WechatCallbackEventTypes.JoinMeetingBeforeHost, WechatCallbackEventTypes.JoinWaitingRoom,
            WechatCallbackEventTypes.OpenScreenShare, WechatCallbackEventTypes.CloseScreenShare,
            WechatCallbackEventTypes.QuitWaitingRoom, WechatCallbackEventTypes.JoinFromMeetingRoom,
            WechatCallbackEventTypes.MoveToWaitingRoom, WechatCallbackEventTypes.RoleChange,
            WechatCallbackEventTypes.WebinarRoleChange, WechatCallbackEventTypes.WebinarWarmUpUpload,
            WechatCallbackEventTypes.PstnStatusUpdate, WechatCallbackEventTypes.MediumUpload,
            WechatCallbackEventTypes.StartRecording, WechatCallbackEventTypes.PauseRecording,
            WechatCallbackEventTypes.ResumeRecording, WechatCallbackEventTypes.StopRecording,
            WechatCallbackEventTypes.RecordingComplete, WechatCallbackEventTypes.DeleteRecording,
            WechatCallbackEventTypes.Enroll, WechatCallbackEventTypes.CancelEnroll,
            WechatCallbackEventTypes.MeetingRoomResponse, WechatCallbackEventTypes.StartMeeting,

            // 家校沟通族 2 键（92032/92052/92050/92051/96716/96717 族事件值 + 97281 批量）。
            WechatCallbackEventTypes.ChangeSchoolContact, WechatCallbackEventTypes.ChangeSchoolContactBatch,

            // 会话内容存档族 1 键（95039，仅自建）。
            WechatCallbackEventTypes.MsgAuditNotify,

            // 微盘族 9 键（wedrive_space_change 3 键 + wedrive_file_change 5 键 + 容量不足 1 键；
            // 97898~97903 / 97972~97978 / 97932~97937）。
            WechatCallbackEventTypes.WedriveInsufficientCapacity,
            WechatCallbackEventTypes.DismissSpace, WechatCallbackEventTypes.SpaceMemberChange,
            WechatCallbackEventTypes.SpaceSecuritySettingsChange,
            WechatCallbackEventTypes.CreateFile, WechatCallbackEventTypes.RenameFile,
            WechatCallbackEventTypes.UpdateFile, WechatCallbackEventTypes.DeleteFile,
            WechatCallbackEventTypes.MoveFile,

            // 直播族 1 键（living_status_change；Event 节点即事件键）。
            WechatCallbackEventTypes.LivingStatusChange,

            // OA 审批族 1 键（sys_approval_change；Event 节点即事件键，载荷在 ApprovalInfo 内）。
            WechatCallbackEventTypes.SysApprovalChange,
        };

        var registered = registry.RegisteredKeys;
        registered.Should().HaveCount(120,
            "官方有强类型载荷的事件键共 120 个（17 + 安全管理 1 + 微信客服 1 + 客户联系/获客族 5 + 90240 的 24" +
            " + 应用版本付费订单回调族 6 + 接口调用许可族 4 + 邮箱族 2 + 文档族 5 + 智能表格族 6 + 日程族 5" +
            " + 会议族 30 + 家校沟通族 2 + 会话内容存档 1 + 微盘族 9 + 直播族 1 + OA 审批族 1）");
        foreach (var key in expectedKeys)
        {
            registered.Should().Contain(key, $"官方事件键 {key} 必须登记契约");
            registry.TryResolve(key, out var contract).Should().BeTrue();
            contract!.Accessor.Should().NotBeNull();
        }

        // 授权族走信封（ADR-8），不得登记载荷契约。
        foreach (var authKey in new[]
                 {
                     WechatCallbackEventTypes.SuiteTicket, WechatCallbackEventTypes.CreateAuth,
                     WechatCallbackEventTypes.ResetPermanentCode, WechatCallbackEventTypes.ChangeAuth,
                     WechatCallbackEventTypes.CancelAuth, WechatCallbackEventTypes.DelAuth,
                 })
        {
            registered.Should().NotContain(authKey, $"授权族事件键 {authKey} 走信封，不得登记载荷契约（ADR-8）");
        }

        // 微信客服客服账号授权变更：官方 AuthAddOpenKfId/AuthDelOpenKfId 为同级重名多节点形态，
        // 现有声明面（Items 需「容器/子项」两层）无法无损表达 ⇒ 按 ADR-4 降级为 GenericCallbackPayload，
        // 不得以「仅取首个同名节点」的有损映射登记（静默丢字段正是本体系要消灭的缺陷）。
        // 待上游映射面支持重名兄弟聚合（如 RepeatedItems 形态）后，同批登记载荷并把它移入 expectedKeys。
        registered.Should().NotContain(WechatCallbackEventTypes.KfAccountAuthChange,
            "kf_account_auth_change 的重名多节点形态超出声明面，登记有损映射即静默丢字段（ADR-4 降级）");

        // P2 生成器一致性闸：生成物登记键集必须与载荷类 [WechatCallbackContract] 特性声明的并集一致 ——
        // 特性漏声明 / 生成器漂移在此红；官方契约面变更仍以 expectedKeys（硬编码官方清单）为权威锚点。
        var declaredKeys = typeof(OfficialPayloadContracts).Assembly.GetTypes()
            .SelectMany(t => t.GetCustomAttributes<WechatCallbackContractAttribute>(inherit: false))
            .SelectMany(a => a.EventTypes)
            .ToHashSet(StringComparer.Ordinal);
        declaredKeys.Should().HaveCount(120, "[WechatCallbackContract] 特性声明的事件键并集应为 120 个");
        registered.Should().BeEquivalentTo(declaredKeys, "生成器登记的键集必须与 [WechatCallbackContract] 特性声明并集一致（生成器漂移闸）");
    }

    /// <summary>
    /// 契约守卫 CB4c（v2.2 新增）：每条官方契约必须<b>显式</b>声明事件键级开放面（ADR-15；P2 后权威声明在载荷类 [WechatCallbackContract] 特性）。
    /// </summary>
    [Fact]
    public void OfficialPayloadContracts_ShouldDeclareOpenSurfaceExplicitly()
    {
        var registry = new WechatPayloadContractRegistry();
        OfficialPayloadContracts.RegisterAll(registry);

        foreach (var key in registry.RegisteredKeys)
        {
            registry.TryResolve(key, out var contract).Should().BeTrue();
            contract!.OpenSurfaces.Should().NotBeEmpty(
                $"契约 {key} 必须显式声明事件键级开放面（不得隐式继承族默认，见 ADR-15）");
            foreach (var surface in contract.OpenSurfaces)
            {
                surface.SupportedAppTypes.Should().NotBe(WechatAppTypeSet.None,
                    $"契约 {key} 的开放面组合对不得为空模式集合");
            }

            contract.RequiredEvent.Should().NotBeNull($"契约 {key} 必须声明 RequiredEvent（防同名 ChangeType 跨族串门）");
        }

        // 三模式开放面（ADR-14：一份契约覆盖三类应用）。
        registry.TryResolve(WechatCallbackEventTypes.CreateUser, out var contact).Should().BeTrue();
        contact!.OpenSurfaces.Should().ContainSingle("通讯录族官方接入方式单一（应用数据通道）");
        contact.OpenSurfaces[0].SupportedAppTypes.Should().Be(WechatAppTypeSet.All);
        contact.OpenSurfaces[0].RequiredChannel.Should().Be(WechatCallbackChannel.App);

        registry.TryResolve(WechatCallbackEventTypes.CreateChain, out var chain).Should().BeTrue();
        chain!.OpenSurfaces.Should().ContainSingle();
        chain.OpenSurfaces[0].SupportedAppTypes.Should().Be(WechatAppTypeSet.Internal,
            "上下游变更族官方仅向自建应用开放");
        chain.OpenSurfaces[0].RequiredChannel.Should().Be(WechatCallbackChannel.App);

        registry.TryResolve(WechatCallbackEventTypes.ChangeDomainIp, out var security).Should().BeTrue();
        security!.OpenSurfaces.Should().ContainSingle();
        security.OpenSurfaces[0].SupportedAppTypes.Should().Be(WechatAppTypeSet.Internal,
            "安全管理族官方仅向自建应用开放（100080：第三方/代开发暂不支持）");
        security.OpenSurfaces[0].RequiredChannel.Should().Be(WechatCallbackChannel.App);

        registry.TryResolve(WechatCallbackEventTypes.KfMsgOrEvent, out var kf).Should().BeTrue();
        kf!.OpenSurfaces.Should().ContainSingle();
        kf.OpenSurfaces[0].SupportedAppTypes.Should().Be(WechatAppTypeSet.All,
            "微信客服族三类应用均可接收（94670/94699/96426：自建配置 + 第三方/代开发权限）");
        kf.OpenSurfaces[0].RequiredChannel.Should().Be(WechatCallbackChannel.App);

        registry.TryResolve(WechatCallbackEventTypes.ChangeSchoolContact, out var school).Should().BeTrue();
        school!.OpenSurfaces.Should().HaveCount(2,
            "家校沟通族的官方接入方式按应用模式分通道（92032/92052/96716/96717 App 通道 + 92050/92051/97281 套件通道）");
        school.OpenSurfaces.Should().Contain(s =>
            s.SupportedAppTypes == (WechatAppTypeSet.Internal | WechatAppTypeSet.Provider) &&
            s.RequiredChannel == WechatCallbackChannel.App);
        school.OpenSurfaces.Should().Contain(s =>
            s.SupportedAppTypes == WechatAppTypeSet.ThirdParty &&
            s.RequiredChannel == WechatCallbackChannel.Suite);
    }

    /// <summary>
    /// 契约守卫 CB4e（v2.2 落地）：注册期<b>不得宽于官方族默认</b>的 fail-fast 校验（ADR-15）。
    /// </summary>
    /// <remarks>
    /// 断言的是**校验器真实生效**（而非仅「代码存在」）：
    /// ① 官方契约表自身通过校验（正向）；② 故意把「仅自建」的上下游族键声明为三类应用全开放 ⇒ 必须抛；
    /// ③ 通道与官方不一致 ⇒ 必须抛；④ 无官方基线的 <c>Unknown</c> 族 ⇒ 不校验（宿主私有事件不受误伤）；
    /// ⑤ 客户联系族声明放宽为三类应用 × 应用通道 ⇒ 必须抛（官方矩阵按模式分通道）；
    /// ⑥ 客户联系族的官方双组合对声明 ⇒ 通过。
    /// 第 ④ 项同时是**防空转**对照：若校验器写成「一律抛」，本用例会失败。
    /// </remarks>
    [Fact]
    public void ContractRegistration_ShouldRejectOpenSurfaceWiderThanFamilyDefault()
    {
        var accessor = ContactUserChangedPayload.PayloadFieldMap is IPayloadContractAccessor a
            ? a
            : throw new InvalidOperationException("测试前置：载荷映射表须实现 IPayloadContractAccessor。");

        // ① 正向：官方声明（All + App，族为 ContactChange）通过。
        var official = WechatPayloadContract.CreateWithOpenSurface(
            WechatCallbackEventTypes.CreateUser, accessor,
            WechatAppTypeSet.All, WechatCallbackChannel.App,
            WechatCallbackEventTypes.ChangeContact, WechatCallbackEventFamily.ContactChange);
        official.OpenSurfaces.Should().ContainSingle().Which.SupportedAppTypes.Should().Be(WechatAppTypeSet.All);

        // ② 越权放宽：上下游族（官方仅自建）被声明为三类应用全开放 ⇒ 必须 fail-fast。
        var widened = () => WechatPayloadContract.CreateWithOpenSurface(
            WechatCallbackEventTypes.CreateChain, accessor,
            WechatAppTypeSet.All, WechatCallbackChannel.App,
            WechatCallbackEventTypes.ChangeChain, WechatCallbackEventFamily.ChainChange);
        widened.Should().Throw<ArgumentException>()
            .WithMessage("*宽于*", "CB4e：声明宽于官方族默认必须在组合根期失败（否则事件键闸形同虚设）");

        // ③ 通道不一致（官方为 Suite 的授权族被声明为 App）⇒ 必须 fail-fast。
        var wrongChannel = () => WechatPayloadContract.CreateWithOpenSurface(
            "suite_ticket", accessor,
            WechatAppTypeSet.ThirdParty | WechatAppTypeSet.Provider, WechatCallbackChannel.App,
            requiredEvent: null, requiredFamily: WechatCallbackEventFamily.Authorization);
        wrongChannel.Should().Throw<ArgumentException>().WithMessage("*通道*");

        // ④ 无官方基线（Unknown 族）⇒ 不校验，宿主私有事件可自由声明。
        var custom = WechatPayloadContract.CreateWithOpenSurface(
            "host_private_event", accessor,
            WechatAppTypeSet.ThirdParty | WechatAppTypeSet.Provider, WechatCallbackChannel.App,
            requiredEvent: "host_private_event", requiredFamily: WechatCallbackEventFamily.Unknown);
        custom.OpenSurfaces.Should().ContainSingle().Which.SupportedAppTypes.Should().Be(
            WechatAppTypeSet.ThirdParty | WechatAppTypeSet.Provider,
            "CB4e：官方未文档化的事件键无基线可比对，声明由宿主负责（不得误伤）");

        // ⑤ 客户联系族（官方矩阵按模式分通道）被声明为「三类应用 × 应用通道」⇒ 通道维度已矛盾 ⇒ fail-fast。
        var externalWidened = () => WechatPayloadContract.CreateWithOpenSurface(
            WechatCallbackEventTypes.ChangeExternalContact, accessor,
            WechatAppTypeSet.All, WechatCallbackChannel.App,
            WechatCallbackEventTypes.ChangeExternalContact, WechatCallbackEventFamily.ExternalContactChange);
        externalWidened.Should().Throw<ArgumentException>().WithMessage("*宽于*",
            "CB4e：App 通道的官方模式集合为自建·代开发，三方越权即宽于官方默认");

        // ⑥ 正向：客户联系族的官方双组合对声明（自建·代开发×App + 第三方×Suite）通过。
        var externalOfficial = WechatPayloadContract.CreateWithOpenSurfaces(
            WechatCallbackEventTypes.ChangeExternalContact, accessor,
            new[]
            {
                new WechatOpenSurface(
                    WechatAppTypeSet.Internal | WechatAppTypeSet.Provider, WechatCallbackChannel.App),
                new WechatOpenSurface(WechatAppTypeSet.ThirdParty, WechatCallbackChannel.Suite),
            },
            WechatCallbackEventTypes.ChangeExternalContact, WechatCallbackEventFamily.ExternalContactChange);
        externalOfficial.OpenSurfaces.Should().HaveCount(2,
            "客户联系/获客族的官方接入方式按应用模式分通道 ⇒ 单一组合对无法表达");
    }

    /// <summary>
    /// 契约守卫 CB4d（v2.2 新增；P2 修订）：三模式无关性（ADR-14）—— 载荷与转换器层<b>不得</b>出现应用模式分支。
    /// <para>
    /// P2 修订：载荷类上的 <c>[WechatCallbackContract]</c> 特性声明（开放面<b>数据</b>，与原契约表同性质）
    /// 不属「分支」，扫描前剔除该特性块 —— <c>WechatAppType</c>/<c>WechatCallbackChannel</c> 出现在
    /// <c>if</c>/<c>switch</c>/<c>?:</c> 等行为分支处仍会被本断言打红。
    /// </para>
    /// </summary>
    [Fact]
    public void PayloadSurface_MustBeAppModeAgnostic()
    {
        var payloadTypes = new[]
        {
            typeof(ContactUserChangedPayload), typeof(ContactPartyChangedPayload),
            typeof(ContactTagChangedPayload), typeof(BatchJobCompletedPayload),
            typeof(ChainChangedPayload), typeof(GenericCallbackPayload),
            typeof(PlainEventPayload), typeof(AgentAlertPayload),
            typeof(MenuScanCodePayload), typeof(MenuPicPayload),
            typeof(MenuLocationSelectPayload), typeof(LocationReportedPayload),
            typeof(ApprovalStatusChangedPayload), typeof(TemplateCardEventPayload),
            typeof(ExternalContactChangedPayload), typeof(ExternalChatChangedPayload),
            typeof(ExternalTagChangedPayload), typeof(CustomerAcquisitionPayload),
            typeof(SecurityDomainIpChangedPayload), typeof(KfMsgOrEventPayload), typeof(WechatPayloadConverter),
            typeof(SchoolContactChangedPayload), typeof(SchoolContactBatchChangedPayload),
            typeof(MsgAuditNotifyPayload),
            typeof(UnlicensedNotifyPayload), typeof(LicenseOrderPayload), typeof(LicenseAutoActivatePayload),
        };

        foreach (var type in payloadTypes)
        {
            var source = ReadSource(type);
            // 剔除声明性开放面特性块（[WechatCallbackContract(...)] 跨多行）后再做文本扫描。
            var codeWithoutContractAttributes = Regex.Replace(
                source, @"\[WechatCallbackContract\(.*?\)\]", string.Empty,
                RegexOptions.Singleline | RegexOptions.CultureInvariant);
            codeWithoutContractAttributes.Should().NotContain("WechatAppType",
                $"{type.Name} 不得按应用类型分支（ADR-14：一份契约覆盖三模式；特性声明中的开放面数据除外）");
            codeWithoutContractAttributes.Should().NotContain("WechatCallbackChannel",
                $"{type.Name} 不得按回调通道分支（ADR-14；特性声明中的开放面数据除外）");
        }
    }

    // ---------------------------------------------------------------- CB5

    /// <summary>
    /// 契约守卫 CB5：回调凭据唯一来源 = <see cref="Mud.Wechat.Work.Callback.WechatCallbackOptions.Apps"/>
    /// （v1.2 D3）——不回流 <see cref="Mud.Wechat.Work.Abstractions.Configuration.WechatAppConfig"/>；
    /// 应用级配置不得有 AppKey 属性（字典键唯一权威，v1.2）。
    /// </summary>
    [Fact]
    public void CallbackOptions_ShouldKeepSingleSourceOfCredentialTruth()
    {
        var appConfigPath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Work.Abstractions", "Configuration", "WechatAppConfig.cs");
        var appConfigSource = File.ReadAllText(appConfigPath);
        // 仅断言属性声明形态（文档注释中允许提及回调凭据的迁移史）。
        appConfigSource.Should().NotContain("public string PushToken", "回调凭据不得回流主配置（回调运维面独立）");
        appConfigSource.Should().NotContain("public string PushEncodingAESKey", "回调凭据不得回流主配置");

        var callbackOptionsPath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Work.Callback", "WechatCallbackOptions.cs");
        var optionsSource = File.ReadAllText(callbackOptionsPath);

        optionsSource.Should().Contain("public Dictionary<string, WechatAppCallbackOptions> Apps",
            "D3：Apps 为回调凭据唯一来源（多应用）");
        optionsSource.Should().Contain("class WechatAppCallbackOptions",
            "应用级配置类与主配置同类文件（audit-config-keys.ps1 按文件扫描）");

        // 「单体兼容字段已删除」断言限定在 WechatCallbackOptions 主类段内
        // （WechatAppCallbackOptions 应用级的同名属性是合法凭据承载）。
        var appOptionsIndex = optionsSource.IndexOf("class WechatAppCallbackOptions", StringComparison.Ordinal);
        var mainClassSection = optionsSource.Substring(0, appOptionsIndex);
        mainClassSection.Should().NotContain("public string PushToken", "v1.2：单体兼容字段已删除");
        mainClassSection.Should().NotContain("public string PushEncodingAESKey", "v1.2：单体兼容字段已删除");
        mainClassSection.Should().NotContain("public string CorpId", "v1.2：单体兼容字段已删除");

        // 应用级配置类段内不得有 AppKey 属性（字典键唯一权威，防双写死配置）。
        var appOptionsStart = optionsSource.IndexOf("class WechatAppCallbackOptions", StringComparison.Ordinal);
        appOptionsStart.Should().BeGreaterThan(0, "应用级配置类与主配置同类文件（audit-config-keys.ps1 按文件扫描）");
        var appOptionsSection = optionsSource.Substring(appOptionsStart);
        var closingBrace = appOptionsSection.IndexOf('}');
        var appOptionsBody = closingBrace > 0
            ? appOptionsSection.Substring(0, closingBrace)
            : appOptionsSection;
        appOptionsBody.Should().NotContain("public string AppKey", "v1.2：AppKey 与字典键双写属死配置形态");
    }

    // ---------------------------------------------------------------- CB6

    /// <summary>
    /// 契约守卫 CB6（协议文本，v1 方案 §5.1）：GET echostr 验签复用 <c>VerifySignature</c>+<c>Decrypt</c>
    /// 且不消费抗重放指纹（v1.2 D8）；被动应答扩展点的 <c>Encrypt</c>+<c>ComputeSignature</c> 算法保持。
    /// </summary>
    [Fact]
    public void EchoAndPassiveReply_ShouldMatchOfficialProtocol()
    {
        var receiverPath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Work.Callback", "WechatCallbackReceiver.cs");
        var receiverSource = File.ReadAllText(receiverPath);

        // EchoAsync 方法体：复用 VerifySignature + Decrypt，且不触碰指纹守卫。
        var echoStart = receiverSource.IndexOf("public Task<string> EchoAsync", StringComparison.Ordinal);
        echoStart.Should().BeGreaterThan(0, "URL 验证入口必须存在（官方 90930 硬门槛）");
        var echoEnd = receiverSource.IndexOf("private WechatAppCallbackOptions ResolveApp", StringComparison.Ordinal);
        var echoBody = echoEnd > echoStart
            ? receiverSource.Substring(echoStart, echoEnd - echoStart)
            : receiverSource.Substring(echoStart);
        echoBody.Should().Contain("VerifySignature", "echo 验签复用现有算法（echostr 充当 encrypt 参与项）");
        echoBody.Should().Contain("Decrypt", "echo 解密复用现有算法");
        echoBody.IndexOf("VerifySignature", StringComparison.Ordinal)
            .Should().BeLessThan(echoBody.IndexOf("Decrypt", StringComparison.Ordinal),
                "P1-2：URL 验证中验签必须先于解密（攻击者无 token 不得触达解密）");
        echoBody.Should().NotContain("TryMarkAsync", "D8：echo 不消费抗重放指纹（幂等验证）");
        echoBody.Should().Contain("ValidateTimestampWindow", "时效窗口闸保持 fail-closed");

        // 被动应答扩展点（本期不实现组装，算法基座不得移除）。
        // v3：加解密内核已下沉叶层（协议与安全内核单一事实来源），断言路径随之迁移。
        var cryptoPath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Abstractions", "Callback", "WechatCallbackCrypto.cs");
        var cryptoSource = File.ReadAllText(cryptoPath);
        cryptoSource.Should().Contain("public static string Encrypt(", "被动应答包 Encrypt 算法基座");
        cryptoSource.Should().Contain("public static string ComputeSignature(", "MsgSignature 算法基座");
    }

    // ---------------------------------------------------------------- CB7

    /// <summary>
    /// 契约守卫 CB7：Abstractions 的回调契约层不得出现 XML 类型
    /// （v1 方案 §3.1 上移边界——信封仅字段契约，不向 Abstractions 引入 System.Xml.Linq 依赖）。
    /// </summary>
    /// <remarks>
    /// <b>v2.2 修正（原始实现是守卫盲区）</b>：原断言用
    /// <c>Directory.GetFiles(dir, "*.cs")</c> —— <b>非递归</b>。
    /// v2.2 新增 <c>Callback/Payloads/</c> 子目录后，该目录下的文件<b>完全脱离</b>本守卫覆盖。
    /// 现改为 <c>SearchOption.AllDirectories</c> 并排除 <c>obj</c>/<c>bin</c>，
    /// 以覆盖整个回调契约层（含载荷端口层）。
    /// </remarks>
    [Fact]
    public void CallbackEnvelope_ShouldNotExposeXmlTypes()
    {
        var callbackDir = Path.Combine(GetSolutionRoot(), "Mud.Wechat.Work.Abstractions", "Callback");
        Directory.Exists(callbackDir).Should().BeTrue($"未找到 Abstractions 回调契约目录：{callbackDir}");

        var files = Directory.GetFiles(callbackDir, "*.cs", SearchOption.AllDirectories)
            .Where(f => !f.Contains(
                            Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar,
                            StringComparison.OrdinalIgnoreCase)
                        && !f.Contains(
                            Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar,
                            StringComparison.OrdinalIgnoreCase))
            .ToArray();

        files.Should().NotBeEmpty("回调契约层应至少含一个源文件（守卫空转即失效）");

        foreach (var file in files)
        {
            foreach (var line in File.ReadAllLines(file))
            {
                // 跳过注释行（含 ///）：XML 文档注释中常出现「不得出现 XElement」这类
                // **说明性文字**，朴素物理行匹配会把它误判为实现依赖。
                if (line.TrimStart().StartsWith("//", StringComparison.Ordinal))
                    continue;

                line.Should().NotContain(NoXmlMarkers.Namespace, $"{file} 不得引入 XML 命名空间");
                NoXmlMarkers.AssertNoXmlType(line, file);
            }
        }
    }

    /// <summary>
    /// 契约守卫 CB14（v2.2 新增；v3 收敛）：Mud.Wechat 的 XML 触点必须唯一 ——
    /// <c>WechatCallbackReceiver</c>（请求体 <c>Encrypt</c> 提取）之外的文件不得出现 XML 类型。
    /// </summary>
    /// <remarks>
    /// <b>v3</b>：载荷投影 <c>XElementPayloadSource</c> 已随「协议与安全内核」下沉
    /// <c>Mud.Wechat.Abstractions/Callback</c>，故本包允许清单由 2 项收敛为 1 项；
    /// 叶层另有等价守卫（CB-L1i：叶层回调子域内 XML 触点唯一）。
    /// </remarks>
    [Fact]
    public void CallbackPackage_ShouldKeepXmlTouchPointsUnique()
    {
        var allowed = new[] { "WechatCallbackReceiver.cs" };

        var files = Directory.GetFiles(
                Path.Combine(GetSolutionRoot(), "Mud.Wechat.Work.Callback"), "*.cs", SearchOption.AllDirectories)
            .Where(f => !f.Contains(
                            Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar,
                            StringComparison.OrdinalIgnoreCase)
                        && !f.Contains(
                            Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar,
                            StringComparison.OrdinalIgnoreCase))
            .ToArray();

        foreach (var file in files)
        {
            var name = Path.GetFileName(file);
            if (allowed.Contains(name, StringComparer.Ordinal))
                continue;

            foreach (var line in File.ReadAllLines(file))
            {
                // 同 CB7：跳过注释行，避免「说明性文字」被误判为实现依赖。
                if (line.TrimStart().StartsWith("//", StringComparison.Ordinal))
                    continue;

                NoXmlMarkers.AssertNoXmlType(line, $"{name}（XML 触点须唯一，ADR-6）");
            }
        }
    }

    /// <summary>
    /// XML 依赖标记与匹配规则（CB7 / CB14 共用）。
    /// </summary>
    /// <remarks>
    /// <b>必须用<b>词边界</b>正则而非 <c>Contains</c></b>：本仓库存在标识符
    /// <c>XElementPayloadSource</c>（合法的 XML 触点类名），其<b>包含子串</b> "XElement" ——
    /// 朴素子串匹配会把引用该类的文件误判为「引入 XML 类型」。
    /// <c>\bXElement\b</c> 要求其后为非单词字符，故不会命中 <c>XElementPayloadSource</c>。
    /// </remarks>
    private static class NoXmlMarkers
    {
        internal const string Namespace = "System.Xml.Linq";

        private static readonly string[] TypePatterns = { @"\bXDocument\b", @"\bXElement\b" };

        internal static void AssertNoXmlType(string line, string subject)
        {
            foreach (var pattern in TypePatterns)
            {
                Regex.IsMatch(line, pattern).Should().BeFalse(
                    subject + " 不得出现 XML 类型（匹配 " + pattern + "）：" + line.Trim());
            }
        }
    }

    // ---------------------------------------------------------------- CB8

    /// <summary>
    /// 契约守卫 CB8（P0-1，对齐《回调解决方案 v1》§七）：加解密不得出现 .NET 内置 16 块 PKCS7
    /// （官方报文 pad∈[17..32] 时内置校验会误判非法填充而解密失败）；必须
    /// <c>PaddingMode.None</c> + 手工 32 块填充补位/剥离。
    /// </summary>
    [Fact]
    public void CallbackCrypto_ShouldUseManualPkcs7PaddingOf32Bytes()
    {
        // v3：加解密内核已下沉叶层 Mud.Wechat.Abstractions/Callback（守卫随源迁址，见 CB-MP-10）。
        var cryptoPath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Abstractions", "Callback", "WechatCallbackCrypto.cs");
        File.Exists(cryptoPath).Should().BeTrue($"未找到回调加解密实现：{cryptoPath}");

        var source = File.ReadAllText(cryptoPath);

        source.Should().NotContain("PaddingMode.PKCS7",
            "CB8：.NET 内置 PKCS7 为 16 字节块校验，官方报文 pad∈[17..32] 时解密必抛（P0-1 回归守卫）");
        source.Should().Contain("PaddingMode.None",
            "CB8：解密/加密必须走 PaddingMode.None + 手工 32 块填充补位/剥离");
        source.Should().Contain("StripPkcs7Padding",
            "CB8：Decrypt 必须手工剥离 32 块 PKCS7 填充（否则尾部填充并入 receiveid）");
    }

    // ---------------------------------------------------------------- CB9

    /// <summary>
    /// 契约守卫 CB9（P1-1/D2）：接收器的指纹闸（<c>TryMarkAsync</c>）必须位于
    /// <c>WechatCallbackCrypto.Decrypt</c> 之后——防「标记先于解密」回归
    /// （解密失败消耗指纹 ⇒ 官方重试被拒，at-least-once 破坏）。
    /// </summary>
    [Fact]
    public void CallbackFingerprintMark_ShouldFollowDecrypt()
    {
        var receiverPath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Work.Callback", "WechatCallbackReceiver.cs");
        var source = File.ReadAllText(receiverPath);

        var markIndex = source.IndexOf("TryMarkAsync", StringComparison.Ordinal);
        var decryptIndex = source.IndexOf("WechatCallbackCrypto.Decrypt(", StringComparison.Ordinal);

        markIndex.Should().BePositive("CB9：接收器必须调用 TryMarkAsync（P0-2 第二道闸不得被移除）");
        decryptIndex.Should().BePositive("CB9：接收器必须经 WechatCallbackCrypto.Decrypt 解密");
        markIndex.Should().BeGreaterThan(decryptIndex,
            "CB9：指纹标记必须位于解密之后（P1-1/D2：解密失败不消耗指纹，官方重试可重新进入管线）");
    }

    // ---------------------------------------------------------------- CB10

    /// <summary>
    /// 契约守卫 CB10（区分企业自建 / 服务商代开发 / 第三方应用）：回调通道枚举
    /// <c>WechatCallbackChannel</c>（App=1 应用数据通道 / Suite=2 套件指令通道）必须存在，
    /// 且 <c>WechatAppCallbackOptions</c> 必须暴露 <c>AppType</c> / <c>Channel</c> / <c>ReceiveId</c>
    /// 三个配置面属性——这是「应用类型 × 回调通道」语义（receiveid 校验 + 开放面闸）的类型契约。
    /// </summary>
    [Fact]
    public void CallbackAppOptions_ShouldExposeAppTypeAndChannelSurface()
    {
        var channelPath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Work.Abstractions", "Enums", "WechatCallbackChannel.cs");
        File.Exists(channelPath).Should().BeTrue($"未找到回调通道枚举（勿移动文件，CB10 按路径断言）：{channelPath}");
        var channelSource = File.ReadAllText(channelPath);
        channelSource.Should().Contain("App = 1", "CB10：应用数据通道取值（应用级 change_contact/batch_job_result/change_chain）");
        channelSource.Should().Contain("Suite = 2", "CB10：套件指令/票据通道取值（suite_ticket / 授权族）");

        var optionsPath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Work.Callback", "WechatCallbackOptions.cs");
        var optionsSource = File.ReadAllText(optionsPath);
        optionsSource.Should().Contain("public WechatAppType AppType", "CB10：应用类型配置面（区分企业自建/第三方/代开发）");
        optionsSource.Should().Contain("public WechatCallbackChannel Channel", "CB10：回调通道配置面（App/Suite）");
        optionsSource.Should().Contain("public string ReceiveId", "CB10：接收方 ID 配置面（receiveid 校验依据）");
    }

    // ---------------------------------------------------------------- CB11

    /// <summary>
    /// 契约守卫 CB11：<c>ValidateReceiveId</c> 必须按「应用类型 × 回调通道」三元分流 receiveid 语义——
    /// 自建 App 通道 / 第三方·代开发 Suite 通道为<b>静态</b>接收方 ID（比对 <see cref="Mud.Wechat.Work.Callback.WechatAppCallbackOptions.ReceiveId"/>），
    /// 第三方·代开发 App 通道为<b>动态授权企业 CorpId</b>（比对外层 <c>ToUserName</c>，静态 ReceiveId 命中其一亦通过）。
    /// </summary>
    [Fact]
    public void ReceiveIdValidation_ShouldDistinguishAppTypeByChannel()
    {
        var receiverPath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Work.Callback", "WechatCallbackReceiver.cs");
        var source = File.ReadAllText(receiverPath);

        source.Should().Contain("ValidateReceiveId(", "CB11：receiveid 校验必须为独立方法（单点收敛）");
        source.Should().Contain(
            "app.AppType != WechatAppType.Internal && app.Channel == WechatCallbackChannel.App",
            "CB11：第三方/代开发「应用数据通道」必须是动态授权企业 CorpId 分支");
        source.Should().Contain("toUserName",
            "CB11：动态授权企业 CorpId 必须与外层 ToUserName 比对");
        source.Should().Contain("string.Equals(expected, receiveId",
            "CB11：静态通道（自建 App / 第三方·代开发 Suite）必须比对配置的 ReceiveId");
        source.Should().Contain("WechatCallbackFailureKind.ReceiveIdMismatch",
            "CB11：receiveid 不一致必须显式拒绝（fail-closed）");
    }

    // ---------------------------------------------------------------- CB12

    /// <summary>
    /// 契约守卫 CB12：<c>IsEventFamilyAllowed</c> 必须实现「应用类型 × 回调通道」的开放面合法性矩阵——
    /// 授权族仅套件通道（第三方/代开发）、上下游变更族仅自建应用 + 应用通道、通讯录/异步族经应用通道（三类应用）、
    /// 无法判别族不拦截。
    /// </summary>
    [Fact]
    public void EventFamilyGate_ShouldEnforceOpenSurfaceMatrix()
    {
        var optionsPath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Work.Callback", "WechatCallbackOptions.cs");
        var source = File.ReadAllText(optionsPath);

        source.Should().Contain("case WechatCallbackEventFamily.Authorization:", "CB12：授权族分支配齐");
        source.Should().Contain("Channel == WechatCallbackChannel.Suite", "CB12：授权族仅套件通道");
        source.Should().Contain(
            "AppType == WechatAppType.ThirdParty || AppType == WechatAppType.Provider",
            "CB12：套件通道仅第三方应用/服务商代开发");

        source.Should().Contain("case WechatCallbackEventFamily.ChainChange:", "CB12：上下游变更族分支配齐");
        source.Should().Contain(
            "Channel == WechatCallbackChannel.App && AppType == WechatAppType.Internal",
            "CB12：上下游变更族仅自建应用 + 应用通道（95796）");
        source.Should().Contain("case WechatCallbackEventFamily.SecurityChange:", "CB12：安全事件族分支配齐");

        source.Should().Contain("case WechatCallbackEventFamily.ContactChange:", "CB12：通讯录变更族分支配齐");
        source.Should().Contain("case WechatCallbackEventFamily.BatchJob:", "CB12：异步任务族分支配齐");
        source.Should().Contain("case WechatCallbackEventFamily.KfEvent:", "CB12：微信客服族分支配齐（三类应用 × 应用通道）");
        source.Should().Contain("case WechatCallbackEventFamily.ExternalContactChange:",
            "CB12：客户联系变更族分支配齐（官方矩阵按模式分通道，92277 指令回调 URL）");
        source.Should().Contain("case WechatCallbackEventFamily.CustomerAcquisition:",
            "CB12：获客助手族分支配齐");
        source.Should().Contain("case WechatCallbackEventFamily.SchoolContactChange:",
            "CB12：家校通讯录变更族分支配齐（官方矩阵按模式分通道，92050/92051/97281 指令回调 URL）");
        source.Should().Contain("case WechatCallbackEventFamily.Unknown:", "CB12：无法判别族不拦截（兜底处理器处置）");
    }

    // ---------------------------------------------------------------- CB13

    /// <summary>
    /// 契约守卫 CB13：分发器合法性闸（<c>IsEventFamilyAllowed</c>）必须位于拦截器 <c>BeforeHandleAsync</c>
    /// <b>之前</b>，且不适用事件族以 <c>WechatCallbackDispatchOutcome.Rejected</c> 返回（中间件映射 200，不触发重推）。
    /// </summary>
    [Fact]
    public void DispatcherFamilyGate_ShouldPrecedeInterceptors_AndReturnRejected()
    {
        var dispatcherPath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Work.Callback", "WechatCallbackDispatcher.cs");
        var source = File.ReadAllText(dispatcherPath);

        var gateIndex = source.IndexOf("IsEventFamilyAllowed", StringComparison.Ordinal);
        // 注意：枚举注释（「拦截器中断（BeforeHandleAsync 返回 false）」）也含字面量，须以「.BeforeHandleAsync(」
        // 锚定实际拦截器调用点，避免与文档注释误匹配（CB13 锚点漂移防误报）。
        var beforeIndex = source.IndexOf(".BeforeHandleAsync(", StringComparison.Ordinal);

        gateIndex.Should().BePositive("CB13：分发器必须调用 IsEventFamilyAllowed 合法性闸");
        beforeIndex.Should().BePositive("CB13：分发器必须保留拦截器 Before 阶段");
        gateIndex.Should().BeLessThan(beforeIndex,
            "CB13：合法性闸必须先于拦截器（不适用事件族不得触达业务拦截器/处理器）");
        source.Should().Contain("return WechatCallbackDispatchOutcome.Rejected;",
            "CB13：不适用事件族以 Rejected 返回（中间件映射 200，不触发企业微信重推）");
    }

    /// <summary>
    /// 契约守卫 CB13b（v2.2 落地）：<b>事件键级开放面闸</b>必须存在且先于拦截器，并且先于载荷读取。
    /// </summary>
    /// <remarks>
    /// <para>
    /// CB13 只锁了<b>族级</b>闸的次序。v2.2 新增的<b>事件键级</b>闸（ADR-15）是并列的第二道：
    /// 族级闸按「事件族」判定，而宿主注册新 <c>Event</c> 值会落 <c>Unknown</c> 族而被族闸放行
    /// ⇒ 必须有第二道按事件键的契约声明判定，且与族闸同样<b>先于拦截器</b>。
    /// </para>
    /// <para>
    /// 同时断言事件键闸先于<b>载荷读取</b>（<c>IWechatPayloadReader</c> 的调用点）：这是 §3.9.5
    /// 「<c>JobType</c> 级差异不得做成安全闸」的机器化表达 —— 闸不得后移到解析之后。
    /// </para>
    /// </remarks>
    [Fact]
    public void DispatcherEventKeyGate_ShouldExistAndPrecedeInterceptors()
    {
        var dispatcherPath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Work.Callback", "WechatCallbackDispatcher.cs");
        var source = File.ReadAllText(dispatcherPath);

        var familyGateIndex = source.IndexOf("IsEventFamilyAllowed", StringComparison.Ordinal);
        var keyGateIndex = source.IndexOf("IsOpenFor(", StringComparison.Ordinal);
        var beforeIndex = source.IndexOf(".BeforeHandleAsync(", StringComparison.Ordinal);
        var readerIndex = source.IndexOf("_payloadReader.Read(", StringComparison.Ordinal);

        familyGateIndex.Should().BePositive("CB13b：分发器必须保留族级闸");
        keyGateIndex.Should().BePositive(
            "CB13b：分发器必须调用事件键级闸 contract.IsOpenFor(...)（ADR-15；缺此闸则宿主注册新 Event 值会绕过开放面）");
        beforeIndex.Should().BePositive("CB13b：分发器必须保留拦截器 Before 阶段");

        familyGateIndex.Should().BeLessThan(beforeIndex, "CB13b：族级闸先于拦截器");
        keyGateIndex.Should().BeLessThan(beforeIndex,
            "CB13b：事件键级闸必须先于拦截器（两道闸都不得触达业务拦截器/处理器）");

        // §3.9.5：闸在解析前执行 —— 「载荷级安全闸」是被明确否决的过度设计。
        if (readerIndex > 0)
        {
            keyGateIndex.Should().BeLessThan(readerIndex,
                "CB13b：事件键闸必须先于载荷读取（禁止「解析后重判开放面」的载荷级安全闸，见方案 §3.9.5）");
        }

        // 事件键未登记时应落回族级闸结论（协议外报文不拦截，与 v1 行为一致）。
        source.Should().Contain("TryResolve(eventType",
            "CB13b：事件键闸须以「键未登记 ⇒ 落回族级闸」为默认（协议外报文不拦截）");
    }

    /// <summary>
    /// 契约守卫 CB23（v2.2 落地）：<b>禁止载荷级安全闸</b>（防 §3.9.5 的过度设计回流）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 载荷体系内**不得**出现「按载荷字段（如 <c>JobType</c>）判定是否接收事件」的代码路径。
    /// 官方开放面约束的对象是<b>事件面</b>（如「上下游变更回调仅自建」），而非某个字段值；
    /// 把字段级差异做成闸需把闸后移到解析之后，会打破「非法事件不触达业务」的不变量。
    /// </para>
    /// <para>
    /// 判定：<b>载荷层</b>（载荷类型 / 转换器）不得读取批次任务字段做接收判定；
    /// 该差异属<b>语义过滤</b>，只能出现在处理器内。
    /// </para>
    /// </remarks>
    [Fact]
    public void PayloadLayer_ShouldNotImplementSafetyGate()
    {
        var payloadLayer = new[]
        {
            typeof(ContactUserChangedPayload), typeof(ContactPartyChangedPayload),
            typeof(ContactTagChangedPayload), typeof(BatchJobCompletedPayload),
            typeof(ChainChangedPayload), typeof(GenericCallbackPayload),
            typeof(ExternalContactChangedPayload), typeof(ExternalChatChangedPayload),
            typeof(ExternalTagChangedPayload), typeof(CustomerAcquisitionPayload),
            typeof(SecurityDomainIpChangedPayload), typeof(KfMsgOrEventPayload),
            typeof(WechatPayloadConverter),
        };

        foreach (var type in payloadLayer)
        {
            var source = ReadSource(type);

            // 载荷层不得为「接收与否」做判定：不应出现分发结果类型或闸语义的引用。
            source.Should().NotContain("WechatCallbackDispatchOutcome",
                $"{type.Name} 不得参与分发判定（载荷层只做字段映射，见方案 §3.9.5）");
            source.Should().NotContain("IsEventFamilyAllowed",
                $"{type.Name} 不得引用族级闸（开放面判定只在分发器）");
            source.Should().NotContain("IsOpenFor(",
                $"{type.Name} 不得引用事件键级闸（开放面判定只在分发器）");
        }

        // JobType 级差异必须留在处理器可达的载荷字段上（而非闸），即 BatchJobCompletedPayload 保留 JobType 字段。
        typeof(BatchJobCompletedPayload)
            .GetProperty("JobType")
            .Should().NotBeNull("CB23：`import_chain_contact` 等 JobType 级差异是**语义过滤**（处理器内一行 if），" +
                                "故 JobType 必须作为普通载荷字段保留，而不是被闸消费");
    }

    // ---------------------------------------------------------------- CB24

    /// <summary>
    /// 契约守卫 CB24（回调处理器契约分析器方案）：<c>Mud.Wechat.Callback.Analyzers</c> 诊断型分析器契约面。
    /// <para>
    /// 编号说明：<c>CB14</c>（XML 触点唯一）与 <c>CB23</c>（禁止载荷级安全闸）已被占用，故本守卫顺延为 CB24。
    /// </para>
    /// <para>
    /// 分两半：① <b>文本/路径断言</b>——本工程不引用 Analyzers 程序集与 Microsoft.CodeAnalysis；
    /// ② <b>反射断言</b>——分析器只靠硬编码 metadata name 识别类型（不得引用被分析程序集），
    /// 该名称一旦漂移，规则会**静默失效**（GetTypeByMetadataName 返回 null ⇒ 全规则空跑），
    /// 故必须由本工程（引用真实 Abstractions）反射锁死。
    /// </para>
    /// </summary>
    [Fact]
    public void CallbackHandlerAnalyzer_ShouldDeclareMatchingDiagnostics()
    {
        var analyzersRoot = Path.Combine(GetSolutionRoot(), "Mud.Wechat.Callback.Analyzers");

        // ① 分析器源码声明的诊断 ID 集合 双向等于 AnalyzerReleases.Unshipped.md 的登记（防漏登/残留）。
        var analyzerSource = File.ReadAllText(Path.Combine(analyzersRoot, "WechatCallbackHandlerAnalyzer.cs"));
        var declaredIds = Regex.Matches(analyzerSource, @"id:\s*""(MUDCB\d+)""")
            .Cast<System.Text.RegularExpressions.Match>()
            .Select(m => m.Groups[1].Value)
            .ToHashSet(StringComparer.Ordinal);

        var unshippedSource = File.ReadAllText(Path.Combine(analyzersRoot, "AnalyzerReleases.Unshipped.md"));
        var registeredIds = Regex.Matches(unshippedSource, @"^(MUDCB\d+)\s+\|", RegexOptions.Multiline)
            .Cast<System.Text.RegularExpressions.Match>()
            .Select(m => m.Groups[1].Value)
            .ToHashSet(StringComparer.Ordinal);

        declaredIds.Should().NotBeEmpty("分析器必须声明至少一条诊断规则（否则 MUDCB 段空跑）");
        declaredIds.Should().BeEquivalentTo(registeredIds,
            "CB24：SupportAnalysis 声明的诊断 ID 与 AnalyzerReleases.Unshipped.md 登记必须双向一致" +
            "（声明未登记 → RS2007/RS2000；登记了已删规则 → 漂移）");

        // ② 分析器 csproj 形态：netstandard2.0 单 TFM / IsRoslynComponent / 不引用 Workspaces / 移除根 props 运行时依赖。
        var csprojSource = File.ReadAllText(Path.Combine(analyzersRoot, "Mud.Wechat.Callback.Analyzers.csproj"));
        csprojSource.Should().Contain("<TargetFrameworks>netstandard2.0</TargetFrameworks>",
            "CB24：Roslyn 分析器必须 netstandard2.0 单 TFM（跨宿主加载硬约束）");
        csprojSource.Should().Contain("<IsRoslynComponent>true</IsRoslynComponent>", "CB24：分析器工程标记");
        csprojSource.Should().Contain("<IsPackable>false</IsPackable>",
            "CB24：分析器不独立打包（随 Callback nupkg 内嵌）");
        csprojSource.Should().Contain("<EnforceExtendedAnalyzerRules>true</EnforceExtendedAnalyzerRules>",
            "CB24：须开启扩展分析器规则（RS1xxx 拦截分析器自身缺陷）");
        csprojSource.Should().Contain("<IncludeBuildOutput>false</IncludeBuildOutput>",
            "CB24：分析器无 lib 分发形态，不参与 lib 资产");
        csprojSource.Should().NotContain("PackageReference Include=\"Microsoft.CodeAnalysis.CSharp.Workspaces\"",
            "CB24：不得引用 Workspaces（命令行编译进程不加载该程序集，RS1038）");

        // 根 Directory.Build.props 按 netstandard2.0 注入的 5 个运行时依赖必须逐一移除（含 Options：
        // 它会传递引入 DI.Abstractions / Primitives / Bcl.AsyncInterfaces，并把 Options.SourceGeneration
        // 当作分析器加载进本工程自身编译，违背「分析器程序集最小化」意图）。
        foreach (var injected in new[]
                 {
                     "Microsoft.Extensions.DependencyInjection.Abstractions",
                     "Microsoft.Extensions.Logging.Abstractions",
                     "Microsoft.Extensions.Configuration.Binder",
                     "Microsoft.Extensions.Options",
                     "System.Text.Json",
                 })
        {
            csprojSource.Should().Contain($"<PackageReference Remove=\"{injected}\" />",
                $"CB24：分析器不得继承根 props 注入的 {injected}");
        }

        // AnalyzerReleases.Shipped.md 必须存在（空文件亦可）——缺失会让发布跟踪解析失败。
        File.Exists(Path.Combine(analyzersRoot, "AnalyzerReleases.Shipped.md"))
            .Should().BeTrue("CB24：AnalyzerReleases.Shipped.md 必须存在（RS2000发布跟踪需要成对文件）");

        // ③ Callback 含 analyzers/dotnet/cs 打包资产，直接引用本仓分析器 DLL（netstandard2.0 单 TFM，路径固定）。
        var callbackCsprojSource = File.ReadAllText(Path.Combine(
            GetSolutionRoot(), "Mud.Wechat.Work.Callback", "Mud.Wechat.Work.Callback.csproj"));
        callbackCsprojSource.Should().Contain("analyzers/dotnet/cs", "CB24：随包下发分析器资产");
        callbackCsprojSource.Should().Contain(
            "Mud.Wechat.Callback.Analyzers.dll",
            "CB24：打包 ItemGroup 引用本仓分析器 DLL（排除上游 Mud.HttpUtils.Generator 与本仓 Callback.Generator）");

        // ④ 工程登记：slnx 必须含分析器工程（否则 verify-build 步骤 1/2 的「随 slnx 构建」口径漏项）。
        File.ReadAllText(Path.Combine(GetSolutionRoot(), "Mud.Wechat.slnx"))
            .Should().Contain("Mud.Wechat.Callback.Analyzers/Mud.Wechat.Callback.Analyzers.csproj",
                "CB24：分析器工程必须在 slnx 的 /src/ 下登记");

        // ⑤ 狗粮面：Demo 必须以 Analyzer 形态引用分析器（普通引用不会在 Demo 源码上生效）。
        var demoRoot = Path.Combine(GetSolutionRoot(), "Demos", "Mud.Wechat.Work.ContactCallbackDemo");
        File.ReadAllText(Path.Combine(demoRoot, "Mud.Wechat.Work.ContactCallbackDemo.csproj"))
            .Should().Contain("OutputItemType=\"Analyzer\"",
                "CB24：Demo 狗粮面必须以 Analyzer 形态引用分析器，否则 4 条规则在 Demo 上不生效");

        // ⑥ Demo 处理器一律引用 WechatCallbackEventTypes 常量（方案 §7.3 第 4 条；字面量即 MUDCB004）。
        var handlerFiles = Directory.GetFiles(Path.Combine(demoRoot, "Handlers"), "*.cs");
        handlerFiles.Should().NotBeEmpty("CB24：Demo Handlers 目录为空，定位失败");
        foreach (var file in handlerFiles)
        {
            foreach (System.Text.RegularExpressions.Match key in Regex.Matches(
                         File.ReadAllText(file), @"SupportedEventType\s*=>\s*(?<expr>[^;]+)"))
            {
                key.Groups["expr"].Value.Trim().Should().NotStartWith("\"",
                    $"CB24：{Path.GetFileName(file)} 的 SupportedEventType 写成字符串字面量" +
                    $"（MUDCB004：字面量是笔误与漂移源），应引用 WechatCallbackEventTypes 常量");
            }
        }
    }

    /// <summary>
    /// 契约守卫 CB24 的另一半：分析器用于识别回调类型的<b>硬编码 metadata name</b>必须与真实类型一致。
    /// </summary>
    /// <remarks>
    /// 分析器按红线要求<b>不得引用被分析程序集</b>，故只能靠字符串形式的 metadata name 识别
    /// <c>WechatCallbackPayloadHandler&lt;T&gt;</c> / <c>IWechatCallbackEventHandler&lt;T&gt;</c> /
    /// <c>WechatCallbackContractAttribute</c> / <c>GenericCallbackPayload</c>。
    /// 一旦 Abstractions 改命名空间或改名，<c>GetTypeByMetadataName</c> 返回 <c>null</c> ⇒
    /// 规则<b>静默空跑</b>（0 诊断、无任何报错），只有本守卫（反射真实类型）能发现。
    /// </remarks>
    [Fact]
    public void CallbackHandlerAnalyzer_MetadataNames_ShouldMatchRuntimeTypes()
    {
        var analyzerSource = File.ReadAllText(Path.Combine(
            GetSolutionRoot(), "Mud.Wechat.Callback.Analyzers", "WechatCallbackHandlerAnalyzer.cs"));

        var expected = new (string ConstName, string MetadataName)[]
        {
            ("PayloadHandlerBaseMetadataName", typeof(WechatCallbackPayloadHandler<>).FullName!),
            ("TypedHandlerInterfaceMetadataName", typeof(IWechatCallbackEventHandler<>).FullName!),
            ("ContractAttributeMetadataName", typeof(WechatCallbackContractAttribute).FullName!),
            ("GenericPayloadMetadataName", typeof(GenericCallbackPayload).FullName!),
        };

        foreach (var (constName, metadataName) in expected)
        {
            metadataName.Should().NotBeNullOrWhiteSpace($"CB24：{constName} 对应的真实类型应可解析");
            analyzerSource.Should().Contain($"\"{metadataName}\"",
                $"CB24：分析器常量 {constName} 的 metadata name 与真实类型不一致" +
                $"（期望 \"{metadataName}\"）—— 不一致时 GetTypeByMetadataName 返回 null，规则静默空跑");
        }
    }

    /// <summary>
    /// 读取某个类型的源码文本（按「类型名 + .cs」在仓库内定位，排除 <c>obj</c>/<c>bin</c>）。
    /// </summary>
    /// <remarks>
    /// 用于「源码形态」类守卫（如三模式无关性）：这类约束无法用反射表达
    /// （反射只能看见成员，看不见方法体内的分支），只能以文本扫描锁定。
    /// </remarks>
    private static string ReadSource(Type type)
    {
        var root = GetSolutionRoot();
        var files = Directory.GetFiles(root, type.Name + ".cs", SearchOption.AllDirectories)
            .Where(f => !f.Contains(
                            Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar,
                            StringComparison.OrdinalIgnoreCase)
                        && !f.Contains(
                            Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar,
                            StringComparison.OrdinalIgnoreCase))
            .ToArray();

        files.Should().NotBeEmpty("找不到 " + type.Name + " 的源码文件（守卫 ReadSource 定位失败）");
        return File.ReadAllText(files[0]);
    }

    /// <summary>解决方案根目录定位（与 <c>WechatContractGuards.GetSolutionRoot</c> 同款判据）。</summary>
    private static string GetSolutionRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null && !File.Exists(Path.Combine(directory.FullName, "Mud.Wechat.slnx")))
        {
            directory = directory.Parent!;
        }

        return directory!.FullName;
    }
}
