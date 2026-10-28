// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.KfSession;

/// <summary>
/// 获取客户会话状态响应（<c>getkfsession</c>，<c>GET /customservice/kfsession/getsession</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：<b>GET</b>，业务 Query 参数 <c>openid</c>（<b>必填</b>）；<b>无请求体</b>
/// ⇒ 故只以参数形态暴露 <c>openid</c>，不设请求 DTO。
/// </para>
/// <para>官方响应仅 <c>createtime</c>（会话接入时间，timestamp）与 <c>kf_account</c>（接待客服账号）。</para>
/// <para>官方错误码：<c>40003</c>（invalid openid）/ <c>65400</c>（未开通或未升级新版客服功能）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "KfSession")]
public class MpGetKfSessionResponse : MpResponse
{
    /// <summary>会话接入时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("createtime")]
    public long CreateTime { get; set; }

    /// <summary>接待客服账号。</summary>
    [JsonPropertyName("kf_account")]
    public string? KfAccount { get; set; }
}
