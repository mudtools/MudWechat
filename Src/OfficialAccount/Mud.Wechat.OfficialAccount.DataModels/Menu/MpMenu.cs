// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Menu;

/// <summary>
/// 自定义菜单按钮（一级与二级菜单<b>共用同一类型</b>：官方两处字段表逐字段一致）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方字段约束（逐项核验，勿弱化）</b>：
/// </para>
/// <list type="bullet">
/// <item><c>name</c>：菜单标题，<b>不超过 16 个字节</b>；<b>子菜单不超过 60 个字节</b>
/// （界面显示层面：一级最多 4 个汉字、二级最多 8 个汉字，超出以 <c>...</c> 代替；错误码 <c>40018</c>）。</item>
/// <item><c>key</c>：菜单 KEY 值（消息接口推送用），<b>不超过 128 字节</b>；<c>click</c> 等点击类型必须（错误码 <c>40019</c>）。</item>
/// <item><c>url</c>：网页链接，<b>不超过 1024 字节</b>；<c>view</c> / <c>miniprogram</c> 类型必填
/// （错误码 <c>40020</c> / <c>40027</c>；<b>域名非法</b> <c>40055</c>/<c>40054</c>）。</item>
/// <item><c>media_id</c>：永久素材合法 id；<c>media_id</c> 与 <c>view_limited</c> 类型必须。</item>
/// <item><c>appid</c>：小程序 appid（<b>仅认证账号可配置</b>，且<b>必须小写</b>）；<c>miniprogram</c> 类型必填。</item>
/// <item><c>pagepath</c>：小程序页面路径；<c>miniprogram</c> 类型必须。</item>
/// <item><c>article_id</c>：发布后获得的合法 <c>article_id</c>；<c>article_id</c> 与
/// <c>article_view_limited</c> 类型必须。</item>
/// <item><c>type</c> 与 <c>sub_button</c> <b>互斥</b>：同一节点要么自身是动作按钮，要么是含二级菜单的容器。</item>
/// </list>
/// <para>
/// <b>官方接口变更提醒</b>（2025-11-25 变更日志更新 <c>appid</c> 字段描述）：草稿接口灰度完成后，
/// <b>不再支持图文信息类型的 <c>media_id</c> 与 <c>view_limited</c></b>，应改用
/// <c>article_id</c> 与 <c>article_view_limited</c>。
/// </para>
/// <para>
/// <b>类型取值与本 SDK 的建模取舍</b>：官方 <c>type</c> 枚举 12 项（<c>click</c> / <c>view</c> /
/// <c>scancode_push</c> / <c>scancode_waitmsg</c> / <c>pic_sysphoto</c> / <c>pic_photo_or_album</c> /
/// <c>pic_weixin</c> / <c>location_select</c> / <c>media_id</c> / <c>article_id</c> /
/// <c>article_view_limited</c> / <c>miniprogram</c>），常量见 <c>MpMenuButtonTypes</c>。
/// 其中 3~8 类事件仅支持 iPhone 5.4.1+ / Android 5.4+ 客户端；9~11 类为「第三方平台旗下<b>未认证</b>公众号」
/// 准备且<b>无事件推送</b>，自建/已认证账号不必使用。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Menu")]
public class MpMenuButton
{
    /// <summary>菜单的响应动作类型（与 <see cref="SubButton"/> 互斥）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>菜单标题（一级 ≤ 16 字节；子菜单 ≤ 60 字节）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>菜单 KEY 值（≤ 128 字节；<c>click</c> 等点击类型必须）。</summary>
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    /// <summary>网页链接（≤ 1024 字节；<c>view</c> / <c>miniprogram</c> 类型必填）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>永久素材合法 <c>media_id</c>（<c>media_id</c> 与 <c>view_limited</c> 类型必须）。</summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }

    /// <summary>小程序 appid（仅认证账号可配置，<b>必须小写</b>；<c>miniprogram</c> 类型必填）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>小程序页面路径（<c>miniprogram</c> 类型必须）。</summary>
    [JsonPropertyName("pagepath")]
    public string? PagePath { get; set; }

    /// <summary>发布后获得的合法 <c>article_id</c>（<c>article_id</c> 与 <c>article_view_limited</c> 类型必须）。</summary>
    [JsonPropertyName("article_id")]
    public string? ArticleId { get; set; }

    /// <summary>二级菜单结构体数组（与 <see cref="Type"/> <b>互斥</b>）。</summary>
    [JsonPropertyName("sub_button")]
    public List<MpMenuButton>? SubButton { get; set; }
}

/// <summary>默认菜单（官方 <c>menu</c> 对象，仅含一级菜单数组）。</summary>
[HttpJsonSerializable(SerializerClassName = "Menu")]
public class MpMenu
{
    /// <summary>一级菜单数组（官方：<b>1~3 个</b>）。</summary>
    [JsonPropertyName("button")]
    public List<MpMenuButton>? Button { get; set; }
}

/// <summary>
/// 个性化菜单匹配规则（<c>matchrule</c>）；官方要求<b>至少一个非空字段</b>。
/// </summary>
/// <remarks>
/// 官方「注意事项」：个性化菜单支持用户标签，当用户身上的标签<b>超过 1 个</b>时，
/// <b>以最后打上的标签</b>为匹配依据；匹配规则包含隐私字段会被拒（错误码 <c>65320 match rule violates privacy</c>）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Menu")]
public class MpMenuMatchRule
{
    /// <summary>用户标签的 id（可通过用户标签管理接口获取）。</summary>
    [JsonPropertyName("tag_id")]
    public string? TagId { get; set; }

    /// <summary>客户端版本，当前只具体到系统型号：<c>IOS(1)</c> / <c>Android(2)</c> / <c>Others(3)</c>；不填则不做匹配。</summary>
    [JsonPropertyName("client_platform_type")]
    public string? ClientPlatformType { get; set; }
}

/// <summary>
/// 个性化菜单（<c>button</c> + <c>matchrule</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何同一类型既作请求体又作响应元素</b>：官方「创建个性化菜单」请求体与「获取自定义菜单配置」的
/// <c>conditionalmenu</c> 元素<b>字段表逐字段一致</b>（仅 <c>button</c> + <c>matchrule</c>），
/// 拆成两个同构类属纯冗余。
/// </para>
/// <para>
/// <b>不含 <c>menuid</c></b>：官方「获取自定义菜单配置」的 <c>conditionalmenu</c> 字段表<b>未列</b>
/// <c>menuid</c>（该字段仅由「创建个性化菜单」响应下发）⇒ 建模为两个类型：本类型（查询用）与
/// <see cref="MpAddConditionalMenuResponse"/>（创建响应）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Menu")]
public class MpConditionalMenu
{
    /// <summary>一级菜单数组（官方：<b>1~3 个</b>）。</summary>
    [JsonPropertyName("button")]
    public List<MpMenuButton>? Button { get; set; }

    /// <summary>菜单匹配规则（至少一个非空字段）。</summary>
    [JsonPropertyName("matchrule")]
    public MpMenuMatchRule? MatchRule { get; set; }
}
