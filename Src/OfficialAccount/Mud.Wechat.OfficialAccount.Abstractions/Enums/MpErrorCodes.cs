// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Abstractions.Enums;

/// <summary>
/// 微信公众号 / 服务号业务错误码（HTTP 200 + errcode 通道；取值以官方各端点错误码表为准）。
/// </summary>
/// <remarks>
/// <para>
/// 令牌失效码集合 <c>{40001, 40014, 42001}</c> 与 <c>MpTokenInvalidationDetector</c> 的判定集合
/// 保持一致（组件 errcode 恢复链路）；其余非零 errcode 走业务异常
/// （<see cref="Exceptions.MpException"/>）。
/// </para>
/// <para>
/// <b>40001 的双义（本 SDK 的显式裁决）</b>：官方语义为「获取 access_token 时 AppSecret 错误，
/// <b>或 access_token 无效 / 非最新</b>」——同一码跨「签发失败」与「令牌失效」两义。
/// 令牌签发接口（<c>IMpAuthentication</c>）<b>不带 [Token]、不进恢复链路</b>，其 40001 由
/// <see cref="Exceptions.MpException"/> 抛业务异常；业务端点（带 <c>[Token]</c>）的 40001
/// 归入失效码集合，交恢复链路自动刷新重试（尤其覆盖「token 已被其它进程刷新为非最新」场景）。
/// </para>
/// </remarks>
public static class MpErrorCodes
{
    /// <summary>成功。</summary>
    public const int Success = 0;

    /// <summary>系统繁忙（官方 -1，请开发者稍候再试）。</summary>
    public const int SystemError = -1;

    /// <summary>access_token 无效 / 非最新（亦用于签发接口的 AppSecret 错误）。</summary>
    public const int InvalidCredential = 40001;

    /// <summary>不合法的凭证类型（grant_type）。</summary>
    public const int InvalidGrantType = 40002;

    /// <summary>不合法的 AppID。</summary>
    public const int InvalidAppId = 40013;

    /// <summary>无效的 AppSecret。</summary>
    public const int InvalidSecret = 40125;

    /// <summary>调用接口的 IP 地址不在白名单中。</summary>
    public const int IpNotInWhitelist = 40164;

    /// <summary>缺少 appid 参数。</summary>
    public const int MissingAppId = 41002;

    /// <summary>缺少 secret 参数。</summary>
    public const int MissingAppSecret = 41004;

    /// <summary>需要 POST 请求（<c>getStableAccessToken</c> 仅支持 POST）。</summary>
    public const int RequirePostMethod = 43002;

    /// <summary>AppSecret 已被冻结（需解冻后再次调用）。</summary>
    public const int FrozenSecret = 40243;

    /// <summary>access_token 无效。</summary>
    public const int InvalidAccessToken = 40014;

    /// <summary>access_token 过期。</summary>
    public const int ExpiredAccessToken = 42001;

    /// <summary>调用超过天级频率限制（可调用 <c>clear_quota</c> 恢复额度）。</summary>
    public const int DailyQuotaExceeded = 45009;

    /// <summary>API 调用太频繁（分钟级配额）。</summary>
    public const int MinuteQuotaExceeded = 45011;

    /// <summary>此次调用需要管理员确认。</summary>
    public const int AdminConfirmationRequired = 89503;

    /// <summary>该 IP 调用请求已被公众号管理员拒绝（请 24 小时后再试）。</summary>
    public const int IpRejectedRetryAfterDay = 89506;

    /// <summary>该 IP 调用请求已被公众号管理员拒绝（请 1 小时后再试）。</summary>
    public const int IpRejectedRetryAfterHour = 89507;

    /// <summary>网络通信检测：未设置回调 URL。</summary>
    public const int CallbackCheckInvalidUrl = 40201;

    /// <summary>网络通信检测：不正确的 action 参数。</summary>
    public const int CallbackCheckInvalidAction = 40202;

    /// <summary>网络通信检测：不正确的运营商参数。</summary>
    public const int CallbackCheckInvalidOperator = 40203;

    // ---------------------------------------------------------------- 用户管理·标签管理（M2，取值逐页核验官方文档）

    /// <summary>invalid openid：不合法的 OpenID（确认该用户是否已关注公众号，或是否为其他公众号的 OpenID）。</summary>
    public const int InvalidOpenId = 40003;

    /// <summary>invalid openid list size：不合法的 openid 列表长度。</summary>
    public const int InvalidOpenIdListSize = 40032;

    /// <summary>empty post data：传递的参数为空（如删除标签未带 <c>tag.id</c>）。</summary>
    public const int EmptyPostData = 44002;

    /// <summary>reach max api daily quota limit：超出接口每日调用限制。</summary>
    public const int DailyQuotaReached = 45009;

    /// <summary>标签数超限（官方：一个公众号最多可以创建 100 个标签，适当删减标签）。</summary>
    public const int TagCountExceeded = 45056;

    /// <summary>can't modify sys tag：禁止修改系统标签。</summary>
    public const int SystemTagImmutable = 45058;

    /// <summary>单用户标签数超过限制（官方：标签功能支持公众号为用户打上最多 20 个标签）。</summary>
    public const int UserTagCountExceeded = 45059;

    /// <summary>invalid tag name：检查标签名。</summary>
    public const int InvalidTagName = 45157;

