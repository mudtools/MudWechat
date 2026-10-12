// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.KfAccount;

/// <summary>
/// 获取所有客服账号响应（<c>getkflist</c>，<c>GET /cgi-bin/customservice/getkflist</c>）。
/// </summary>
/// <remarks>
/// 官方契约：<b>GET 且无请求体</b>；业务 Query 仅可选 <c>business_id</c>（客服子商户的 business_id，
/// 普通账号不需要填）。故本端点只以参数形态暴露 <c>business_id</c>，**不设请求 DTO**。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "KfAccount")]
public class MpGetKfListResponse : MpResponse
{
    /// <summary>客服列表。</summary>
    [JsonPropertyName("kf_list")]
    public List<MpKfAccount>? KfList { get; set; }
}
