// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.License;

/// <summary>
/// 接口许可账号码信息（<c>active_info</c>，<c>/cgi-bin/license/get_active_info_by_code</c> 与
/// <c>/cgi-bin/license/batch_get_active_info_by_code</c> 响应共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "License")]
public class LicenseActiveInfo
{
    /// <summary>获取或设置账号激活码。</summary>
    [JsonPropertyName("active_code")]
    public string? ActiveCode { get; set; }

    /// <summary>获取或设置账号类型：<c>1</c>-基础账号，<c>2</c>-互通账号。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>
    /// 获取或设置账号状态：<c>1</c>-未绑定 / <c>2</c>-已绑定且有效 / <c>3</c>-已过期 /
    /// <c>4</c>-待转移（企业开启自动激活时，成员离职或者被移出可见范围，第二天凌晨会更新为该状态）/
    /// <c>5</c>-已合并（激活码本身激活了 userid，后续使用新的激活码重新激活了该 userid，则该码变为已合并状态。
    /// 若被合并时该激活码未过期则合并后会重置 expire_time 为合并时间；若被合并时激活码已过期则不重置 expire_time。
    /// 注：该状态的激活码是已经失效的，不能重新用于激活或者继承）/ <c>6</c>-已分配给下游。
    /// </summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>
    /// 获取或设置账号绑定激活的企业成员 userid。
    /// <para>官方口径：未激活则不返回该字段；返回加密的 userid。</para>
    /// </summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>获取或设置创建时间（unix 时间戳，订单支付成功后立即创建）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>
    /// 获取或设置首次激活绑定用户的时间（unix 时间戳）。
    /// <para>官方口径：未激活则不返回该字段。</para>
    /// </summary>
    [JsonPropertyName("active_time")]
    public long? ActiveTime { get; set; }

    /// <summary>
    /// 获取或设置过期时间（unix 时间戳，为首次激活绑定的时间加上购买时长）。
    /// <para>官方口径：未激活则不返回该字段。</para>
    /// </summary>
    [JsonPropertyName("expire_time")]
    public long? ExpireTime { get; set; }

    /// <summary>
    /// 获取或设置合并信息。
    /// <para>官方口径：合并的激活码或者被合并的激活码才返回该字段。</para>
    /// </summary>
    [JsonPropertyName("merge_info")]
    public LicenseActiveMergeInfo? MergeInfo { get; set; }

    /// <summary>
    /// 获取或设置分配信息。
    /// <para>官方口径：当激活码在上下游/企业互联场景下，从上游分配给下游时，
    /// 获取上游或者下游企业该激活码详情时返回。</para>
    /// </summary>
    [JsonPropertyName("share_info")]
    public LicenseActiveShareInfo? ShareInfo { get; set; }
}
