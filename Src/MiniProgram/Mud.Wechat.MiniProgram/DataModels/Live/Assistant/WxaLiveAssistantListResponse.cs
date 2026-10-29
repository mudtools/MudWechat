// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 查询直播间小助手应答（<c>GET /wxaapi/broadcast/room/getassistantlist</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_getassistantlist.html"/>。
/// </para>
/// <para>
/// <b>本域唯一「官方 GET 页给出请求体表」的一族</b>：本页与 <c>getpushurl</c> / <c>getsharedcode</c> /
/// <c>getsubanchor</c> 四页在 GET 端点下把业务参数（<c>roomId</c> 等）写在「请求体 Request Payload」表内，
/// 但请求地址行只带 <c>access_token</c> ⇒ 本 SDK 一律按 <c>[Query]</c> 落地（全仓零 GET + <c>[Body]</c> 先例，
/// 声明请求体后组件行为不可预期），与交易保障域 <c>get_penalty_list</c> 的裁决同口径。
/// </para>
/// <para>
/// <b><see cref="MaxCount"/> 是小助手数量上限的权威来源</b>（官方示例 10）：添加前应以此为准，
/// 而非在 SDK 内写死常量。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveAssistantListResponse : WxaResponse
{
    /// <summary>小助手列表（<c>list</c>，官方类型 objarray），见 <see cref="WxaLiveAssistantItem"/>。</summary>
    [JsonPropertyName("list")]
    public List<WxaLiveAssistantItem>? List { get; set; }

    /// <summary>小助手个数（<c>count</c>，官方类型 number）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>小助手最大个数（<c>maxCount</c>，官方类型 number，官方示例为 10）。</summary>
    [JsonPropertyName("maxCount")]
    public long? MaxCount { get; set; }
}
