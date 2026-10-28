// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.CustomerAcquisition;

/// <summary>
/// 删除获客链接请求体（<c>/cgi-bin/externalcontact/customer_acquisition/delete_link</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "CustomerAcquisition")]
public class DeleteAcquisitionLinkRequest
{
    /// <summary>
    /// 获取或设置获客链接 id（官方必填；需为当前应用创建的链接）。
    /// </summary>
    [JsonPropertyName("link_id")]
    public string? LinkId { get; set; }
}