    /// <summary>tag name too long：调小标签名长度（官方限制 30 个字符以内）。</summary>
    public const int TagNameTooLong = 45158;

    /// <summary>invalid tag id：非法的标签。</summary>
    public const int InvalidTagId = 45159;

    /// <summary>openid much req：一般是因为对同个 openid 并发打标 / 取消标签导致（应串行化同一 openid 的标签变更）。</summary>
    public const int ConcurrentTaggingConflict = 45169;

    /// <summary>
    /// some openid fail：<b>部分</b> openid 失败（响应体 <c>fail_openid_list</c> 给出失败的 openid，可定向重试）。
    /// </summary>
    /// <remarks>整批重放会对已成功的 openid 重复打标，故调用方应读取 <c>fail_openid_list</c> 做定向重试。</remarks>
    public const int SomeOpenIdFailed = 45171;

    /// <summary>post data format error：参数格式错误。</summary>
    public const int PostDataFormatError = 47001;

    /// <summary>api unauthorized：接口功能未授权（可在「公众平台官网 - 开发者中心页」查看接口权限）。</summary>
    public const int ApiUnauthorized = 48001;

    /// <summary>not match openid with appid：传入的 openid 不属于此 AppID。</summary>
    public const int OpenIdAppIdMismatch = 49003;

    /// <summary>user limited：用户受限，可能是用户账号被冻结或注销。</summary>
    public const int UserLimited = 50002;

    /// <summary>user is unsubscribed：用户未关注公众号。</summary>
    public const int UserUnsubscribed = 50005;

    /// <summary>access clientip is not registered：第三方平台出口 IP 未设置（仅第三方平台调用场景）。</summary>
    public const int ThirdPartyClientIpNotRegistered = 61004;

    // ---------------------------------------------------------------- 用户管理·用户信息（M2，取值逐页核验官方文档）

    /// <summary>invalid remark name：备注名非法（官方限制「长度必须小于 30 字节」）。</summary>
    public const int InvalidRemarkName = 40092;

    /// <summary>require subscribe：该 openid 未关注当前账号（设置备注名等场景）。</summary>
    public const int RequireSubscribe = 43004;

    /// <summary>
    /// 平台级「系统繁忙」码（拉黑用户接口，官方解决方案「稍后重试」）——属<b>可重试</b>错误。
    /// </summary>
    /// <remarks>官方新增码段（非 4xxxx / 6xxxx 段），故单列常量并注明可重试语义。</remarks>
    public const int BatchBlacklistSystemBusy = 268487001;

    /// <summary>平台级「系统繁忙」码（获取关注者列表接口，官方解决方案「稍后重试」）——属<b>可重试</b>错误。</summary>
    public const int GetFansSystemBusy = 268487002;

    // ---------------------------------------------------------------- 自定义菜单（M2，取值逐页核验官方文档）

    /// <summary>invalid button size：不合法的按钮个数（官方：最多 3 个一级菜单）。</summary>
    public const int InvalidButtonSize = 40016;

    /// <summary>invalid button type：不合法的按钮类型。</summary>
    public const int InvalidButtonType = 40017;

    /// <summary>invalid button name size：不合法的按钮名字长度（一级 ≤ 16 字节 / 子菜单 ≤ 60 字节）。</summary>
    public const int InvalidButtonNameSize = 40018;

    /// <summary>invalid button key size：不合法的按钮 KEY 长度（≤ 128 字节）。</summary>
    public const int InvalidButtonKeySize = 40019;

    /// <summary>invalid button url size：不合法的按钮 URL 长度（≤ 1024 字节）。</summary>
    public const int InvalidButtonUrlSize = 40020;

    /// <summary>invalid sub button size：不合法的子菜单按钮个数（官方：每个一级最多 5 个二级）。</summary>
    public const int InvalidSubButtonSize = 40023;

    /// <summary>invalid sub button type：不合法的子菜单按钮类型。</summary>
    public const int InvalidSubButtonType = 40024;

    /// <summary>invalid sub button url size：不合法的子菜单按钮 URL 长度。</summary>
    public const int InvalidSubButtonUrlSize = 40027;

    /// <summary>
    /// invalid charset：请求字符非法——官方描述明确「<b>请检查请求是否包含 <c>\uxxxx</c> 格式的字符，
    /// 会导致创建失败</b>」。
    /// </summary>
    /// <remarks>
    /// <b>对 SDK 的硬约束</b>：JSON 序列化<b>不得</b>把非 ASCII（如中文菜单名）转义为 <c>\uXXXX</c>，
    /// 否则菜单创建直接失败。故本产品线在注册期把序列化编码器放宽为
    /// <c>JavaScriptEncoder.UnsafeRelaxedJsonEscaping</c>（见 MP 注册期 remarks），
    /// 由用例锁定「中文菜单名不得出现 <c>\u</c> 转义」。
    /// </remarks>
    public const int InvalidCharsetEscaped = 40033;

    /// <summary>invalid sub button url domain：不合法的子菜单按钮 url 域名。</summary>
    public const int InvalidSubButtonUrlDomain = 40054;

    /// <summary>invalid button url domain：不合法的菜单按钮 url 域名。</summary>
    public const int InvalidButtonUrlDomain = 40055;

    /// <summary>Article ID 无效（图文 ID 无效）。</summary>
    public const int InvalidArticleId = 53600;

