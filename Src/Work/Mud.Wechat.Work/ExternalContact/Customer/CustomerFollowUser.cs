// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Customer;

/// <summary>
/// 企业成员客户跟进信息（获取客户详情 <c>follow_user[]</c> 元素；标签为完整结构）。
/// </summary>
/// <remarks>
/// <see cref="RemarkMobiles"/> 第三方应用和代开发应用不可获取。
/// <see cref="AddWay"/> 为添加来源（官方固定枚举：0-未知、1-扫描二维码、2-搜索手机号、3-名片分享、4-群聊、
/// 5-手机通讯录、6-微信联系人、8-安装第三方应用时自动添加的客服人员、9-搜索邮箱、10-视频号添加、
/// 11-通过日程参与人添加、12-通过会议参与人添加、13-添加微信好友对应的企业微信、14-通过智慧硬件专属客服添加、
/// 15-通过上门服务客服添加、16-通过获客链接添加、17-通过定制开发添加、18-通过需求回复添加、
/// 21-通过第三方售前客服添加、22-通过可能的商务伙伴添加、24-通过接受微信账号收到的好友申请添加、
/// 201-内部成员共享、202-管理员/负责人分配）；<see cref="State"/> 为企业自定义渠道参数，二者语义不同。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Customer")]
public class CustomerFollowUser
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
    /// 获取或设置该成员所打标签列表（含分组名 / 标签名 / 标签类型）。
    /// </summary>
    [JsonPropertyName("tags")]
    public List<CustomerFollowTag>? Tags { get; set; }

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
    /// 获取或设置发起添加的 userid（成员主动添加为成员 userid；客户主动添加为客户的外部联系人 userid；
    /// 共享 / 管理员分配为对应的成员 / 管理员 userid）。
    /// </summary>
    [JsonPropertyName("oper_userid")]
    public string? OperUserid { get; set; }

    /// <summary>
    /// 获取或设置该成员添加此客户的来源（add_way 枚举，详见类型注释）。
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
