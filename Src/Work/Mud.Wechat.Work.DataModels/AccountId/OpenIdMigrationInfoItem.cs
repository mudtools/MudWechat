// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.AccountId;

/// <summary>
/// 代开发应用迁移状态查询结果元素。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "AccountId")]
public class OpenIdMigrationInfoItem
{
    /// <summary>
    /// 获取或设置 id 类型：1 - corpid/userid，3 - external_userid。
    /// </summary>
    [JsonPropertyName("openid_type")]
    public int? OpenIdType { get; set; }

    /// <summary>
    /// 获取或设置 id 回收状态：0 - 未升级，1 - 已升级。
    /// </summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }
}