    /// <summary>match rule violates privacy：匹配规则包含隐私字段。</summary>
    public const int MatchRulePrivacyViolation = 65320;

    // ---------------------------------------------------------------- 转换 openid（M2，取值逐页核验官方文档）

    /// <summary>appid wrong：<c>from_appid</c> 参数错误（与调用的账号没有迁移关系）。</summary>
    public const int ChangeOpenIdAppIdWrong = 63178;

    /// <summary>openid_list empty：<c>openid_list</c> 为空。</summary>
    public const int ChangeOpenIdListEmpty = 63182;

    /// <summary>appid error：appid 没有迁移关系。</summary>
    public const int ChangeOpenIdAppIdError = 63183;

    // ---------------------------------------------------------------- 客服消息（M2，取值逐页核验官方文档）

    /// <summary>invalid account type：账号类型不符合要求（如非服务号/公众号形态调用小程序专属接口）。</summary>
    public const int InvalidAccountType = 40200;

    /// <summary>客服消息图文条数超出限制（官方 <c>mpnews</c> 分支条数限制 1 条以内）。</summary>
    public const int NewsCountExceeded = 45008;

    /// <summary>invalid command：<c>command</c> 字段取值不对（客服输入状态）。</summary>
    public const int InvalidTypingCommand = 45072;

    /// <summary>下发输入状态前需在 <b>30 秒内</b>与该用户有过消息交互。</summary>
    public const int TypingNeedsRecentInteraction = 45080;

    /// <summary>you are already typing：已在输入状态，不可重复下发。</summary>
    public const int AlreadyTyping = 45081;

    /// <summary>为保护未成年人权益，该条消息发送失败（官方 <c>70000</c>，<b>非通用错误码</b>）。</summary>
    public const int MinorProtectionRejected = 70000;

    /// <summary>未开通或未升级到新版客服功能（获取聊天记录专用码）。</summary>
    public const int NewCustomServiceNotEnabled = 65400;

    /// <summary>查询参数不合法（获取聊天记录）。</summary>
    public const int MsgRecordParamInvalid = 65416;

    /// <summary>查询时间段超出限制（官方：查询时间段不能超过 <b>24 小时</b>）。</summary>
    public const int MsgRecordTimeRangeTooLong = 65417;

    // ---------------------------------------------------------------- 客服管理 / 会话控制（M2，取值逐页核验官方文档）

    /// <summary>invalid file type / 不支持的媒体类型（如客服头像上传文件格式不对）。</summary>
    public const int InvalidFileType = 40005;

    /// <summary>无效客服账号。</summary>
    public const int InvalidKfAccount = 65401;

    /// <summary>客服账号尚未绑定微信号，不能投入使用。</summary>
    public const int KfAccountNotBound = 65402;

    /// <summary>客服昵称不合法（官方：昵称最长 16 个字）。</summary>
    public const int IllegalKfNickname = 65403;

    /// <summary>客服账号不合法（官方：账号前缀最多 10 字符，仅英文/数字/下划线，后缀为公众号微信号且长度不超过 30 字符）。</summary>
    public const int IllegalKfAccount = 65404;

    /// <summary>账号数目已达到上限，不能继续添加（官方：每个账号最多 <b>100</b> 个客服账号）。</summary>
    public const int KfAccountCountExceeded = 65405;

    /// <summary>已经存在的客服账号。</summary>
    public const int KfAccountExists = 65406;

    /// <summary>邀请对象已经是该账号客服。</summary>
    public const int InviteeAlreadyWorker = 65407;

    /// <summary>已向该微信发送过邀请（本账号已有一个邀请给该微信）。</summary>
    public const int InviteeAlreadyInvited = 65408;

    /// <summary>无效的微信号。</summary>
    public const int InvalidWeChatId = 65409;

    /// <summary>邀请对象绑定的客服账号数达到上限。</summary>
    public const int InviteeBindingLimitReached = 65410;

    /// <summary>该账号已有一个等待确认的邀请，不能重复邀请。</summary>
    public const int PendingInvitationExists = 65411;

    /// <summary>该客服账号已经绑定微信号，不能进行邀请。</summary>
    public const int KfAccountAlreadyBound = 65412;

    /// <summary>不存在对应用户的会话信息。</summary>
    public const int NoEffectiveSession = 65413;

    /// <summary>客户正在被其他客服接待。</summary>
    public const int CustomerServedByAnother = 65414;

    /// <summary>指定的客服不在线（创建会话前置条件：客服须已绑定微信号<b>且在线</b>）。</summary>
    public const int WorkerNotOnline = 65415;

    // ---------------------------------------------------------------- 素材管理（P0-a，取值逐页核验官方文档）

    /// <summary>invalid media type：不合法的媒体文件类型（新增临时素材，type 取值须为 image/voice/video/thumb）。</summary>
    public const int InvalidMediaType = 40004;

    /// <summary>invalid media_id：无效的媒体 ID（获取临时素材 / 获取高清语音素材，素材不存在或已超 3 天有效期）。</summary>
    public const int InvalidMediaId = 40007;

    /// <summary>invalid image size：图片尺寸太大（上传发表内容中的图片，官方限制 jpg/png 且 1MB 以下）。</summary>
    public const int InvalidImageSize = 40009;

    // ---------------------------------------------------------------- 模板消息（P0-c，取值逐页核验官方文档；服务号专属域）

