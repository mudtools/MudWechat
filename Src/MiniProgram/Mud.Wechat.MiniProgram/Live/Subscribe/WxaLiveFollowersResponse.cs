// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 获取长期订阅用户应答（<c>POST /wxa/business/get_wxa_followers</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/subscribe-management/api_getfollowers.html"/>。
/// </para>
/// <para>
/// <b><see cref="PageBreak"/> 是下一页游标</b>：官方原文「获取下一页时带上该值」⇒ 翻页循环须把它回填到
/// <see cref="WxaLiveFollowersRequest.PageBreak"/>，首页请求不带该字段。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveFollowersResponse : WxaResponse
{
    /// <summary>长期订阅用户列表（<c>followers</c>，官方类型 array），见 <see cref="WxaLiveFollowerItem"/>。</summary>
    [JsonPropertyName("followers")]
    public List<WxaLiveFollowerItem>? Followers { get; set; }

    /// <summary>翻页标记（<c>page_break</c>，官方类型 number；取下一页时回填）。</summary>
    [JsonPropertyName("page_break")]
    public long? PageBreak { get; set; }
}
