// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.AccountId;

/// <summary>
/// 申请群 ID 的升级请求体（<c>/cgi-bin/idconvert/apply_to_upgrade_chatid</c>）：
/// 设置代开发应用群 ID 完成升级的时间；升级生效前可使用新旧两种群 ID 调用相关接口，生效后必须使用新群 ID。
/// </summary>
/// <remarks>
/// 官方限制：仅代开发应用可调用；应用需具有「客户联系 -&gt; 基础客户信息」权限；
/// upgrade_time 不得设置早于当前时间或 7 天之后的时间。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "AccountId")]
public class ApplyToUpgradeChatIdRequest
{
    /// <summary>
    /// 获取或设置完成升级的时间戳（官方必填；不得早于当前时间或 7 天之后）。
    /// </summary>
    [JsonPropertyName("upgrade_time")]
    public int? UpgradeTime { get; set; }
}
