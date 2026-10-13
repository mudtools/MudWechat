// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 「媒资 id」请求体（<c>getmedia</c> 与 <c>deletemedia</c> 共用）。
/// </summary>
/// <remarks>
/// <para><b>共用裁决</b>：获取媒资详细信息与删除媒资的官方请求体字段集完全一致（单字段 <c>media_id</c>，均必填）⇒ 共用本类型；删除端点应答只有 <c>errcode</c>/<c>errmsg</c>，直接用 <see cref="Mud.Wechat.MiniProgram.DataModels.WxaResponse"/>。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_media/api_getmedia.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodMediaIdRequest
{
    /// <summary>media_id（官方类型 <c>number</c>，<b>必填</b>）：媒资文件id</summary>
    [JsonPropertyName("media_id")]
    public long? MediaId { get; set; }
}
