// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 修改客服账号请求体（<c>/cgi-bin/kf/account/update</c>）。
/// <para>
/// 修改已有客服账号的名称与头像，两个字段均可选填，不需要修改的可不填；
/// 只能通过 API 管理企业指定的客服账号。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class UpdateKfAccountRequest
{
    /// <summary>
    /// 获取或设置要修改的客服账号 ID（官方必填，不多于 64 个字节）。
    /// </summary>
    [JsonPropertyName("open_kfid")]
    public string? OpenKfId { get; set; }

    /// <summary>
    /// 获取或设置新的客服名称（不多于 16 个字符；不需要修改可不填）。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置新的客服头像临时素材 media_id（可调用上传临时素材接口获取，不多于 128 个字节；不需要修改可不填）。
    /// </summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }
}
