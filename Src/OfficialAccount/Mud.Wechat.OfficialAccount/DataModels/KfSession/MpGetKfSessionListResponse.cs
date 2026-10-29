// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.KfSession;

/// <summary>
/// 获取客服会话列表响应（<c>getkfsessionlist</c>，<c>GET /customservice/kfsession/getsessionlist</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：<b>GET</b>，业务 Query 参数 <c>kf_account</c>（<b>必填</b>，完整客服账号）；
/// <b>无请求体</b> ⇒ 只以参数形态暴露。
/// </para>
/// <para>官方错误码：<c>0</c> / <c>65401</c>（无效客服账号）/ <c>65402</c>（客服账号尚未绑定微信号）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "KfSession")]
public class MpGetKfSessionListResponse : MpResponse
{
    /// <summary>会话列表。</summary>
    [JsonPropertyName("sessionlist")]
    public List<MpKfSession>? SessionList { get; set; }
}
