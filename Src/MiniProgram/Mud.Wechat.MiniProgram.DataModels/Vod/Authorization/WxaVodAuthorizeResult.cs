// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 逐剧目授权/解除授权结果（<c>drama_id</c> + 该剧目级 <c>errcode</c>/<c>errmsg</c>）。
/// </summary>
/// <remarks>
/// <para><b>四处共用裁决</b>：<c>authorizedrama</c>/<c>deauthorizedrama</c>/<c>authorizecopyright</c>/<c>deauthorizecopyright</c> 四页的 <c>result(Array)</c> 子表字段名完全相同 ⇒ 共用本类型。</para>
/// <para><b>逐剧目结果码不是接口结果码</b>：接口级 <c>errcode</c> 在外层 <see cref="Mud.Wechat.MiniProgram.DataModels.WxaResponse"/>，本类型内的 <c>errcode</c> 描述「这一部剧」的成败 ⇒ <b>接口成功不代表全部剧目授权成功</b>，宿主须逐项判定。</para>
/// <para><b><c>errmsg</c> 在成功项里不出现</b>（官方示例：<c>{"drama_id":100200,"errcode":0}</c>）⇒ 属性可空。官方 <c>deauthorizedrama</c> 页把 <c>errmsg</c> 类型列作 <c>number</c>，同页示例证明是字符串 ⇒ 照示例取 <c>string</c>。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizedrama/api_authorizedrama.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodAuthorizeResult
{
    /// <summary>drama_id（官方类型 <c>number</c>）：剧目ID。</summary>
    [JsonPropertyName("drama_id")]
    public long? DramaId { get; set; }

    /// <summary>errcode（官方类型 <c>number</c>）：错误码。</summary>
    [JsonPropertyName("errcode")]
    public long? Errcode { get; set; }

    /// <summary>errmsg（官方类型 <c>string</c>）：错误原因。</summary>
    [JsonPropertyName("errmsg")]
    public string? Errmsg { get; set; }
}