    /// <summary>invalid message type：不合法的消息类型（发送模板消息）。</summary>
    public const int InvalidMessageType = 40008;

    /// <summary>invalid template_id size：不合法的 template_id 长度（发送模板消息）。</summary>
    public const int InvalidTemplateIdSize = 40036;

    /// <summary>invalid template_id：不合法的 template_id（发送 / 选用模板）。</summary>
    public const int InvalidTemplateId = 40037;

    /// <summary>invalid url size：不合法的 URL 长度（发送模板消息）。</summary>
    public const int InvalidUrlSize = 40039;

    /// <summary>invalid keyword_name_list：需要传入正确的 keyword_name_list（选用模板）。</summary>
    public const int InvalidKeywordNameList = 40246;

    /// <summary>need new category template：请使用类目模板库 ID 进行添加（选用模板）。</summary>
    public const int NeedNewCategoryTemplate = 40247;

    /// <summary>禁止发送营销内容（发送模板消息）。</summary>
    public const int MarketingContentRejected = 40249;

    /// <summary>模板被限制下发（发送模板消息）。</summary>
    public const int TemplateDeliveryLimited = 43116;

    /// <summary>参数不符合模板参数规则（发送模板消息，如 thing01.DATA is invalid；与 47001 的「参数为空/格式错误」不同码）。</summary>
    public const int TemplateParamInvalid = 47003;

    // ---------------------------------------------------------------- openApi 管理（P1-a，取值逐页核验官方文档）

    /// <summary>api 禁止清零调用次数：清零次数达到上限（clear_quota / clear_quota/v2，两接口合计每月 10 次）。</summary>
    public const int ClearQuotaLimitReached = 48006;

    /// <summary>rid 不存在（openapi/rid/get；rid 有效期仅 7 天）。</summary>
    public const int RidNotFound = 76001;

    /// <summary>rid 为空或格式错误（openapi/rid/get）。</summary>
    public const int RidInvalid = 76002;

    /// <summary>无权查询该 rid：当前账号无权限（rid 属其他账号调用所产生；openapi/rid/get 与额度查询共用语义面）。</summary>
    public const int RidPermissionDenied = 76003;

    /// <summary>rid 过期：仅支持持续 7 天内的 rid（openapi/rid/get）。</summary>
    public const int RidExpired = 76004;

    /// <summary>cgi_path not found：cgi_path 填错了（openapi/quota/get 与 openapi/quota/clear）。</summary>
    public const int CgiPathNotFound = 76021;

    /// <summary>could not use this cgi_path：当前调用接口使用的 token 与 api 所属账号不符（含「/xxx/sns/xxx」类接口不支持查询）。</summary>
    public const int CgiPathPermissionDenied = 76022;

    // ---------------------------------------------------------------- 群发消息 / 一次性订阅 / 二维码（P0-e，取值逐页核验官方文档）

    /// <summary>invalid tag id：tag_id 不存在（sendall）。</summary>
    public const int MassTagIdNotFound = 40152;

    /// <summary>invalid image count：图片个数超限（sendall，官方解决方案「减少图片个数」）。</summary>
    public const int MassImageCountExceeded = 40215;

    /// <summary>invalid msgtype：msgtype 参数错误（mass/send）。</summary>
    public const int MassMsgTypeInvalid = 45162;

    /// <summary>当前周期内的发表次数已用完（sendall；核验页面无频次上限数值，由官方此码表达）。</summary>
    public const int MassQuotaExhausted = 45028;

    /// <summary>已接广告，不支持 api 群发（sendall）。</summary>
    public const int MassAdContractBlocked = 45062;

    /// <summary>相同 clientmsgid 已存在群发记录（响应携带已存在任务的 msgid，24 小时防重窗口）。</summary>
    public const int MassClientMsgIdExists = 45065;

    /// <summary>相同 clientmsgid 重试速度过快（官方：请间隔 1 分钟重试）。</summary>
    public const int MassClientMsgIdRetryTooFast = 45066;

    /// <summary>clientmsgid 长度超过限制（官方 ≤ 32 字节）。</summary>
    public const int MassClientMsgIdTooLong = 45067;

    /// <summary>已不支持商品消息与卡券发表（官方 45113，附公告链接）。</summary>
    public const int MassWxCardUnsupported = 45113;

    /// <summary>该草稿最后一次是系统自动保存的，不允许群发（须在公众平台手动保存后重试）。</summary>
    public const int MassAutoSavedDraftBlocked = 48021;

    /// <summary>api 上传的视频不允许用 api 发表（sendall）。</summary>
    public const int MassApiUploadVideoBlocked = 48022;

    /// <summary>不符合声明文字原创的要求（sendall / mass/send，41040）。</summary>
    public const int MassOriginalityRequired = 41040;

    /// <summary>入参至少需要 2 个 openid（mass/send；touser 上限 10000 由 40032 表达）。</summary>
    public const int MassOpenIdListTooSmall = 40130;

    /// <summary>群发仍在审批流程中（API 群发安全保护；请稍等或联系管理员确认）。</summary>
    public const int MassApprovalPending = 89504;

    /// <summary>群发进入管理员确认流程（API 群发安全保护；管理员拒绝或 30 分钟无确认即失败）。</summary>
    public const int MassAdminConfirmPending = 89505;

