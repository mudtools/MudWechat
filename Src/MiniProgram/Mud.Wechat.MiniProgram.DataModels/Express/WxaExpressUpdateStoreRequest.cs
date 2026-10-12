// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>更新门店请求体（<c>POST /cgi-bin/express/intracity/updatestore</c>）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_updatestore"/>。</para>
/// <para><c>keys</c> 为门店定位（<c>wx_store_id</c> 与 <c>out_store_id</c> 二选一），<c>content</c> 为待更新内容（均可选，只更新传入字段）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressUpdateStoreRequest
{
    /// <summary>门店编号（<c>keys</c>，必填），见 <see cref="WxaExpressUpdateStoreKey"/>。</summary>
    [JsonPropertyName("keys")]
    public WxaExpressUpdateStoreKey? Key { get; set; }

    /// <summary>更新内容（<c>content</c>，必填），见 <see cref="WxaExpressUpdateStoreContent"/>。</summary>
    [JsonPropertyName("content")]
    public WxaExpressUpdateStoreContent? Content { get; set; }
}
