// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>获取机构绑定的推客信息请求（官方 <c>promoter/get_bind_sharer_list</c> 请求体）。</summary>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterGetBindSharerListRequest
{
    /// <summary>获取或设置翻页参数（官方 <c>next_key</c>，必填，第一页为空，后续传上一页返回的值）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }

    /// <summary>获取或设置每页推客数（官方 <c>page_size</c>，必填，最大 20）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置待查询推客的 appid（官方 <c>sharer_appid</c>）。</summary>
    [JsonPropertyName("sharer_appid")]
    public string? SharerAppId { get; set; }

    /// <summary>获取或设置待查询推客的小程序 openid（官方 <c>sharer_openid</c>，必须是机构绑定小程序的用户 openid，不能是其它小程序的 openid）。</summary>
    [JsonPropertyName("sharer_openid")]
    public string? SharerOpenId { get; set; }
}
