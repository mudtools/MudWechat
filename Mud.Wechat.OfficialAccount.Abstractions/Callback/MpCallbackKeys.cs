// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Abstractions.Callback;

/// <summary>
/// 公众号消息加解密方式（服务器配置「消息加解密方式」三值）。
/// </summary>
/// <remarks>
/// 官方原文：明文模式「不加密，明文发送，安全系数较低，不建议使用」；
/// 兼容模式「明文、密文共存，不建议使用」；安全模式「纯密文，安全系数高，强烈推荐使用」。
/// </remarks>
public enum MpCallbackSecurityMode
{
    /// <summary>明文模式：不加密（无 <c>Encrypt</c> 节点），拒收密文。</summary>
    Plain = 0,

    /// <summary>兼容模式：明文与密文共存，按请求实际形态分支。</summary>
    Compatible = 1,

    /// <summary>安全模式（默认）：纯密文，明文一律拒收。</summary>
    Safe = 2,
}

/// <summary>
/// 接收普通消息的 <c>MsgType</c> 取值（官方接收普通消息页）。
/// </summary>
/// <remarks>
/// 解码示例：text（文本）/ image（图片）/ voice（语音）/ video（视频）/ shortvideo（小视频）/
/// location（地理位置）/ link（链接）。事件消息的 <c>MsgType</c> 恒为 <see cref="Event"/>。
/// </remarks>
public static class MpCallbackMessageTypes
{
    /// <summary>事件消息类型（<c>MsgType = event</c>，与 <c>Event</c> 节点配合）。</summary>
    public const string Event = "event";

    /// <summary>文本消息。</summary>
    public const string Text = "text";

    /// <summary>图片消息。</summary>
    public const string Image = "image";

    /// <summary>语音消息。</summary>
    public const string Voice = "voice";

    /// <summary>视频消息。</summary>
    public const string Video = "video";

    /// <summary>小视频消息。</summary>
    public const string ShortVideo = "shortvideo";

    /// <summary>地理位置消息。</summary>
    public const string Location = "location";

    /// <summary>链接消息。</summary>
    public const string Link = "link";
}

/// <summary>
/// 被动回复的消息类型（<c>MsgType</c>）取值（官方「被动回复用户消息」页，V9 已核验）。
/// </summary>
/// <remarks>
/// <b>与 <see cref="MpCallbackMessageTypes"/> 的区别</b>：后者是**收到**的消息类型（含 <c>shortvideo</c>/<c>location</c>/<c>link</c>），
/// 本类是**回复**的消息类型（含 <c>music</c>/<c>news</c>，不含 <c>shortvideo</c>/<c>location</c>/<c>link</c>）——
/// 两组键**不可混用**。官方另有灰度中的 <c>transfer_biz_ai_ivr</c>（仅公共字段），本轮不建模。
/// </remarks>
public static class MpCallbackReplyTypes
{
    /// <summary>文本回复（<c>Content</c>）。</summary>
    public const string Text = "text";

    /// <summary>图片回复（<c>Image/MediaId</c>；官方约束：不支持 gif 动图）。</summary>
    public const string Image = "image";

    /// <summary>语音回复（<c>Voice/MediaId</c>）。</summary>
    public const string Voice = "voice";

    /// <summary>视频回复（<c>Video/MediaId</c> 必填，Title/Description 可选）。</summary>
    public const string Video = "video";

    /// <summary>音乐回复（<c>Music</c>；仅 <c>ThumbMediaId</c> 必填，<c>HQMusicUrl</c> 在 WIFI 优先）。</summary>
    public const string Music = "music";

    /// <summary>图文回复（<c>ArticleCount</c> + <c>Articles/item</c>；官方上限 8 条，六类消息场景仅 1 条）。</summary>
    public const string News = "news";
}

/// <summary>
/// 自定义菜单事件推送的 <c>Event</c> 取值（官方自定义菜单事件推送页）。
/// </summary>
/// <remarks>
/// 官方补充约束：点击菜单弹出子菜单<b>不产生上报</b>；第 3~8 个事件
/// （<c>scancode_push</c>～<c>location_select</c>）仅支持 iOS 微信 5.4.1+ / Android 微信 5.4+。
/// <b>不得凭记忆补入</b> <c>media_id</c>/<c>view_limited</c>（本轮官方页面未列出，须逐页核验后增量补）。
/// </remarks>
public static class MpCallbackEventTypes
{
    /// <summary>
    /// 关注事件（官方「接收事件推送」页）。无专有字段的普通关注；**扫码关注**（未关注）同为本键，
    /// 但携带 <c>EventKey</c>（<c>qrscene_</c> 前缀 + 场景值 ID）与 <c>Ticket</c>。
    /// </summary>
    public const string Subscribe = "subscribe";

