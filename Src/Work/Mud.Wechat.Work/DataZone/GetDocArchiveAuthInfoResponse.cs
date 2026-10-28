// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.DataZone;

/// <summary>
/// 获取数据与智能专区文档存档授权信息响应体（<c>/cgi-bin/docdata/get_auth_info</c>；
/// 官方仅向第三方应用开放，企业自建应用与服务商代开发均不支持）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataZone")]
public class GetDocArchiveAuthInfoResponse : WechatWorkResponse
{
    /// <summary>获取或设置授权状态（官方 status）：1 - 试用中；2 - 试用已结束。</summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }

    /// <summary>获取或设置企业授权存档范围（官方 auth_scope；<see cref="DataZoneAuthScope"/>）。</summary>
    [JsonPropertyName("auth_scope")]
    public DataZoneAuthScope? AuthScope { get; set; }

    /// <summary>获取或设置试用开始时间（官方 begin_time，Unix 时间戳；仅授权状态 status = 1 时返回）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>获取或设置企业试用到期时间（官方 end_time，Unix 时间戳；仅授权状态 status = 1 时返回）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置企业授权存档范围的去重人数（官方 auth_user_count）。</summary>
    [JsonPropertyName("auth_user_count")]
    public long? AuthUserCount { get; set; }
}
