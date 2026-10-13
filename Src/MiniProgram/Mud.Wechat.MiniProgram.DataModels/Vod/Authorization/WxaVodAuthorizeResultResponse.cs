// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 「逐剧目授权结果」应答（<c>errcode</c> / <c>errmsg</c> / <c>result</c>），四处授权写端点共用。
/// </summary>
/// <remarks>
/// <para><b>四处共用裁决</b>：<c>authorizedrama</c>/<c>deauthorizedrama</c>/<c>authorizecopyright</c>/<c>deauthorizecopyright</c> 四页的返回体表逐字段相同（<c>errcode</c>、<c>errmsg</c>、<c>result</c>，元素面见 <see cref="WxaVodAuthorizeResult"/>）⇒ 共用本类型；按端点拆四个同形包装只会误导「四者应答有差异」。</para>
/// <para><b>逐项判定</b>：接口级 <c>errcode</c> 为 0 只表示请求被受理，每一部剧的成败在 <c>result</c> 内逐剧目给出。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizedrama/api_authorizedrama.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodAuthorizeResultResponse : WxaResponse
{
    /// <summary>result（官方类型 <c>objarray</c>）：剧目的授权结果。</summary>
    [JsonPropertyName("result")]
    public List<WxaVodAuthorizeResult>? Result { get; set; }
}
