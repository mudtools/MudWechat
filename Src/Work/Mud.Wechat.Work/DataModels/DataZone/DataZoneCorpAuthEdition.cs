// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.DataZone;

/// <summary>
/// 企业授权的会话版本（官方 <c>auth_edition_list</c> 元素结构说明）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataZone")]
public class DataZoneCorpAuthEdition
{
    /// <summary>获取或设置企业授权的会话版本类型（官方 edition）：1 - 内部会话；2 - 内外部会话；3 - 内外部会话及语音通话。</summary>
    [JsonPropertyName("edition")]
    public long? Edition { get; set; }

    /// <summary>获取或设置企业授权存档范围（官方 auth_scope；<see cref="DataZoneAuthScope"/>）。
    /// <para>实际生效人员（参见「获取会话存档授权成员列表」）取决于服务商购买的人数。</para></summary>
    [JsonPropertyName("auth_scope")]
    public DataZoneAuthScope? AuthScope { get; set; }

    /// <summary>
    /// 获取或设置会话版本的当前状态（官方 status）：1 - 试用中；2 - 试用已到期；3 - 付费使用中；4 - 付费使用已到期；
    /// 5 - 免费使用中；6 - 免费使用已到期；7 - 付费待生效。
    /// </summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }

    /// <summary>
    /// 获取或设置开始生效时间（官方 begin_time，Unix 时间戳）。
    /// <para>状态为试用/付费/免费使用中或已到期时，为对应开始生效时间；付费待生效时为预期开始生效时间。
    /// 所有版本共用同一试用期，试用期已到期后授权且未购买的版本此字段不返回。</para>
    /// </summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>获取或设置结束生效时间（官方 end_time，Unix 时间戳；状态语义与 begin_time 相同）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置会话授权的时长（官方 msg_duration_days，单位：天，一年按 365 天计）。</summary>
    [JsonPropertyName("msg_duration_days")]
    public long? MsgDurationDays { get; set; }

    /// <summary>获取或设置企业授权存档的去重人数（官方 auth_user_count）。</summary>
    [JsonPropertyName("auth_user_count")]
    public long? AuthUserCount { get; set; }
}
