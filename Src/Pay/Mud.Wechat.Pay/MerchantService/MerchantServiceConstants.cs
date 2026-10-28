// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.MerchantService;

/// <summary>
/// 投诉单状态（官方 <c>complaint_state</c>，string(30)）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012533431"/>（查询投诉单列表，
/// 2026-10-14 逐字段核验）。官方以「<c>PENDING-待处理</c>」这种<b>内联破折号</b>形式印在描述里，
/// 而非「可选取值」列表 ⇒ 判别值取破折号<b>之前</b>的英文段。
/// </para>
/// <para>
/// <b>状态推进由官方驱动、不是商户可写的</b>：首次 <c>response</c> 会把 <c>PENDING</c> 推到 <c>PROCESSING</c>，
/// <c>complete</c> 与「驳回退款」会把单据推到 <c>PROCESSED</c>；「全额退款到账」亦自动扭转为已处理完成。
/// 故本枚举<b>只用于读</b>，不得作为请求参数提交。
/// </para>
/// </remarks>
public static class MerchantServiceComplaintStates
{
    /// <summary>待处理（官方 <c>PENDING</c>）：新投诉，尚无任何商户回复。</summary>
    public const string Pending = "PENDING";

    /// <summary>处理中（官方 <c>PROCESSING</c>）：商户已首次回复用户。</summary>
    public const string Processing = "PROCESSING";

    /// <summary>已处理完成（官方 <c>PROCESSED</c>）：终态。</summary>
    public const string Processed = "PROCESSED";
}

/// <summary>
/// 投诉资料凭证的媒体业务类型（官方 <c>complaint_media_list.media_type</c>）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012533431"/>（2026-10-14 核验）。
/// <b>只有两值</b>：<see cref="UserComplaintImage"/> 是用户提交投诉时上传的图，
/// <see cref="OperationImage"/> 是协商过程中用户 / 商户 / 微信支付客服任一方上传的图。
/// 前者官方在协商历史页明确「<c>image_list</c> 不含用户提交的凭证」，
/// 故取凭证一律走本字段所在的 <c>complaint_media_list</c>。
/// </remarks>
public static class MerchantServiceMediaTypes
{
    /// <summary>用户提交投诉时上传的图片凭证（官方 <c>USER_COMPLAINT_IMAGE</c>）。</summary>
    public const string UserComplaintImage = "USER_COMPLAINT_IMAGE";

    /// <summary>协商处理期间上传的图片凭证（官方 <c>OPERATION_IMAGE</c>）。</summary>
    public const string OperationImage = "OPERATION_IMAGE";
}

/// <summary>
/// 投诉问题类型（官方 <c>problem_type</c>）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012533431"/>（2026-10-14 核验）。
/// 官方原文：「<b>类型为申请退款的单据是需要最高优先处理的单据</b>」⇒ <see cref="Refund"/> 不只是分类，
/// 而是处置优先级；且 <c>apply_refund_amount</c> 仅在该类型下出现。
/// </remarks>
public static class MerchantServiceProblemTypes
{
    /// <summary>申请退款（官方 <c>REFUND</c>）：需最高优先处理，且可带 <c>apply_refund_amount</c>。</summary>
    public const string Refund = "REFUND";

    /// <summary>服务权益未生效（官方 <c>SERVICE_NOT_WORK</c>）：支付分类投诉常见。</summary>
    public const string ServiceNotWork = "SERVICE_NOT_WORK";

    /// <summary>其他类型（官方 <c>OTHERS</c>）。</summary>
    public const string Others = "OTHERS";
}

/// <summary>
/// 用户标签（官方 <c>user_tag_list</c> 元素，array[string]）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012533431"/>（2026-10-14 核验）。
/// <b>是列表不是单值</b>：同一用户可同时带多个标签，建成单值字符串会丢信息。
/// </remarks>
public static class MerchantServiceUserTags
{
    /// <summary>满足极速退款条件（官方 <c>TRUSTED</c>）。</summary>
    public const string Trusted = "TRUSTED";

    /// <summary>高风险投诉（官方 <c>HIGH_RISK</c>）：官方要求按运营口径优先妥善处理。</summary>
    public const string HighRisk = "HIGH_RISK";
}

