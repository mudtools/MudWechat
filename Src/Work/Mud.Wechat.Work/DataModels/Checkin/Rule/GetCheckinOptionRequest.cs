// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 获取员工打卡规则请求体（<c>/cgi-bin/checkin/getcheckinoption</c>）。
/// </summary>
/// <remarks>
/// <para>官方限制：用户列表不超过 100 个，若用户超过 100 个请分批获取；用户在不同日期的规则不一定相同，请按天获取。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class GetCheckinOptionRequest
{
    /// <summary>
    /// 获取或设置需要获取规则的日期当天 0 点的 Unix 时间戳（官方必填）。
    /// </summary>
    [JsonPropertyName("datetime")]
    public long? Datetime { get; set; }

    /// <summary>
    /// 获取或设置需要获取打卡规则的用户列表（官方必填；不超过 100 个，若超过 100 个请分批获取）。
    /// </summary>
    [JsonPropertyName("useridlist")]
    public List<string>? UserIdList { get; set; }
}
