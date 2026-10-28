// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 微信客服客户基础信息（<c>/cgi-bin/kf/customer/batchget</c> 的 <c>customer_list</c> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfCustomerInfo
{
    /// <summary>
    /// 获取或设置微信客户的 external_userid。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public string? ExternalUserId { get; set; }

    /// <summary>
    /// 获取或设置微信昵称。
    /// </summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>
    /// 获取或设置微信头像 URL（第三方 / 代开发应用不可获取）。
    /// </summary>
    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }

    /// <summary>
    /// 获取或设置性别（第三方 / 代开发应用统一返回 0）。
    /// </summary>
    [JsonPropertyName("gender")]
    public int? Gender { get; set; }

    /// <summary>
    /// 获取或设置 unionid（需绑定微信开发者账号才能获取；第三方应用不可获取，
    /// 须自行获取 unionid 后通过「Unionid 与 external_userid 关联接口」关联）。
    /// </summary>
    [JsonPropertyName("unionid")]
    public string? UnionId { get; set; }

    /// <summary>
    /// 获取或设置客户 48 小时内最后一次进入会话的上下文
    /// （仅当请求 need_enter_session_context = 1 时返回）。
    /// </summary>
    [JsonPropertyName("enter_session_context")]
    public KfEnterSessionContext? EnterSessionContext { get; set; }
}

/// <summary>
/// 客户进入会话的上下文（<see cref="KfCustomerInfo.EnterSessionContext"/>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfEnterSessionContext
{
    /// <summary>
    /// 获取或设置进入会话的场景值（获取客服账号链接时开发者自定义的 scene）。
    /// </summary>
    [JsonPropertyName("scene")]
    public string? Scene { get; set; }

    /// <summary>
    /// 获取或设置进入会话的自定义参数（客服链接拼接的 scene_param，原样返回）。
    /// </summary>
    [JsonPropertyName("scene_param")]
    public string? SceneParam { get; set; }

    /// <summary>
    /// 获取或设置视频号信息（仅当客户从视频号进入会话时返回）。
    /// </summary>
    [JsonPropertyName("wechat_channels")]
    public KfWechatChannelsInfo? WechatChannels { get; set; }
}

/// <summary>
/// 客户进入会话的视频号来源信息（<see cref="KfEnterSessionContext.WechatChannels"/>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfWechatChannelsInfo
{
    /// <summary>
    /// 获取或设置视频号名称（场景值 1 / 2 / 3 时返回）。
    /// </summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>
    /// 获取或设置视频号小店名称（场景值 4 / 5 时返回）。
    /// </summary>
    [JsonPropertyName("shop_nickname")]
    public string? ShopNickname { get; set; }

    /// <summary>
    /// 获取或设置视频号场景值：1 - 视频号主页，2 - 直播间商品列表页，3 - 商品橱窗页，
    /// 4 - 小店商品详情页，5 - 小店订单页。
    /// </summary>
    [JsonPropertyName("scene")]
    public int? Scene { get; set; }
}
