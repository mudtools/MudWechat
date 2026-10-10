// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.ContactWay;

/// <summary>
/// 获取企业已配置的「联系我」列表响应体（<c>/cgi-bin/externalcontact/list_contact_way</c>）。
/// <para>该接口不包含临时会话模式的「联系我」，且仅可查询 2021-07-10 之后创建的记录。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ContactWay")]
public class ListContactWayResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置「联系我」配置列表（元素仅含 <c>config_id</c>，详情须逐条调用「获取『联系我』方式」）。
    /// </summary>
    [JsonPropertyName("contact_way")]
    public List<ContactWayIdItem>? ContactWay { get; set; }

    /// <summary>
    /// 获取或设置分页游标（用于查询下一个分页，无更多数据时不会返回该字段）。
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }
}

/// <summary>
/// 「联系我」配置 id 项（获取「联系我」列表响应中 <c>contact_way[]</c> 的元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ContactWay")]
public class ContactWayIdItem
{
    /// <summary>
    /// 获取或设置联系方式的配置 id。
    /// </summary>
    [JsonPropertyName("config_id")]
    public string? ConfigId { get; set; }
}
