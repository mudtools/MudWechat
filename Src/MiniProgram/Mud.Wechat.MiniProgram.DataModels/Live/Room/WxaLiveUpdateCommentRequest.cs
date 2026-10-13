// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 禁言管理请求体（<c>POST /wxaapi/broadcast/room/updatecomment</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_updatecomment.html"/>。
/// </para>
/// <para>
/// <b>官方参数表与官方示例字段名不一致（照录参数表）</b>：本页「请求体 Request Payload」表为
/// <c>id</c> + <c>banComment</c>，而页内「请求示例」写 <c>{ "roomId": 6474, "banComment": 1 }</c>
/// —— 本 SDK 按<b>参数表</b>建模为 <c>id</c>（与删除直播间同键，官方原文「房间ID」）；
/// 差异属官方页面自身不一致，已在本注释留档，宿主若遇 <c>200002</c> 参数错误可回退验证示例拼写。
/// </para>
/// <para>
/// <b>与创建时的 <c>closeComment</c> 区分</b>：<c>closeComment</c> 是「直播开始前关闭评论入口、开始后不可开启」，
/// 本端点是「直播中禁言 / 取消禁言」，两者字段语义不互通。
/// </para>
/// <para>调用额度：10000 次/天。官方错误码：<c>300022</c> / <c>300023</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveUpdateCommentRequest
{
    /// <summary>房间 ID（<c>id</c>，必填；官方示例处写作 <c>roomId</c>，本 SDK 照参数表取 <c>id</c>）。</summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>禁言开关（<c>banComment</c>，必填）：1 禁言、0 取消禁言。</summary>
    [JsonPropertyName("banComment")]
    public long? BanComment { get; set; }
}