    /// <summary>invalid title size：消息标题超限（一次性订阅消息，title 15 字以内；官方解决方案列留空，照录）。</summary>
    public const int OneTimeSubscribeTitleSizeInvalid = 40062;

    /// <summary>invalid action name：action 值有误（qrcode/create）。</summary>
    public const int QrcodeActionNameInvalid = 40052;

    /// <summary>invalid action info：action_info 不合法（qrcode/create；官方解决方案列留空，照录）。</summary>
    public const int QrcodeActionInfoInvalid = 40053;

    // ---------------------------------------------------------------- 草稿 / 发布 / 商品卡片 / 留言（P2，取值逐页核验官方文档）

    /// <summary>invalid index value：index 参数不合法（draft/update；官方解决方案列留空，照录）。</summary>
    public const int DraftIndexInvalid = 40114;

    /// <summary>invalid content_source_url：原文地址不合法（draft/update）。</summary>
    public const int DraftContentSourceUrlInvalid = 41039;

    /// <summary>invalid content：内容不合法（draft/update）。</summary>
    public const int DraftContentInvalid = 45166;

    /// <summary>账号已被限制带货能力（draft/add，请删除商品后重试）。</summary>
    public const int CommerceAbilityLimited = 53404;

    /// <summary>插入商品信息有误（draft/add，检查参数及商品状态）。</summary>
    public const int CommerceProductInfoInvalid = 53405;

    /// <summary>请先开通带货能力（draft/add）。</summary>
    public const int CommerceAbilityNotEnabled = 53406;

    /// <summary>该草稿未通过发布检查（freepublish/submit，检查草稿信息）。</summary>
    public const int PublishDraftCheckFailed = 53503;

    /// <summary>需前往公众平台官网使用草稿（freepublish/submit）。</summary>
    public const int PublishDraftMpOnly = 53504;

    /// <summary>请手动保存成功后再发表（freepublish/submit）。</summary>
    public const int PublishDraftNotManuallySaved = 53505;

    /// <summary>不合法的商品 ID（商品卡片）。</summary>
    public const int ProductCardProductIdInvalid = 10170001;

    /// <summary>不支持的文章类型（商品卡片）。</summary>
    public const int ProductCardArticleTypeUnsupported = 10170002;

    /// <summary>不支持的卡片类型（商品卡片）。</summary>
    public const int ProductCardCardTypeUnsupported = 10170003;

    /// <summary>without comment privilege：没有留言权限（留言管理全域前置）。</summary>
    public const int CommentPrivilegeMissing = 88000;

    /// <summary>msg_data is not exists：图文不存在（留言管理）。</summary>
    public const int CommentMsgDataNotExists = 88001;

    /// <summary>article is limit for safety：文章存在敏感信息（comment/open）。</summary>
    public const int CommentArticleSafetyLimited = 88002;

    /// <summary>elected comment upper limit：精选评论数已达上限（comment/markelect）。</summary>
    public const int CommentElectLimitReached = 88003;

    /// <summary>comment was deleted by user：已被用户删除，无法精选（comment/markelect）。</summary>
    public const int CommentDeletedByUser = 88004;

    /// <summary>already reply：已经回复过了（comment/reply/add）。</summary>
    public const int CommentAlreadyReplied = 88005;

    /// <summary>reply content beyond max len or content len is zero：回复超过长度限制或为空（comment/reply/add；官方文案「或为 0」照录）。</summary>
    public const int CommentReplyContentInvalid = 88007;

    /// <summary>comment is not exists：该评论不存在（留言管理）。</summary>
    public const int CommentNotExists = 88008;

    /// <summary>count range error：获取数目越界（comment/list；count 50 以上被拒绝；官方描述原文拼写「cout」照录）。</summary>
    public const int CommentCountOutOfRange = 88010;

    /// <summary>invalid signature：无效的签名（comment/reply/delete 页错误码表——本接口无签名参数，疑官方全局码表残留，照录）。</summary>
    public const int CommentReplySignatureInvalid = 87009;

    // ---------------------------------------------------------------- 数据统计（P2-d，取值逐页核验官方文档；21 端点错误码表跨页复用）

    /// <summary>date format error：日期格式错误（数据统计全域；getarticlesummary 页把 61500 描述为 date range error——官方跨页措辞不一，照录）。</summary>
    public const int DataCubeDateFormatError = 61500;

    /// <summary>date range error：日期跨度超过限制（数据统计全域；各端点跨度上限措辞不一，见 MpDateRangeRequest remarks）。</summary>
    public const int DataCubeDateRangeError = 61501;

    /// <summary>data not ready please try later：指定日期数据尚未生成（数据统计全域；getupstreammsg 页描述为「未完成数据统计处理」，照录）。</summary>
    public const int DataCubeDataNotReady = 61503;

    // ---------------------------------------------------------------- 智能接口（P3-a，取值逐页核验官方文档）

    /// <summary>invalid voice size：不合法的语音文件大小（voice/addvoicetorecofortext）。</summary>
    public const int VoiceSizeInvalid = 40010;

    /// <summary>invalid args size：不合法的参数（voice/translatecontent）。</summary>
    public const int ArgsSizeInvalid = 40035;

    /// <summary>invalid image url：图片 URL 错误（OCR 与图像处理全域共用）。</summary>
    public const int OcrImageUrlInvalid = 101000;

