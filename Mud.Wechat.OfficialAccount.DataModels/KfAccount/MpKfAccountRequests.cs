// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.KfAccount;

/// <summary>
/// 添加客服账号请求体（<c>addkfaccount</c>，<c>POST /customservice/kfaccount/add</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>账号形状硬约束（官方原文）</b>：<c>kf_account</c> 为「账号前缀@公众号微信号」——
/// 账号前缀<b>最多 10 个字符</b>，必须是英文、数字或下划线；后缀为公众号微信号，
/// <b>长度不超过 30 个字符</b>。昵称 <c>nickname</c> <b>最长 16 个字</b>。
/// </para>
/// <para><b>数量上限</b>：每个账号最多添加 <b>100</b> 个客服账号（超限 <c>65405</c>）。</para>
/// <para>
/// <b>前置条件</b>：如账号为公众号，<b>必须先在公众平台官网为公众号设置微信号后</b>才能使用该能力
/// （否则无可用后缀）。普通账号（公众号 / 服务号 / 小程序）<b>不需要填</b> <c>business_id</c>。
/// </para>
/// <para>
/// <b>官方无 <c>password</c> 字段</b>：字段表仅 <c>kf_account</c> / <c>nickname</c> / <c>business_id</c>
/// ⇒ SDK 不建模密码（避免给出「可设密码」的错误暗示）。
/// </para>
/// <para>响应仅 <c>errcode</c>/<c>errmsg</c>（由 <see cref="MpResponse"/> 承载）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "KfAccount")]
public class MpAddKfAccountRequest
{
    /// <summary>账号前缀最大字符数（官方：英文 / 数字 / 下划线）。</summary>
    public const int AccountPrefixMaxLength = 10;

    /// <summary>公众号微信号后缀最大字符数（官方）。</summary>
    public const int AccountSuffixMaxLength = 30;

    /// <summary>客服昵称最大字数（官方：最长 16 个字）。</summary>
    public const int NicknameMaxLength = 16;

    /// <summary>每个账号可添加的客服账号数上限（官方：最多 100 个）。</summary>
    public const int MaxKfAccountCount = 100;

    /// <summary>完整客服账号（「账号前缀@公众号微信号」）。</summary>
    [JsonPropertyName("kf_account")]
    public string KfAccount { get; set; } = string.Empty;

    /// <summary>客服昵称（最长 16 个字）。</summary>
    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = string.Empty;

    /// <summary>客服子商户的 business_id（<b>普通账号不需要填</b>）。</summary>
    [JsonPropertyName("business_id")]
    public string? BusinessId { get; set; }
}

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

/// <summary>
/// 删除客服账号请求体（<c>delkfaccount</c>，<c>POST /customservice/kfaccount/del</c>）。
/// </summary>
/// <remarks>官方字段表仅 <c>kf_account</c>（必填）。</remarks>
[HttpJsonSerializable(SerializerClassName = "KfAccount")]
public class MpDelKfAccountRequest
{
    /// <summary>完整客服账号（「账号前缀@公众号微信号」）。</summary>
    [JsonPropertyName("kf_account")]
    public string KfAccount { get; set; } = string.Empty;
}

/// <summary>
/// 邀请绑定客服账号请求体（<c>invitekfworker</c>，<c>POST /customservice/kfaccount/inviteworker</c>）。
/// </summary>
/// <remarks>
/// <para>官方字段表：<c>kf_account</c>（完整客服账号）+ <c>invite_wx</c>（接收绑定邀请的客服微信号）。</para>
/// <para>
/// 官方错误码（全为该业务的专有码）：<c>65407</c>（已是本账号客服）/ <c>65408</c>（已发送邀请）/
/// <c>65409</c>（无效微信号）/ <c>65410</c>（绑定数量超限）/ <c>65411</c>（存在待确认邀请）/
/// <c>65412</c>（该客服账号已绑定微信号）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "KfAccount")]
public class MpInviteKfWorkerRequest
{
    /// <summary>完整客服账号（「账号前缀@公众号微信号」）。</summary>
    [JsonPropertyName("kf_account")]
    public string KfAccount { get; set; } = string.Empty;

    /// <summary>接收绑定邀请的客服微信号。</summary>
    [JsonPropertyName("invite_wx")]
    public string InviteWx { get; set; } = string.Empty;
}