    /// <summary>
    /// 取消关注事件。**官方硬约束**：「为保护用户数据隐私，开发者收到取消关注事件时必须删除该用户的所有信息」
    /// —— 处理器须据此清理本地用户数据（不得忽略）。
    /// </summary>
    public const string Unsubscribe = "unsubscribe";

    /// <summary>扫描带参数二维码事件（已关注；EventKey = 场景值 ID，Ticket = 二维码 ticket）。</summary>
    public const string Scan = "SCAN";

    /// <summary>
    /// 上报地理位置事件（EventKey 无关；专有字段 <c>Latitude</c>/<c>Longitude</c>/<c>Precision</c>）。
    /// </summary>
    /// <remarks>
    /// <b>与普通 <c>location</c> 消息不是同一结构</b>：后者字段为 <c>Location_X</c>/<c>Location_Y</c>/<c>Scale</c>/<c>Label</c>
    /// 且事件键为小写 <c>location</c>（<see cref="MpCallbackMessageTypes.Location"/>）——两者大小写敏感、不得混用。
    /// 上报频率：进入会话时一次，进入后每 5 秒一次（公众平台可改设置）。
    /// </remarks>
    public const string Location = "LOCATION";

    /// <summary>点击菜单拉取消息时的事件推送（EventKey 为菜单 KEY 值）。</summary>
    public const string Click = "CLICK";

    /// <summary>点击菜单跳转链接时的事件推送（EventKey 为跳转 URL，附 MenuId）。</summary>
    public const string View = "VIEW";

    /// <summary>扫码推事件（EventKey + ScanCodeInfo）。</summary>
    public const string ScanCodePush = "scancode_push";

    /// <summary>扫码推事件且弹出「消息接收中」提示框（EventKey + ScanCodeInfo）。</summary>
    public const string ScanCodeWaitMsg = "scancode_waitmsg";

    /// <summary>弹出系统拍照发图的事件推送（EventKey + SendPicsInfo）。</summary>
    public const string PicSysPhoto = "pic_sysphoto";

    /// <summary>弹出拍照或者相册发图的事件推送（EventKey + SendPicsInfo）。</summary>
    public const string PicPhotoOrAlbum = "pic_photo_or_album";

    /// <summary>弹出微信相册发图器的事件推送（EventKey + SendPicsInfo）。</summary>
    public const string PicWeixin = "pic_weixin";

    /// <summary>弹出地理位置选择器的事件推送（EventKey + SendLocationInfo）。</summary>
    public const string LocationSelect = "location_select";

    /// <summary>点击菜单跳转小程序的事件推送（EventKey 为小程序路径，附 MenuId）。</summary>
    public const string ViewMiniProgram = "view_miniprogram";
}

/// <summary>
/// 卡券事件推送的 <c>Event</c> 取值（官方「卡券事件推送」页，V2 已核验；共 12 键）。
/// </summary>
/// <remarks>
/// <b>官方通用约束</b>：5 秒未响应即断连并重试共 3 次；消息排重推荐 <c>FromUserName + CreateTime</c>；
/// 无法及时处理可直接回复空串（SDK 默认回 <c>success</c>）。
/// </remarks>
public static class MpCardEventTypes
{
    /// <summary>卡券通过审核（字段 <c>CardId</c>；官方示例中不通过才带 <c>RefuseReason</c>）。</summary>
    public const string CardPassCheck = "card_pass_check";

    /// <summary>卡券未通过审核（<c>CardId</c> + <c>RefuseReason</c>）。</summary>
    public const string CardNotPassCheck = "card_not_pass_check";

    /// <summary>用户领取卡券（字段最多：含是否转赠领取、领取场景值、UnionId 等）。</summary>
    public const string UserGetCard = "user_get_card";

    /// <summary>用户转赠卡券（含是否转赠退回、是否群转赠）。</summary>
    public const string UserGiftingCard = "user_gifting_card";

    /// <summary>用户删除卡券。</summary>
    public const string UserDelCard = "user_del_card";

    /// <summary>卡券核销（含核销来源 <c>ConsumeSource</c>、核销员、自助核销验证码等）。</summary>
    public const string UserConsumeCard = "user_consume_card";

    /// <summary>微信买单（含交易号、门店 ID 与实付/应付金额，单位分）。</summary>
    public const string UserPayFromPayCell = "user_pay_from_pay_cell";

    /// <summary>用户点击/进入会员卡（需创建会员卡时开启 <c>need_push_on_view</c>，开发者须自行评估推送压力）。</summary>
    public const string UserViewCard = "user_view_card";

    /// <summary>用户从卡券进入服务号会话（识别卡券来源用户身份）。</summary>
    public const string UserEnterSessionFromCard = "user_enter_session_from_card";

    /// <summary>会员卡内容更新（积分/余额变动）。</summary>
    public const string UpdateMemberCard = "update_member_card";

    /// <summary>库存报警（初始库存 &gt; 200 且当前 ≤ 100 时触发，每 12 小时一次）。</summary>
    public const string CardSkuRemind = "card_sku_remind";

