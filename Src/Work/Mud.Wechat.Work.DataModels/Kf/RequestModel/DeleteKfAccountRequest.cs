// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 删除客服账号请求体（<c>/cgi-bin/kf/account/del</c>）。
/// <para>
/// 只能通过 API 管理企业指定的客服账号：企业须在管理后台
/// 「微信客服-通过API管理微信客服账号」处设置对应账号允许 API 管理。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class DeleteKfAccountRequest
{
    /// <summary>
    /// 获取或设置客服账号 ID（官方必填，不多于 64 个字节）。
    /// </summary>
    [JsonPropertyName("open_kfid")]
    public string? OpenKfId { get; set; }
}
