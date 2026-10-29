// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.MerchantService;

/// <summary>
/// 即时服务消息体（官方 <c>message</c>；协商历史侧同构字段叫 <c>normal_message</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4017151596"/>（回复需要即时服务的投诉单）
/// 与 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012533439"/>（查询投诉单协商历史），
/// 2026-10-14 两面七十九行逐字段比对<b>一致</b> ⇒ 官方原文即「与发送消息的结构一致」，本仓据此刻<b>一份</b>类型。
/// </para>
/// <para>
/// <b>两面复用是本域最大的复用</b>：同一棵树既是「商户发出去的消息」（请求侧 <c>message</c>），
/// 也是「商户已发出去的消息的回读形态」（协商历史 <c>normal_message</c>）。
/// </para>
/// <para>
/// <b>除 <c>blocks</c> 外两字段皆选填</b>；<c>custom_data</c> 官方 string(4096)，是商户自定义透传信息，
/// 用户点击动作时官方会原样带回（见 <see cref="MerchantServiceClickMessage"/>）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceNormalMessage
{
    /// <summary>消息内容块列表（<c>blocks</c>，选填 array[object]），块类型见 <see cref="MerchantServiceMessageBlockTypes"/>。</summary>
    [JsonPropertyName("blocks")]
    public MerchantServiceMessageBlock[]? Blocks { get; set; }

    /// <summary>发送者身份类别（<c>sender_identity</c>，选填 string），取值见 <see cref="MerchantServiceSenderIdentities"/>。</summary>
    [JsonPropertyName("sender_identity")]
    public string? SenderIdentity { get; set; }

    /// <summary>自定义透传信息（<c>custom_data</c>，选填 string(4096)）。</summary>
    [JsonPropertyName("custom_data")]
    public string? CustomData { get; set; }
}

/// <summary>
/// 消息块（官方 <c>blocks[]</c>）：六种类型共用一个「判别式 + 六个可空子对象」的扁平结构。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4017151596"/>（2026-10-14 核验）。
/// </para>
/// <para>
/// <b>为何做成并集而非六个子类</b>：本仓 AOT 红线（PAY-B6）禁反射多态序列化，
/// 官方报文本身就是「<c>type</c> 决定哪个键有值」的并集表 ⇒ 类型形态照官方，
/// 一致性由调用方保证、错配由官方拒绝（守卫 MC-B5 锁定「SDK 不做客户端校验」）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceMessageBlock
{
    /// <summary>消息块类型（<c>type</c>，选填 string），取值见 <see cref="MerchantServiceMessageBlockTypes"/>。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>文本块内容（<c>text</c>，当 <c>type</c> 为 <c>TEXT</c> 时使用）。</summary>
    [JsonPropertyName("text")]
    public MerchantServiceTextBlock? Text { get; set; }

    /// <summary>图片块内容（<c>image</c>，当 <c>type</c> 为 <c>IMAGE</c> 时使用）。</summary>
    [JsonPropertyName("image")]
    public MerchantServiceImageBlock? Image { get; set; }

    /// <summary>链接块内容（<c>link</c>，当 <c>type</c> 为 <c>LINK</c> 时使用）。</summary>
    [JsonPropertyName("link")]
    public MerchantServiceLinkBlock? Link { get; set; }

    /// <summary>FAQ 列表（<c>faq_list</c>，当 <c>type</c> 为 <c>FAQ_LIST</c> 时使用）。</summary>
    [JsonPropertyName("faq_list")]
    public MerchantServiceFaqList? FaqList { get; set; }

    /// <summary>按钮块内容（<c>button</c>，当 <c>type</c> 为 <c>BUTTON</c> 时使用）。</summary>
    [JsonPropertyName("button")]
    public MerchantServiceButtonBlock? Button { get; set; }

    /// <summary>按钮组块内容（<c>button_group</c>，当 <c>type</c> 为 <c>BUTTON_GROUP</c> 时使用）。</summary>
    [JsonPropertyName("button_group")]
    public MerchantServiceButtonGroupBlock? ButtonGroup { get; set; }
}

