// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 剧集替换项（<c>old</c> 旧剧集 media_id + <c>new</c> 新剧集 media_id）。
/// </summary>
/// <remarks>
/// <para><b>共用裁决</b>：<c>auditdrama</c>（重新提审时替换未通过剧集）与 <c>submitreplacedramamedias</c>（提交替换剧集审核）的官方子表字段名完全相同 ⇒ 共用本类型。</para>
/// <para><b>元素名 <c>new</c> 是 C# 关键字</b> ⇒ 属性名取 <c>New</c>，线格式靠 <c>JsonPropertyName</c> 钉回 <c>"new"</c>。</para>
/// <para><b>官方表两侧必填标记均为「否」</b>（照录）：实际替换须成对给出，SDK 不做校验。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_auditdrama.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodReplaceMediaItem
{
    /// <summary>old（官方类型 <c>number</c>）：旧的剧集media_id</summary>
    [JsonPropertyName("old")]
    public long? Old { get; set; }

    /// <summary>new（官方类型 <c>number</c>）：新的剧集media_id</summary>
    [JsonPropertyName("new")]
    public long? New { get; set; }
}