    /// <summary>certificate not found：未检测到证件（OCR 系 idcard / bankcard / bizlicense / menu）。</summary>
    public const int OcrCertificateNotFound = 101001;

    /// <summary>decode image failed：图片解码失败（OCR 系；官方解决方案列写「图片大小超过限制，resp_type = 0: 2MB，resp_type = 1: 10MB」但全文未定义 resp_type，照录）。</summary>
    public const int OcrImageDecodeFailed = 101002;

    /// <summary>not enough market quota：市场额度不足（OCR 系；官方解决方案列留空，照录）。</summary>
    public const int OcrMarketQuotaNotEnough = 101003;

    // ---------------------------------------------------------------- 扫二维码打开小程序 / 长信息与短链（P3-b，取值逐页核验官方文档）

    /// <summary>接口请求太快：超过 5 次/秒频率限制（qrcodejump* 四端点）。</summary>
    public const int QrcodeJumpRequestTooFast = 44990;

    /// <summary>invalid appid：appid 参数不合法（qrcodejumpget / qrcodejumpadd）。</summary>
    public const int QrcodeJumpInvalidAppId = 40166;

    /// <summary>链接错误（qrcodejumpadd；请检查链接合法性）。</summary>
    public const int QrcodeJumpLinkError = 85066;

    /// <summary>test url is not the sub prefix：测试链接不是子链接（qrcodejumpadd）。</summary>
    public const int QrcodeJumpTestUrlNotSubPrefix = 85068;

    /// <summary>check confirm file fail：校验文件失败（qrcodejumpadd；普通二维码场景）。</summary>
    public const int QrcodeJumpConfirmFileFailed = 85069;

    /// <summary>URL 命中黑名单 / 个人类型小程序无法设置二维码规则（qrcodejumpadd；官方一行承载两义，照录）。</summary>
    public const int QrcodeJumpUrlBlacklisted = 85070;

    /// <summary>链接重复：请勿重复添加（qrcodejumpadd）。</summary>
    public const int QrcodeJumpLinkDuplicated = 85071;

    /// <summary>链接被占用：检查链接归属（qrcodejumpadd）。</summary>
    public const int QrcodeJumpLinkOccupied = 85072;

    /// <summary>规则数已满：前缀个数已满（qrcodejumpadd）。</summary>
    public const int QrcodeJumpRuleQuotaFull = 85073;

    /// <summary>小程序未发布：小程序必须先发布代码才可以发布二维码跳转规则（qrcodejumppublish）。</summary>
    public const int QrcodeJumpMiniProgramNotPublished = 85074;

    /// <summary>can not access：个人类型小程序无法设置二维码规则（qrcodejumpget / qrcodejumppublish / qrcodejumpdelete）。</summary>
    public const int QrcodeJumpPersonalMiniProgramDenied = 85075;

    /// <summary>check ICP fail：检查 ICP 失败（qrcodejumpadd）。</summary>
    public const int QrcodeJumpIcpCheckFailed = 85076;

    /// <summary>数据异常：请删除后重新添加（qrcodejumppublish）。</summary>
    public const int QrcodeJumpDataAbnormal = 85095;

    /// <summary>beyond publish count this month：本月发布次数达到上限（qrcodejumppublish，原文标注 100 次/月）。</summary>
    public const int QrcodeJumpPublishQuotaExceeded = 886000;

    /// <summary>系统繁忙，请重试（qrcodejump* 全域；与 -1 并存，照录）。</summary>
    public const int QrcodeJumpSystemBusy = 886001;

    // 注：shorten/* 的 empty post data（44002）复用既存 EmptyPostData 常量（原注释「如删除标签未带 tag.id」
    // 为标签域场景举例，本域语义为「POST 数据包为空」——官方同一码值跨域复用，不复造常量）。

    /// <summary>data format error：解析 JSON/XML 内容错误（shorten/* 两端点）。</summary>
    public const int DataFormatError = 47001;

    /// <summary>参数错误 / argument invalid（shorten/* 两端点；fetch 页原文为「模板参数不准确」，本接口无模板概念，疑通用文案残留，照录）。</summary>
    public const int ShortLinkArgumentInvalid = 47003;

    /// <summary>long_data 长度超过限制（shorten/gen；上限 4KB）。</summary>
    public const int ShortenLongDataTooLong = 9410010;

    /// <summary>expire_seconds 超过限制（shorten/gen；上限 30 天）。</summary>
    public const int ShortenExpireOutOfRange = 9410011;

    /// <summary>short_key 不存在 / 过期 / 不属于本账号（shorten/fetch；官方错误码描述列留空，照录）。</summary>
    public const int ShortenKeyNotExists = 9410012;

    // ---------------------------------------------------------------- 微信门店·门店小程序（P4 首域，取值逐页核验官方文档）

    /// <summary>invalid args：参数错误（update_store；qrcodejumpget 页亦列 40097 且描述为「参数有误」，共用本常量）。</summary>
    public const int InvalidArgs = 40097;

    /// <summary>this appid does not have permission：无调用权限（apply_merchant；官方原文「仅开放给电商类目（电商平台、商家自营、跨境电商）」，照录）。</summary>
    public const int StoreAppIdPermissionDenied = 43104;

    /// <summary>需要补充资料：需填写 org_code 与 other_files 字段（apply_merchant；两字段官方标非必填，必填性矛盾照录）。</summary>
    public const int StoreSupplementRequired = 85024;

