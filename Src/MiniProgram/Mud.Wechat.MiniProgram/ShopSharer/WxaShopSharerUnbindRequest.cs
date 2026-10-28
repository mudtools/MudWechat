// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.ShopSharer;

/// <summary>
/// 批量解绑分享员请求（<c>POST /shop/sharer/unbind</c>，请求体只有 <c>openids</c> 一支键）。
/// </summary>
/// <remarks>
/// <para><b>与 <see cref="WxaShopSharerBindRequest"/> 不合并</b>：两支官方语义相反（邀请 / 解绑）、
/// 应答结构亦不同（本支无 <c>result_list</c>），请求体同形是巧合而非同源 ⇒ 类型独立建模（守卫 SHOPSH5 锁定）。</para>
/// <para><b>敏感面</b>：<c>openid</c> 禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopSharer")]
public class WxaShopSharerUnbindRequest
{
    /// <summary>要解绑的分享员 openid 列表（<c>openids</c>，string array，必填）。禁落日志。</summary>
    [JsonPropertyName("openids")]
    public List<string>? OpenIds { get; set; }
}