/// <summary>
/// 支付分服务单状态（官方 <c>service_order_info.state</c>）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012533431"/>（2026-10-14 核验）。
/// <b>⚠️ 快照语义</b>：官方原文「此处上传的是用户发起投诉时的服务单状态，<b>不会实时更新</b>」⇒
/// 不得用它判断服务单当前状态，须另调支付分查询接口。
/// </remarks>
public static class MerchantServiceServiceOrderStates
{
    /// <summary>服务订单进行中（官方 <c>DOING</c>）。</summary>
    public const string Doing = "DOING";

    /// <summary>服务订单已取消（官方 <c>REVOKED</c>）。</summary>
    public const string Revoked = "REVOKED";

    /// <summary>服务订单待支付（官方 <c>WAITPAY</c>）。</summary>
    public const string WaitPay = "WAITPAY";

    /// <summary>服务订单已完成（官方 <c>DONE</c>）。</summary>
    public const string Done = "DONE";
}

/// <summary>
/// 投诉补充信息类型（官方 <c>additional_info.type</c>）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012533431"/>（2026-10-14 核验）。
/// <b>官方当前只给一个取值</b>（共享充电宝场景）⇒ 本枚举只有一个成员是事实而非疏漏；
/// 官方新增类型时须同批扩展，不得预先造出官方没有的键。
/// </remarks>
public static class MerchantServiceAdditionalInfoTypes
{
    /// <summary>共享充电宝类投诉（官方 <c>SHARE_POWER_TYPE</c>）：此时 <c>share_power_info</c> 有值。</summary>
    public const string SharePowerType = "SHARE_POWER_TYPE";
}

/// <summary>
/// 退款审批动作（官方 <c>update-refund-progress</c> 的 <c>action</c>）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012467256"/>（2026-10-14 核验）。
/// <para>
/// <b>两支后续动作不对称（官方原文明写）</b>：<see cref="Approve"/> 后<b>还须额外调用退款接口</b>
/// 发起原路退款，退款到账后单据状态才自动扭转为已处理完成；<see cref="Reject"/> 则<b>立即</b>扭转单据为
/// 已处理完成。故「审批通过」不等于「已退款」，把 <see cref="Approve"/> 当终态会漏掉真正的退款动作。
/// </para>
/// </remarks>
public static class MerchantServiceRefundProgressActions
{
    /// <summary>拒绝退款（官方 <c>REJECT</c>）：驳回后单据自动扭转为处理完成。</summary>
    public const string Reject = "REJECT";

    /// <summary>同意退款（官方 <c>APPROVE</c>）：仍需商户另行发起原路退款。</summary>
    public const string Approve = "APPROVE";
}

/// <summary>
/// 即时服务消息块类型（官方 <c>message.blocks[].type</c>）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4017151596"/>（2026-10-14 核验；
/// 协商历史页的 <c>normal_message</c> 与之结构一致）。
/// <b>本值决定 <c>blocks[]</c> 内哪一个子对象有值</b>（<c>TEXT</c>→<c>text</c>、<c>IMAGE</c>→<c>image</c>、
/// <c>LINK</c>→<c>link</c>、<c>FAQ_LIST</c>→<c>faq_list</c>、<c>BUTTON</c>→<c>button</c>、
/// <c>BUTTON_GROUP</c>→<c>button_group</c>）⇒ 官方是「判别式 + 载荷」的扁平并集形态，
/// 本仓以<b>一个全可空类型</b>承载（AOT 禁反射多态序列化，见红线 PAY-B6），判别式即本枚举。
/// </remarks>
public static class MerchantServiceMessageBlockTypes
{
    /// <summary>文本块（官方 <c>TEXT</c>）。</summary>
    public const string Text = "TEXT";

    /// <summary>图片块（官方 <c>IMAGE</c>）。</summary>
    public const string Image = "IMAGE";

    /// <summary>链接块（官方 <c>LINK</c>）。</summary>
    public const string Link = "LINK";

    /// <summary>FAQ 列表块（官方 <c>FAQ_LIST</c>）。</summary>
    public const string FaqList = "FAQ_LIST";