    /// <summary>this phone reach bind limit：管理员手机登记数量超上限，该主体不能开通门店（apply_merchant）。</summary>
    public const int StoreAdminPhoneBindLimit = 85025;

    /// <summary>this wechat account reach bind limit：该微信号已绑定 5 个管理员（apply_merchant）。</summary>
    public const int StoreAdminWeChatBindLimit = 85026;

    /// <summary>this idcard reach bind limit：管理员身份证已登记 5 次（apply_merchant）。</summary>
    public const int StoreAdminIdCardBindLimit = 85027;

    /// <summary>this contractor reach bind limit：该主体登记数量超上限，不能开通门店（apply_merchant）。</summary>
    public const int StoreContractorBindLimit = 85028;

    /// <summary>nickname has used：商家名称已被占用（apply_merchant）。</summary>
    public const int StoreNicknameUsed = 85029;

    /// <summary>invalid nickname size：昵称长度非法（apply_merchant；官方原文 4-30 字符、一中文占两字符）。</summary>
    public const int StoreNicknameSizeInvalid = 85030;

    /// <summary>nickname is forbidden：不能使用该名称（apply_merchant）。</summary>
    public const int StoreNicknameForbidden = 85031;

    /// <summary>nickname is complained：名称处于侵权投诉保护期（apply_merchant）。</summary>
    public const int StoreNicknameComplained = 85032;

    /// <summary>nickname is illegal：名称含违反公众平台协议或法律法规内容（apply_merchant）。</summary>
    public const int StoreNicknameIllegal = 85033;

    /// <summary>nickname is protected：名称在改名 15 天保护期内（apply_merchant）。</summary>
    public const int StoreNicknameProtected = 85034;

    /// <summary>nickname is forbidden for different contractor：需与该账号相同主体才可申请（apply_merchant）。</summary>
    public const int StoreNicknameContractorMismatch = 85035;

    /// <summary>introduction is illegal：介绍内容违规（apply_merchant）。</summary>
    public const int StoreIntroductionIllegal = 85036;

    /// <summary>store has added：请勿添加重复门店（add_store）。</summary>
    public const int StoreAlreadyAdded = 85038;

    /// <summary>store has added by others：此门店状态不能被获取信息（add_store）。</summary>
    public const int StoreNotAccessible = 85039;

    /// <summary>store has added by yourself：此门店已被绑定，无需重复绑定（add_store；官方描述拼写「yourseld」照录）。</summary>
    public const int StoreAlreadyBound = 85040;

    /// <summary>credential has used：该经营资质已添加（add_store）。</summary>
    public const int StoreCredentialUsed = 85041;

    /// <summary>nearby reach limit：附近地点添加数量达到上限（add_store）。</summary>
    public const int StoreNearbyLimitReached = 85042;

    /// <summary>reach headimg or introduction quota limit：头像或简介修改达每月上限（apply_merchant）。</summary>
    public const int StoreHeadImageQuotaLimit = 85049;

    /// <summary>verifying don't apply again：审核中，勿重复提交（apply_merchant）。</summary>
    public const int StoreAuditing = 85050;

    /// <summary>please apply merchant first：需先成功创建门店小程序再调用（apply_merchant / update_store）。</summary>
    public const int StoreMerchantNotApplied = 85053;

    /// <summary>poi_id is null：门店小程序尚未升级成功，需填写 poi_id 进行门店迁移（add_store）。</summary>
    public const int StorePoiIdRequired = 85054;

    /// <summary>map_poi_id is invalid：map_poi_id 无效（add_store）。</summary>
    public const int StoreMapPoiIdInvalid = 85055;

    /// <summary>mediaid is invalid：临时 mediaid 无效（apply_merchant / add_store）。</summary>
    public const int StoreMediaIdInvalid = 85056;

    /// <summary>poi_id is not exist：门店不存在（update_store）。</summary>
    public const int StorePoiNotExists = 65115;

    /// <summary>store status is invalid：该门店状态不允许更新（update_store）。</summary>
    public const int StoreStatusInvalid = 65118;

    // ---------------------------------------------------------------- 微信发票（P4 第三域，取值逐页核验官方文档）
    // 说明：官方在「微信发票」**多个页面重复列出同一张族级错误码表**（商户开票 / 开票平台 / 发票报销三族共用，
    // 部分码（如 72023/72024）语义仅对特定端点成立）——本段即该**族级共用错误码面**的去重结果，
    // 不按端点逐页重复建模。4xx 段另有 40078/40097（40097 复用既存 InvalidArgs）。

    /// <summary>invalid card status：card_id 未授权（沙箱未加测试白名单；正式为公众号未开通卡券权限或建卡与插卡间隔过短）。</summary>
    public const int InvoiceCardStatusInvalid = 40078;

    /// <summary>unauthorized create invoice：没有操作权限（检查是否已开通相应权限）。</summary>
    public const int InvoiceUnauthorized = 72015;

    /// <summary>invalid invoice title：发票抬头不一致。</summary>
    public const int InvoiceTitleMismatch = 72017;

    /// <summary>invoice has been lock by others：发票已被其他公众号锁定。</summary>
    public const int InvoiceLockedByOthers = 72023;

    /// <summary>invoice status error：发票状态错误。</summary>
    public const int InvoiceStatusError = 72024;

