// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Kf;

/// <summary>设置 / 取消客服账号角色（管理员）请求体（<c>POST /customservice/kfaccount/setadmin</c>、<c>canceladmin</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>kf-mgnt/kf-message/api_setkfadmin.html</c>、<c>api_cancelkfadmin.html</c>。</para>
/// <para><c>kfaccount</c> 为<b>完整客服账号</b>（官方原文：账号前缀@公众号微信号），同一请求体两端点复用。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class WxaKfAccountRequest
{
    /// <summary>完整客服账号（<c>kfaccount</c>，必填；格式：账号前缀@公众号微信号）。</summary>
    [JsonPropertyName("kfaccount")]
    public string? KfAccount { get; set; }
}

/// <summary>注册客服子商户请求体（<c>POST /cgi-bin/business/register</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>kf-mgnt/kf-management/api_registerbusiness.html</c>。</para>
/// <para>
/// <b>子商户形态（官方原文）</b>：一个小程序账号可为平台内的商户创建多个子商户账号，创建后
/// 在小程序客服组件唤起子商户单独的会话；注册成功后以 <c>mch_id</c> 作为唯一标识参与后续
/// <c>update / get / list</c> 操作。<c>brand_icon</c> 为<b>图片永久素材</b>的媒体 ID。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class WxaBusinessRegisterRequest
{
    /// <summary>商户号（<c>mch_id</c>，必填；子商户唯一标识，后续更新 / 查询均以其定位）。</summary>
    [JsonPropertyName("mch_id")]
    public string? MchId { get; set; }

    /// <summary>品牌名称（<c>brand_name</c>，必填）。</summary>
    [JsonPropertyName("brand_name")]
    public string? BrandName { get; set; }

    /// <summary>品牌图标（<c>brand_icon</c>，必填；图片永久素材媒体 ID）。</summary>
    [JsonPropertyName("brand_icon")]
    public string? BrandIcon { get; set; }
}

/// <summary>更新客服子商户信息请求体（<c>POST /cgi-bin/business/update</c>）。</summary>
/// <remarks>官方文档：<c>kf-mgnt/kf-management/api_updatebusiness.html</c>。<c>mch_id</c> 必填定位子商户，品牌字段为待更新内容。</remarks>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class WxaBusinessUpdateRequest
{
    /// <summary>商户号（<c>mch_id</c>，必填）。</summary>
    [JsonPropertyName("mch_id")]
    public string? MchId { get; set; }

    /// <summary>品牌名称（<c>brand_name</c>）。</summary>
    [JsonPropertyName("brand_name")]
    public string? BrandName { get; set; }

    /// <summary>品牌图标（<c>brand_icon</c>；图片永久素材媒体 ID）。</summary>
    [JsonPropertyName("brand_icon")]
    public string? BrandIcon { get; set; }
}

/// <summary>拉取单个客服子商户请求体（<c>POST /cgi-bin/business/get</c>）。</summary>
/// <remarks>官方文档：<c>kf-mgnt/kf-management/api_getbusiness.html</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class WxaBusinessGetRequest
{
    /// <summary>商户号（<c>mch_id</c>，必填）。</summary>
    [JsonPropertyName("mch_id")]
    public string? MchId { get; set; }
}

/// <summary>客服子商户品牌信息（<c>business_info</c> 内嵌对象）。</summary>
/// <remarks>字段以官方页面为准；SDK 不做本地校验。</remarks>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class WxaBusinessInfo
{
    /// <summary>商户号（<c>mch_id</c>）。</summary>
    [JsonPropertyName("mch_id")]
    public string? MchId { get; set; }

    /// <summary>品牌名称（<c>brand_name</c>）。</summary>
    [JsonPropertyName("brand_name")]
    public string? BrandName { get; set; }

    /// <summary>品牌图标（<c>brand_icon</c>；图片永久素材媒体 ID）。</summary>
    [JsonPropertyName("brand_icon")]
    public string? BrandIcon { get; set; }
}

/// <summary>拉取单个客服子商户应答（<c>business_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class WxaBusinessGetResponse : WxaResponse
{
    /// <summary>子商户信息（<c>business_info</c>），见 <see cref="WxaBusinessInfo"/>。</summary>
    [JsonPropertyName("business_info")]
    public WxaBusinessInfo? BusinessInfo { get; set; }
}

/// <summary>拉取多个客服子商户应答（<c>business_list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class WxaBusinessListResponse : WxaResponse
{
    /// <summary>子商户列表（<c>business_list</c>），见 <see cref="WxaBusinessInfo"/>。</summary>
    [JsonPropertyName("business_list")]
    public List<WxaBusinessInfo>? BusinessList { get; set; }
}

/// <summary>微信客服定位请求体（<c>POST /customservice/work/{get,bind,unbind}</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>kf-work/api_getkfworkbound.html</c>、<c>api_bindkfwork.html</c>、<c>api_unbindkfwork.html</c>。</para>
/// <para><c>kf_openid</c> 为<b>微信客服账号标识</b>（官方原文），同一请求体三端点复用。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class WxaKfWorkRequest
{
    /// <summary>微信客服账号标识（<c>kf_openid</c>，必填）。</summary>
    [JsonPropertyName("kf_openid")]
    public string? KfOpenid { get; set; }
}

/// <summary>查询微信客服绑定情况应答（<c>kf_openid</c>）。</summary>
/// <remarks>官方文档：<c>kf-work/api_getkfworkbound.html</c>；字段以官方页面为准。</remarks>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class WxaKfWorkGetResponse : WxaResponse
{
    /// <summary>已绑定的微信客服账号标识（<c>kf_openid</c>）。</summary>
    [JsonPropertyName("kf_openid")]
    public string? KfOpenid { get; set; }
}

/// <summary>客服输入状态请求体（<c>POST /cgi-bin/message/custom/business/typing</c>）。</summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/kf-mgnt/kf-message/api_typing"/>
/// （2026-10-12 核验）。
/// </para>
/// <para>
/// <b>路由按账号类型二分（官方原文）</b>：公众号 / 服务号用 <c>/cgi-bin/message/custom/typing</c>，
/// <b>小程序 / 小游戏</b>用 <c>/cgi-bin/message/custom/business/typing</c> —— 本线只承载后者。
/// </para>
/// <para>
/// <b>时限约束（官方错误码反推）</b>：<c>45080</c> 要求下发前 30 秒内与该用户有过消息交互；
/// <c>45081</c> 表示已在输入状态、不可重复下发；<c>45072</c> 为 <c>command</c> 取值非法。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class WxaKfTypingRequest
{
    /// <summary>接收用户 OpenId（<c>touser</c>，必填）。</summary>
    [JsonPropertyName("touser")]
    public string? ToUser { get; set; }

    /// <summary>输入状态指令（<c>command</c>，必填），取值见 <see cref="WxaKfTypingCommands"/>。</summary>
    [JsonPropertyName("command")]
    public string? Command { get; set; }
}

/// <summary>客服输入状态指令取值（<c>command</c>，官方固定值）。</summary>
/// <remarks>
/// 官方请求示例<b>仅演示</b> <c>Typing</c>（下发「正在输入」状态）；错误码 <c>45072</c>（invalid command）
/// 表明官方对取值有白名单约束，故本类<b>不臆造</b>其它取值——确需更多取值时须先核官方页面再扩展。
/// </remarks>
public static class WxaKfTypingCommands
{
    /// <summary>下发「正在输入」状态（<c>Typing</c>）。</summary>
    public const string Typing = "Typing";
}