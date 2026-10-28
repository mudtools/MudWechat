// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Kf;

/// <summary>设置 / 取消客服账号角色（管理员）请求体（<c>POST /customservice/kfaccount/setadmin</c>、<c>canceladmin</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>kf-mgnt/kf-message/api_setkfadmin.html</c>、<c>api_cancelkfadmin.html</c>。</para>
/// <para><c>kfaccount</c> 为<b>完整客服账号</b>（官方原文：账号前缀@公众号微信号），同一请求体两端点复用。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class WxaKfAccountRequest
{
    /// <summary>完整客服账号（<c>kfaccount</c>，必填；格式：账号前缀@公众号微信号）。</summary>
    [JsonPropertyName("kfaccount")]
    public string? KfAccount { get; set; }
}