    /// <summary>按钮块（官方 <c>BUTTON</c>）。</summary>
    public const string Button = "BUTTON";

    /// <summary>按钮组块（官方 <c>BUTTON_GROUP</c>）。</summary>
    public const string ButtonGroup = "BUTTON_GROUP";
}

/// <summary>
/// 文本块颜色（官方 <c>blocks[].text.color</c>）。
/// </summary>
/// <remarks>官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4017151596"/>（2026-10-14 核验）。</remarks>
public static class MerchantServiceMessageTextColors
{
    /// <summary>默认文本内容颜色（官方 <c>DEFAULT</c>）。</summary>
    public const string Default = "DEFAULT";

    /// <summary>次要文本内容颜色（官方 <c>SECONDARY</c>）。</summary>
    public const string Secondary = "SECONDARY";
}

/// <summary>
/// 图片样式类型（官方 <c>blocks[].image.image_style_type</c>）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4017151596"/>（2026-10-14 核验）。
/// <b>取值自带 <c>IMAGE_STYLE_TYPE_</c> 前缀</b>（不是裸 <c>NARROW</c>/<c>WIDE</c>）⇒ 照官方原文，
/// 「顺手简化」会让官方按未知值拒绝。
/// </remarks>
public static class MerchantServiceImageStyleTypes
{
    /// <summary>窄图（官方 <c>IMAGE_STYLE_TYPE_NARROW</c>）。</summary>
    public const string Narrow = "IMAGE_STYLE_TYPE_NARROW";

    /// <summary>宽图（官方 <c>IMAGE_STYLE_TYPE_WIDE</c>）。</summary>
    public const string Wide = "IMAGE_STYLE_TYPE_WIDE";
}

/// <summary>
/// 动作类型（官方 <c>action.action_type</c>，四棵子树共用同一张表）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4017151596"/>（2026-10-14 核验）。
/// <b>条件必填写在描述里而非必填列</b>：官方原文「<c>ACTION_TYPE_SEND_MESSAGE</c> ⇒ 此时 <c>message_info</c> 必填、
/// <c>ACTION_TYPE_JUMP_URL</c> ⇒ 此时 <c>jump_url</c> 必填、<c>ACTION_TYPE_JUMP_MINI_PROGRAM</c> ⇒ 跳小程序」，
/// 而表内三者的「必填」全为 false ⇒ SDK 不做客户端校验，错配由官方拒绝（守卫 MC-B5）。
/// </remarks>
public static class MerchantServiceActionTypes
{
    /// <summary>回复消息：用户点击后发送消息，此时 <c>message_info</c> 必填（官方 <c>ACTION_TYPE_SEND_MESSAGE</c>）。</summary>
    public const string SendMessage = "ACTION_TYPE_SEND_MESSAGE";

    /// <summary>链接：用户点击后跳转链接，此时 <c>jump_url</c> 必填（官方 <c>ACTION_TYPE_JUMP_URL</c>）。</summary>
    public const string JumpUrl = "ACTION_TYPE_JUMP_URL";

    /// <summary>小程序：用户点击后跳转小程序页面（官方 <c>ACTION_TYPE_JUMP_MINI_PROGRAM</c>）。</summary>
    public const string JumpMiniProgram = "ACTION_TYPE_JUMP_MINI_PROGRAM";
}

/// <summary>
/// 按钮组布局方式（官方 <c>button_group.button_layout</c>）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4017151596"/>（2026-10-14 核验）。
/// 默认 <c>LAYOUT_HORIZONTAL</c>；<b><c>LAYOUT_UNKNOWN</c> 是官方列出的真实取值</b>（描述为空），
/// 照录不删 —— 删除即把一个官方会返回的值变成未知。
/// </remarks>
public static class MerchantServiceButtonLayouts
{
    /// <summary>未知布局（官方 <c>LAYOUT_UNKNOWN</c>，官方页描述为空）。</summary>
    public const string Unknown = "LAYOUT_UNKNOWN";

    /// <summary>水平排列（官方 <c>LAYOUT_HORIZONTAL</c>，默认值）。</summary>
    public const string Horizontal = "LAYOUT_HORIZONTAL";