    /// <summary>券点流水详情（**不含 <c>CardId</c>**，字段为订单号/状态/券点数量等）。</summary>
    public const string CardPayOrder = "card_pay_order";

    /// <summary>会员卡激活（用户一键激活或修改会员卡信息后推送）。</summary>
    public const string SubmitMemberCardUserInfo = "submit_membercard_user_info";
}

/// <summary>
/// 「用户授权信息变更事件推送」的 <c>Event</c> 取值（官方 H5 授权页，V5 已核验；共 3 键）。
/// </summary>
/// <remarks>
/// <b>官方合规约束（不得忽略）</b>：三个事件分别要求「及时更新或清理头像昵称」「及时删除用户信息」
/// 「依法依规履行个人信息保护义务」；<c>user_info_modified</c> 仅推送给**最近 30 天内授权过**的服务号。
/// 报文另携带 <c>OpenID</c>/<c>UnionID</c>/<c>AppID</c>（可直接取信封外的三个专有字段）。
/// </remarks>
public static class MpAuthorizationEventTypes
{
    /// <summary>授权用户资料变更（资料有风险被平台清理时通知，需主动更新/清理本地头像昵称）。</summary>
    public const string UserInfoModified = "user_info_modified";

    /// <summary>授权用户资料撤回（需及时删除用户信息；<c>RevokeInfo</c> 指示撤回范围）。</summary>
    public const string UserAuthorizationRevoke = "user_authorization_revoke";

    /// <summary>授权用户完成注销（需依法履行个人信息保护义务，删除或匿名化处理）。</summary>
    public const string UserAuthorizationCancellation = "user_authorization_cancellation";
}

/// <summary>
/// 订阅通知事件推送的 <c>Event</c> 取值（官方「订阅通知的事件推送」页，V3 已核验；共 3 键）。
/// </summary>
/// <remarks>
/// <b>三键的项列表结构一致</b>（外层包裹节点名各异：<c>SubscribeMsgPopupEvent</c>/<c>SubscribeMsgChangeEvent</c>/
/// <c>SubscribeMsgSentEvent</c>，内部均为若干 <c>List</c> 项 —— **项元素名官方固定为 <c>List</c>**，
/// 且一次订阅可携带多个模板 id ⇒ 必须支持多项）。
/// </remarks>
public static class MpSubscriptionEventTypes
{
    /// <summary>用户操作订阅通知弹窗（图文/H5 场景内订阅；项的 <c>SubscribeStatusString</c> 取 accept/reject）。</summary>
    public const string Popup = "subscribe_msg_popup_event";

    /// <summary>用户管理订阅通知（在服务通知管理页操作；<b>仅推送拒收</b>，项内**不含** <c>PopupScene</c>）。</summary>
    public const string Change = "subscribe_msg_change_event";

    /// <summary>发送订阅通知结果（调用 bizsend 后异步回执；项内 <c>ErrorCode = 0</c> 表示成功）。</summary>
    public const string Sent = "subscribe_msg_sent_event";
}

/// <summary>
/// 微信认证事件推送的 <c>Event</c> 取值（官方「微信认证事件推送」页，V4 已核验；共 6 键）。
/// </summary>
/// <remarks>
/// 官方业务链：资质认证成功 <b>一定早于</b>名称认证成功；名称认证成功后才在客户端获得打勾标识；
/// 名称认证失败时**仍有接口权限**（仅不打勾）；<c>annual_renew</c> 提示需尽快年审；
/// <c>verify_expired</c> 表示已过期、需重新发起认证。推送对象为**账号管理权限集**持有方
/// （第三方平台代收时推送到套件的「消息与事件接收 URL」）。
/// </remarks>
public static class MpVerificationEventTypes
{
    /// <summary>资质认证成功（此刻起获得认证相关接口权限）。字段：<c>ExpiredTime</c>。</summary>
    public const string QualificationVerifySuccess = "qualification_verify_success";

    /// <summary>资质认证失败。字段：<c>FailTime</c> + <c>FailReason</c>。</summary>
    public const string QualificationVerifyFail = "qualification_verify_fail";

    /// <summary>名称认证成功（客户端开始显示打勾标识）。字段：<c>ExpiredTime</c>。</summary>
    public const string NamingVerifySuccess = "naming_verify_success";

    /// <summary>名称认证失败（**仍有接口权限**，仅无打勾标识）。字段：<c>FailTime</c> + <c>FailReason</c>。</summary>
    public const string NamingVerifyFail = "naming_verify_fail";

    /// <summary>年审通知（<c>ExpiredTime</c> 为认证过期时间戳，需尽快年审）。</summary>
    public const string AnnualRenew = "annual_renew";

    /// <summary>认证过期失效通知（<c>ExpiredTime</c> 为已过期时间，需重新发起微信认证）。</summary>
    public const string VerifyExpired = "verify_expired";
}
