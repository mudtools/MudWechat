// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Customer;

/// <summary>
/// 企业成员客户跟进信息（批量获取客户详情 <c>external_contact_list[].follow_info</c>；
/// 与获取客户详情的 follow_user 结构对应，但标签只返回企业标签与规则组标签的 tag_id，个人标签不返回）。
/// </summary>
public class CustomerFollowInfo
{
    /// <summary>
    /// 获取或设置添加了此外部联系人的企业成员 userid。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>
    /// 获取或设置该成员对此外部联系人的备注。
    /// </summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>
    /// 获取或设置该成员对此外部联系人的描述。
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// 获取或设置该成员添加此外部联系人的时间戳。
    /// </summary>
    [JsonPropertyName("createtime")]
    public long? Createtime { get; set; }

    /// <summary>
    /// 获取或设置该成员所打企业标签 / 规则组标签的 tag_id 列表（个人标签不返回）。
    /// </summary>
    [JsonPropertyName("tag_id")]
    public List<string>? TagId { get; set; }

    /// <summary>
    /// 获取或设置该成员对此微信客户备注的企业名称（仅微信客户有该字段）。
    /// </summary>
    [JsonPropertyName("remark_corp_name")]
    public string? RemarkCorpName { get; set; }

    /// <summary>
    /// 获取或设置该成员对此客户备注的手机号码列表（第三方应用和代开发应用不可获取）。
    /// </summary>
    [JsonPropertyName("remark_mobiles")]
    public List<string>? RemarkMobiles { get; set; }

    /// <summary>
    /// 获取或设置发起添加的 userid。
    /// </summary>
    [JsonPropertyName("oper_userid")]
    public string? OperUserid { get; set; }

    /// <summary>
    /// 获取或设置该成员添加此客户的来源（add_way 枚举，同获取客户详情）。
    /// </summary>
    [JsonPropertyName("add_way")]
    public int? AddWay { get; set; }

    /// <summary>
    /// 获取或设置企业自定义的 state 参数（区分客户通过哪个「联系我」或获客链接添加）。
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>
    /// 获取或设置视频号来源信息（add_way = 10 时返回）。
    /// </summary>
    [JsonPropertyName("wechat_channels")]
    public WechatChannelsInfo? WechatChannels { get; set; }
}