/// <summary>文本消息块（官方 <c>blocks[].text</c>）。</summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4017151596"/>（2026-10-14 核验）。
/// <b><c>is_bold</c> 官方是 boolean 且无「必填」</b> ⇒ 不得建成 <c>bool</c>（缺失与 false 是两种事实）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceTextBlock
{
    /// <summary>文字内容（<c>text</c>，选填 string）。</summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>文本颜色（<c>color</c>，选填 string），取值见 <see cref="MerchantServiceMessageTextColors"/>。</summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    /// <summary>是否粗体（<c>is_bold</c>，选填 boolean）。</summary>
    [JsonPropertyName("is_bold")]
    public bool? IsBold { get; set; }
}

/// <summary>图片消息块（官方 <c>blocks[].image</c>）。</summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4017151596"/>（2026-10-14 核验）。
/// <b><c>media_id</c> 须先经「商户上传反馈图片」通道取得</b>（见
/// <c>Mud.Wechat.Pay.MerchantService.IWechatPayComplaintMediaService</c>）：即时服务面不接受图片 URL，
/// 只接受该接口返回的媒体标识。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceImageBlock
{
    /// <summary>媒体 ID（<c>media_id</c>，选填 string）：图片上传通道返回的标识。</summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }

    /// <summary>图片样式类型（<c>image_style_type</c>，选填 string），取值见 <see cref="MerchantServiceImageStyleTypes"/>。</summary>
    [JsonPropertyName("image_style_type")]
    public string? ImageStyleType { get; set; }
}

/// <summary>链接消息块（官方 <c>blocks[].link</c>）：文案 + 动作 + 失效配置。</summary>
/// <remarks>官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4017151596"/>（2026-10-14 核验）。</remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceLinkBlock
{
    /// <summary>链接文案（<c>text</c>，选填 string）。</summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>动作（<c>action</c>，选填）：四棵子树共用同一张动作表 ⇒ 共用 <see cref="MerchantServiceAction"/>。</summary>
    [JsonPropertyName("action")]
    public MerchantServiceAction? Action { get; set; }

    /// <summary>失效配置（<c>invalid_info</c>，选填）。</summary>
    [JsonPropertyName("invalid_info")]
    public MerchantServiceInvalidInfo? InvalidInfo { get; set; }
}

/// <summary>FAQ 列表（官方 <c>blocks[].faq_list</c>）。</summary>
/// <remarks>官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4017151596"/>（2026-10-14 核验）。</remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceFaqList
{
    /// <summary>FAQ 列表（<c>faqs</c>，选填 array[object]）。</summary>
    [JsonPropertyName("faqs")]
    public MerchantServiceFaq[]? Faqs { get; set; }
}

/// <summary>单条 FAQ（官方 <c>blocks[].faq_list.faqs[]</c>）。</summary>
/// <remarks>官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4017151596"/>（2026-10-14 核验）。</remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceFaq
{
    /// <summary>FAQ 的 ID（<c>faq_id</c>，选填 string）。</summary>
    [JsonPropertyName("faq_id")]
    public string? FaqId { get; set; }

    /// <summary>FAQ 内容（<c>faq_title</c>，选填 string）。</summary>
    [JsonPropertyName("faq_title")]
    public string? FaqTitle { get; set; }

    /// <summary>动作（<c>action</c>，选填）。</summary>
    [JsonPropertyName("action")]
    public MerchantServiceAction? Action { get; set; }
}

/// <summary>
/// 按钮消息块（官方 <c>blocks[].button</c>）：文案 + 动作 + <b>失效配置</b>。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4017151596"/>（2026-10-14 核验）。
/// </para>
/// <para>
/// <b>与按钮组的项<b>不</b>是同一张表</b>：单按钮带 <c>invalid_info</c>，而 <c>button_group.buttons[]</c>
/// 只有 <c>text</c> + <c>action</c> ⇒ 按「表相同才共用」的口径<b>分建</b>两类型
/// （<see cref="MerchantServiceButtonBlock"/> 与 <see cref="MerchantServiceButtonItem"/>），
/// 合成一型会让按钮组项多出一个官方不认的键（守卫 MC-B6 锁定分建裁决）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceButtonBlock
{
    /// <summary>按钮文本（<c>text</c>，选填 string）。</summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>动作（<c>action</c>，选填）。</summary>
    [JsonPropertyName("action")]
    public MerchantServiceAction? Action { get; set; }

    /// <summary>失效配置（<c>invalid_info</c>，选填）。</summary>
    [JsonPropertyName("invalid_info")]
    public MerchantServiceInvalidInfo? InvalidInfo { get; set; }
}

