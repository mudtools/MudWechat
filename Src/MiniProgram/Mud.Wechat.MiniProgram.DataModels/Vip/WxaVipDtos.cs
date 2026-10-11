// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vip;

/// <summary>
/// 小程序「会员服务」域官方 DTO（<c>/wxa/vip/*</c> 6 端点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：微信小店文档 → 会员营销 → 小程序会员服务 → 小程序相关
/// （<c>https://developers.weixin.qq.com/doc/store/shop/API/wxamember/wxa/</c>，2026-10-12 逐页核验）。
/// 字段命名一律照抄官方参数表原文拼写（snake_case 转下划线原样保留）。
/// </para>
/// <para>
/// <b>归属裁决</b>：该 6 端点官方原文明确「小程序 API 请使用小程序身份，即小程序的
/// access_token 调用」⇒ 按小店线设计方案 §4.1 结论 A 归小程序线（小店线 CH-V1 锁定
/// Channels 程序集不得声明 <c>/wxa/vip/*</c> 路由）。
/// </para>
/// </remarks>

/// <summary>新增小程序会员信息请求体（<c>POST /wxa/vip/user/info/add</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>https://developers.weixin.qq.com/doc/store/shop/API/wxamember/wxa/api_v3_wxa_adduserinfo</c>。</para>
/// <para>
/// <b><c>session_id</c> 时效（官方原文）</b>：用户操作的会话 id，由小店传递给小程序，
/// <b>有效期 10 分钟</b>——宿主不得持久化复用，过期调用报参数异常（errcode 1）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class WxaVipAddUserRequest
{
    /// <summary>用户操作的会话 id（<c>session_id</c>，必填；由小店传递给小程序，有效期 10 分钟）。</summary>
    [JsonPropertyName("session_id")]
    public string? SessionId { get; set; }

    /// <summary>小程序用户 openid（<c>wxa_openid</c>，必填；须为本小程序 appid 的用户，否则报 errcode 2）。</summary>
    [JsonPropertyName("wxa_openid")]
    public string? WxaOpenId { get; set; }

    /// <summary>小程序会员信息（<c>info</c>，必填），见 <see cref="WxaVipMemberInfo"/>。</summary>
    [JsonPropertyName("info")]
    public WxaVipMemberInfo? Info { get; set; }
}

/// <summary>小程序会员信息（请求侧 <c>info</c>；<c>add</c> / <c>update</c> 两端点参数表完全一致 ⇒ 共用）。</summary>
/// <remarks>
/// <para>
/// <b>历史字段禁建模（官方变更日志，2026-10-12 核验）</b>：官方曾提供
/// <c>receive_register_coupon</c>（注册领券开关），现已从参数表<b>删除</b>——
/// 不得建模该字段，也不得回潮。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class WxaVipMemberInfo
{
    /// <summary>小程序定义的用户会员身份（<c>identity</c>，必填）。</summary>
    [JsonPropertyName("identity")]
    public string? Identity { get; set; }

    /// <summary>小程序定义的用户会员 id（<c>outer_userid</c>，选填；小程序侧自有会员体系的外部主键）。</summary>
    [JsonPropertyName("outer_userid")]
    public string? OuterUserId { get; set; }
}

/// <summary>更新小程序会员信息请求体（<c>POST /wxa/vip/user/info/update</c>）。</summary>
/// <remarks>官方文档：<c>https://developers.weixin.qq.com/doc/store/shop/API/wxamember/wxa/api_v3_wxa_updateuserinfo</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class WxaVipUpdateUserRequest
{
    /// <summary>小程序用户 openid（<c>wxa_openid</c>，必填）。</summary>
    [JsonPropertyName("wxa_openid")]
    public string? WxaOpenId { get; set; }

    /// <summary>小程序会员信息（<c>info</c>，必填；更新后小店侧同步展示最新会员信息）。</summary>
    [JsonPropertyName("info")]
    public WxaVipMemberInfo? Info { get; set; }
}

/// <summary>
/// 单个 <c>wxa_openid</c> 请求体（<c>user/info/get</c> 与 <c>user/info/delete</c> 参数表完全一致 ⇒ 共用，
/// 对齐小店线 Channels Vip 域「共用 openid 请求」先例）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class WxaVipOpenidRequest
{
    /// <summary>小程序用户 openid（<c>wxa_openid</c>，必填）。</summary>
    [JsonPropertyName("wxa_openid")]
    public string? WxaOpenId { get; set; }
}

/// <summary>获取小程序会员信息应答（<c>POST /wxa/vip/user/info/get</c>）。</summary>
/// <remarks>官方文档：<c>https://developers.weixin.qq.com/doc/store/shop/API/wxamember/wxa/api_v3_wxa_getuserinfo</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class WxaVipUserInfoResponse : WxaResponse
{
    /// <summary>小程序会员信息（<c>info</c>；未关联时缺省），见 <see cref="WxaVipUserInfo"/>。</summary>
    [JsonPropertyName("info")]
    public WxaVipUserInfo? Info { get; set; }
}

