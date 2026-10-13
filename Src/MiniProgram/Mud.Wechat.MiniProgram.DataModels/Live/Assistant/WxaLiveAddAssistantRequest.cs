// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 批量添加直播间小助手请求体（<c>POST /wxaapi/broadcast/room/addassistant</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_addveassistant.html"/>。
/// </para>
/// <para>
/// <b>本页官方拼写异常（照录不修）</b>：小助手三页的官方文件名为 <c>api_addveassistant.html</c>、
/// 接口英文名为 <c>addveAssistant</c>（多出 <c>ve</c>），而路由是 <c>room/addassistant</c>、
/// 云调用方法是 <c>liveBroadcast.addAssistant</c> —— SDK 对外只使用路由与云调用口径，
/// 该拼写差异仅留档于注释，不进入类型名。
/// </para>
/// <para>
/// <b>数量上限判据来自查询侧</b>：添加接口自身错误码表只有 <c>-1</c>（系统繁忙），小助手上限要看
/// <c>getassistantlist</c> 的 <c>maxCount</c>（官方示例为 10）⇒ SDK 不做本地数量校验。
/// </para>
/// <para>调用额度：10000 次/天。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveAddAssistantRequest
{
    /// <summary>房间 ID（<c>roomId</c>，必填）。</summary>
    [JsonPropertyName("roomId")]
    public long? RoomId { get; set; }

    /// <summary>用户数组（<c>users</c>，必填；官方类型 objarray），见 <see cref="WxaLiveAssistantUser"/>。</summary>
    [JsonPropertyName("users")]
    public List<WxaLiveAssistantUser>? Users { get; set; }
}