/// <summary>按钮组消息块（官方 <c>blocks[].button_group</c>）。</summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4017151596"/>（2026-10-14 核验）。
/// <b>失效配置挂在组上而非各按钮上</b>（组内项无 <c>invalid_info</c>）⇒ 整组同生同灭。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceButtonGroupBlock
{
    /// <summary>按钮组（<c>buttons</c>，选填 array[object]）。</summary>
    [JsonPropertyName("buttons")]
    public MerchantServiceButtonItem[]? Buttons { get; set; }

    /// <summary>按钮布局方式（<c>button_layout</c>，选填 string），默认 <c>LAYOUT_HORIZONTAL</c>。</summary>
    [JsonPropertyName("button_layout")]
    public string? ButtonLayout { get; set; }

    /// <summary>失效配置（<c>invalid_info</c>，选填）。</summary>
    [JsonPropertyName("invalid_info")]
    public MerchantServiceInvalidInfo? InvalidInfo { get; set; }
}

/// <summary>
/// 按钮组内的单个按钮（官方 <c>blocks[].button_group.buttons[]</c>）——<b>与单按钮块分建</b>，官方此处无 <c>invalid_info</c>。
/// </summary>
/// <remarks>官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4017151596"/>（2026-10-14 核验）。</remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceButtonItem
{
    /// <summary>按钮文本（<c>text</c>，选填 string）。</summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>动作（<c>action</c>，选填）。</summary>
    [JsonPropertyName("action")]
    public MerchantServiceAction? Action { get; set; }
}

/// <summary>
/// 动作（官方 <c>action</c>）：<b>链接 / FAQ / 单按钮 / 按钮组四棵子树的动作表逐行一致 ⇒ 共用本一型</b>。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4017151596"/>（2026-10-14 逐行核验四处动作表）。
/// </para>
/// <para>
/// <b>条件必填写在描述里、必填列全为 false</b>：<c>ACTION_TYPE_SEND_MESSAGE</c> 要求 <c>message_info</c>、
/// <c>ACTION_TYPE_JUMP_URL</c> 要求 <c>jump_url</c>、<c>ACTION_TYPE_JUMP_MINI_PROGRAM</c> 要求小程序信息。
/// 官方页对小程序支只写了「此时」半句（正文截断）⇒ 本仓<b>不</b>推断其必填键组合（守卫 MC-B5）。
/// </para>
/// <para>
/// <b><c>action_id</c> 是点击回读的连接键</b>：用户点击产生的留言会把该值带回
/// <see cref="MerchantServiceClickMessage.ActionId"/>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceAction
{
    /// <summary>动作类型（<c>action_type</c>，选填 string），取值见 <see cref="MerchantServiceActionTypes"/>。</summary>
    [JsonPropertyName("action_type")]
    public string? ActionType { get; set; }

    /// <summary>跳转链接（<c>jump_url</c>，选填 string）：<c>ACTION_TYPE_JUMP_URL</c> 时官方要求必填。</summary>
    [JsonPropertyName("jump_url")]
    public string? JumpUrl { get; set; }

    /// <summary>跳转的小程序（<c>mini_program_jump_info</c>，选填）：两键皆选填，与回复支的三键必填形态<b>分建</b>。</summary>
    [JsonPropertyName("mini_program_jump_info")]
    public MerchantServiceActionMiniProgramJumpInfo? MiniProgramJumpInfo { get; set; }

    /// <summary>回复消息内容（<c>message_info</c>，选填）：<c>ACTION_TYPE_SEND_MESSAGE</c> 时官方要求必填。</summary>
    [JsonPropertyName("message_info")]
    public MerchantServiceActionMessageInfo? MessageInfo { get; set; }

    /// <summary>动作 ID（<c>action_id</c>，选填 string）：用户点击后随留言回传。</summary>
    [JsonPropertyName("action_id")]
    public string? ActionId { get; set; }
}

