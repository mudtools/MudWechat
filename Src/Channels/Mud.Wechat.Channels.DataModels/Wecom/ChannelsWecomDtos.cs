// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Wecom;

/// <summary>获取关联账号企微 id（<c>get_wecom_id</c>）请求体。</summary>
/// <remarks>
/// 官方契约：<b>POST</b> + JSON 请求体；官方错误码 914910001（参数非法）/ 914911001（未绑定企业为关联账号）/
/// 914911002（查询不到 user_id）/ 914911003（企业微信未授权微信小店）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wecom")]
public class ChannelsWecomIdRequest
{
    /// <summary>获取或设置企业 corp_id（官方 <c>corp_id</c>，必填；须为店铺的关联账号，见企微文档基本概念 corpid 小节）。</summary>
    [JsonPropertyName("corp_id")]
    public string CorpId { get; set; } = string.Empty;

    /// <summary>获取或设置成员 user_id（官方 <c>user_id</c>，选填；须属于 corp_id 且在微信小店应用可见范围内）。</summary>
    [JsonPropertyName("user_id")]
    public string? UserId { get; set; }
}

/// <summary>获取关联账号企微 id（<c>get_wecom_id</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Wecom")]
public class ChannelsWecomIdResponse : ChannelsResponse
{
    /// <summary>获取或设置回包信息（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsWecomId? Data { get; set; }
}

/// <summary>企微在小店侧的映射 id（官方 <c>data</c> 对象，<c>get_wecom_id</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Wecom")]
public class ChannelsWecomId
{
    /// <summary>获取或设置企业微信 id（官方 <c>wecom_corp_id</c>；可传给店铺 H5 链接 / 二维码 / 口令端点的 wecom_corp_id 参数）。</summary>
    [JsonPropertyName("wecom_corp_id")]
    public string? WecomCorpId { get; set; }

    /// <summary>获取或设置企业微信账号 id（官方 <c>wecom_user_id</c>；可传给店铺 H5 链接 / 二维码 / 口令端点的 wecom_user_id 参数）。</summary>
    [JsonPropertyName("wecom_user_id")]
    public string? WecomUserId { get; set; }
}