    /// <summary>垂直排列（官方 <c>LAYOUT_VERTICAL</c>）。</summary>
    public const string Vertical = "LAYOUT_VERTICAL";
}

/// <summary>
/// 消息发送者身份（官方 <c>message.sender_identity</c>）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4017151596"/>（2026-10-14 核验）。
/// 标识消息由人工还是机器发送 —— 即时服务面同时支持客服坐席与机器人，本字段是二者唯一的报文区分。
/// </remarks>
public static class MerchantServiceSenderIdentities
{
    /// <summary>未知（官方 <c>UNKNOWN</c>）。</summary>
    public const string Unknown = "UNKNOWN";

    /// <summary>人工（官方 <c>MANUAL</c>）。</summary>
    public const string Manual = "MANUAL";

    /// <summary>机器（官方 <c>MACHINE</c>）。</summary>
    public const string Machine = "MACHINE";
}

/// <summary>
/// 投诉协商历史操作类型（官方 <c>negotiation-historys</c> 的 <c>operate_type</c>，共 <b>25</b> 值）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012533439"/>（2026-10-14 逐值核验；
/// 商户面与服务商面本域字段表<b>逐行一致</b> ⇒ 一份枚举覆盖两面）。
/// </para>
/// <para>
/// <b>⚠️ 官方两处拼写错误必须照录</b>：<see cref="UserConfirmComplaintSolved"/> 印为 <c>USER_COMFIRM_COMPLAINT</c>
/// （COMFIRM）、<see cref="MerchantRefuseRefund"/> 印为 <c>MERCHANT_REFUSE_RERUND</c>（RERUND）。
/// 「顺手纠正」成正确英文会让该条协商历史永远匹配不上 —— 守卫 MC-B4 逐值锁定含错拼。
/// </para>
/// <para>
/// <b>两值是历史专用</b>：<see cref="UserRevokeComplaint"/> 与 <see cref="UserConfirmComplaintSolved"/>
/// 官方注明「只存在于历史投诉单的协商历史中」⇒ 新单读不到它们不是缺陷。
/// </para>
/// </remarks>
public static class MerchantServiceNegotiationOperateTypes
{
    /// <summary>用户提交投诉（官方 <c>USER_CREATE_COMPLAINT</c>）。</summary>
    public const string UserCreateComplaint = "USER_CREATE_COMPLAINT";

    /// <summary>用户继续投诉（官方 <c>USER_CONTINUE_COMPLAINT</c>）。</summary>
    public const string UserContinueComplaint = "USER_CONTINUE_COMPLAINT";

    /// <summary>用户留言（官方 <c>USER_RESPONSE</c>）。</summary>
    public const string UserResponse = "USER_RESPONSE";

    /// <summary>平台留言（官方 <c>PLATFORM_RESPONSE</c>）。</summary>
    public const string PlatformResponse = "PLATFORM_RESPONSE";

    /// <summary>商户留言（官方 <c>MERCHANT_RESPONSE</c>）。</summary>
    public const string MerchantResponse = "MERCHANT_RESPONSE";

    /// <summary>商户申请结单（官方 <c>MERCHANT_CONFIRM_COMPLETE</c>）。</summary>
    public const string MerchantConfirmComplete = "MERCHANT_CONFIRM_COMPLETE";

    /// <summary>用户提交投诉系统通知（官方 <c>USER_CREATE_COMPLAINT_SYSTEM_MESSAGE</c>）。</summary>
    public const string UserCreateComplaintSystemMessage = "USER_CREATE_COMPLAINT_SYSTEM_MESSAGE";

    /// <summary>投诉单发起全额退款系统通知（官方 <c>COMPLAINT_FULL_REFUNDED_SYSTEM_MESSAGE</c>）。</summary>
    public const string ComplaintFullRefundedSystemMessage = "COMPLAINT_FULL_REFUNDED_SYSTEM_MESSAGE";