/// <summary>小程序会员信息（响应侧 <c>info</c>，<c>user/info/get</c> 参数表）。</summary>
/// <remarks>
/// <b>字段集边界</b>：本 DTO 照官方 <c>user/info/get</c> 参数表<b>不含</b> <c>wxa_openid</c>
/// （该字段仅 <c>user/list/get</c> 列表元素携带）——响应侧与请求侧、单查与列表的
/// 字段集差异均精确表达，禁止「顺手统一」。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class WxaVipUserInfo
{
    /// <summary>小程序定义的用户会员身份（<c>identity</c>）。</summary>
    [JsonPropertyName("identity")]
    public string? Identity { get; set; }

    /// <summary>小程序定义的用户会员 id（<c>outer_userid</c>）。</summary>
    [JsonPropertyName("outer_userid")]
    public string? OuterUserId { get; set; }

    /// <summary>用户当前已关联的小店 appid 列表（<c>related_shop_appid</c>，官方类型 array）。</summary>
    [JsonPropertyName("related_shop_appid")]
    public List<string>? RelatedShopAppIds { get; set; }
}

/// <summary>获取小程序会员列表请求体（<c>POST /wxa/vip/user/list/get</c>）。</summary>
/// <remarks>官方文档：<c>https://developers.weixin.qq.com/doc/store/shop/API/wxamember/wxa/api_v3_wxa_getuserlist</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class WxaVipGetUserListRequest
{
    /// <summary>偏移量（<c>offset</c>，必填；官方类型 number）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>本次读取数量（<c>limit</c>，必填；官方类型 number）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }
}

/// <summary>获取小程序会员列表应答（<c>POST /wxa/vip/user/list/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class WxaVipGetUserListResponse : WxaResponse
{
    /// <summary>小程序会员列表（<c>list</c>，官方类型 objarray），见 <see cref="WxaVipUserListItem"/>。</summary>
    [JsonPropertyName("list")]
    public List<WxaVipUserListItem>? List { get; set; }
}

/// <summary>小程序会员列表条目（<c>user/list/get</c> 返回 <c>list[]</c>）。</summary>
/// <remarks>
/// <para>
/// <b>与单查响应 <c>info</c> 的字段集差异照录</b>：列表元素<b>多</b>携带 <c>wxa_openid</c>
/// （官方原文如此），故不复用 <see cref="WxaVipUserInfo"/>。
/// </para>
/// <para>
/// <b>疑点照录</b>：官方「请求示例」JSON 中该字段拼作 <c>out_userid</c>（漏 er），
/// 与参数表 <c>outer_userid</c> 不一致——SDK 按参数表建模，序列化名以官方参数表为准。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class WxaVipUserListItem
{
    /// <summary>小程序定义的用户会员身份（<c>identity</c>）。</summary>
    [JsonPropertyName("identity")]
    public string? Identity { get; set; }

    /// <summary>小程序定义的用户会员 id（<c>outer_userid</c>；官方参数表拼写，请求示例中的 out_userid 为笔误）。</summary>
    [JsonPropertyName("outer_userid")]
    public string? OuterUserId { get; set; }

    /// <summary>小程序用户 openid（<c>wxa_openid</c>；仅列表元素携带，单查响应 info 无此字段）。</summary>
    [JsonPropertyName("wxa_openid")]
    public string? WxaOpenId { get; set; }

    /// <summary>用户已绑定的小店 appid 列表（<c>related_shop_appid</c>，官方类型 array）。</summary>
    [JsonPropertyName("related_shop_appid")]
    public List<string>? RelatedShopAppIds { get; set; }
}

/// <summary>获取小程序已关联小店列表应答（<c>POST /wxa/vip/shop/list/get</c>）。</summary>
/// <remarks>官方文档：<c>https://developers.weixin.qq.com/doc/store/shop/API/wxamember/wxa/api_v3_wxa_getshoplist</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class WxaVipGetShopListResponse : WxaResponse
{
    /// <summary>已关联小店列表（<c>list</c>，官方类型 objarray），见 <see cref="WxaVipShopListItem"/>。</summary>
    [JsonPropertyName("list")]
    public List<WxaVipShopListItem>? List { get; set; }
}

/// <summary>已关联小店条目（<c>shop/list/get</c> 返回 <c>list[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class WxaVipShopListItem
{
    /// <summary>已关联的小店 appid（<c>shop_appid</c>）。</summary>
    [JsonPropertyName("shop_appid")]
    public string? ShopAppId { get; set; }
}