/// <summary>
/// 动作内的小程序跳转信息（官方 <c>action.mini_program_jump_info</c>，<b>两键皆选填</b>）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4017151596"/>（2026-10-14 核验）。
/// <b>与「回复用户」支的同名对象不是一回事</b>：那里 <c>appid</c>/<c>path</c>/<c>text</c> 三键<b>全必填</b>
/// 且多出 <c>text</c> ⇒ 分建两型（另一型见 <see cref="MerchantServiceComplaintMiniProgramJumpInfo"/>），
/// 共用即把必填性写进错误的一侧（守卫 MC-B6）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceActionMiniProgramJumpInfo
{
    /// <summary>小程序 AppID（<c>appid</c>，选填 string）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>小程序页面路径（<c>path</c>，选填 string）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }
}

/// <summary>动作内「点击后回复的消息」（官方 <c>action.message_info</c>）。</summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4017151596"/>（2026-10-14 核验）。
/// <b><c>content</c> 是整棵消息树里唯一在此子树标必填的叶子</b>（官方 true）⇒ 建成 <c>string?</c> 仍要传值，
/// 传 null 由官方拒绝，SDK 不做客户端校验。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceActionMessageInfo
{
    /// <summary>回复的消息内容（<c>content</c>，官方<b>必填</b> string）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>自定义透传字段（<c>custom_data</c>，选填 string）。</summary>
    [JsonPropertyName("custom_data")]
    public string? CustomData { get; set; }
}

/// <summary>
/// 失效配置（官方 <c>invalid_info</c>，链接 / 单按钮 / 按钮组三处共用同一张表）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4017151596"/>（2026-10-14 核验）。
/// <b><c>expired_time</c> 官方原文要求 rfc3339 且带毫秒与时区</b>（示例 <c>2015-05-20T13:29:35+08:00</c>）；
/// 本仓以 <c>string</c> 承载、不做客户端格式校验（线内口径：官方是 string 就照 string）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceInvalidInfo
{
    /// <summary>过期时间（<c>expired_time</c>，选填 string，rfc3339）。</summary>
    [JsonPropertyName("expired_time")]
    public string? ExpiredTime { get; set; }

    /// <summary>是否可多次点击（<c>multi_clickable</c>，选填 boolean）。</summary>
    [JsonPropertyName("multi_clickable")]
    public bool? MultiClickable { get; set; }
}

/// <summary>
/// 用户点击产生的回复留言（官方 <c>click_message</c>，仅在协商历史 <c>operate_type == USER_CLICK_RESPONSE</c> 时出现）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012533439"/>（2026-10-14 逐字段核验）。
/// 只读面 —— 本类型不出现在任何请求体里。
/// </para>
/// <para>
/// <b>⚠️ <c>action_id</c> 官方标 string(4096)</b>，而请求侧动作的 <c>action_id</c> 是无长度声明的 string，
/// 且描述写「自定义透传数据」而非「动作 id」（官方两处描述互换感强）⇒ 字段名照官方、语义按官方页面原文，
/// 本仓不做二次解释。
/// </para>
/// <para>
/// <b><c>clicked_log_id</c> 是溯源键</b>：指向被点击的那条操作流水，可与
/// <see cref="MerchantServiceNegotiationHistoryItem.LogId"/> 关联还原「谁点了哪条回复」。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceClickMessage
{
    /// <summary>回复的消息内容（<c>message_content</c>，选填 string(1024)）。</summary>
    [JsonPropertyName("message_content")]
    public string? MessageContent { get; set; }

    /// <summary>自定义透传数据（<c>action_id</c>，选填 string(4096)）。</summary>
    [JsonPropertyName("action_id")]
    public string? ActionId { get; set; }

    /// <summary>被点击的操作流水号（<c>clicked_log_id</c>，选填 string(128)）。</summary>
    [JsonPropertyName("clicked_log_id")]
    public string? ClickedLogId { get; set; }
}