    /// <summary>用户继续投诉系统通知（官方 <c>USER_CONTINUE_COMPLAINT_SYSTEM_MESSAGE</c>）。</summary>
    public const string UserContinueComplaintSystemMessage = "USER_CONTINUE_COMPLAINT_SYSTEM_MESSAGE";

    /// <summary>用户主动撤诉（官方 <c>USER_REVOKE_COMPLAINT</c>，仅历史投诉单）。</summary>
    public const string UserRevokeComplaint = "USER_REVOKE_COMPLAINT";

    /// <summary>用户确认投诉解决（仅历史投诉单；官方拼写为 <c>USER_COMFIRM_COMPLAINT</c>，照录）。</summary>
    public const string UserConfirmComplaintSolved = "USER_COMFIRM_COMPLAINT";

    /// <summary>平台催办（官方 <c>PLATFORM_HELP_APPLICATION</c>）。</summary>
    public const string PlatformHelpApplication = "PLATFORM_HELP_APPLICATION";

    /// <summary>用户申请平台协助（官方 <c>USER_APPLY_PLATFORM_HELP</c>）。</summary>
    public const string UserApplyPlatformHelp = "USER_APPLY_PLATFORM_HELP";

    /// <summary>商户同意退款申请（官方 <c>MERCHANT_APPROVE_REFUND</c>）。</summary>
    public const string MerchantApproveRefund = "MERCHANT_APPROVE_REFUND";

    /// <summary>商户拒绝退款申请（官方拼写为 <c>MERCHANT_REFUSE_RERUND</c>，照录；此时操作内容展示拒绝原因）。</summary>
    public const string MerchantRefuseRefund = "MERCHANT_REFUSE_RERUND";

    /// <summary>用户提交满意度调查结果（官方 <c>USER_SUBMIT_SATISFACTION</c>，操作内容为满意度分数）。</summary>
    public const string UserSubmitSatisfaction = "USER_SUBMIT_SATISFACTION";

    /// <summary>服务订单已取消（官方 <c>SERVICE_ORDER_CANCEL</c>）。</summary>
    public const string ServiceOrderCancel = "SERVICE_ORDER_CANCEL";

    /// <summary>服务订单已完成（官方 <c>SERVICE_ORDER_COMPLETE</c>）。</summary>
    public const string ServiceOrderComplete = "SERVICE_ORDER_COMPLETE";

    /// <summary>投诉单发起部分退款系统通知（官方 <c>COMPLAINT_PARTIAL_REFUNDED_SYSTEM_MESSAGE</c>）。</summary>
    public const string ComplaintPartialRefundedSystemMessage = "COMPLAINT_PARTIAL_REFUNDED_SYSTEM_MESSAGE";

    /// <summary>投诉单退款到账系统通知（官方 <c>COMPLAINT_REFUND_RECEIVED_SYSTEM_MESSAGE</c>）。</summary>
    public const string ComplaintRefundReceivedSystemMessage = "COMPLAINT_REFUND_RECEIVED_SYSTEM_MESSAGE";

    /// <summary>投诉单受托退款系统通知（官方 <c>COMPLAINT_ENTRUSTED_REFUND_SYSTEM_MESSAGE</c>）。</summary>
    public const string ComplaintEntrustedRefundSystemMessage = "COMPLAINT_ENTRUSTED_REFUND_SYSTEM_MESSAGE";

    /// <summary>用户申请平台协助（官方 <c>USER_APPLY_PLATFORM_SERVICE</c>）。</summary>
    public const string UserApplyPlatformService = "USER_APPLY_PLATFORM_SERVICE";

    /// <summary>用户取消平台协助（官方 <c>USER_CANCEL_PLATFORM_SERVICE</c>）。</summary>
    public const string UserCancelPlatformService = "USER_CANCEL_PLATFORM_SERVICE";

    /// <summary>客服结束平台协助（官方 <c>PLATFORM_SERVICE_FINISHED</c>）。</summary>
    public const string PlatformServiceFinished = "PLATFORM_SERVICE_FINISHED";

    /// <summary>用户因点击产生留言（官方 <c>USER_CLICK_RESPONSE</c>，此时 <c>click_message</c> 有值）。</summary>
    public const string UserClickResponse = "USER_CLICK_RESPONSE";
}