    /// <summary>invoice token error：wx_invoice_token 无效。</summary>
    public const int InvoiceTokenError = 72025;

    /// <summary>invoice never set pay mch info：未设置微信支付商户信息（需先 set_pay_mch）。</summary>
    public const int InvoicePayMchNotSet = 72028;

    /// <summary>invoice never set auth field：未设置授权字段（需先 set_auth_field）。</summary>
    public const int InvoiceAuthFieldNotSet = 72029;

    /// <summary>invalid mchid：微信支付商户号无效。</summary>
    public const int InvoiceMchIdInvalid = 72030;

    /// <summary>invalid params：参数错误（含无效参数名或未通过后台校验的值）。</summary>
    public const int InvoiceParamsInvalid = 72031;

    /// <summary>biz reject insert：财政电子票据已被拒绝领取（order_id 曾被用于拒绝开票即不可再插卡）。</summary>
    public const int InvoiceBizRejectInsert = 72035;

    /// <summary>invoice is busy：票据状态正在修改，请稍后再试。</summary>
    public const int InvoiceBusy = 72036;

    /// <summary>invoice order never auth：订单没有授权（s_pappid / 执收单位 appid / order_id 不匹配）。</summary>
    public const int InvoiceOrderNotAuthorized = 72038;

    /// <summary>invoice must be lock first：发票须先锁定（如报销前须先 LOCK）。</summary>
    public const int InvoiceMustLockFirst = 72039;

    /// <summary>invoice pdf error：Pdf 无效，请提供真实有效的 pdf。</summary>
    public const int InvoicePdfError = 72040;

    /// <summary>billing_code and billing_no repeated：票据号码与票据代码重复。</summary>
    public const int InvoiceBillingRepeated = 72042;

    /// <summary>billing_code or billing_no size error：票据号码或票据代码长度错误。</summary>
    public const int InvoiceBillingSizeError = 72043;

    /// <summary>scan text out of time：发票抬头二维码超时（scantitle）。</summary>
    public const int InvoiceScanTextOutOfTime = 72044;

    /// <summary>biz contact is empty：商户联系方式为空（需先 set_contact）。</summary>
    public const int InvoiceContactEmpty = 72063;

    /// <summary>sys error make out invoice failed：开票失败。</summary>
    public const int InvoiceMakeOutFailed = 73000;

    /// <summary>wxopenid error：微信 openid 错误。</summary>
    public const int InvoiceOpenIdError = 73001;

    /// <summary>ddh orderid empty：订单号（ddh）为空。</summary>
    public const int InvoiceOrderIdEmpty = 73002;

    /// <summary>fpqqlsh empty：发票请求流水号为空。</summary>
    public const int InvoiceFpqqlshEmpty = 73003;

    /// <summary>kplx empty：开票类型为空。</summary>
    public const int InvoiceKplxEmpty = 73004;

    /// <summary>nsrmc empty：纳税人名称为空。</summary>
    public const int InvoiceNsrmcEmpty = 73007;

    /// <summary>nsrdz empty：纳税人地址为空。</summary>
    public const int InvoiceNsrdzEmpty = 73008;

    /// <summary>nsrdh empty：纳税人电话为空。</summary>
    public const int InvoiceNsrdhEmpty = 73009;

    /// <summary>ghfmc empty：购货方名称为空。</summary>
    public const int InvoiceGhfmcEmpty = 73010;

    /// <summary>kpr empty：开票人为空。</summary>
    public const int InvoiceKprEmpty = 73011;

    /// <summary>jshj empty：价税合计为空。</summary>
    public const int InvoiceJshjEmpty = 73012;

    /// <summary>hjje empty：合计金额为空。</summary>
    public const int InvoiceHjjeEmpty = 73013;

    /// <summary>hjse empty：合计税额为空。</summary>
    public const int InvoiceHjseEmpty = 73014;

    /// <summary>hylx empty：行业类型为空。</summary>
    public const int InvoiceHylxEmpty = 73015;

    /// <summary>nsrsbh empty：纳税人识别号为空。</summary>
    public const int InvoiceNsrsbhEmpty = 73016;

    /// <summary>ka plat error：开票平台错误。</summary>
    public const int InvoiceKaPlatError = 73100;

    /// <summary>nsrsbh not cmp：纳税人识别号不匹。</summary>
    public const int InvoiceNsrsbhNotCmp = 73101;

    /// <summary>sys error：微信开票平台系统错误。</summary>
    public const int InvoicePlatformSystemError = 73102;

    /// <summary>Kp plat make invoice timeout：开票平台开票超时。</summary>
    public const int InvoicePlatformMakeTimeout = 73105;

    /// <summary>Fpqqlsh exist with different ddh：发票请求流水号已存在且对应不同订单号。</summary>
    public const int InvoiceFpqqlshExistWithDifferentOrder = 73106;

    /// <summary>Fpqqlsh is processing：发票请求流水号正在处理中。</summary>
    public const int InvoiceFpqqlshProcessing = 73107;

    /// <summary>This ddh with other fpqqlsh already exist：该订单号已存在其他发票请求流水号。</summary>
    public const int InvoiceOrderExistWithOtherFpqqlsh = 73108;

    /// <summary>fpqqlsh first 6 byte not cmp：发票请求流水号前 6 字节不匹配。</summary>
    public const int InvoiceFpqqlshPrefixNotCmp = 73110;
}
