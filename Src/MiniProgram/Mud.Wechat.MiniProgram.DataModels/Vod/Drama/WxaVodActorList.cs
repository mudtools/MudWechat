// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 演员信息袋（官方 <c>Body.actor_list</c>：外层是对象、内层 <c>actor</c> 才是数组）。
/// </summary>
/// <remarks>
/// <para><b>不是「数组直挂」而是「对象包数组」（照录官方包装形态）</b>：<c>actor_list</c> 为 object、其 <c>actor</c> 为 objarray ⇒ 若把它做成 <c>List&lt;WxaVodActor&gt;</c> 会直接破坏线格式。</para>
/// <para><b>共用裁决</b>：<c>auditdrama</c>、<c>modifydramabasicinfo</c> 两请求页与 <c>getdrama</c>、<c>listdramas</c> 两响应页的演员字段面逐字段相同 ⇒ 四处共用本类型与 <see cref="WxaVodActor"/>。</para>
/// <para><b>人数区间（官方原文）</b>：需填写 2-5 位演员；<c>drama_type=2</c>（真人）时必填，漫剧（1）与数字真人（3）无需填写。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_auditdrama.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodActorList
{
    /// <summary>actor（官方类型 <c>objarray</c>，<b>必填</b>）：演员列表</summary>
    [JsonPropertyName("actor")]
    public List<WxaVodActor>? Actor { get; set; }
}
