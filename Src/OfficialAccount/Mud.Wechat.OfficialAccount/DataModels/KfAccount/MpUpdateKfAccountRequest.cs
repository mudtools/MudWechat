// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.KfAccount;

/// <summary>
/// 修改客服账号请求体（<c>updatekfaccount</c>，<c>POST /customservice/kfaccount/update</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何不为「与添加共用 DTO」而合并</b>：官方修改页字段表<b>仅 <c>kf_account</c> + <c>nickname</c></b>，
/// <b>无 <c>business_id</c></b> ⇒ 共用会把添加侧的可选字段混入修改请求（语义污染）。
/// 两页形状确属不同，故分别声明。
/// </para>
/// <para>官方错误码：<c>0</c> / <c>65400</c> / <c>65401</c>（无效客服账号）/ <c>65403</c>（昵称不合法）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "KfAccount")]
public class MpUpdateKfAccountRequest
{
    /// <summary>完整客服账号（「账号前缀@公众号微信号」）。</summary>
    [JsonPropertyName("kf_account")]
    public string KfAccount { get; set; } = string.Empty;

    /// <summary>客服昵称（最长 16 个字）。</summary>
    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = string.Empty;
}
