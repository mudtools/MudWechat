// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.AcquisitionComponent;

/// <summary>
/// 获客助手组件获取获客链接列表响应体（<c>/cgi-bin/externalcontact/customer_acquisition/list_link</c>）。
/// </summary>
/// <remarks>
/// 组件版仅可获取<b>授权给获客助手组件</b>的链接（与自建/第三方直连版 <c>CustomerAcquisition</c> 域的
/// 全量链接列表语义不同）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "AcquisitionComponent")]
public class GetAcquisitionComponentLinkListResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置获客链接 id 列表。
    /// </summary>
    [JsonPropertyName("link_id_list")]
    public List<string>? LinkIdList { get; set; }

    /// <summary>
    /// 获取或设置分页游标，在下次请求时填写以获取之后分页的记录；已无更多数据则不返回该字段。
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }
}
